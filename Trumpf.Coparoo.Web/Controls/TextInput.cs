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

namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Text input control object.
    /// Expects an input html element with attribute type="text".
    /// </summary>
    public class TextInput : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.XPath(".//input[@type='text']");

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name => Node.GetAttribute("name");

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get { return Node.GetAttribute("value"); }

            set
            {
                var text = Text;
                if (text != value)
                {
                    if (text != string.Empty)
                    {
                        Node.Clear();
                    }

                    Node.SendKeys(value);
                }
            }
        }
    }
}
