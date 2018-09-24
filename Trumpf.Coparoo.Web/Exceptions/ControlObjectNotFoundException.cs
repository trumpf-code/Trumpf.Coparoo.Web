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
    /// Control object not found exception class.
    /// </summary>
    public class ControlObjectNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlObjectNotFoundException"/> class.
        /// </summary>
        public ControlObjectNotFoundException(Type ControlObjectType)
            : base("Control object " + ControlObjectType.ToString() + " could not be resolved: no matching class with parameterless default constructor found; are all assemblies already loaded?")
        {
        }
    }
}