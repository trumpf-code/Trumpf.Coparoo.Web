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
    using System.Linq;
    using Trumpf.Coparoo.Web.Controls;

    [TestClass]
    public class SelectTests : ControlTests
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenASelectHas3Options_ThenTheOptionEnumerationHas3Items()
        {
            PrepareAndExecute<Tab>(
                nameof(WhenASelectHas3Options_ThenTheOptionEnumerationHas3Items),
                HtmlContents,
                tab =>
                {
                    // Act
                    var select = tab.Find<Select>();
                    var displayed = select.Displayed;
                    var count = select.Options.Count();
                    var values = select.Options.Select(e => e.Value).ToList();
                    var selected = select.Options.Select(e => e.IsSelected).ToList();

                    // Check
                    Assert.IsTrue(displayed);
                    Assert.AreEqual(3, count);
                    CollectionAssert.AreEqual(new[] { "a", "b", "c" }, values);
                    CollectionAssert.AreEqual(new[] { true, false, false }, selected);

                    System.Threading.Thread.Sleep(5000);
                });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenTheSecondOptionIsSelected_ThenTheSecondElementReturnIsSelectedTrue()
        {
            PrepareAndExecute<Tab>(
                nameof(WhenTheSecondOptionIsSelected_ThenTheSecondElementReturnIsSelectedTrue),
                HtmlContents,
                tab =>
                {
                    // Act
                    var select = tab.Find<Select>();
                    select.Options.ElementAt(1).Select();
                    var selected = select.Options.Select(e => e.IsSelected).ToList();

                    // Check
                    CollectionAssert.AreEqual(new[] { false, true, false }, selected);
                });
        }

        private string HtmlContents
            => HtmlStart + $"<select><option value=\"a\">A</option><option value=\"b\">B</option><option value=\"c\">C</option></select>" + HtmlEnd;
    }
}
