namespace Trumpf.Coparoo.Web.Controls.Table
{
    using System.Collections.Generic;
    using System.Linq;

    using OpenQA.Selenium;

    /// <summary>
    /// Table row control object.
    /// </summary>
    public class TableRow : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("tr");

        /// <summary>
        /// Gets the enumeration of cell control objects.
        /// </summary>
        public IEnumerable<TableCell> Cells => FindAll<TableCell>();

        /// <summary>
        /// Returns the cell element at a specified index in the cells sequence.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Cells sequence is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of cell elements in the cells sequence.</exception>
        /// <param name="index">The zero-based index of the cell to retrieve.</param>
        /// <returns>The cell element at the specified position in the cells sequence.</returns>
        public TableCell CellAt(int index) => Cells.ElementAt(index);
    }
}
