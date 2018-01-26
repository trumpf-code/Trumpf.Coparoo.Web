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

#if !DEBUG
namespace Trumpf.Coparoo.Web.Demo
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Web.PageTests;

    [TestClass]
    public class PageTests
    {
        [TestMethod]
        public void MenuTests()
        {
            var vdi = new VdiTab();
            try
            {
                vdi.On<Menu>().Test(true);
            }
            finally
            {
                vdi.Quit();
            }
        }

        [TestMethod]
        public void SeminarTests()
        {
            var vdi = new VdiTab();
            try
            {
                vdi.On<Events>().Test(true);
            }
            finally
            {
                vdi.Quit();
            }
        }

        [TestMethod]
        public void TestOfTests()
        {
            var vdi = new VdiTab();
            try
            {
                vdi.TestBottomUp(true);
            }
            finally
            {
                vdi.Quit();
            }
        }
    }
}
#endif