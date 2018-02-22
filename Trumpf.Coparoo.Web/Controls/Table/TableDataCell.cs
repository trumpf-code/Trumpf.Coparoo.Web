namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;

    /// <summary>
    /// Table data cell object.
    /// </summary>
    public class TableDataCell : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("td");

        /// <summary>
        /// Searches for a control object like provided and returns it.
        /// </summary>
        public T As<T>() where T : ControlObject
        {
            return Find<T>();
        }
    }
}
