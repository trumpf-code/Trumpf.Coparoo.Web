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

namespace Trumpf.Coparoo.Web.Demo
{
    using System.Collections.Generic;

    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Controls;

    public class Events : PageObject, IChildOf<VdiPage>
    {
        protected override By SearchPattern => By.ClassName("vdi-event-list-view");

        public ITextInput SearchText => Find<TextInput>(By.Id("name"));
        public IButton Search => Find<Button>();
        public IEnumerable<IEventTitleRow> EventList => FindAll<EventTitleRow>();

        public void SearchFor(string searchText)
        {
            SearchText.ScrollTo();
            SearchText.Text = searchText;
            Search.Click();
        }
        public override void Goto()
        {
            if (!Displayed)
                Goto<Menu>().Events.Click();
        }
    }
}