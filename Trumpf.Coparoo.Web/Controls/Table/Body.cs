namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Partial Table control object.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// Table body control object.
        /// </summary>
        public class Body : Segment
        {
            /// <summary>
            /// Gets the search pattern.
            /// </summary>
            protected override By SearchPattern => By.TagName("tbody");
        }
    }
}
