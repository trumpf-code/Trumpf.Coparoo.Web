namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Link control object.
    /// </summary>
    public class Link : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("a");

        /// <summary>
        /// Gets the link text.
        /// </summary>
        public string Text => Node.Text;

        /// <summary>
        /// Gets the link URL.
        /// </summary>
        public string URL => Node.GetAttribute("href");
    }
}
