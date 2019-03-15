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

    /// <summary>
    /// Root page object node class.
    /// </summary>
    internal class TabObjectNode : UIObjectNode, ITabObjectNode, ITabObjectNodeInternal
    {
        private readonly NodeLocator nodeLocator = new NodeLocator();
        internal IWebDriver webDriver;

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// It's the same as the root process.
        /// </summary>
        public override ISearchContext RootSearchContext => TryRootSearchContext;

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found
        /// It's the same as the root process.
        /// </summary>
        public override ISearchContext TryRootSearchContext => Driver;

        /// <summary>
        /// Gets the root node.
        /// </summary>
        protected override ISearchContext Parent => throw new InvalidOperationException("A root has no parent");

        /// <summary>
        /// Gets the root node if possible, and otherwise return null.
        /// </summary>
        protected override ISearchContext TryParent => throw new InvalidOperationException("A root has no parent");

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        public override By SearchPattern => throw new NotImplementedException();

        /// <summary>
        /// Gets the driver generator.
        /// </summary>
        public Func<IWebDriver> Creator { get; set; }

        /// <summary>
        /// Gets the node locator.
        /// </summary>
        INodeLocator ITabObjectNodeInternal.NodeLocator => nodeLocator;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Configuration Configuration { get; } = new Configuration();

        /// <summary>
        /// Gets the statistics.
        /// </summary>
        public Statistics Statistics { get; } = new Statistics();

        /// <summary>
        /// Gets or sets the driver.
        /// </summary>
        public IWebDriver Driver
        {
            get
            {
                return webDriver ?? (webDriver = Creator());
            }
            set
            {
                webDriver = value;
            }
        }

        /// <summary>
        /// Open the web page.
        /// </summary>
        /// <param name="url">The url to open.</param>
        public void Open(string url) => Driver.Url = url;
    }
}