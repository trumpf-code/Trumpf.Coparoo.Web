namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;

    /// <summary>
    /// Table head control object.
    /// </summary>
    public class TableHead : TableSegment
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("thead");
    }
}
