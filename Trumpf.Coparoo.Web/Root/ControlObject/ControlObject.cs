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

namespace Trumpf.Coparoo.Web
{
    using System;

    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web.Internal;
    using Trumpf.Coparoo.Web.Logging.Tree;

    /// <summary>
    /// Control object base class.
    /// </summary>
    public abstract class ControlObject : UIObject, IControlObjectInternal
    {
        /// <summary>
        /// Gets the node type of this UI object.
        /// </summary>
        internal override NodeType NodeType => NodeType.ControlObject;

        /// <summary>
        /// Gets a fresh node object.
        /// </summary>
        internal override IUIObjectNode CreateNode => new UIObjectNode();

        /// <summary>
        /// Gets the search pattern used to locate the control object.
        /// </summary>
        protected abstract By SearchPattern { get; }

        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="pattern">The search pattern used to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="autoSnap">Whether to snap automatically on the first access.</param>
        void IControlObjectInternal.Init(By pattern, Predicate<ISearchContext> predicate, bool autoSnap) => Node.Init(pattern ?? SearchPattern, predicate, autoSnap);

        /// <summary>
        /// Cast the control object.
        /// </summary>
        /// <typeparam name="TControl">The type to cast to.</typeparam>
        /// <returns>The control object with the new type.</returns>
        public TControl Cast<TControl>() where TControl : IControlObject
        {
            var result = (TControl)Activator.CreateInstance(RootInternal.UIObjectInterfaceResolver.Resolve<TControl>());
            (result as IUIObjectInternal).Init(Parent, false);
            (result as IControlObjectInternal).Init(NodeInternal.SearchPattern, Node.Predicate, false);
            (result as IUIObjectInternal).Index = NodeInternal.Index;
            return result;
        }

        /// <summary>
        /// Click the control.
        /// </summary>
        public virtual void Click() => Node.Click();
    }
}