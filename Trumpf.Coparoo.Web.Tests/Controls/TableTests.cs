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
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Web.Controls;

    [TestClass]
    public class TableTests : ControlTests
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [TestMethod]
        public void WhenATableIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
        {
            PrepareAndExecute<Tab>(nameof(WhenATableIsAccessed_ThenItCanBeFoundAndThePropertiesFit), HtmlStart + "<table><thead><tr><th>1</th><th>2</th></tr></thead><tfoot><tr><td>3</td><td>4</td></tr></tfoot><tbody><tr><td>5</td><td>6</td></tr><tr><td>7</td><td>8</td></tr></tbody></table>" + HtmlEnd, tab =>
            {
                // Act
                var table = tab.Find<Table>();
                var exists = table.Exists.TryWaitFor();
                var headerRows = table.Header.Rows.Count();
                var contentRows = table.Content.Rows.Count();
                var footerRows = table.Footer.Rows.Count(); ;
                var allRows = table.AllRows.Count();

                // Check
                Assert.IsTrue(exists);
                Assert.AreEqual(1, headerRows);
                Assert.AreEqual(2, contentRows);
                Assert.AreEqual(1, footerRows);
                Assert.AreEqual(4, allRows);
            });
        }
    }
}
