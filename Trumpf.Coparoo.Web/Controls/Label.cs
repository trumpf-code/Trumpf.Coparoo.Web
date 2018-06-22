namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Label control object.
    /// </summary>
    public class Label : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("label");

        /// <summary>
        /// Gets the label text.
        /// </summary>
        public string Text => Node.Text;
    }
}
