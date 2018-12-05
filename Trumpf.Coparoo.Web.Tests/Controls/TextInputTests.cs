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
    public class TextInputTests : ControlTests
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenATextInputIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
        {
            var name = Random;
            var text = Random;
            PrepareAndExecute(
                nameof(WhenATextInputIsAccessed_ThenItCanBeFoundAndThePropertiesFit),
                HtmlContents(name, text),
                tab =>
                {
                    // Act
                    var textInput = tab.Find<TextInput>();
                    var exists = textInput.Exists.TryWaitFor();
                    var textInputText = textInput.Text;
                    var textInputName = textInput.Name;

                    // Check
                    Assert.True(exists);
                    Assert.AreEqual(name, textInputName);
                    Assert.AreEqual(text, textInputText);
                });
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenATextInputIsSet_ThenTheNewValueCanAlsoBeRetrieved()
        {
            var name = Random;
            var text = Random;
            PrepareAndExecute(
                nameof(WhenATextInputIsAccessed_ThenItCanBeFoundAndThePropertiesFit),
                HtmlContents(name, text),
                tab =>
                {
                    // Act
                    var textInput = tab.Find<TextInput>();

                    var oldValue = textInput.Text;
                    var newValue = Random + Random;
                    var get = textInput.Text = newValue;

                    // Check
                    Assert.AreEqual(text, oldValue);
                    Assert.AreEqual(get, newValue);
                });
        }

        private string HtmlContents(string name, string text) => HtmlStart + $"<input type=\"text\" name=\"{name}\" value=\"{text}\">" + HtmlEnd;
    }
}
