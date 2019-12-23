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

namespace Trumpf.Coparoo.Web.Controls.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for table control object.
    /// </summary>
    public interface ITable : IControlObject
    {
        /// <summary>
        /// Gets the table head control object.
        /// </summary>
        IHead Header { get; }

        /// <summary>
        /// Gets the table body control object.
        /// </summary>
        IBody Content { get; }

        /// <summary>
        /// Gets the table foot control object.
        /// </summary>
        IFoot Footer { get; }

        /// <summary>
        /// Gets a sorted enumeration of all row control objects (including header, body and footer rows).
        /// </summary>
        IEnumerable<IRow> AllRows { get; }
    }
}
