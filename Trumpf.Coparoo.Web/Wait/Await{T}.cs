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

    using Exceptions;

    /// <summary>
    /// Object retrieval with throwing and non-throwing waiting methods.
    /// </summary>
    internal class Await<T> : IAwait<T>
    {
        private readonly Func<T> getValue;
        private readonly string name;
        private readonly Type owner;
        private readonly Func<TimeSpan> positiveTimeout;
        private readonly Func<TimeSpan> waitTimeout;
        private readonly Func<bool> showDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="Await{T}"/> class.
        /// </summary>
        /// <param name="getValue">The function to evaluate for truth.</param>
        /// <param name="name">The property name for logging purposes.</param>
        /// <param name="owner">The owner type.</param>
        /// <param name="waitTimeout">The timeout.</param>
        /// <param name="positiveTimeout">The positive timeout for dialog-waits.</param>
        /// <param name="showDialog">Whether to show a dialog while waiting.</param>
        internal Await(Func<T> getValue, string name, Type owner, Func<TimeSpan> waitTimeout, Func<TimeSpan> positiveTimeout, Func<bool> showDialog)
        {
            this.getValue = getValue;
            this.name = name;
            this.owner = owner;
            this.waitTimeout = waitTimeout;
            this.positiveTimeout = positiveTimeout;
            this.showDialog = showDialog;
        }

        /// <summary>
        /// Gets a fresh value evaluation.
        /// </summary>
        public T Value => getValue();

        /// <summary>
        /// Implicit conversion to the underlying type.
        /// </summary>
        /// <param name="other">The value to convert.</param>
        public static implicit operator T(Await<T> other) => other.Value;

        #region TryWait
        /// <summary>
        /// Try wait until the page object is visible on screen.
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor(Predicate<T> expectation) => TryWait.For(() => expectation(Value), waitTimeout());

        /// <summary>
        /// Try wait until the page object is visible on screen.
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor(Predicate<T> expectation, TimeSpan timeout) => TryWait.For(() => expectation(Value), timeout);
        #endregion

        #region Wait
        /// <summary>
        /// Wait until the property evaluates to true.
        /// Show a dialog.
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <param name="expectationText">Expectation text, i.e. the predicate expressed as a human-readable text.</param>
        public void WaitFor(Predicate<T> expectation, string expectationText) => WaitFor(expectation, expectationText, waitTimeout());

#if !NETSTANDARD2_0
        /// <summary>
        /// Wait until the property evaluates to true.
        /// Show a dialog.
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <param name="expectationText">Expectation text, i.e. the predicate expressed as a human-readable text.</param>
        /// <param name="timeout">The timeout.</param>
        public void WaitFor(Predicate<T> expectation, string expectationText, TimeSpan timeout)
        {
            string message = $"{name} in {owner.Name}: {expectationText}";
            if (showDialog())
            {
                DialogWait.For(() => Value, value => expectation(value), message, timeout, positiveTimeout(), TimeSpan.FromMilliseconds(100));
            }
            else
            {
                if (!TryWaitFor(expectation, timeout))
                {
                    throw new WaitForException(owner, message);
                }
            }
        }
#else
        /// <summary>
        /// Wait until the property evaluates to true.
        /// Show a dialog.
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <param name="expectationText">Expectation text, i.e. the predicate expressed as a human-readable text.</param>
        /// <param name="timeout">The timeout.</param>
        public void WaitFor(Predicate<T> expectation, string expectationText, TimeSpan timeout)
        {
            if(showDialog()) {
                throw new NotSupportedException("Showing a waiting dialog is not supported under .NET Standard. Please use the .NET 4.5 version.");
            }

            string message = $"{name} in {owner.Name}: {expectationText}";

            if (!TryWaitFor(expectation, timeout))
            {
                throw new WaitForException(owner, message);
            }
        }
#endif
        #endregion

        #region Others
        /// <summary>
        /// Gets the value as string.
        /// </summary>
        /// <returns>The value.</returns>
        public override string ToString() => Value.ToString();
        #endregion
    }
}