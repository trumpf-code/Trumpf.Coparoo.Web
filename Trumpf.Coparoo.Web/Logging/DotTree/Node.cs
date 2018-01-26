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
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Node class.
    /// </summary>
    internal class Node
    {
        private string id;

        /// <summary>
        /// Gets or sets the node type.
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// Gets or sets the frame color.
        /// </summary>
        public Color FrameColor { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string Id
        {
            get { return Math.Abs(id.GetHashCode()).ToString(); }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the node caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets the node label.
        /// </summary>
        private string Label => "<<I>" + GetDescription(NodeType) + ":</I> " + Caption.Replace(Environment.NewLine, "\\n") + ">";

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Compare two nodes.
        /// </summary>
        /// <param name="obj">The other node.</param>
        /// <returns>Whether both nodes have the same ID.</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(GetType()))
            {
                return (obj as Node).Id.Equals(Id);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() => Id + "[ " + "color=black, margin=0.05, shape=box, style=filled, fillcolor=" + GetDescription(FrameColor) + ", fontsize=20, fontname=Arial" + " label = " + Label + "];";

        /// <summary>
        /// Gets the description of an enum.
        /// </summary>
        /// <param name="value">The enum.</param>
        /// <exception cref="InvalidOperationException">Thrown if the enum does not have a <see cref="DescriptionAttribute"/> attribute.</exception>
        /// <returns>Returns the description.</returns>
        internal static string GetDescription(Enum value)
        {
            System.Reflection.FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null)
            {
                throw new InvalidOperationException(string.Format(@"The enum does not have a {0} attribute.", typeof(DescriptionAttribute).FullName));
            }

            return attribute.Description;
        }
    }
}
