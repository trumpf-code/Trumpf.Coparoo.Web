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
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Exceptions;
    using Trumpf.Coparoo.Web.PageTests;

    /// <summary>
    /// Test class.
    /// </summary>
    [TestClass]
    [DoNotParallelize] // due to ugly static counter
    public class Runner
    {
        private A Root
        {
            get
            {
                var result = new A();
                result.Configuration.DependencyRegistrator
                    .Register<IDontKnow, DontKnow>();

                return result;
            }
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenATestsClassHasUnregisteredTypes_ThenAnExceptionIsThrown()
            => Assert.ThrowsException<TypeResolutionFailedException>(() => new A().On<B>().Test());

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenThePageTestsOfBAreExecuted_ThenTwoTestsExecuteInTotal()
        {
            // Prepare
            new BT(new DontKnow()).Reset();


            Root.On<B>().Test();
            var testsExecuted = new BT(new DontKnow()).Counter;

            // Check
            Assert.AreEqual(2, testsExecuted);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenThePageTestsOfBAreExecuted_ThenOneTestExecutesInTotal()
        {
            // Prepare
            new CT().Reset();

            // Act
            Root.On<C>().Test();

            // Check
            Assert.AreEqual(1, new CT().Counter);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTheBottomUpTestIsCalledOnTheRoot_ThenThreeTestsAreExecuted()
        {
            // Prepare
            new BT(new DontKnow()).Reset();
            new CT().Reset();

            // Act
            Root.TestBottomUp();
            var testsOfB = new BT(new DontKnow()).Counter;
            var testsOfC = new CT().Counter;

            // Check
            Assert.AreEqual(2, testsOfB);
            Assert.AreEqual(1, testsOfC);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTheDotTreeIsWrittenToDisk_ThenAFileIsCreated()
        {
            var (prefix, dotFile, pdfFile) = TreeWriterHelper.GenerateFileNames();
            var dotExists = false;
            var pdfExists = false;
            try
            {
                // Act
                var root = Root;
                root.TestBottomUp(true, treeBaseFilename: prefix);
                dotExists = File.Exists(dotFile);
                pdfExists = File.Exists(pdfFile);

                // Check
                Assert.IsTrue(dotExists || pdfExists);
            }
            finally
            {
                // Clean up
                if (dotExists)
                {
                    File.Delete(dotFile);
                }

                if (pdfExists)
                {
                    File.Delete(pdfFile);
                }
            }
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

        /// <summary>
        /// Helper interface.
        /// </summary>
        private interface IDontKnow
        {
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class DontKnow : IDontKnow
        {
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class BT : MyPageObjectTests<B>
        {
            private readonly IDontKnow dontKnow;

            public BT(IDontKnow dontKnow)
                => this.dontKnow = dontKnow;

            /// <summary>
            /// Test method.
            /// </summary>
            [PageTest]
            public void B1()
            {
                Assert.IsNotNull(dontKnow);
                Assert.AreEqual(typeof(DontKnow), dontKnow.GetType());
                Increment();
            }

            /// <summary>
            /// Test method.
            /// </summary>
            [PageTest]
            public void B2()
            {
                Increment();
            }
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class CT : MyPageObjectTests<C>
        {
            /// <summary>
            /// Test method.
            /// </summary>
            [PageTest]
            public void C1()
            {
                Increment();
            }
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private abstract class MyPageObjectTests<T> : PageObjectTests<T> where T : IPageObject
        {
            private static int counter = 0;

            /// <summary>
            /// Gets the counter.
            /// </summary>
            public int Counter
            {
                get { return counter; }
            }

            /// <summary>
            /// Gets a value indicating whether tests are ready to run.
            /// </summary>
            public override bool ReadyToRun
            {
                get { return true; }
            }

            /// <summary>
            /// Increment the counter.
            /// </summary>
            protected void Increment()
            {
                counter++;
            }

            /// <summary>
            /// Reset counter.
            /// </summary>
            public void Reset()
            {
                counter = 0;
            }

            /// <summary>
            /// Do nothing before tests.
            /// </summary>
            public override void BeforeFirstTest()
            {
            }
        }
    }
}