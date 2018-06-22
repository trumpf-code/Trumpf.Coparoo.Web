namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Div control object.
    /// </summary>
    public class Div : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("div");
    }
}
