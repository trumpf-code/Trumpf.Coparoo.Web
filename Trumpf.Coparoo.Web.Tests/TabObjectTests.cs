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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Exceptions;

    [TestClass]
    public class TabObjectTests : ControlTests
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenCastingTabObjects_ThenTheCorrectTabObjectsAreReturned()
        {
            PrepareAndExecute<Tab>(nameof(WhenCastingTabObjects_ThenTheCorrectTabObjectsAreReturned), HtmlStart + $"<button type=\"button\">text</button>" + HtmlEnd, tab =>
            {
                // Act
                var t1 = tab.Cast<T1>();
                var b1 = t1.On<B1>();
                var e1 = b1.Exists.TryWaitFor();

                var t2 = tab.Cast<T2>();
                var b2 = t2.On<B2>();
                var e2 = b2.Exists.TryWaitFor();

                // Check
                Assert.IsTrue(e1);
                Assert.IsTrue(e2);
            });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenCastingATabObjectAndThenBack_ThenTheCorrectTabObjectIsReturned()
        {
            PrepareAndExecute<Tab>(nameof(WhenCastingATabObjectAndThenBack_ThenTheCorrectTabObjectIsReturned), HtmlStart + $"<button type=\"button\">text</button>" + HtmlEnd, tab =>
            {
                // Prepare
                var t1 = tab.Cast<T1>();
                var t2 = t1.Cast<T2>(); // cast
                var t3 = t2.Cast<T1>(); // cast back

                // Act
                var b1 = t3.On<B1>();
                var e1 = b1.Exists.TryWaitFor();

                // Check
                Assert.IsTrue(e1);
            });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenCastingATabObject_ThenOnlyPageObjectInTheCastedPageObjectTreeCanBeAccessed()
        {
            // Prepare
            ITabObject tab = new Tab();

            // Act
            var t1 = tab.Cast<T1>();
            var t2 = tab.Cast<T2>();

            // Check
            Assert.ThrowsException<PageObjectNotFoundException<B2>>(() => t1.On<B2>());
            Assert.ThrowsException<PageObjectNotFoundException<B1>>(() => t2.On<B1>());
        }

        protected class T1 : Tab
        {
        }

        protected class T2 : Tab
        {
        }

        protected class B1 : PageObject, IChildOf<T1>
        {
            protected override By SearchPattern => By.TagName("button");
        }

        protected class B2 : PageObject, IChildOf<T2>
        {
            protected override By SearchPattern => By.TagName("button");
        }
    }
}
