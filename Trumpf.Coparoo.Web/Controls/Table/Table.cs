namespace Trumpf.Coparoo.Web.Controls
{
    using System.Collections.Generic;
    using System.Linq;

    using OpenQA.Selenium;

    /// <summary>
    /// Table control object.
    /// </summary>
    public partial class Table : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.TagName("table");

        /// <summary>
        /// Gets the table head control object.
        /// </summary>
        public Head Header => Find<Head>();

        /// <summary>
        /// Gets the table body control object.
        /// </summary>
        public Body Content => Find<Body>();

        /// <summary>
        /// Gets the table foot control object.
        /// </summary>
        public Foot Footer => Find<Foot>();

        /// <summary>
        /// Gets a sorted enumeration of all row control objects (including header, body and footer rows).
        /// </summary>
        public IEnumerable<Row> AllRows => Header.Rows.Concat(Content.Rows).Concat(Footer.Rows);
    }
}
