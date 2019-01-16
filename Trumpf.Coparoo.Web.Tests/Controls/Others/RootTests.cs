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

namespace Trumpf.Coparoo.Tests
{
    using NUnit.Framework;
    using OpenQA.Selenium;
    using System;

    [TestFixture]
    public class RootTests : ControlTests
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenButtonsAreSearchedFromTheRootsNode_ThenOneButtonIsReturned()
        {
            PrepareAndExecute<TabHelper>(nameof(WhenButtonsAreSearchedFromTheRootsNode_ThenOneButtonIsReturned), HtmlStart + $"<button type=\"button\">bu</button>" + HtmlEnd, tab =>
            {
                // Act
                var buttonCount = tab.CountButtons;

                // Check
                Assert.AreEqual(1, buttonCount);
            });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenCallingIWebElementOperationOnATabNode_ThenAnInvalidOperationExceptionIsThrown()
        {
            PrepareAndExecute<TabHelper>(nameof(WhenCallingIWebElementOperationOnATabNode_ThenAnInvalidOperationExceptionIsThrown), HtmlStart + $"<button type=\"button\">bu</button>" + HtmlEnd, tab =>
            {
                Assert.Throws<InvalidOperationException>(() => tab.Click());
                Assert.Throws<InvalidOperationException>(() => tab.SomeProperty());
                Assert.Throws<InvalidOperationException>(() => tab.ScrollTo());
                Assert.Throws<InvalidOperationException>(() => { var location = tab.Location; });
                Assert.Throws<InvalidOperationException>(() => { var enabled = tab.Enabled; });
            });
        }

        protected class TabHelper : Tab
        {
            public int CountButtons
                => Node.FindElements(By.TagName("button")).Count;

            public void Click()
                => Node.Click();

            public string SomeProperty()
                => Node.GetProperty("asdksa");

            public bool IsDisplayedProperty()
                => base.IsDisplayed;
        }
    }
}
