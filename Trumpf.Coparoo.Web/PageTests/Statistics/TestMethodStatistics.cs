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

namespace Trumpf.Coparoo.Web.PageTests.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Test methods statistics class.
    /// </summary>
    internal class TestMethodStatistics
    {
        private List<TestMethodStatistic> testMethodStats = new List<TestMethodStatistic>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodStatistics"/> class.
        /// </summary>
        /// <param name="methodInfo">The test method info.</param>
        public TestMethodStatistics(MethodInfo methodInfo)
        {
            this.MethodInfo = methodInfo;
        }

        /// <summary>
        /// Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a test failed.
        /// </summary>
        public bool AnyFailed => testMethodStats.Any(t => !t.SuccessHasValue ? false : !t.Success);

        /// <summary>
        /// Add test method statistic.
        /// </summary>
        /// <param name="c1">The test method statistics to extend.</param>
        /// <param name="c2">The test method statistic to add.</param>
        /// <returns>The extended test method statistics.</returns>
        public static TestMethodStatistics operator +(TestMethodStatistics c1, TestMethodStatistic c2)
        {
            if (c1.MethodInfo != c2.MethodInfo)
            {
                throw new Exception();
            }

            c1.testMethodStats.Add(c2);

            return c1;
        }

        /// <summary>
        /// Merge test method statistics.
        /// </summary>
        /// <param name="c1">The test method statistics to extend.</param>
        /// <param name="c2">The test method statistics to add.</param>
        /// <returns>The extended test method statistics.</returns>
        public static TestMethodStatistics operator +(TestMethodStatistics c1, TestMethodStatistics c2) => new TestMethodStatistics(c1.MethodInfo) { testMethodStats = c1.testMethodStats.Concat(c2.testMethodStats).ToList() };

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() => string.Join("\\n\\n", testMethodStats);
    }
}
