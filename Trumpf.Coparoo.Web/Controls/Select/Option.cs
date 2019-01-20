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

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Trumpf.Coparoo.Web.Controls
{
    /// <summary>
    /// Option control object.
    /// </summary>
    public class Option : ControlObject, IOption
    {
        /// <summary>
        /// Gets the option value.
        /// </summary>
        public string Value
            => Node.GetAttribute("value");

        /// <inheritdoc/>
        protected override By SearchPattern 
            => By.TagName(nameof(Option));

        /// <summary>
        /// Gets a value indicating whether the option is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return SelectElement.SelectedOption.Text.Equals(Node.Text);
            }
        }

        /// <summary>
        /// Select this option.
        /// </summary>
        public void Select()
        {
            if (!IsSelected)
                SelectElement.SelectByText(Node.Text);
        }

        private SelectElement SelectElement 
            => new SelectElement(Parent.Node);
    }
}
