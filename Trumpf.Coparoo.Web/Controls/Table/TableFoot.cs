namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;

    /// <summary>
    /// Table foot control object.
    /// </summary>
    public class TableFoot : TableSegment
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("tfoot");
    }
}
