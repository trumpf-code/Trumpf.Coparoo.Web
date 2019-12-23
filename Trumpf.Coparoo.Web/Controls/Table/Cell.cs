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
    using Trumpf.Coparoo.Web;

    /// <summary>
    /// Partial Table control object.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// Table data cell object.
        /// </summary>
        public class Cell : ControlObject
        {
            /// <summary>
            /// Gets the search pattern.
            /// </summary>
            protected override By SearchPattern => By.XPath(".//th|.//td");

            /// <summary>
            /// Searches for a control object like provided and returns it.
            /// </summary>
            /// <typeparam name="T">Control object type.</typeparam>
            /// <returns>Cell object as a control object like provided.</returns>
            public T As<T>() where T : IControlObject => Find<T>();

            /// <summary>
            /// Returns true if node element is header cell, otherwise false.
            /// </summary>
            public bool IsHeaderCell => Node.TagName.Equals("th");

            /// <summary>
            /// Returns true if node element is data cell, otherwise false.
            /// </summary>
            public bool IsDataCell => Node.TagName.Equals("td");
        }
    }
}
