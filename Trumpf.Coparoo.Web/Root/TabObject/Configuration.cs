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
    using System.Linq.Expressions;

    using Stashbox;

    /// <summary>
    /// The configuration class.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        internal Configuration()
            => Expression.Empty();

        /// <summary>
        /// Gets the page test class dependency registrator.
        /// </summary>
        public IDependencyRegistrator DependencyRegistrator { get; } = new StashboxContainer();

        /// <summary>
        /// Gets or sets the default timeout for waiting methods.
        /// </summary>
        public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(20);

        /// <summary>
        /// Gets or sets the default positive timeout for the waiting dialog, i.e. how long the dialog should remain visible in case the expected condition evaluates true.
        /// </summary>
        public TimeSpan PositiveWaitTimeout { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Gets or sets a value indicating whether to show a dialog when waiting for a condition.
        /// </summary>
        public bool ShowWaitingDialog { get; set; } = true;

        internal object Resolve(Type typeFrom)
            => ((StashboxContainer)DependencyRegistrator).Resolve(typeFrom);
    }
}