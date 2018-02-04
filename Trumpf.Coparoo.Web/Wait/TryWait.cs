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
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Wait helper returning on timeout.
    /// </summary>
    public static class TryWait
    {
        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool For(Func<bool> function)
        {
            return For(function, TimeSpan.FromSeconds(20));
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool For<T>(Func<T> function, Predicate<T> condition)
        {
            return For(function, condition, TimeSpan.FromSeconds(20));
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool For(Func<bool> function, TimeSpan timeout)
        {
            return InternalWait(function, e => e, timeout);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool For<T>(Func<T> function, Predicate<T> condition, TimeSpan timeout)
        {
            return InternalWait(function, condition, timeout);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool For<T>(Func<T> function, Predicate<T> condition, TimeSpan timeout, TimeSpan retryPause)
        {
            return InternalWait(function, condition, timeout, retryPause);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        /// <returns>If the condition turned true.</returns>
        private static bool InternalWait<T>(Func<T> function, Predicate<T> condition, TimeSpan? timeout = null, TimeSpan? retryPause = null)
        {
            try
            {
                Wait.RetryUntilSuccessOrTimeout(function, condition, timeout, retryPause);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Wait until the specified property equals the specified value for defined time period or until the specified time limit is reached.
        /// </summary>
        /// <param name="function">Condition to evaluate in loop.</param>
        /// <param name="timeout">The time limit for the whole operation.</param>
        /// <param name="stableTime">The time the condition should evaluate true.</param>
        /// <returns>True if the specified property equals the specified value; else, false.</returns>
        public static bool UntilStableFor(Func<bool> function, TimeSpan timeout, TimeSpan stableTime)
        {
            Stopwatch watch = Stopwatch.StartNew();
            while (watch.Elapsed < timeout)
            {
                if (!function())
                {
                    Thread.Sleep(100);
                    continue;
                }

                TimeSpan elapsedBeforeSucceeded = watch.Elapsed;
                if (elapsedBeforeSucceeded + stableTime > timeout)
                {
                    return false;
                }

                //wait for cool-down period
                while (watch.Elapsed - elapsedBeforeSucceeded < stableTime)
                {
                    if (!function())
                    {
                        return UntilStableFor(function, timeout - watch.Elapsed, stableTime);
                    }

                    Thread.Sleep(100);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool UntilStable<T>(Func<T> function) where T : struct
        {
            return UntilStableInternal(function, null, null);
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool UntilStable<T>(Func<T> function, TimeSpan retryPause) where T : struct
        {
            return UntilStableInternal(function, null, retryPause);
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        /// <returns>If the condition turned true.</returns>
        public static bool UntilStable<T>(Func<T> function, TimeSpan timeout, TimeSpan retryPause) where T : struct
        {
            return UntilStableInternal(function, timeout, retryPause);
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        /// <returns>If the condition turned true.</returns>
        private static bool UntilStableInternal<T>(Func<T> function, TimeSpan? timeout, TimeSpan? retryPause) where T : struct
        {
            try
            {
                Wait.UntilStableInternal(function, timeout, retryPause);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}