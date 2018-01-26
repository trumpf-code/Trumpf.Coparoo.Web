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
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;

    /// <summary>
    /// Test class.
    /// </summary>
    [TestClass]
    public class ExplicitOnCondition
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenMultiplePathsToAPageObjectExistInThePageObjectTree_ThenTheExplicitOnConditionIsEffective()
        {
            SetAndCheck(typeof(A));
            SetAndCheck(typeof(B));
        }

        /// <summary>
        /// Set the expected type and check the ON-method.
        /// </summary>
        /// <param name="expectedParentType">The expected parent type.</param>
        private void SetAndCheck(Type expectedParentType)
        {
            // Act
            C.T = expectedParentType;
            var visibleOnScreen = new A().On<C>(e => e.Displayed);

            // Check
            Assert.AreEqual(expectedParentType, visibleOnScreen.Parent.GetType());
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class A : TabObject
        {
            protected override string Url => null;
            protected override Func<IWebDriver> Creator => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class B : PageObject, IChildOf<A>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class C : PageObject, IChildOf<A>, IChildOf<B>
        {
            /// <summary>
            /// Gets or sets the expected type.
            /// </summary>
            public static Type T { get; set; }

            /// <summary>
            /// Gets a value indicating whether the expected type is the parent.
            /// </summary>
            protected override bool IsDisplayed => Parent.GetType().Equals(T);

            /// <summary>
            /// Gets the search pattern.
            /// </summary>
            protected override By SearchPattern => null;
        }
    }
}