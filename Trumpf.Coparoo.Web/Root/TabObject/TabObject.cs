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
    using Exceptions;
    using OpenQA.Selenium;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using Trumpf.Coparoo.Web.Internal;
    using Trumpf.Coparoo.Web.Logging.Tree;
    using Trumpf.Coparoo.Web.Waiting;

    /// <summary>
    /// Tab object base class.
    /// </summary>
    public abstract class TabObject : PageObject, ITabObjectInternal, ITabObject
    {
        private bool opened;
        private readonly PageObjectLocator pageObjectLocator;
        private readonly UIObjectInterfaceResolver objectInterfaceResolver;
        internal const string DEFAULT_FILE_PREFIX = "Coparoo";
        internal const string DEFAULT_DOT_PATH = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
        private readonly Dictionary<Type, HashSet<Type>> dyanamicParentToChildMap = new Dictionary<Type, HashSet<Type>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TabObject"/> class.
        /// </summary>
        public TabObject()
        {
            TabObjectNode n = new TabObjectNode { Creator = Creator };
            Init(null, n);
            pageObjectLocator = new PageObjectLocator(this);
            objectInterfaceResolver = new UIObjectInterfaceResolver(this);

            Configuration.PositiveWaitTimeout = TimeSpan.FromSeconds(0);
            Configuration.WaitTimeout = TimeSpan.FromMinutes(1);
            Configuration.ShowWaitingDialog = true;
        }

        /// <summary>
        /// Gets the node type of this UI object.
        /// </summary>
        internal override NodeType NodeType => NodeType.RootObject;

        /// <summary>
        /// Gets a fresh node object.
        /// </summary>
        internal override IUIObjectNode CreateNode => new TabObjectNode();

        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => throw new NotImplementedException();

        /// <summary>
        /// Gets or sets the (static) TestExecute driver.
        /// </summary>
        public IWebDriver Driver
        {
            get { return ((TabObjectNode)Node).Driver; }
            set { ((TabObjectNode)Node).Driver = value; }
        }

        /// <summary>
        /// Gets the node locator.
        /// </summary>
        INodeLocator ITabObjectInternal.NodeLocator => ((ITabObjectNodeInternal)Node).NodeLocator;

        /// <summary>
        /// Gets the page object locator.
        /// </summary>
        IPageObjectLocator ITabObjectInternal.PageObjectLocator => pageObjectLocator;

        /// <summary>
        /// Gets the UI object interface resolver.
        /// </summary>
        IUIObjectInterfaceResolver ITabObjectInternal.UIObjectInterfaceResolver => objectInterfaceResolver;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Configuration Configuration => ((TabObjectNode)Node).Configuration;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Statistics Statistics => ((TabObjectNode)Node).Statistics;

        /// <summary>
        /// Gets the URL.
        /// </summary>
        protected abstract string Url { get; }

        /// <summary>
        /// Gets the driver creator.
        /// </summary>
        protected abstract Func<IWebDriver> Creator { get; }

        /// <summary>
        /// Gets a value indicating whether the web page is visible on screen.
        /// </summary>
        protected override bool IsDisplayed => opened;

        /// <summary>
        /// Resolve the root object interface to a loaded process object in the current app domain.
        /// </summary>
        /// <typeparam name="TRootObject">The interface to resolve.</typeparam>
        /// <returns>The root object.</returns>
        public static TRootObject Resolve<TRootObject>() where TRootObject : ITabObject
        {
            var matches = PageTests.Locate.Types.Where(e => !e.IsAbstract && e.GetConstructor(Type.EmptyTypes) != null && typeof(TRootObject).IsAssignableFrom(e)).ToArray();
            if (!matches.Any())
            {
                throw new TabObjectNotFoundException<TRootObject>();
            }
            else
            {
                var type = matches.First();
                return (TRootObject)Activator.CreateInstance(type);
            }
        }

        /// <summary>
        /// Register the page object type with the given object.
        /// </summary>
        /// <typeparam name="TChildPageObject">The child page object type.</typeparam>
        /// <typeparam name="TParentPageObject">The parent page object type.</typeparam>
        /// <returns>Whether the value was not yet registered.</returns>
        public bool ChildOf<TChildPageObject, TParentPageObject>()
        {
            if (typeof(TChildPageObject).IsAbstract || typeof(TChildPageObject).IsInterface)
            {
                throw new InvalidOperationException($"{typeof(TChildPageObject).FullName} must not be abstract nor an interface.");
            }

            if (typeof(TParentPageObject).IsAbstract || typeof(TParentPageObject).IsInterface)
            {
                throw new InvalidOperationException($"{typeof(TParentPageObject).FullName} must not be abstract nor an interface.");
            }

            if (dyanamicParentToChildMap.TryGetValue(typeof(TParentPageObject), out HashSet<Type> o))
            {
                return o.Add(typeof(TChildPageObject));
            }
            else
            {
                dyanamicParentToChildMap.Add(typeof(TParentPageObject), new HashSet<Type>() { typeof(TChildPageObject) });
                return true;
            }
        }

        /// <summary>
        /// Cast the tab object.
        /// </summary>
        /// <typeparam name="TTab">The type to cast to.</typeparam>
        /// <returns>The tab object with the new type.</returns>
        public TTab Cast<TTab>() where TTab : ITabObject
        {
            var result = Resolve<TTab>();
            result.Driver = Driver;
            return result;
        }

        /// <summary>
        /// Scroll to the object.
        /// Not supported for tab objects.
        /// </summary>
        public override void ScrollTo()
            => throw new InvalidOperationException($"Tab objects do not support the 'ScrollTo' operation.");

        /// <summary>
        /// Gets the location.
        /// Not supported for tab objects.
        /// </summary>
        public override Point Location
            => throw new InvalidOperationException($"Tab objects do not support the 'Location' operation.");

        /// <summary>
        /// Gets or sets a value indicating whether the UI object can respond to user interaction.
        /// Not supported for tab objects.
        /// </summary>
        public override Bool Enabled
            => throw new InvalidOperationException($"Tab objects do not support the 'Enabled' operation.");

        /// <summary>
        /// Gets the dynamically registered children of a page object.
        /// </summary>
        /// <param name="pageObjectType">The parent page object type.</param>
        /// <returns>The registered children types.</returns>
        IEnumerable<Type> ITabObjectInternal.DynamicChildren(Type pageObjectType) => dyanamicParentToChildMap.TryGetValue(pageObjectType, out HashSet<Type> value) ? value : new HashSet<Type>();

        /// <summary>
        /// Write the page object tree in the PDF format.
        /// </summary>
        /// <param name="filename">The filename to write to with extension.</param>
        /// <param name="dotBinaryPath">The hint path to the dot.exe binary file.</param>
        /// <returns>The absolute file name that was written.</returns>
        public string WriteTree(string filename = DEFAULT_FILE_PREFIX, string dotBinaryPath = DEFAULT_DOT_PATH) => ((ITreeObject)this).Tree.WriteGraph(filename, dotBinaryPath);

        /// <summary>
        /// Opens the tab if this has not yet been done.
        /// </summary>
        public override void Goto()
        {
            if (!opened)
            {
                Open();
            }
        }

        /// <summary>
        /// Open the web page in a new tab (will open a new browser).
        /// </summary>
        public void Open()
        {
            ((TabObjectNode)Node).Open(Url);
            opened = true;
        }

        /// <summary>
        /// Close the tab.
        /// </summary>
        public void Close()
        {
            Driver.Close();
            opened = false;
        }

        /// <summary>
        /// Close every associated window.
        /// </summary>
        public void Quit()
        {
            Driver.Quit();
            opened = false;
        }

        /// <summary>
        /// Gets or sets the URL of the tab.
        /// </summary>
        public string CurrentURL
        {
            get { return Driver.Url; }
            set { Driver.Url = value; }
        }

        /// <summary>
        /// Gets an await-object.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The display name used in timeout exceptions.</param>
        /// <returns>The wrapped object.</returns>
        IAwait<T> ITabObjectInternal.Await<T>(Func<T> function, string name) => new Await<T>(function, name, GetType(), () => Configuration.WaitTimeout, () => Configuration.PositiveWaitTimeout, () => Configuration.ShowWaitingDialog);

        /// <summary>
        /// Take a screen shot.
        /// </summary>
        /// <param name="path">Write screenshot to this PNG file name (pass with extension). null = just return the bitmap.</param>
        /// <returns>The screen shot.</returns>
        public Bitmap TakeScreenshot(string path = null)
        {
            if (Root.Driver is ITakesScreenshot)
            {
                IWebDriver driver = Root.Driver;
                byte[] byteArray = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
                Bitmap screenshot = new Bitmap(new System.IO.MemoryStream(byteArray));

                if (path != null)
                    screenshot.Save(path, ImageFormat.Png);

                return screenshot;
            }
            else
            {
                throw new InvalidOperationException($"Driver '{Root.Driver.GetType().FullName}' does not implement '{nameof(ITakesScreenshot)}'.");
            }
        }
    }
}
