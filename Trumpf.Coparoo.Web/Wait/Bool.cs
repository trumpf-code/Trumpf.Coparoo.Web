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
    using System.Linq.Expressions;

    /// <summary>
    /// Boolean equipped with throwing and non-throwing waiting methods.
    /// </summary>
    public class Bool : IBool
    {
        private readonly IAwait<bool> waiter;
        private readonly Func<bool> snap;
        private readonly Action unsnap;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bool"/> class with a default value.
        /// </summary>
        public Bool()
            => Expression.Empty();

        /// <summary>
        /// Initializes a new instance of the <see cref="Bool"/> class.
        /// </summary>
        /// <param name="waiter">The Boolean to wait for.</param>
        /// <param name="snap">Snap the object.</param>
        /// <param name="unsnap">Unsnap the object.</param>
        public Bool(IAwait<bool> waiter, Func<bool> snap, Action unsnap)
        {
            this.waiter = waiter;
            this.snap = snap;
            this.unsnap = unsnap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bool"/> class.
        /// </summary>
        /// <param name="waiter">The Boolean to wait for.</param>
        /// <param name="snap">Snap the object.</param>
        /// <param name="unsnap">Unsnap the object.</param>
        public IBool Init(IAwait<bool> waiter, Func<bool> snap, Action unsnap)
            => new Bool(waiter, snap, unsnap);

        /// <summary>
        /// Gets a fresh value evaluation.
        /// </summary>
        public bool Value => waiter.Value;

        #region TryWait
        /// <summary>
        /// Try wait until the page object is visible on screen.
        /// </summary>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor() => waiter.TryWaitFor(value => value);

        /// <summary>
        /// Try wait until the page object is visible on screen.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor(TimeSpan timeout) => waiter.TryWaitFor(value => value, timeout);

        /// <summary>
        /// Try wait until the page object is not visible on screen.
        /// </summary>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitForFalse() => ActSnapped(() => waiter.TryWaitFor(value => !value));

        /// <summary>
        /// Try wait until the page object is not visible on screen.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitForFalse(TimeSpan timeout) => ActSnapped(() => waiter.TryWaitFor(value => !value, timeout));
        #endregion

        #region Wait
        /// <summary>
        /// Wait until the property evaluates to true.
        /// </summary>
        public void WaitFor() => waiter.WaitFor(value => value, "is true");

        /// <summary>
        /// Wait until the property evaluates to true.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void WaitFor(TimeSpan timeout) => waiter.WaitFor(value => value, "is true", timeout);

        /// <summary>
        /// Wait until the page object is not visible on screen anymore.
        /// </summary>
        public void WaitForFalse() => ActSnapped(() => waiter.WaitFor(value => !value, "is false"));

        /// <summary>
        /// Wait until the page object is not visible on screen anymore.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void WaitForFalse(TimeSpan timeout) => ActSnapped(() => waiter.WaitFor(value => !value, "is false", timeout));
        #endregion

        #region Helpers
        /// <summary>
        /// Execute an action in snapped context.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void ActSnapped(Action action) => ActSnapped(() => { action(); return true; });

        /// <summary>
        /// Execute a function in snapped context.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <returns>The function result.</returns>
        private bool ActSnapped(Func<bool> function)
        {
            var result = snap() ? function() : true;
            unsnap();
            return result;
        }
        #endregion
    }
}
