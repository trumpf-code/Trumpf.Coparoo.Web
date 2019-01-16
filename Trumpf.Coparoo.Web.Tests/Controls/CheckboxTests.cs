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
    using Trumpf.Coparoo.Web.Controls;

    [TestFixture]
    public class CheckboxTests : ControlTests
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenACheckboxIsUnchecked_ThenItsCheckedPropertyReturnsFalse()
        {
            var name = Random;
            var value = Random;
            var check = false;
            PrepareAndExecute<Tab>(
                nameof(WhenACheckboxIsUnchecked_ThenItsCheckedPropertyReturnsFalse),
                HtmlContents(name, value, check),
                tab =>
                {
                    // Act
                    var checkbox = tab.Find<Checkbox>();
                    var exists = checkbox.Exists.TryWaitFor();
                    var checkboxName = checkbox.Name;
                    var checkboxValue = checkbox.Value;
                    var checkboxChecked = checkbox.Checked;

                    // Check
                    Assert.True(exists);
                    Assert.AreEqual(checkboxName, name);
                    Assert.AreEqual(checkboxValue, value);
                    Assert.AreEqual(checkboxChecked, check);
                });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenACheckboxIsToggled_ThenTheCheckedPropertyTogglesAsWell()
        {
            var check = false;
            PrepareAndExecute<Tab>(
                nameof(WhenACheckboxIsToggled_ThenTheCheckedPropertyTogglesAsWell),
                HtmlContents("", "", check),
                tab =>
                {
                    // Prepare
                    var checkbox = tab.Find<Checkbox>();

                    // Act
                    var c0 = checkbox.Checked;
                    checkbox.Checked = true;
                    var c1 = checkbox.Checked;
                    checkbox.Checked = false;
                    var c2 = checkbox.Checked;
                    checkbox.Checked = true;
                    var c3 = checkbox.Checked;
                    checkbox.Checked = false;
                    var c4 = checkbox.Checked;

                    // Check
                    Assert.AreEqual(false, c0);
                    Assert.AreEqual(true, c1);
                    Assert.AreEqual(false, c2);
                    Assert.AreEqual(true, c3);
                    Assert.AreEqual(false, c4);
                });
        }

        private string HtmlContents(string name, string value, bool check) => HtmlStart + $"<input type=\"checkbox\" name=\"{name}\" value=\"{value}\" {(check ? "checked" : "")}>" + HtmlEnd;
    }
}
