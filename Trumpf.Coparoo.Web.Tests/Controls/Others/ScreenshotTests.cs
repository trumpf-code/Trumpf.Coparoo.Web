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
    using NUnit.Framework;
    using System.IO;
    using Trumpf.Coparoo.Web.Controls;

    [TestFixture]
    public class ScreenshotTests : ControlTests
    {
        private const string ControlImage = "control.png";
        private const string BrowserImage = "browser.png";

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenScreenshotsAreTaken_ThenFilesAreCreated()
        {
            PrepareAndExecute<Tab>(nameof(WhenScreenshotsAreTaken_ThenFilesAreCreated), HtmlStart + $"<button type=\"button\">bu</button>" + HtmlEnd, tab =>
            {
                // Act
                tab.TakeScreenshot(BrowserImage);
                tab.Find<Button>().TakeScreenshot(ControlImage);

                // Check
                FileAssert.Exists(BrowserImage);
                FileAssert.Exists(ControlImage);
                FileAssert.AreNotEqual(BrowserImage, ControlImage);
                Assert.Greater(new FileInfo(BrowserImage).Length, new FileInfo(ControlImage).Length);
            });
        }
    }
}