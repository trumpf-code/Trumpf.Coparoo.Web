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
    /// Invalid ChildOf usage exception class.
    /// </summary>
    public class ChildOfUsageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildOfUsageException"/> class.
        /// </summary>
        /// <param name="type">The page object type.</param>
        /// <param name="text">The additional exception text.</param>
        public ChildOfUsageException(Type type, string text)
            : base("Invalid use of " + typeof(IChildOf<>).Name + " for page object " + type.FullName + ". " + text)
        {
        }
    }
}