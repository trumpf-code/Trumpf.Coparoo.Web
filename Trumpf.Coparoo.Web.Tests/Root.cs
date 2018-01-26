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

#if DEBUG
namespace Trumpf.Coparoo.Tests
{
    using System;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;

    /// <summary>
    /// Test class.
    /// </summary>
    [TestFixture]
    public class Root
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenRootIsCalledOnRoot_ThenTheRootIsReturned()
        {
            // Act
            var root = new A();
            var rootOfRoot = (root as IUIObjectInternal).Root;

            // Check
            Assert.AreEqual(root, rootOfRoot);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenWeGetTheRootOfTheChildOfTheRoot_ThenTheRootIsReturned()
        {
            // Act
            var root = new A();
            var child = root.On<B>();
            var rootOfTheRoot = (root as IUIObjectInternal).Root;
            var rootOfTheChild = (child as IUIObjectInternal).Root;

            // Check
            Assert.AreEqual(rootOfTheRoot, rootOfTheChild);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenWeGetTheRootOfTheChildOfChildOfTheRoot_ThenTheRootIsReturned()
        {
            // Act
            var root = new A();
            var grandchild = root.On<C>();
            var rootOfTheRoot = (root as IUIObjectInternal).Root;
            var rootOfTheGrandchild = (grandchild as IUIObjectInternal).Root;

            // Check
            Assert.AreEqual(rootOfTheRoot, rootOfTheGrandchild);
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
        private class C : PageObject, IChildOf<B>
        {
            protected override By SearchPattern => null;
        }
    }
}
#endif