namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Button control object.
    /// </summary>
    public class Button : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("button");

        /// <summary>
        /// Gets the button text.
        /// </summary>
        public string Text => Node.Text;
    }
}
