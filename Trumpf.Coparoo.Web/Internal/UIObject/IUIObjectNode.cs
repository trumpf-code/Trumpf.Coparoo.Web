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

namespace Trumpf.Coparoo.Web.Internal
{
    using System;

    using OpenQA.Selenium;

    /// <summary>
    /// The UI node interface.
    /// </summary>
    public interface IUIObjectNode : IWebElement
    {
        /// <summary>
        /// Gets the filter predicate.
        /// </summary>
        Predicate<ISearchContext> Predicate { get; }

        /// <summary>
        /// Snap the node object, i.e. associate it with a specific node in the UI tree. This disables lazy node evaluation.
        /// </summary>
        /// <returns>Whether the snap succeeded.</returns>
        bool TrySnap();

        /// <summary>
        /// Unsnap the object.
        /// </summary>
        /// <returns>Whether the node was snapped before.</returns>
        bool TryUnsnap();

        /// <summary>
        /// Find a web element.
        /// </summary>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="result">The control or null.</param>
        /// <returns>Whether the control was found.</returns>
        bool TryFindElement(By pattern, out IWebElement result);

        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="pattern">The search pattern used to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="autoSnap">Whether to snap automatically on the first access.</param>
        void Init(By pattern, Predicate<ISearchContext> predicate, bool autoSnap);
    }
}