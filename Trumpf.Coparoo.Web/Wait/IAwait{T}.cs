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

namespace Trumpf.Coparoo.Web.Waiting
{
    using System;

    /// <summary>
    /// Interface for await-objects.
    /// </summary>
    /// <typeparam name="T">The underlying type.</typeparam>
    public interface IAwait<T>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Try-wait for the expectation to become true.
        /// Return false on timeout.
        /// Use the default timeout as specified by configuration in the root object.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <returns>Whether the expectation became true.</returns>
        bool TryWaitFor(Predicate<T> expectation);

        /// <summary>
        /// Try-wait for the expectation to become true.
        /// Return false on timeout.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Whether the expectation became true.</returns>
        bool TryWaitFor(Predicate<T> expectation, TimeSpan timeout);

        /// <summary>
        /// Wait for the expectation to become true.
        /// Throw on timeout.
        /// Use the default timeout as specified by configuration in the root object.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="expectationText">The expectation text.</param>
        void WaitFor(Predicate<T> expectation, string expectationText);

        /// <summary>
        /// Wait for the expectation to become true.
        /// Throw on timeout.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="expectationText">The expectation text.</param>
        /// <param name="timeout">The timeout.</param>
        void WaitFor(Predicate<T> expectation, string expectationText, TimeSpan timeout);

        /// <summary>
        /// Gets the value as string.
        /// </summary>
        /// <returns>The value.</returns>
        string ToString();
    }
}