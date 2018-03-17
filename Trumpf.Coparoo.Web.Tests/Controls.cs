// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Controls;

    /// <summary>
    /// Test class.
    /// Certain project changed to work around https://github.com/Fody/Costura/issues/235.
    /// </summary>
    [TestFixture]
    public class ControlObjects
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenAButtonIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
        {
            var text = Random;
            PrepareAndExecute(nameof(WhenAButtonIsAccessed_ThenItCanBeFoundAndThePropertiesFit), HtmlStart + $"<button type=\"button\">{text}</button>" + HtmlEnd, tab =>
            {
                // Act
                var button = tab.Find<Button>();
                var exists = button.Exists.TryWaitFor();
                var buttonText = button.Text;

                // Check
                Assert.True(exists);
                Assert.AreEqual(text, buttonText);
            });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenALinkIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
        {
            var text = Random;
            var url = $"http://{Random}/";
            PrepareAndExecute(nameof(WhenALinkIsAccessed_ThenItCanBeFoundAndThePropertiesFit), HtmlStart + $"<a href=\"{url}\">{text}</a>" + HtmlEnd, tab =>
            {
                // Act
                var link = tab.Find<Link>();
                var exists = link.Exists.TryWaitFor();
                var linkText = link.Text;
                var linkUrl = link.URL;

                // Check
                Assert.True(exists);
                Assert.AreEqual(text, linkText);
                Assert.AreEqual(url, linkUrl);
            });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenATableIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
        {
            PrepareAndExecute(nameof(WhenATableIsAccessed_ThenItCanBeFoundAndThePropertiesFit), HtmlStart + "<table><thead><tr><th>1</th><th>2</th></tr></thead><tfoot><tr><td>3</td><td>4</td></tr></tfoot><tbody><tr><td>5</td><td>6</td></tr><tr><td>7</td><td>8</td></tr></tbody></table>" + HtmlEnd, tab =>
            {
                // Act
                var table = tab.Find<Table>();
                var exists = table.Exists.TryWaitFor();
                var headerRows = table.Header.Rows.Count();
                var contentRows = table.Content.Rows.Count();
                var footerRows = table.Footer.Rows.Count(); ;
                var allRows = table.AllRows.Count();

                // Check
                Assert.True(exists);
                Assert.AreEqual(1, headerRows);
                Assert.AreEqual(2, contentRows);
                Assert.AreEqual(1, footerRows);
                Assert.AreEqual(4, allRows);
            });
        }

        #region Helpers
        /// <summary>
        /// Write an HTML file, open it in the browser and execute an action on the browser tab.
        /// </summary>
        /// <param name="htmlfileContent">The content of the HTML file.</param>
        /// <param name="actAndCheck">The action to execute on the tab.</param>
        private static void PrepareAndExecute(string fileBaseName, string htmlfileContent, Action<Tab> actAndCheck)
        {
            string htmlFileName = null;
            Tab tab = null;
            try
            {
                // Prepare
                htmlFileName = Path.Combine(Path.GetTempPath(), fileBaseName + ".html");
                File.WriteAllText(htmlFileName, htmlfileContent);
                tab = tab = new Tab("file:///" + htmlFileName).Goto<Tab>();

                // Execute
                actAndCheck(tab);
            }
            finally
            {
                tab.Quit();
                File.Delete(htmlFileName);
            }
        }

        /// <summary>
        /// The tab object.
        /// </summary>
        private class Tab : TabObject
        {
            private string file;
            public Tab(string file) => this.file = file;
            protected override string Url => file;
            protected override Func<IWebDriver> Creator => () => new FirefoxDriver();
        }

        private static Random random = new Random();
        private static string Random => new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 10).Select(s => s[random.Next(s.Length)]).ToArray());
        private const string HtmlStart = "<!DOCTYPE html><html><body>";
        private const string HtmlEnd = "</body></html>";
        #endregion
    }
}
