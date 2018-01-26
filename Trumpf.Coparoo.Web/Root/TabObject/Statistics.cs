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

namespace Trumpf.Coparoo.Web
{
    using System;

    /// <summary>
    /// The statistics class.
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// Gets or sets the access counter.
        /// </summary>
        public int AccessCounter { get; set; }

        /// <summary>
        /// Gets or sets the in-cache counter.
        /// </summary>
        public int InCache { get; set; }

        /// <summary>
        /// Gets or sets the accessible counter.
        /// </summary>
        public int AccessibleNodes { get; set; }

        /// <summary>
        /// Gets or sets the visible on screen counter.
        /// </summary>
        public int VisibleOnScreenNodes { get; set; }

        /// <summary>
        /// Gets or sets the caching enabled counter.
        /// </summary>
        public int CachingEnabledCounter { get; set; }

        /// <summary>
        /// Gets the counter values as string.
        /// </summary>
        public string Summary
        {
            get
            {
                string result = string.Empty;
                result += "AccessCounter          " + AccessCounter + Environment.NewLine;
                result += "InCache                " + InCache + Environment.NewLine;
                result += "AccessibleNodes        " + AccessibleNodes + Environment.NewLine;
                result += "VisibleOnScreenNodes   " + VisibleOnScreenNodes + Environment.NewLine;
                result += "CachingEnabledCounter  " + CachingEnabledCounter + Environment.NewLine;
                return result;
            }
        }

        /// <summary>
        /// Reset the counters.
        /// </summary>
        public void Reset()
        {
            AccessCounter = 0;
            InCache = 0;
            AccessibleNodes = 0;
            VisibleOnScreenNodes = 0;
            CachingEnabledCounter = 0;
        }
    }
}