namespace Trumpf.Coparoo.Web.Controls
{
    using System;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    /// <summary>
    /// Select control object.
    /// </summary>
    public class Select : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("select");

        /// <summary>
        /// Gets the Selenium SelectElement.
        /// </summary>
        public SelectElement SelectElement => new SelectElement(Node);
        
        /// <summary>
        /// Gets the text of the selected option.
        /// </summary>
        public string Text => SelectElement.SelectedOption.Text;

        /// <summary>
        /// Select all options by the text displayed. 
        /// </summary>
        /// <param name="text">The text of the option to be selected. If an exact match is not found, this method will perform a substring match.</param>
        public void SelectByText(string text) => SelectElement.SelectByText(text);

        /// <summary>
        /// Gets the index of the selected option.
        /// </summary>
        public int Index => Int32.Parse(SelectElement.SelectedOption.GetAttribute("index"));

        /// <summary>
        /// Select the option by the index, as determined by the "index" attribute of the element. 
        /// </summary>
        /// <param name="index">The value of the index attribute of the option to be selected.</param>
        public void SelectByIndex(int index) => SelectElement.SelectByIndex(index);

        /// <summary>
        /// Gets the value of the selected option.
        /// </summary>
        public string Value => SelectElement.SelectedOption.GetAttribute("value");

        /// <summary>
        /// Select an option by value.
        /// </summary>
        /// <param name="value">The value of the option to be selected.</param>
        public void SelectByValue(string value) => SelectElement.SelectByValue(value);
    }
}
