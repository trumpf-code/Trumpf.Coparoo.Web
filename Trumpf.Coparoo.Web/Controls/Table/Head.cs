namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Partial Table control object.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// Table head control object.
        /// </summary>
        public class Head : Segment
        {
            /// <summary>
            /// Gets the search pattern.
            /// </summary>
            protected override By SearchPattern => By.TagName("thead");
        }
    }
}
