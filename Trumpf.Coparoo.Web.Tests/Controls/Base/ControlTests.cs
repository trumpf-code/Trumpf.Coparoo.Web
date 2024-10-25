﻿// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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

namespace Trumpf.Coparoo.Tests
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using System;
    using System.IO;
    using System.Linq;
    using Trumpf.Coparoo.Web;

    /// <summary>
    /// Test class.
    /// Certain csproj settings changed to work around the issue described in https://github.com/Fody/Costura/issues/235.
    /// </summary>
    public abstract class ControlTests
    {
        /// <summary>
        /// Write an HTML file, open it in the browser and execute an action on the browser tab.
        /// </summary>
        /// <param name="htmlfileContent">The content of the HTML file.</param>
        /// <param name="actAndCheck">The action to execute on the tab.</param>
        protected void PrepareAndExecute<T>(string fileBaseName, string htmlfileContent, Action<T> actAndCheck)
            where T : Tab, new()
        {
            string htmlFileName = null;
            T tab = null;
            try
            {
                // Prepare
                htmlFileName = Path.Combine(Path.GetTempPath(), fileBaseName + ".html");
                File.WriteAllText(htmlFileName, htmlfileContent);
                tab = tab = new T { File = "file:///" + htmlFileName }.Goto<T>();

                // Execute
                actAndCheck(tab);
            }
            finally
            {
                tab?.Quit();
                File.Delete(htmlFileName);
            }
        }

        /// <summary>
        /// The tab object.
        /// </summary>
        protected class Tab : TabObject
        {
            public string File { get; set; }
            protected override string Url => File;
            protected override Func<IWebDriver> Creator =>
                () =>
            {
                ChromeOptions options = new ChromeOptions();

                options.AddArgument("headless");
                options.AddArgument("no-sandbox");
                options.AddArgument("proxy-server='direct://'");
                options.AddArgument("proxy-bypass-list=*");

                var driver = new ChromeDriver(options);

                return driver;
            };
        }

        protected static Random random = new Random();
        protected static string Random => new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 10).Select(s => s[random.Next(s.Length)]).ToArray());
        protected const string HtmlStart = "<!DOCTYPE html><html><body>";
        protected const string HtmlEnd = "</body></html>";
    }
}
