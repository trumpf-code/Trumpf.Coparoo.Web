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
    using NUnit.Framework;
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Logging.Tree;
    using Trumpf.Coparoo.Web.PageTests;

    /// <summary>
    /// Test class.
    /// </summary>
    [TestFixture]
    public class Runner
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenThePageTestsOfBAreExecuted_ThenTwoTestsExecuteInTotal()
        {
            // Prepare
            new BT().Reset();

            // Act
            new A().On<B>().Test();
            var testsExecuted = new BT().Counter;

            // Check
            Assert.AreEqual(2, testsExecuted);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenThePageTestsOfBAreExecuted_ThenOneTestExecutesInTotal()
        {
            // Prepare
            new CT().Reset();

            // Act
            new A().On<C>().Test();
            var testsExecuted = new CT().Counter;

            Assert.AreEqual(1, new CT().Counter);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheBottomUpTestIsCalledOnTheRoot_ThenThreeTestsAreExecuted()
        {
            // Prepare
            new BT().Reset();
            new CT().Reset();

            // Act
            new A().TestBottomUp();
            var testsOfB = new BT().Counter;
            var testsOfC = new CT().Counter;

            // Check
            Assert.AreEqual(2, testsOfB);
            Assert.AreEqual(1, testsOfC);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheDotTreeIsWrittenToDisk_ThenAFileIsCreated()
        {
            A root;
            var dotFile = $"{TabObject.DEFAULT_FILE_PREFIX}.dot";
            var pdfFile = $"{TabObject.DEFAULT_FILE_PREFIX}.pdf";
            var fullDotPath = Path.GetFullPath(dotFile);
            var fullPdfPath = Path.GetFullPath(pdfFile);
            var dotExists = false;
            var pdfExists = false;
            try
            {
                // Act
                root = new A();
                root.TestBottomUp(true);
                dotExists = File.Exists(fullDotPath);
                pdfExists = File.Exists(fullPdfPath);

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
        /// Test method.
        ///
        /// TODO: reactivate.
        /// Failed : Trumpf.Coparoo.Tests.Runner.WhenTheDotTreeIsGenerated_ThenItContainsTheTestResults
        /// Expected: True
        /// But was:  False
        /// at Trumpf.Coparoo.Tests.Runner.WhenTheDotTreeIsGenerated_ThenItContainsTheTestResults()[0x0003c] in <6f1f588e7ba94c55a681f6266ea2076f>:0 
        /// Not reproducible locally.
        /// </summary>
        /// [Test]
        public void WhenTheDotTreeIsGenerated_ThenItContainsTheTestResults()
        {
            string file = null;
            A root;
            Tree tree;
            try
            {
                // Act
                root = new A();
                tree = root.TestBottomUpInternal();
                file = tree.WriteGraph();

                // Check
                Assert.AreEqual(8, tree.NodeCount);
                Assert.IsTrue(File.Exists(file));
            }
            finally
            {
                // Clean up
                if (file != null && File.Exists(file))
                {
                    File.Delete(file);
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
        /// Helper class.
        /// </summary>
        private class BT : MyPageObjectTests<B>
        {
            /// <summary>
            /// Test method.
            /// </summary>
            [PageTest]
            public void B1()
            {
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