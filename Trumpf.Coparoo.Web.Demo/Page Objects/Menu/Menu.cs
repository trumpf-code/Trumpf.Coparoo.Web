﻿// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System.Linq;

    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Controls;

    public class Menu : PageObject, IChildOf<VdiPage>, IMenu
    {
        protected override By SearchPattern => By.CssSelector(".vdi-main-menu");
        public IEnumerable<ILink> Buttons => FindAll<ILink>()
            .OrderFromLeftToRight()
            .Where(e => e.Displayed.Value);

        public ILink Events => Buttons.ElementAt(0);
        public ILink Place => Buttons.ElementAt(1);
        public ILink Contact => Buttons.ElementAt(2);
        public ILink AboutUs => Buttons.ElementAt(3);

        public override void Goto()
        {
            if (!Displayed.Value)
                Parent.Goto();
        }
    }
}