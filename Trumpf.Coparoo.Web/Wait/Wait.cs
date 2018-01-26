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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Wait helper throwing on timeout.
    /// </summary>
    public static class Wait
    {
        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        public static void For(Func<bool> function)
        {
            For(function, TimeSpan.FromSeconds(20));
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition to evaluate.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition)
        {
            For(function, condition, TimeSpan.FromSeconds(20));
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        public static void For(Func<bool> function, TimeSpan timeout)
        {
            RetryUntilSuccessOrTimeout(function, e => e, timeout);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition, TimeSpan timeout)
        {
            RetryUntilSuccessOrTimeout(function, condition, timeout);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition, TimeSpan timeout, TimeSpan retryPause)
        {
            RetryUntilSuccessOrTimeout(function, condition, timeout, retryPause);
        }

        /// <summary>
        /// Waits until a function's return value changes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="a">Action to execute.</param>s
        /// <param name="function">The function to test for stabilization.</param>
        public static void ActAndWaitForChange<T>(Action a, Func<T> function)
        {
            T before = function();
            a();

            For(() =>
            {
                try
                {
                    return Different(before, function());
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        public static void UntilStable<T>(Func<T> function) where T : struct
        {
            UntilStableInternal(function, null, null);
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        public static void UntilStable<T>(Func<T> function, TimeSpan retryPause) where T : struct
        {
            UntilStableInternal(function, null, retryPause);
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        public static void UntilStable<T>(Func<T> function, TimeSpan timeout, TimeSpan retryPause) where T : struct
        {
            UntilStableInternal(function, timeout, retryPause);
        }

        /// <summary>
        /// Waits until a function stabilizes.
        /// </summary>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="function">The function to test for stabilization.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        internal static void UntilStableInternal<T>(Func<T> function, TimeSpan? timeout, TimeSpan? retryPause) where T : struct
        {
            T? last = null;
            bool condition(T arg)
            {
                bool stable = last.HasValue && last.Value.Equals(arg);
                last = arg;
                return stable;
            }

            RetryUntilSuccessOrTimeout(function, condition, timeout, retryPause);
        }

        /// <summary>
        /// Determine if the values are different.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="before">The first value.</param>
        /// <param name="after">The second value.</param>
        /// <returns>Whether both values unequal.</returns>
        private static bool Different<T>(T before, T after)
        {
            return (before != null || after != null) && !(after != null ? after.Equals(before) : before.Equals(after));
        }

        /// <summary>
        /// Executes <paramref name="function"/> until its result is not <c>null</c>.
        /// </summary>
        /// <returns>The first result of <paramref name="function"/> which is not <c>null</c>.</returns>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        /// <typeparam name="T">The type of the variable.</typeparam>
        internal static T RetryUntilSuccessOrTimeout<T>(Func<T> function, Predicate<T> condition, TimeSpan? timeout = null, TimeSpan? retryPause = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(20);
            retryPause = retryPause ?? TimeSpan.FromMilliseconds(100);

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<T> result = new List<T>();
            do
            {
                result.Add(function());
                if (condition(result.Last()))
                {
                    return result.Last();
                }

                System.Threading.Thread.Sleep(retryPause.Value);
            }
            while (stopwatch.Elapsed < timeout);

            throw new TimeoutException(string.Format("Condition did not turn true within the maximum waiting time period of {0}s; polling results: {1}", timeout.Value.TotalSeconds, string.Join(", ", result.Select(e => e == null ? "null" : e.ToString()))));
        }
    }
}