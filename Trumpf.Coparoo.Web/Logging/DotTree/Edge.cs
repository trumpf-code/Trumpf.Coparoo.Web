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

namespace Trumpf.Coparoo.Web.Logging.Tree
{
    /// <summary>
    /// Edge class.
    /// </summary>
    internal class Edge
    {
        private const char quotation = '\"';

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        public EdgeStyle Style { get; set; } = EdgeStyle.Solid;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() => $"{From} -> {To} [label={quotation}{Label}{quotation}, fontname=Arial, style={Node.GetDescription(Style)}];";
    }
}
