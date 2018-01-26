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

namespace Trumpf.Coparoo.Web
{
    using System.Collections.Generic;

    using Trumpf.Coparoo.Web.Internal;

    /// <summary>
    /// Page object interface.
    /// </summary>
    public interface IPageObject : IUIObject
    {
        /// <summary>
        /// Gets the parent page object.
        /// </summary>
        new IPageObject Parent { get; }

        /// <summary>
        /// Gets the child page objects.
        /// Ignoring generic type definition page objects.
        /// </summary>
        /// <returns>The child page objects.</returns>
        IEnumerable<IPageObject> Children();

        /// <summary>
        /// Goto the page object, i.e. perform necessary action to make the page object visible on screen.
        /// </summary>
        void Goto();
    }
}