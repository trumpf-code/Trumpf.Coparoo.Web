﻿// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    /// Interface for table row control object.
    /// </summary>
    public interface IRow : IControlObject
    {
        /// <summary>
        /// Gets the enumeration of cell control objects.
        /// </summary>
        IEnumerable<ICell> Cells { get; }

        /// <summary>
        /// Returns the cell element at a specified index in the cell sequence.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Cell sequence is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of cell elements in the cell sequence.</exception>
        /// <param name="index">The zero-based index of the cell to retrieve.</param>
        /// <returns>The cell element at the specified position in the cell sequence.</returns>
        ICell CellAt(int index);

        /// <summary>
        /// Returns the cell element at a specified index in the cell sequence as a control object like provided.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Cell sequence is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of cell elements in the cell sequence.</exception>
        /// <typeparam name="T">Control object type.</typeparam>
        /// <param name="index">The zero-based index of the cell to retrieve.</param>
        /// <returns>The cell element at the specified position in the cell sequence as a control object like provided.</returns>
        T CellAt<T>(int index) where T : IControlObject;
    }
}
