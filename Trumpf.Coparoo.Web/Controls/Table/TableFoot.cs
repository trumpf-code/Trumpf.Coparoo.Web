namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Table foot control object.
    /// </summary>
    public class TableFoot : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("tfoot");

        /// <summary>
        /// Gets the enumeration of row control objects.
        /// </summary>
        public IEnumerable<TableRow> Rows => FindAll<TableRow>();

        /// <summary>
        /// Returns the row element at a specified index in the rows sequence.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Rows seqeunce is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of row elements in the rows sequence.</exception>
        /// <param name="index">The zero-based index of the row to retrieve.</param>
        /// <returns>The row element at the specified position in the rows sequence.</returns>
        public TableRow RowAt(int index)
        {
            return Rows.ElementAt(index);
        }
    }
}
