namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;

    /// <summary>
    /// Table head cell control object.
    /// </summary>
    public class TableHeadCell : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("th");

        /// <summary>
        /// Searches for a control object like provided and returns it.
        /// </summary>
        public T As<T>() where T : ControlObject
        {
            return Find<T>();
        }
    }
}
