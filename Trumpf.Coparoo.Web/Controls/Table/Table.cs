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
    using System.Collections.Generic;
    using System.Linq;

    using OpenQA.Selenium;

    /// <summary>
    /// Table control object.
    /// </summary>
    public partial class Table : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("table");

        /// <summary>
        /// Gets the table head control object.
        /// </summary>
        public Head Header => Find<Head>();

        /// <summary>
        /// Gets the table body control object.
        /// </summary>
        public Body Content => Find<Body>();

        /// <summary>
        /// Gets the table foot control object.
        /// </summary>
        public Foot Footer => Find<Foot>();

        /// <summary>
        /// Gets a sorted enumeration of all row control objects (including header, body and footer rows).
        /// </summary>
        public IEnumerable<Row> AllRows => Header.Rows.Concat(Content.Rows).Concat(Footer.Rows);
    }
}
