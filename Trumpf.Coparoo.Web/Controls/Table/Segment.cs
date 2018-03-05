namespace Trumpf.Coparoo.Web.Controls
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Partial Table control object.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// Generic table segment control object.
        /// </summary>
        public abstract class Segment : ControlObject
        {
            /// <summary>
            /// Gets the enumeration of row control objects.
            /// </summary>
            public IEnumerable<Row> Rows => FindAll<Row>();

            /// <summary>
            /// Returns the row element at a specified index in the row sequence.
            /// </summary>
            /// <exception cref="System.ArgumentNullException">Row sequence is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of row elements in the row sequence.</exception>
            /// <param name="index">The zero-based index of the row to retrieve.</param>
            /// <returns>The row element at the specified position in the row sequence.</returns>
            public Row RowAt(int index) => Rows.ElementAt(index);
        }
    }
}
