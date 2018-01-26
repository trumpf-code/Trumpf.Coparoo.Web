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
    using System;
    using System.Collections.Generic;

    using Trumpf.Coparoo.Web.Waiting;

    /// <summary>
    /// Internal interface for root page objects.
    /// </summary>
    internal interface ITabObjectInternal : ITabObject
    {
        /// <summary>
        /// Gets the page object locator.
        /// </summary>
        IPageObjectLocator PageObjectLocator { get; }

        /// <summary>
        /// Gets the UI object interface resolver.
        /// </summary>
        IUIObjectInterfaceResolver UIObjectInterfaceResolver { get; }

        /// <summary>
        /// Gets the node locator.
        /// </summary>
        INodeLocator NodeLocator { get; }

        /// <summary>
        /// Gets the dynamically registered children of a page object.
        /// </summary>
        /// <param name="pageObjectType">The parent page object type.</param>
        /// <returns>The registered children types.</returns>
        IEnumerable<Type> DynamicChildren(Type pageObjectType);

        /// <summary>
        /// Get an await-object.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The display name used in timeout exceptions.</param>
        /// <returns>The wrapped object.</returns>
        IAwait<T> Await<T>(Func<T> function, string name);
    }
}