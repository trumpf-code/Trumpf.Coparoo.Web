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
    using System.Collections.Generic;

    using Logging.Tree;

    /// <summary>
    /// Internal interface for the page object base class.
    /// </summary>
    internal interface ITreeObject : IPageObject
    {
        /// <summary>
        /// Gets a value indicating whether the page object shall be returned, e.g. by the ON-method.
        /// </summary>
        bool OnCondition { get; }

        /// <summary>
        /// Gets the dot tree representation.
        /// Clears image and statistics for this page objects and its (transitive) children.
        /// The tree does not contain generic type definition page objects.
        /// </summary>
        /// <returns>The dot tree representation of the page object tree.</returns>
        Tree Tree { get; }

        /// <summary>
        /// Gets the child page objects.
        /// If a child page object is a generic type definition that matches with the hint type, this type is returned.
        /// The hint is necessary since the type parameters cannot be "guessed".
        /// </summary>
        /// <typeparam name="TPageObjectChildHint">The hint type for a generic type definition page object child.</typeparam>
        /// <returns>The child page objects.</returns>
        IEnumerable<IPageObject> Children<TPageObjectChildHint>();
    }
}