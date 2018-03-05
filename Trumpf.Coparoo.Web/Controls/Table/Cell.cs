namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;

    /// <summary>
    /// Partial Table control object.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// Table data cell object.
        /// </summary>
        public class Cell : ControlObject
        {
            /// <summary>
            /// Gets the search pattern.
            /// </summary>
            protected override By SearchPattern => By.XPath(".//th|.//td");

            /// <summary>
            /// Searches for a control object like provided and returns it.
            /// </summary>
            /// <typeparam name="T">Control object type.</typeparam>
            /// <returns>Cell object as a control object like provided.</returns>
            public T As<T>() where T : ControlObject => Find<T>();

            /// <summary>
            /// Returns true if node element is header cell, otherwise false.
            /// </summary>
            public bool IsHeaderCell => Node.TagName.Equals("th");

            /// <summary>
            /// Returns true if node element is data cell, otherwise false.
            /// </summary>
            public bool IsDataCell => Node.TagName.Equals("td");
        }
    }
}
