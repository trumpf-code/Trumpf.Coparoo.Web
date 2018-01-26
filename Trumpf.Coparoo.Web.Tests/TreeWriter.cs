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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.PageTests;

    /// <summary>
    /// Test class.
    /// </summary>
    [TestClass]
    public class TreeWriter
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTheDotTreeIsGenerated_ThenItContainsTheControlProperties()
        {
            // Data
            var dotFile = $"{TabObject.DEFAULT_FILE_PREFIX}.dot";
            var pdfFile = $"{TabObject.DEFAULT_FILE_PREFIX}.pdf";
            var fullDotPath = Path.GetFullPath(dotFile);
            var fullPdfPath = Path.GetFullPath(pdfFile);
            var dotExists = false;
            var pdfExists = false;

            // Test
            try
            {
                // Act
                var root = new WithoutTests.MyTab();
                var tree = root.TestBottomUpInternal(null, true); // don't execute tests, write graph

                // Check
                dotExists = File.Exists(dotFile);
                pdfExists = File.Exists(pdfFile);
                Assert.AreEqual(5, tree.NodeCount);
                Assert.AreEqual(8, tree.EdgeCount);
                Assert.IsTrue(dotExists || pdfExists);
            }
            finally
            {
                // Clean up
                if (pdfExists)
                {
                    File.Delete(pdfFile);
                }

                if (dotExists)
                {
                    File.Delete(dotFile);
                }
            }
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTestAreExecutedAllSucceedAndTheDotTreeIsGenerated_ThenItContainsTheControlPropertiesAndTestRestults()
        {
            // Environment
            WithTests.LoginTests.Slow = false;
            WithTests.LoginTests.Fail = false;
            WithTests.MenuTests.Slow = false;

            // Data
            var dotFile = $"{TabObject.DEFAULT_FILE_PREFIX}.dot";
            var pdfFile = $"{TabObject.DEFAULT_FILE_PREFIX}.pdf";
            var fullDotPath = Path.GetFullPath(dotFile);
            var fullPdfPath = Path.GetFullPath(pdfFile);
            var dotExists = false;
            var pdfExists = false;

            var fromsExpected = new int[] { 2, 2, 3, 4, 4 };
            var tosExpected = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 4 };

            // Test
            try
            {
                // Act
                var root = new WithTests.MyTab();
                var tree = root.TestBottomUpInternal(null, true);

                // Check
                dotExists = File.Exists(dotFile);
                pdfExists = File.Exists(pdfFile);

                var fromsActual = tree.Edges.GroupBy(e => e.From).Select(e => e.Count()).OrderBy(e => e);
                var tosActual = tree.Edges.GroupBy(e => e.To).Select(e => e.Count()).OrderBy(e => e);

                Assert.IsTrue(fromsActual.SequenceEqual(fromsExpected));
                Assert.IsTrue(tosActual.SequenceEqual(tosExpected));
                Assert.AreEqual(15, tree.EdgeCount);
                Assert.IsTrue(dotExists || pdfExists);
            }
            finally
            {
                // Clean up
                if (pdfExists)
                {
                    File.Delete(pdfFile);
                }

                if (dotExists)
                {
                    File.Delete(dotFile);
                }
            }
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTestOfOnePageObjectAreExecutedAllSucceedAndTheDotTreeIsGenerated_ThenItContainsTheControlPropertiesAndTestRestults()
        {
            // Environment
            WithTests.MenuTests.Slow = false;

            // Data
            var dotFile = $"{TabObject.DEFAULT_FILE_PREFIX}.dot";
            var pdfFile = $"{TabObject.DEFAULT_FILE_PREFIX}.pdf";
            var fullDotPath = Path.GetFullPath(dotFile);
            var fullPdfPath = Path.GetFullPath(pdfFile);
            var dotExists = false;
            var pdfExists = false;

            // Test
            try
            {
                // Act
                var root = new WithTests.MyTab();
                var tree = root.On<WithTests.Menu>().TestInternal(true);

                // Check
                dotExists = File.Exists(dotFile);
                pdfExists = File.Exists(pdfFile);
                Assert.AreEqual(8, tree.NodeCount);
                Assert.AreEqual(11, tree.EdgeCount);
                Assert.IsTrue(dotExists || pdfExists);
            }
            finally
            {
                // Clean up
                if (pdfExists)
                {
                    File.Delete(pdfFile);
                }

                if (dotExists)
                {
                    File.Delete(dotFile);
                }
            }
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void WhenTestAreExecutedAndOneFailsAndTheDotTreeIsGenerated_ThenItContainsTheControlPropertiesAndTestRestults()
        {
            // Environment
            var slow = false;
            var fail = true;
            WithTests.LoginTests.Fail = fail;
            WithTests.LoginTests.Slow = slow;
            WithTests.MenuTests.Slow = slow;

            // Data
            var dotFile = $"{TabObject.DEFAULT_FILE_PREFIX}.dot";
            var pdfFile = $"{TabObject.DEFAULT_FILE_PREFIX}.pdf";
            var fullDotPath = Path.GetFullPath(dotFile);
            var fullPdfPath = Path.GetFullPath(pdfFile);
            var dotExists = false;
            var pdfExists = false;

            // Test
            try
            {
                // Act
                new WithTests.MyTab().TestBottomUpInternal(null, true);
            }
            finally
            {
                // Check
                dotExists = File.Exists(dotFile);
                pdfExists = File.Exists(pdfFile);
                Assert.IsTrue(pdfExists || dotExists);

                // Clean up
                if (pdfExists)
                {
                    File.Delete(pdfFile);
                }

                if (dotExists)
                {
                    File.Delete(dotFile);
                }
            }
        }

        private class WithoutTests
        {
            /// <summary>
            /// Helper class.
            /// </summary>
            public class MyTab : TabObject
            {
                protected override string Url => null;

                protected override Func<IWebDriver> Creator => null;
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            private class Link : ControlObject
            {
                protected override By SearchPattern => null;
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            private class TextBox : ControlObject
            {
                protected override By SearchPattern => null;
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            private class Menu : PageObject, IChildOf<MyTab>
            {
                protected override By SearchPattern => null;
                public Link Home => Find<Link>();
                public Link Products => Find<Link>();
                public Link Login => Find<Link>();
                public IEnumerable<Link> CosPublic => FindAll<Link>();
                internal Link CoInternal => Find<Link>();
                internal IEnumerable<Link> CosInternal => FindAll<Link>();
                private Link CoPrivate => Find<Link>();
                private IEnumerable<Link> CosPrivate => FindAll<Link>();
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            private class Login : PageObject, IChildOf<MyTab>
            {
                protected override By SearchPattern => null;
                public Link SignIn => Find<Link>();
                public TextBox Username => Find<TextBox>();
                public TextBox Password => Find<TextBox>();
            }
        }

        public class WithTests
        {
            /// <summary>
            /// Helper class.
            /// </summary>
            public class MyTab : TabObject
            {
                protected override string Url => null;

                protected override Func<IWebDriver> Creator => null;
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            public class Link : ControlObject
            {
                protected override By SearchPattern => null;
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            public class TextBox : ControlObject
            {
                protected override By SearchPattern => null;
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            public class Menu : PageObject, IChildOf<MyTab>
            {
                protected override By SearchPattern => null;
                public Link Home => Find<Link>();
                public Link Products => Find<Link>();
                public Link Login => Find<Link>();
                public IEnumerable<Link> CosPublic => FindAll<Link>();
                internal Link CoInternal => Find<Link>();
                internal IEnumerable<Link> CosInternal => FindAll<Link>();
                private Link CoPrivate => Find<Link>();
                private IEnumerable<Link> CosPrivate => FindAll<Link>();
            }

            /// <summary>
            /// Helper class.
            /// </summary>
            public class Login : PageObject, IChildOf<MyTab>
            {
                protected override By SearchPattern => null;
                public Link SignIn => Find<Link>();
                public TextBox Username => Find<TextBox>();
                public TextBox Password => Find<TextBox>();
            }

            public class LoginTests : PageObjectTests<Login>
            {
                public static bool Slow { get; set; } = true;
                public static bool Fail { get; set; } = true;

                public override bool ReadyToRun => true;

                public override void BeforeFirstTest()
                {
                }

                [PageTest]
                public void LoginValidUser()
                {
                    if (Slow)
                        Thread.Sleep(2000);
                }

                [PageTest]
                public void LoginInvalidUser()
                {
                    if (Slow)
                        Thread.Sleep(4200);
                }

                [PageTest]
                public void ResetPassword()
                {
                    if (Slow)
                        Thread.Sleep(4700);

                    if (Fail)
                        throw new TimeoutException();
                }
            }

            public class MenuTests : PageObjectTests<Menu>
            {
                public static bool Slow { get; set; } = true;

                public override bool ReadyToRun => true;

                public override void BeforeFirstTest()
                {
                }

                [PageTest]
                public void IterateOverAllMenuItems()
                {
                    if (Slow)
                        Thread.Sleep(2400);
                }

                [PageTest]
                public void OpenAndCloseTheMenu()
                {
                    if (Slow)
                        Thread.Sleep(1600);
                }
            }
        }
    }
}