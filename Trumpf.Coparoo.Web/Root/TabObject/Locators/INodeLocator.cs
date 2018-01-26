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
    using OpenQA.Selenium;

    /// <summary>
    /// The node object locator interface.
    /// </summary>
    internal interface INodeLocator
    {
        /// <summary>
        /// Clear the register.
        /// </summary>
        void Clear();

        /// <summary>
        /// Register the node object type with the given hash code.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="node">The object to register.</param>
        void Register(int hash, ISearchContext node);

        /// <summary>
        /// Try get the registered node for the given hash code.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="result">The object, if any.</param>
        /// <returns>Whether a registered object was found.</returns>
        bool TryGet(int hash, out ISearchContext result);
    }
}