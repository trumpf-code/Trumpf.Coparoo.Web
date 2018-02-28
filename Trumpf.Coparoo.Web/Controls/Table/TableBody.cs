namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;

    /// <summary>
    /// Table body control object.
    /// </summary>
    public class TableBody : TableSegment
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("tbody");
    }
}
