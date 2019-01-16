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
    public class ButtonTests : ControlTests
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenAButtonIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
        {
            var text = Random;
            PrepareAndExecute<Tab>(nameof(WhenAButtonIsAccessed_ThenItCanBeFoundAndThePropertiesFit), HtmlStart + $"<button type=\"button\">{text}</button>" + HtmlEnd, tab =>
            {
                // Act
                var button = tab.Find<Button>();
                var exists = button.Exists.TryWaitFor();
                var buttonText = button.Text;

                // Check
                Assert.True(exists);
                Assert.AreEqual(text, buttonText);
            });
        }
    }
}
