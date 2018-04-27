namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Text input control object.
    /// Expects an input html element with attribute type="text".
    /// </summary>
    public class Input : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.XPath(".//input[@type='text']");

        /// <summary>
        /// Gets or sets the text content.
        /// </summary>
        public string Text
        {
            get { return Node.GetAttribute("value"); }

            set
            {
                if (Text != value)
                {
                    if (Text != string.Empty)
                    {
                        Node.Clear();
                    }

                    Node.SendKeys(value);
                }
            }
        }
    }
}
