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

namespace Trumpf.Coparoo.Web.PageTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exceptions;
    using Trumpf.Coparoo.Web.Internal;

    /// <summary>
    /// UI object type locators.
    /// </summary>
    internal static class Locate
    {
        private static Dictionary<Type, Type[]> parentToChildrenMap;
        private static Type[] pageObjectTypes;
        private static Type[] controlObjectTypes;
        private static Type[] uiaObjectTypes;
        private static readonly Func<Type, bool> pageObjectSelector = t => t.GetInterfaces().Contains(typeof(IPageObject));
        private static readonly Func<Type, bool> controlObjectSelector = t => t.GetInterfaces().Contains(typeof(IControlObject));

        /// <summary>
        /// Gets the retrievable app domain types.
        /// </summary>
        internal static Type[] Types
        {
            get
            {
                var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e =>
                {
                    try
                    {
                        return e.GetTypes();
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine($"Ignoring assembly  : {e.FullName}{Environment.NewLine}Message            : {exception.Message}{Environment.NewLine}Stack trace        : {exception.StackTrace}");
                        return new Type[] { };
                    }
                }).ToArray();
                return types;
            }
        }

        /// <summary>
        /// Gets the page object types in the current app domain.
        /// </summary>
        internal static Type[] PageObjectTypes => pageObjectTypes ?? (pageObjectTypes = UIObjectTypes(pageObjectSelector));

        /// <summary>
        /// Gets the control object types in the current app domain.
        /// </summary>
        internal static Type[] ControlObjectTypes => controlObjectTypes ?? (controlObjectTypes = UIObjectTypes(controlObjectSelector));

        /// <summary>
        /// Gets the page object types.
        /// </summary>
        private static Dictionary<Type, Type[]> ParentToChildMap
        {
            get
            {
                if (parentToChildrenMap == null)
                {
                    parentToChildrenMap = new Dictionary<Type, Type[]>();
                    var t = PageObjectTypes;
                    foreach (Type type in t)
                    {
                        if (!type.IsAbstract)
                        {
                            var interfaces = type.GetInterfaces();
                            var childOfInterfaces = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IChildOf<>));
                            if (childOfInterfaces.Any())
                            {
                                var parentTypes = childOfInterfaces.Select(i => i.GenericTypeArguments.First()).ToArray();
                                parentToChildrenMap.Add(type, parentTypes.Select(e => Resolve(e)).ToArray());
                            }
                        }
                    }
                }

                return parentToChildrenMap;
            }
        }

        /// <summary>
        /// Gets the UI object types in the current app domain.
        /// </summary>
        /// <param name="selector">The selection predicate.</param>
        /// <returns>The type array.</returns>
        internal static Type[] UIObjectTypes(Func<Type, bool> selector = null)
        {
            selector = selector ?? (t => true);
            bool mainSelector(Type t)
            {
                try
                {
                    return t.IsClass && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null && t.GetInterfaces().Contains(typeof(IUIObject));
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Skipping type <{t.Name}> during search for UIObjects: {e.Message}");
                    return false;
                }
            }

            uiaObjectTypes = uiaObjectTypes ?? Types.Where(t => mainSelector(t)).ToArray();
            return uiaObjectTypes.Where(selector).ToArray();
        }

        /// <summary>
        /// Gets the page object types.
        /// </summary>
        /// <param name="pageObject">The parent page object.</param>
        /// <returns>The child types.</returns>
        internal static IEnumerable<Type> ChildTypes(IPageObject pageObject) => ParentToChildMap.Where(e => e.Value.Contains(pageObject.GetType())).Select(e => e.Key);

        /// <summary>
        /// Clear all caches.
        /// </summary>
        internal static void ClearCaches()
        {
            parentToChildrenMap = null;
            pageObjectTypes = null;
            controlObjectTypes = null;
            uiaObjectTypes = null;
        }

        /// <summary>
        /// Resolve any page object interface type to an implementation.
        /// </summary>
        /// <param name="parentType">The type to resolve.</param>
        /// <returns>The resolved type.</returns>
        private static Type Resolve(Type parentType)
        {
            if (!parentType.IsInterface)
            {
                return parentType;
            }
            else
            {
                var matches = PageObjectTypes.Where(p => parentType.IsAssignableFrom(p));
                if (matches.Count() == 0)
                {
                    throw new ChildOfUsageException(parentType, "Could not resolve interface " + parentType.Name + " in " + typeof(IChildOf<>).Name + ".");
                }
                else
                {
                    if (matches.Count() > 1)
                    {
                        Trace.WriteLine($"Found more than one match for {parentType.Name}; taking the first one.");
                    }

                    return matches.First();
                }
            }
        }
    }
}