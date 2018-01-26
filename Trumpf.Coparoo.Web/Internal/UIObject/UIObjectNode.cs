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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;

    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web.Waiting;

    /// <summary>
    /// Page object node class wrapping a UI tree node.
    /// </summary>
    internal class UIObjectNode : IUIObjectNode, IUIObjectNodeInternal
    {
        private By searchPattern;
        private Predicate<ISearchContext> predicate;
        private bool autoSnap = false;
        private ISearchContext mNode;
        private int index = 0;
        private IUIObjectNode mParent;
        private int mHash;
        private ITabObjectNode mRootNode = null;
        private bool enableCaching;

        /// <summary>
        /// Gets the predicate selector.
        /// </summary>
        public Predicate<ISearchContext> Predicate => predicate ?? (_ => true);

        /// <summary>
        /// Gets the root to node search pattern.
        /// </summary>
        public virtual By SearchPattern => searchPattern;

        /// <summary>
        /// Gets the root.
        /// </summary>
        public IWebElement Root => (IWebElement)RootSearchContext;

        /// <summary>
        /// Gets the root.
        /// </summary>
        public IWebElement TryRoot => (IWebElement)TryRootSearchContext;

        /// <summary>
        /// Sets the 0-based control index.
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// </summary>
        public virtual ISearchContext RootSearchContext => mNode ?? GetRoot(() => RootUncached);

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found.
        /// </summary>
        public virtual ISearchContext TryRootSearchContext => mNode ?? GetRoot(() => TryRootUncached);

        /// <summary>
        /// Gets the process node.
        /// </summary>
        public ITabObjectNode RootNode => mRootNode ?? (mRootNode = this is ITabObjectNode ? this as ITabObjectNode : ((IUIObjectNodeInternal)mParent).RootNode);

        /// <summary>
        /// Gets the root node.
        /// </summary>
        protected virtual ISearchContext Parent => (mParent as IUIObjectNodeInternal).RootSearchContext;

        /// <summary>
        /// Gets the root node if possible, and otherwise returns null.
        /// </summary>
        protected virtual ISearchContext TryParent => (mParent as IUIObjectNodeInternal).TryRootSearchContext;

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// </summary>
        internal virtual ISearchContext RootUncached => Index == 0 && predicate == null ? Parent.FindElement(SearchPattern) : Matches().ElementAt(Index);

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found.
        /// </summary>
        internal virtual ISearchContext TryRootUncached
        {
            get
            {
                ISearchContext parent = TryParent;
                if (parent == null)
                {
                    return null;
                }
                else if (predicate == null && index == 0)
                {
                    ISearchContext node;
                    try
                    {
                        node = parent.FindElement(SearchPattern);
                    }
                    catch
                    {
                        // the parent disappeared
                        node = null;
                    }

                    return node;
                }
                else
                {
                    return Matches(parent).ElementAtOrDefault(Index);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether to snap node automatically.
        /// </summary>
        protected virtual bool EnableAutoSnap => autoSnap;

        /// <summary>
        /// Gets the tag name. Also check the Selenium documentation.
        /// </summary>
        public string TagName => Root.TagName;

        /// <summary>
        /// Gets the text. Also check the Selenium documentation.
        /// </summary>
        public string Text => Root.Text;

        /// <summary>
        /// Gets a value indicating whether the element is enabled. Also check the Selenium documentation.
        /// </summary>
        public bool Enabled => Root.Enabled;

        /// <summary>
        /// Gets a value indicating whether the element is selected. Also check the Selenium documentation.
        /// </summary>
        public bool Selected => Root.Selected;

        /// <summary>
        /// Gets the location. Also check the Selenium documentation.
        /// </summary>
        public Point Location => Root.Location;

        /// <summary>
        /// Gets the size. Also check the Selenium documentation.
        /// </summary>
        public Size Size => Root.Size;

        /// <summary>
        /// Gets a value indicating whether the element is displayed. Also check the Selenium documentation.
        /// </summary>
        public bool Displayed => Root.Displayed;

        /// <summary>
        /// Determine whether the node is accessible, i.e. its properties can be retrieved.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Whether the node is accessible or not.</returns>
        internal static bool Accessible(ISearchContext node)
        {
            try
            {
                var temp = (node as IWebElement).Enabled; // if we can access the property, we consider the node "accessible"
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determine whether the node is accessible, i.e. its properties can be retrieved.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Whether the node is accessible or not.</returns>
        internal static bool DisplayedNoThrow(ISearchContext node)
        {
            try
            {
                return (node as IWebElement).Displayed; // if we can access the property, we consider the node "accessible"
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Find all controls matching the search pattern.
        /// </summary>
        /// <param name="parentHint">The parent or null if it should be searched.</param>
        /// <returns>Matching controls.</returns>
        protected virtual IEnumerable<ISearchContext> Matches(ISearchContext parentHint = null)
        {
            parentHint = parentHint ?? TryParent;
            if (parentHint == null)
            {
                return Enumerable.Empty<ISearchContext>();
            }
            else
            {
                return parentHint.FindElements(SearchPattern).Where(c => Predicate(c));
            }
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode() => mHash;

        /// <summary>
        /// Initialize this object.
        /// The parent node is used to search nodes without, hence disabling any caching.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="hash">The node hash.</param>
        /// <param name="enableCaching">Whether to enable caching.</param>
        /// <returns>This object.</returns>
        public IUIObjectNode Init(IUIObjectNode parent, int hash, bool enableCaching)
        {
            mHash = hash;
            mParent = parent;
            this.enableCaching = enableCaching;
            return this;
        }

        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="searchPattern">The search pattern used to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="autoSnap">Whether to snap automatically on the first access.</param>
        public void Init(By searchPattern, Predicate<ISearchContext> predicate = null, bool autoSnap = false)
        {
            this.searchPattern = searchPattern;
            this.predicate = predicate;
            this.autoSnap = autoSnap;
        }

        /// <summary>
        /// Snap the node object, i.e. associate it with a specific node in the UI tree. This disables lazy node evaluation.
        /// </summary>
        /// <returns>Whether the snap succeeded.</returns>
        public bool TrySnap() => (mNode = GetRoot(() => TryRootUncached)) != null;

        /// <summary>
        /// Unsnap the object.
        /// </summary>
        /// <returns>Whether the node was snapped before.</returns>
        public bool TryUnsnap()
        {
            var before = mNode;
            mNode = null;
            return before != mNode;
        }

        /// <summary>
        /// Find a control from the page object root node <see cref="RootSearchContext"/>.
        /// </summary>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="result">The control or null.</param>
        /// <returns>Whether the control was found.</returns>
        public bool TryFindElement(By pattern, out IWebElement result)
        {
            try
            {
                result = Root.FindElement(pattern);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <param name="findNode">The find node function.</param>
        /// <returns>The node control.</returns>
        private ISearchContext GetRoot(Func<ISearchContext> findNode)
        {
            bool cached = ((ITabObjectNodeInternal)RootNode).NodeLocator.TryGet(GetHashCode(), out ISearchContext node);
            bool accessible = cached && Accessible(node);
            bool visibleOnScreen = accessible && DisplayedNoThrow(node);

            var statistics = RootNode.Statistics;

            statistics.AccessCounter += 1;
            statistics.InCache += cached ? 1 : 0;
            statistics.AccessibleNodes += accessible ? 1 : 0;
            statistics.VisibleOnScreenNodes += visibleOnScreen ? 1 : 0;
            statistics.CachingEnabledCounter += enableCaching ? 1 : 0;

            if (!visibleOnScreen)
            {
                // find node
                node = findNode();

                // store node if caching is enabled
                // not storing any node effectively deactivates caching
                if (node != null && enableCaching)
                {
                    ((ITabObjectNodeInternal)RootNode).NodeLocator.Register(GetHashCode(), node);
                }
            }

            mNode = EnableAutoSnap ? node : mNode;

            return node;
        }

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        public void Clear() => Root.Clear();

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SendKeys(string text) => Root.SendKeys(text);

        /// <summary>
        /// Submits this element to the web server.
        /// </summary>
        public void Submit() => Root.Submit();

        /// <summary>
        /// Clicks this element.
        /// </summary>
        public void Click() => Root.Click();

        /// <summary>
        /// Gets the value of the specified attribute for this element.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The attribute's current value. Returns a null if the value is not set.</returns>
        public string GetAttribute(string attributeName) => Root.GetAttribute(attributeName);

        /// <summary>
        /// Gets the value of a JavaScript property of this element.
        /// </summary>
        /// <param name="propertyName">The name JavaScript the JavaScript property to get the value of.</param>
        /// <returns>The JavaScript property's current value. Returns a null if the value is not set or the property does not exist.</returns>
        public string GetProperty(string propertyName) => Root.GetProperty(propertyName);

        /// <summary>
        /// Gets the value of a CSS property of this element.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property to get the value of.</param>
        /// <returns>The value of the specified CSS property.</returns>
        public string GetCssValue(string propertyName) => Root.GetCssValue(propertyName);

        /// <summary>
        /// Finds the first OpenQA.Selenium.IWebElement using the given method.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching OpenQA.Selenium.IWebElement on the current context.</returns>
        public IWebElement FindElement(By by) => Root.FindElement(by);

        /// <summary>
        /// Finds all OpenQA.Selenium.IWebElement within the current context using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>A System.Collections.ObjectModel.ReadOnlyCollection`1 of all OpenQA.Selenium.IWebElement matching the current criteria, or an empty list if nothing matches.</returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by) => Root.FindElements(by);
    }
}