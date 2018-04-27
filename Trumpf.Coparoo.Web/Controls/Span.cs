namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Span control object.
    /// </summary>
    public class Span : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("span");

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text => Node.Text;
    }
}
