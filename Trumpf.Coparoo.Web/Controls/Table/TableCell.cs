namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;

    /// <summary>
    /// Table data cell object.
    /// </summary>
    public class TableCell : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.XPath(".//th|.//td");

        /// <summary>
        /// Searches for a control object like provided and returns it.
        /// </summary>
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
