namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;

    /// <summary>
    /// Checkbox control object.
    /// Expects an input html element with attribute type="checkbox".
    /// </summary>
    public class Checkbox : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.XPath(".//input[@type='checkbox']");

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value => Node.GetAttribute("value");

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => Node.GetAttribute("name");

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox is checked.
        /// </summary>
        public bool Checked
        {
            get { return Node.Selected; }

            set
            {
                if (Checked != value)
                {
                    Node.Click();
                }
            }
        }
    }
}
