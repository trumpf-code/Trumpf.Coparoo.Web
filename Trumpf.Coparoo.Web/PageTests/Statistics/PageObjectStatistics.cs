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

    using Trumpf.Coparoo.Web.Logging.Tree;

    /// <summary>
    /// Page objects statistics class.
    /// </summary>
    internal class PageObjectStatistics
    {
        private Dictionary<Type, PageObjectStatistic> pageObjectStatistic = new Dictionary<Type, PageObjectStatistic>();

        /// <summary>
        /// Gets the pairs of page objects and DOT tree.
        /// </summary>
        internal IEnumerable<KeyValuePair<Type, Tree>> Stats => pageObjectStatistic.SelectMany(stats => stats.Value.Stats.Select(e => new KeyValuePair<Type, Tree>(stats.Key, e.Value.Tree)));

        /// <summary>
        /// Add page object statistics.
        /// </summary>
        /// <param name="source">The page object</param>
        /// <param name="testClassStatistic">The statistics to add.</param>
        internal void Add(IPageObject source, TestClassStatistic testClassStatistic)
        {
            var t = source.GetType();
            if (!pageObjectStatistic.ContainsKey(t))
            {
                pageObjectStatistic[t] = new PageObjectStatistic();
            }

            pageObjectStatistic[t] += testClassStatistic;
        }
    }
}