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

    /// <summary>
    /// Page objects statistics class.
    /// </summary>
    internal class PageObjectStatistic
    {
        /// <summary>
        /// Gets the test class type to test class statistics map.
        /// </summary>
        internal Dictionary<Type, TestClassStatistic> Stats { get; private set; } = new Dictionary<Type, TestClassStatistic>();

        /// <summary>
        /// Merge in test class statistics.
        /// </summary>
        /// <param name="c1">The page object statistics to extend.</param>
        /// <param name="c2">The test class statistics to add.</param>
        /// <returns>The extended page object statistic.</returns>
        public static PageObjectStatistic operator +(PageObjectStatistic c1, TestClassStatistic c2)
        {
            if (c1.Stats.TryGetValue(c2.Type, out TestClassStatistic value))
            {
                c1.Stats[c2.Type] = value + c2;
            }
            else
            {
                c1.Stats.Add(c2.Type, c2);
            }

            return c1;
        }
    }
}