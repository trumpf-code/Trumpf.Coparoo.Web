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

namespace Trumpf.Coparoo.Web.PageTests
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Page test attribute used to mark page object tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class PageTestAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageTestAttribute"/> class.
        /// Used to mark page tests inside a page test class
        /// The is used to sort tests from top to bottom.
        /// </summary>
        /// <param name="line">The method line.</param>
        public PageTestAttribute([CallerLineNumber] int line = 0)
        {
            Line = line;
        }

        /// <summary>
        /// Gets the line where the test method was define.
        /// </summary>
        public int Line { get; private set; }
    }
}
