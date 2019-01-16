// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Web.Internal
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Trumpf.Coparoo.Web.Logging.Tree;
    using Trumpf.Coparoo.Web.Waiting;

    /// <summary>
    /// DOM object base class.
    /// </summary>
    public abstract class UIObject : IUIObjectInternal
    {
        private IUIObjectNode node;

        /// <summary>
        /// Gets the internal root node.
        /// </summary>
        internal ITabObjectInternal RootInternal => Root as ITabObjectInternal;

        /// <summary>
        /// Gets the node type of this UI object.
        /// </summary>
        internal abstract NodeType NodeType { get; }

        /// <summary>
        /// Gets the tab object.
        /// </summary>
        public ITabObject Root => (this as IUIObjectInternal).Root as ITabObject;

        /// <summary>
        /// Gets the root page object.
        /// </summary>
        /// <returns>The root page object.</returns>
        ITabObject IUIObjectInternal.Root => this is Web.ITabObject ? this as Web.ITabObject : (Parent as IUIObjectInternal).Root;

        /// <summary>
        /// Sets the 0-based control index.
        /// </summary>
        int IUIObjectInternal.Index
        {
            set { NodeInternal.Index = value; }
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        IUIObjectNode IUIObject.Node => node;

        /// <summary>
        /// Gets a value indicating whether the page object's node is visible on the screen.
        /// </summary>
        public Bool Displayed => WoolFor(() => IsDisplayed, nameof(Displayed));

        /// <summary>
        /// Gets a value indicating whether the page object's node exists in the UI tree.
        /// </summary>
        public virtual Bool Exists => WoolFor(() => UIObjectNode.Accessible(NodeInternal.TryRoot), nameof(Exists));

        /// <summary>
        /// Gets a System.Drawing.Point object containing the coordinates of the upper-left corner of this element relative to the upper-left corner of the page.
        /// </summary>
        public virtual Point Location => Node.Location;

        /// <summary>
        /// Gets or sets a value indicating whether the UI object can respond to user interaction.
        /// </summary>
        public virtual Bool Enabled => WoolFor(() => Node.Enabled, nameof(Enabled));

        /// <summary>
        /// Gets the parent of this page object.
        /// </summary>
        public IUIObject Parent { get; private set; }

        /// <summary>
        /// Gets the node in the UI tree associated with this object.
        /// </summary>
        protected IUIObjectNode Node => node;

        /// <summary>
        /// Gets the typed root node of the page object.
        /// This is the object this page object accesses to interact with UI elements.
        /// </summary>
        internal IUIObjectNodeInternal NodeInternal => (IUIObjectNodeInternal)Node;

        /// <summary>
        /// Gets a value indicating whether the page object's node is visible on the screen.
        /// </summary>
        protected virtual bool IsDisplayed
        {
            get
            {
                var node = (IWebElement)NodeInternal.TryRoot;
                bool result;
                try
                {
                    result = node != null && node.Displayed;
                }
                catch
                {
                    result = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Initializes this object.
        /// </summary>
        /// <param name="parent">Parent page object.</param>
        /// <param name="root">The root node.</param>
        /// <returns>The initialized object.</returns>
        internal virtual IUIObject Init(IUIObject parent, IUIObjectNode root)
        {
            Parent = parent;
            node = root;

            return this;
        }

        /// <summary>
        /// Get the page object.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The page object.</returns>
        public TPageObject On<TPageObject>(Predicate<TPageObject> condition = null) where TPageObject : IPageObject => RootInternal.PageObjectLocator.Find(condition ?? (_ => true));

        /// <summary>
        /// Snap the page object, i.e. associate it with a specific node in the UI tree. This disables lazy node evaluation.
        /// </summary>
        /// <returns>Whether the snap succeeded.</returns>
        public bool TrySnap() => Node.TrySnap();

        /// <summary>
        /// Unsnap the object.
        /// </summary>
        /// <returns>Whether the node was snapped before.</returns>
        bool IUIObjectInternal.TryUnsnap() => Node.TryUnsnap();

        /// <summary>
        /// Goto the page object.
        /// If the current page object cannot directly navigate to the target, it may forward it to its child page objects.
        /// Throws if the page object cannot be navigated to.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for target page object.</param>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The target page object.</returns>
        public TPageObject Goto<TPageObject>(Predicate<TPageObject> condition = null) where TPageObject : IPageObject
        {
            TPageObject r = On(condition ?? (_ => true));
            r.Goto();
            return r;
        }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="enableCaching">Whether to enable caching.</param>
        /// <returns>The initialized page object.</returns>
        IUIObject IUIObjectInternal.Init(IUIObject parent, bool enableCaching)
        {
            return Init(parent, enableCaching);
        }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="enableCaching">Whether to enable caching.</param>
        /// <returns>The initialized page object.</returns>
        internal virtual IUIObject Init(IUIObject parent, bool enableCaching)
        {
            IUIObjectNode node = CreateNode;
            ((IUIObjectNodeInternal)node).Init(parent.Node, GetHashCode(), enableCaching);
            Init(parent, node);

            return this;
        }

        /// <summary>
        /// Gets a fresh node object.
        /// </summary>
        internal abstract IUIObjectNode CreateNode { get; }

        /// <summary>
        /// Gets an await-object.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The display name used in timeout exceptions.</param>
        /// <returns>The wrapped object.</returns>
        protected IAwait<T> Await<T>(Func<T> function, string name) => RootInternal.Await(function, name);

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => GetType().FullName.GetHashCode();

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <returns>The control object.</returns>
        public virtual TControl Find<TControl>(By pattern = null, Predicate<ISearchContext> predicate = null) where TControl : IControlObject
        {
            var result = (TControl)Activator.CreateInstance(((ITabObjectInternal)Root).UIObjectInterfaceResolver.Resolve<TControl>());
            (result as IUIObjectInternal).Init(this, false);
            (result as IControlObjectInternal).Init(pattern, predicate, true);
            return result;
        }

        /// <summary>
        /// Gets all matching controls.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <returns>The control enumeration.</returns>
        public virtual IEnumerable<TControl> FindAll<TControl>(By pattern = null, Predicate<ISearchContext> predicate = null) where TControl : IControlObject
        {
            int next = 0;
            while (true)
            {
                TControl result = Find<TControl>(pattern, predicate);
                (result as IUIObjectInternal).Index = next++;
                if (!result.Exists)
                {
                    break;
                }

                yield return result;
            }
        }

        /// <summary>
        /// Scroll to the UI object.
        /// </summary>
        public virtual void ScrollTo()
        {
            Actions actions = new Actions(Root.Driver);
            actions.MoveToElement(NodeInternal.Root as IWebElement);
            actions.Perform();
        }

        /// <summary>
        /// Scroll to the UI object.
        /// </summary>
        /// <param name="timeout">The timeout to wait for the object to become visible between page downs.</param>
        public virtual void ScrollTo(TimeSpan timeout)
        {
            ScrollTo();
            Displayed.WaitFor(timeout);
        }

        /// <summary>
        /// Gets a wrapped Boolean.
        /// </summary>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The wrapped Boolean.</returns>
        private Bool WoolFor(Func<bool> function, string name) => new Bool(new Await<bool>(function, name, GetType(), () => Root.Configuration.WaitTimeout, () => Root.Configuration.PositiveWaitTimeout, () => Root.Configuration.ShowWaitingDialog), () => TrySnap(), () => (this as IUIObjectInternal).TryUnsnap());
    }
}