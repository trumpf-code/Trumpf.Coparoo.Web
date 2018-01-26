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
    public class Interfaces
    {
        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IA : ITabObject
        {
        }

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IB : IPageObject
        {
        }

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IC : IPageObject
        {
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTheResolveMethodIsCalledWithAnInterface_ThenTheCorrectRootObjectIsReturned()
        {
            // Act
            var rootObject = TabObject.Resolve<IA>();

            // Check
            Assert.AreEqual(typeof(A), rootObject.GetType());
            Assert.IsTrue(rootObject is IA);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTheOnMethodIsCalledWithInterfaces_ThenThesInterfacesAreResolveToToCorrectType()
        {
            // Act
            var rootObject = TabObject.Resolve<IA>();
            var ia = rootObject.On<IA>();
            var ib = ia.On<IB>();
            var ic = ib.On<IC>();

            // Check
            Assert.IsTrue(ia is IA); // check IA from root
            Assert.AreEqual(typeof(A), ia.GetType());
            Assert.IsTrue(ib is IB); // check IB from IA
            Assert.AreEqual(typeof(B), ib.GetType());
            Assert.IsTrue(ic is IC); // check IC from IB
            Assert.AreEqual(typeof(C), ic.GetType());
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class A : TabObject, IA
        {
            protected override string Url => null;
            protected override Func<IWebDriver> Creator => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class B : PageObject, IB, IChildOf<IA>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class C : PageObject, IC, IChildOf<IB>
        {
            protected override By SearchPattern => null;
        }
    }
}