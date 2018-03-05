namespace Trumpf.Coparoo.Web.Controls
{
    using System.Collections.Generic;
    using System.Linq;

    using OpenQA.Selenium;

    /// <summary>
    /// Partial Table control object.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// Table row control object.
        /// </summary>
        public class Row : ControlObject
        {
            /// <summary>
            /// Gets the search pattern.
            /// </summary>
            protected override By SearchPattern => By.TagName("tr");

            /// <summary>
            /// Gets the enumeration of cell control objects.
            /// </summary>
            public IEnumerable<Cell> Cells => FindAll<Cell>();

            /// <summary>
            /// Returns the cell element at a specified index in the cell sequence.
            /// </summary>
            /// <exception cref="System.ArgumentNullException">Cell sequence is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of cell elements in the cell sequence.</exception>
            /// <param name="index">The zero-based index of the cell to retrieve.</param>
            /// <returns>The cell element at the specified position in the cell sequence.</returns>
            public Cell CellAt(int index) => Cells.ElementAt(index);

            /// <summary>
            /// Returns the cell element at a specified index in the cell sequence as a control object like provided.
            /// </summary>
            /// <exception cref="System.ArgumentNullException">Cell sequence is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of cell elements in the cell sequence.</exception>
            /// <typeparam name="T">Control object type.</typeparam>
            /// <param name="index">The zero-based index of the cell to retrieve.</param>
            /// <returns>The cell element at the specified position in the cell sequence as a control object like provided.</returns>
            public T CellAt<T>(int index) where T : ControlObject => Cells.ElementAt(index).As<T>();
        }
    }
}
