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
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Desktop.Waiting;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Waiting;

    [TestClass]
    public class MsTests
    {
        IVdiTab vdi;

        [TestMethod]
        public void OpenAndQuitTab()
        {
            try
            {
                vdi = new VdiTab();
                vdi.Open();
                vdi.On<VdiPage>().Displayed.WaitFor();
            }
            finally
            {
                vdi.Quit();
            }
        }

        [TestMethod]
        public void TestWithExplicitNavigation()
        {
            try
            {
                vdi = new VdiTab();
                vdi.Open();
                vdi.On<VdiPage>().Displayed.WaitFor();
                vdi.On<Menu>().Events.Click();
                vdi.On<Events>().SearchText.Text = "praxis qualitätssicherung";
                vdi.On<Events>().Search.SendReturn();
                DialogWait.For(() => vdi.On<Events>().EventList.Count(), c => c == 1, "1 seminars in list");
            }
            finally
            {
                vdi.Quit();
            }
        }

        [TestMethod]
        public void TestWithImplicitNavigation()
        {
            try
            {
                vdi = new VdiTab();
                vdi.Goto<Events>().SearchFor("praxis qualitätssicherung");
                DialogWait.For(() => vdi.On<Events>().EventList.Count(), c => c == 1, "1 seminars in list");
            }
            finally
            {
                vdi.Quit();
            }
        }

        [TestMethod]
        public void AbstractVdiTestWithImplicitNavigation()
        {
            vdi = TabObject.Resolve<IVdiTab>();
            try
            {
                vdi.Goto<IEvents>().SearchFor("praxis qualitätssicherung");
                DialogWait.For(() => vdi.On<IEvents>().EventList.Count(), c => c == 1, "1 seminars in list");
            }
            finally
            {
                vdi.Quit();
            }
        }

        [TestMethod]
        public void CreateGraph()
        {
            vdi = new VdiTab();
            var filePath = vdi.WriteTree();
            Console.WriteLine(filePath);
        }
    }
}
#endif