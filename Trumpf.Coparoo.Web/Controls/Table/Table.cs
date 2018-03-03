namespace Trumpf.Coparoo.Web.Controls.Table
{
    using OpenQA.Selenium;

    /// <summary>
    /// Table control object.
    /// </summary>
    public class Table : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("table");

        /// <summary>
        /// Gets the table head control object.
        /// </summary>
        public Head Head => Find<Head>();

        /// <summary>
        /// Gets the table body control object.
        /// </summary>
        public Body Body => Find<Body>();

        /// <summary>
        /// Gets the table foot control object.
        /// </summary>
        public Foot Foot => Find<Foot>();
    }
}
