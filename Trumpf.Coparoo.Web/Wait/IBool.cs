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

using System;

namespace Trumpf.Coparoo.Web.Waiting
{
    /// <summary>
    /// Interface representing a Boolean structure with methods to query and wait for specific conditions.
    /// </summary>
    public interface IBool
    {
        /// <summary>
        /// Property that retrieves the current boolean state.
        /// </summary>
        bool Value { get; }

        /// <summary>
        /// Attempts to wait until the boolean value becomes true.
        /// </summary>
        /// <returns>
        /// True if the wait is successful; otherwise, false.
        /// </returns>
        bool TryWaitFor();

        /// <summary>
        /// Attempts to wait until the boolean value becomes true, with a specified timeout.
        /// </summary>
        /// <param name="timeout">The maximum duration to wait.</param>
        /// <returns>
        /// True if the wait is successful within the timeout period; otherwise, false.
        /// </returns>
        bool TryWaitFor(TimeSpan timeout);

        /// <summary>
        /// Attempts to wait until the boolean value becomes false.
        /// </summary>
        /// <returns>
        /// True if the wait is successful; otherwise, false.
        /// </returns>
        bool TryWaitForFalse();

        /// <summary>
        /// Attempts to wait until the boolean value becomes false, with a specified timeout.
        /// </summary>
        /// <param name="timeout">The maximum duration to wait.</param>
        /// <returns>
        /// True if the wait is successful within the timeout period; otherwise, false.
        /// </returns>
        bool TryWaitForFalse(TimeSpan timeout);

        /// <summary>
        /// Waits until the boolean value becomes true, blocking until the condition is met.
        /// </summary>
        void WaitFor();

        /// <summary>
        /// Waits until the boolean value becomes true, blocking for a specified timeout.
        /// </summary>
        /// <param name="timeout">The maximum duration to wait.</param>
        void WaitFor(TimeSpan timeout);

        /// <summary>
        /// Waits until the boolean value becomes false, blocking until the condition is met.
        /// </summary>
        void WaitForFalse();

        /// <summary>
        /// Waits until the boolean value becomes false, blocking for a specified timeout.
        /// </summary>
        /// <param name="timeout">The maximum duration to wait.</param>
        void WaitForFalse(TimeSpan timeout);
    }
}