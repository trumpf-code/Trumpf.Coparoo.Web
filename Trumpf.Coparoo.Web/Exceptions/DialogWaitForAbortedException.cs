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

namespace Trumpf.Coparoo.Web.Exceptions
{
    using System;

    /// <summary>
    /// Dialog wait for aborted exception.
    /// </summary>
    internal class DialogWaitForAbortedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWaitForTimeoutException"/> class.
        /// </summary>
        public DialogWaitForAbortedException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWaitForTimeoutException"/> class.
        /// </summary>
        /// <param name="expectedCondition">The message that describes the error.</param>
        public DialogWaitForAbortedException(string expectedCondition) : base("User aborted waiting for condition " + expectedCondition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWaitForTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public DialogWaitForAbortedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}