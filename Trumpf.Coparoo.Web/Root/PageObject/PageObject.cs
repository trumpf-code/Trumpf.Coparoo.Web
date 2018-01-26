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
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web.Internal;
    using Trumpf.Coparoo.Web.Logging.Tree;

    /// <summary>
    /// Page object base class.
    /// </summary>
    public abstract class PageObject : TreeObject
    {
        /// <summary>
        /// Gets the node type of this UI object.
        /// </summary>
        internal override NodeType NodeType => NodeType.PageObject;

        /// <summary>
        /// Gets a fresh node object.
        /// </summary>
        internal override IUIObjectNode CreateNode => new UIObjectNode();

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        protected abstract By SearchPattern { get; }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="enableCaching">Whether to enable caching.</param>
        /// <returns>The initialized page object.</returns>
        internal override IUIObject Init(IUIObject parent, bool enableCaching)
        {
            base.Init(parent, enableCaching);
            ((UIObjectNode)Node).Init(SearchPattern);

            return this;
        }
    }
}