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

    /// <summary>
    /// The page object locator interface.
    /// </summary>
    internal interface IPageObjectLocator
    {
        /// <summary>
        /// Gets the cache object count.
        /// </summary>
        int CacheObjectCount { get; }

        /// <summary>
        /// Gets the cache type count.
        /// </summary>
        int CacheTypeCount { get; }

        /// <summary>
        /// Clear the register.
        /// </summary>
        void Clear();

        /// <summary>
        /// Register the page object type with the given object.
        /// </summary>
        /// <typeparam name="TPageObject">The page object type.</typeparam>
        /// <param name="target">The object to register.</param>
        void Register<TPageObject>(IPageObject target);

        /// <summary>
        /// Unregister the page object type.
        /// </summary>
        /// <typeparam name="TPageObject">The page object type.</typeparam>
        void Unregister<TPageObject>();

        /// <summary>
        /// Try get the registered objects for the given page object type.
        /// </summary>
        /// <typeparam name="TPageObject">The page object type.</typeparam>
        /// <param name="pageObject">The object, if any.</param>
        /// <returns>Whether a registered object was found.</returns>
        bool TryGet<TPageObject>(out IEnumerable<TPageObject> pageObject);

        /// <summary>
        /// Find a page object in the page object tree.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
        /// <typeparam name="TPageObject">The page object type to search for.</typeparam>
        /// <returns>The page object.</returns>
        TPageObject Find<TPageObject>(Predicate<TPageObject> condition) where TPageObject : IPageObject;
    }
}