// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Web.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web.Waiting;

    /// <summary>
    /// DOM object interface.
    /// </summary>
    public interface IUIObject
    {
        /// <summary>
        /// Gets the parent of this UI object.
        /// </summary>
        IUIObject Parent { get; }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        IUIObjectNode Node { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the UI object can respond to user interaction.
        /// </summary>
        Bool Enabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its child controls are displayed.
        /// </summary>
        Bool Displayed { get; }

        /// <summary>
        /// Gets a value indicating whether the UI object's node exists in the UI tree.
        /// </summary>
        Bool Exists { get; }

        /// <summary>
        /// Gets a System.Drawing.Point object containing the coordinates of the upper-left corner of this element relative to the upper-left corner of the page.
        /// </summary>
        Point Location { get; }

        /// <summary>
        /// Get the specific page object.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
        /// <typeparam name="TPageObject">Type of the page object.</typeparam>
        /// <returns>Type of page object.</returns>
        TPageObject On<TPageObject>(Predicate<TPageObject> condition = null) where TPageObject : IPageObject;

        /// <summary>
        /// Snap the page object, i.e. associate it with a specific node in the UI tree. This disables lazy node evaluation.
        /// </summary>
        /// <returns>Whether the snap succeeded.</returns>
        bool TrySnap();

        /// <summary>
        /// Goto the page object.
        /// Throws if the page object cannot be navigated to.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for target page object.</param>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The target page object.</returns>
        TPageObject Goto<TPageObject>(Predicate<TPageObject> condition = null) where TPageObject : IPageObject;

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <returns>The control object.</returns>
        TControl Find<TControl>(By pattern = null, Predicate<ISearchContext> predicate = null) where TControl : IControlObject;

        /// <summary>
        /// Gets all matching controls.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <returns>The control enumeration.</returns>
        IEnumerable<TControl> FindAll<TControl>(By pattern = null, Predicate<ISearchContext> predicate = null) where TControl : IControlObject;

        /// <summary>
        /// Scroll to the UI object.
        /// </summary>
        void ScrollTo();

        /// <summary>
        /// Scroll to the UI object.
        /// </summary>
        /// <param name="timeout">The timeout to wait for the object to become visible between page downs.</param>
        void ScrollTo(TimeSpan timeout);
    }
}