namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;

    /// <summary>
    /// Table control object.
    /// </summary>
    public class Table : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("table");

        /// <summary>
        /// Gets the table head control object.
        /// </summary>
        public TableHead Head => Find<TableHead>();

        /// <summary>
        /// Gets the table body control object.
        /// </summary>
        public TableBody Body => Find<TableBody>();

        /// <summary>
        /// Gets the table foot control object.
        /// </summary>
        public TableFoot Foot => Find<TableFoot>();
    }
}
