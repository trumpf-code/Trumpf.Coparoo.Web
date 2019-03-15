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
        /// Helper interface.
        /// </summary>
        private interface ID : IControlObject
        {
        }

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IE : ID
        {
        }

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IF : IControlObject
        {
        }

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IG<I> : IControlObject
            where I : IControlObject
        {
            I Item { get; }
        }

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IH<I> : IControlObject
            where I : IControlObject
        {
            I Item { get; }
        }

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IH<I, I2> : IControlObject
            where I : IControlObject
            where I2 : IControlObject
        {
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenAGenericInterfaceIsSearchedFor_ThenToCorrenspondingGenericImplementationIsResolved()
        {
            // Act
            var rootObject = TabObject.Resolve<IA>();
            IG<IF> control = rootObject.Find<IG<IF>>();
            IF item = control.Item;

            // Check
            Assert.AreEqual(typeof(G<IF>), control.GetType());
            Assert.AreEqual(typeof(F), control.Item.GetType());
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
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenMupltipleControlObjectInterfacesMatch_ThenTheClosestMatchIsReturned()
        {
            // Act
            var rootObject = TabObject.Resolve<IA>();

            var d = rootObject.Find<ID>(); // IE and IF also implement ID, yet we want to get "the closest" implementation, hence D
            var e = rootObject.Find<IE>();
            var f = rootObject.Find<IF>();

            // Check
            Assert.AreEqual(typeof(D), d.GetType());
            Assert.AreEqual(typeof(E), e.GetType());
            Assert.AreEqual(typeof(F), f.GetType());
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

        /// <summary>
        /// Helper class.
        /// </summary>
        private class D : ControlObject, ID
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class E : D, IE
        {
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class F : ControlObject, IF
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class G<I> : ControlObject, IG<I>
            where I : IControlObject
        {
            public I Item
                => Find<I>();

            protected override By SearchPattern => null;

            internal void Init(F f)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class H<I> : ControlObject, IH<I>
            where I : IControlObject
        {
            public I Item
                => Find<I>();

            protected override By SearchPattern => null;

            internal void Init(F f)
            {
                throw new NotImplementedException();
            }
        }
    }
}