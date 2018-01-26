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
    using System.Linq;

    using Logging.Tree;
    using PageTests;

    /// <summary>
    /// Base class of all page objects.
    /// </summary>
    public abstract class TreeObject : UIObject, ITreeObject
    {
        /// <summary>
        /// Gets a value indicating whether the page object shall be returned, e.g. by the ON-method.
        /// </summary>
        bool ITreeObject.OnCondition => OnCondition;

        /// <summary>
        /// Gets a value indicating whether the page object shall be returned, e.g. by the ON-method.
        /// The default is: yes.
        /// </summary>
        protected virtual bool OnCondition => true;

        /// <summary>
        /// Gets the parent of this page object.
        /// </summary>
        public new IPageObject Parent => (IPageObject)base.Parent;

        /// <summary>
        /// Gets the dot tree representation.
        /// The tree does not contain generic type definition page objects.
        /// </summary>
        /// <returns>The dot tree representation of the page object tree.</returns>
        Tree ITreeObject.Tree
        {
            get
            {
                // create new dot tree with this one node for this page object
                Tree result = new Tree(new Node { NodeType = NodeType, Id = GetType().FullName, Caption = GetType().Name });

                // add the dot trees of all children
                foreach (var child in Children())
                {
                    var tree = ((ITreeObject)child).Tree;

                    // merge trees
                    result += tree;

                    // connect trees
                    result += new Edge { To = tree.Root.Id, From = result.Root.Id, Label = string.Empty };
                }

                // add the control object properties, currently ignoring lists etc.
                foreach (var property in GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    var propertyType = property.PropertyType;
                    var isControlObject = typeof(IControlObject).IsAssignableFrom(propertyType);
                    if (isControlObject)
                    {
                        var node = new Node() { Id = property.PropertyType.FullName, Caption = propertyType.Name, NodeType = NodeType.ControlObject, FrameColor = Logging.Tree.Color.Gray };
                        result += node;
                        result += new Edge { To = node.Id, From = result.Root.Id, Label = property.Name, Style = EdgeStyle.Dotted };
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the child page objects.
        /// Ignoring generic type definition page objects.
        /// </summary>
        /// <returns>The child page objects.</returns>
        public IEnumerable<IPageObject> Children()
        {
            return ((ITreeObject)this).Children<object>();
        }

        /// <summary>
        /// Gets the child page objects.
        /// If a child page object is a generic type definition that matches with the hint type, this type is returned.
        /// The hint is necessary since the type parameters cannot be "guessed".
        /// </summary>
        /// <typeparam name="TPageObjectChildHint">The hint type for a generic type definition page object child.</typeparam>
        /// <returns>The child page objects.</returns>
        IEnumerable<IPageObject> ITreeObject.Children<TPageObjectChildHint>()
        {
            List<IPageObject> result = new List<IPageObject>();
            foreach (var childPageObject in Locate.ChildTypes(this).Union(RootInternal.DynamicChildren(GetType())))
            {
                Type toAdd;
                Type hintType = typeof(TPageObjectChildHint);
                if (!childPageObject.IsGenericTypeDefinition)
                {
                    toAdd = childPageObject;
                }
                else if (hintType.IsGenericType && hintType.GetGenericTypeDefinition().Equals(childPageObject))
                {
                    // check if the type definition of the page object we are searching for, is the same as the child page object
                    // use the type parameters passed via the type argument
                    toAdd = hintType;
                }
                else
                {
                    continue;
                }

                // create and initialize child page object
                result.Add((Activator.CreateInstance(toAdd) as IUIObjectInternal).Init(this, true) as IPageObject);
            }

            return result;
        }

        /// <summary>
        /// Goto the page object, i.e. perform necessary action to make the page object visible on screen, do nothing if the page is already visible on screen.
        /// </summary>
        public virtual void Goto()
        {
        }
    }
}