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

namespace Trumpf.Coparoo.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exceptions;
    using Trumpf.Coparoo.Web.Internal;

    /// <summary>
    /// The page object locator class.
    /// </summary>
    internal class PageObjectLocator : IPageObjectLocator
    {
        private Dictionary<Type, HashSet<IPageObject>> typeToPageObjectMap;
        private readonly ITabObject rootObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageObjectLocator"/> class.
        /// </summary>
        /// <param name="processObject">The process object.</param>
        internal PageObjectLocator(ITabObject processObject)
        {
            Clear();
            rootObject = processObject;
        }

        /// <summary>
        /// Gets the cached page object type count.
        /// </summary>
        public int CacheTypeCount => typeToPageObjectMap.Count;

        /// <summary>
        /// Gets the cached object count (for all page object types).
        /// </summary>
        public int CacheObjectCount => typeToPageObjectMap.Sum(e => e.Value.Count);

        /// <summary>
        /// Clear the register.
        /// </summary>
        public void Clear() => typeToPageObjectMap = new Dictionary<Type, HashSet<IPageObject>>();

        /// <summary>
        /// Register the page object type with the given object.
        /// </summary>
        /// <typeparam name="TPageObject">The page object type.</typeparam>
        /// <param name="target">The object to register.</param>
        public void Register<TPageObject>(IPageObject target)
        {
            if (typeToPageObjectMap.TryGetValue(typeof(TPageObject), out HashSet<IPageObject> o))
            {
                o.Add(target as IPageObject);
            }
            else
            {
                typeToPageObjectMap.Add(typeof(TPageObject), new HashSet<IPageObject>(new IPageObjectEqualityComparer()));
                Register<TPageObject>(target);
            }
        }

        /// <summary>
        /// Unregister the page object type.
        /// </summary>
        /// <typeparam name="TPageObject">The page object type.</typeparam>
        public void Unregister<TPageObject>()
        {
            if (typeToPageObjectMap.TryGetValue(typeof(TPageObject), out HashSet<IPageObject> o))
            {
                o.Clear();
            }
        }

        /// <summary>
        /// Try get the registered objects for the given page object type.
        /// </summary>
        /// <typeparam name="TPageObject">The page object type.</typeparam>
        /// <param name="targets">The object, if any.</param>
        /// <returns>Whether a registered object was found.</returns>
        public bool TryGet<TPageObject>(out IEnumerable<TPageObject> targets)
        {
            if (typeToPageObjectMap.TryGetValue(typeof(TPageObject), out HashSet<IPageObject> o))
            {
                targets = o.Cast<TPageObject>();
                return true;
            }

            targets = null;
            return false;
        }

        /// <summary>
        /// Find a page object in the page object tree.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
        /// <typeparam name="TPageObject">The page object type to search for.</typeparam>
        /// <returns>The page object.</returns>
        public TPageObject Find<TPageObject>(Predicate<TPageObject> condition) where TPageObject : IPageObject
        {
            if (TryFind(condition, out TPageObject result))
            {
                return result;
            }

            // clean cache and try once again; maybe the page object assembly was loaded after the last call to Find
            PageTests.Locate.ClearCaches();
            if (TryFind(condition, out result))
            {
                return result;
            }
            else
            {
                throw new PageObjectNotFoundException<TPageObject>();
            }
        }

        /// <summary>
        /// Find a page object in the page object tree.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
        /// <param name="result">The result page object.</param>
        /// <typeparam name="TPageObject">The page object type to search for.</typeparam>
        /// <returns>The page object.</returns>
        private bool TryFind<TPageObject>(Predicate<TPageObject> condition, out TPageObject result) where TPageObject : IPageObject
        {
            Queue<IPageObject> q = new Queue<IPageObject>();

            if (TryGet(out IEnumerable<TPageObject> registeredObjects))
            {
                foreach (var a in registeredObjects)
                {
                    q.Enqueue(a);
                }
            }

            q.Enqueue(rootObject);

            while (q.Count > 0)
            {
                IPageObject current = q.Dequeue();

                // interface types match any implementation, whereas classes must match exactly
                var match = typeof(TPageObject).IsInterface ? current is TPageObject : current.GetType().Equals(typeof(TPageObject));
                if (match)
                {
                    TPageObject page = (TPageObject)current;
                    Register<TPageObject>(page);

                    if ((page as ITreeObject).OnCondition && condition(page))
                    {
                        var parent = page.Parent;
                        result = parent == null ? page : (TPageObject)(Activator.CreateInstance(page.GetType()) as IUIObjectInternal).Init(parent, true);
                        return true;
                    }
                }

                // traverse children
                var c = ((ITreeObject)current).Children<TPageObject>();

                foreach (IPageObject childPageObject in c)
                {
                    // generic page object must be child-less
                    if (current.GetType().IsGenericType)
                    {
                        Trace.TraceWarning($"Ignoring children of generic page object type <{current.GetType().Name}>");
                        continue;
                    }

                    q.Enqueue(childPageObject);
                }
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Equality comparer.
        /// </summary>
        private class IPageObjectEqualityComparer : IEqualityComparer<IPageObject>
        {
            /// <summary>
            /// Equality comparer.
            /// </summary>
            /// <param name="x">The first value.</param>
            /// <param name="y">The second value.</param>
            /// <returns>Whether both page objects are equal.</returns>
            public bool Equals(IPageObject x, IPageObject y)
            {
                if ((!x.GetType().Equals(y.GetType())) || (x.Parent == null && y.Parent != null) || (y.Parent == null && x.Parent != null))
                {
                    return false;
                }
                else if (y.Parent == null && x.Parent == null)
                {
                    return true;
                }
                else
                {
                    return x.Parent.GetType().Equals(y.Parent.GetType());
                }
            }

            /// <summary>
            /// Get the hash code.
            /// Put all in one bucket.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>The hash code.</returns>
            public int GetHashCode(IPageObject obj) => 1;
        }
    }
}