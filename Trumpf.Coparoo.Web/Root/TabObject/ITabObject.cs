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

    /// <summary>
    /// Tab object interface.
    /// </summary>
    public interface ITabObject : IPageObject
    {
        /// <summary>
        /// Gets or sets the driver.
        /// </summary>
        IWebDriver Driver { get; set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        Configuration Configuration { get; }

        /// <summary>
        /// Gets the statistics.
        /// </summary>
        Statistics Statistics { get; }

        /// <summary>
        /// Write the page object tree in the PDF format.
        /// </summary>
        /// <param name="filename">The filename to write to with extension.</param>
        /// <param name="dotBinaryPath">The hint path to the dot.exe binary file.</param>
        /// <returns>The absolute file name that was written.</returns>
        string WriteTree(string filename = TabObject.DEFAULT_FILE_PREFIX, string dotBinaryPath = TabObject.DEFAULT_DOT_PATH);

        /// <summary>
        /// Register the page object type with the given object.
        /// </summary>
        /// <typeparam name="TParentPageObject">The parent page object type.</typeparam>
        /// <typeparam name="TChildPageObject">The child page object type.</typeparam>
        /// <returns>Whether the value not yet registered.</returns>
        bool ChildOf<TParentPageObject, TChildPageObject>();

        /// <summary>
        /// Open the web page in a new tab (the browser must already run).
        /// </summary>
        void Open();

        /// <summary>
        /// Close the tab.
        /// </summary>
        void Close();

        /// <summary>
        /// Close every associated window.
        /// </summary>
        void Quit();

        /// <summary>
        /// Get the current page URL.
        /// </summary>
        /// <returns>Current page URL</returns>
        string GetCurrentURL();
    }
}