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

    using OpenQA.Selenium;
    
    /// <summary>
    /// Internal interface for the control object base class.
    /// </summary>
    internal interface IControlObjectInternal : IControlObject
    {
        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="pattern">The search pattern used to locate the control.</param>
        /// <param name="pred">The search predicate, if any.</param>
        /// <param name="autoSnap">Whether to snap automatically on the first access.</param>
        void Init(By pattern, Predicate<ISearchContext> pred, bool autoSnap);
    }
}