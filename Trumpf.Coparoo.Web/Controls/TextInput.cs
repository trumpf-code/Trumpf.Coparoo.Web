namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Text input control object.
    /// Expects an input html element with attribute type="text".
    /// </summary>
    public class TextInput : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.XPath(".//input[@type='text']");

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get { return Node.GetAttribute("value"); }

            set
            {
                var text = Text;
                if (text != value)
                {
                    if (text != string.Empty)
                    {
                        Node.Clear();
                    }

                    Node.SendKeys(value);
                }
            }
        }
    }
}
