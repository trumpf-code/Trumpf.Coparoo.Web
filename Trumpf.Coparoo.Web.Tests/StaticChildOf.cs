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

#if DEBUG 
namespace Trumpf.Coparoo.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using Trumpf.Coparoo.Web;
    using Trumpf.Coparoo.Web.Exceptions;
    using Trumpf.Coparoo.Web.Internal;

    /// <summary>
    /// Test class.
    /// </summary>
    [TestFixture]
    public class PageObjectLocatorStatic
    {
        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheRootObjectIsFreshh_ThenTheCacheIsEmpty()
        {
            // Act
            var root = new A();

            // Check
            Assert.AreEqual(0, (root as ITabObjectInternal).PageObjectLocator.CacheObjectCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenThePageObjectsAreAccessed_ThenTheyAreCached()
        {
            // Act
            var root = new A();
            var a = root.On<A>();
            var b = root.On<B>();

            // Check
            Assert.AreEqual(2, (root as ITabObjectInternal).PageObjectLocator.CacheObjectCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenThePageObjectCacheIsCleared_ThenItIsEmpty()
        {
            // Act
            var root = new A() as ITabObjectInternal;
            var a = root.On<A>();
            var b = root.On<B>();
            root.PageObjectLocator.Clear();

            // Check
            Assert.AreEqual(0, root.PageObjectLocator.CacheObjectCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenAPageObjectIsUnregistered_ThenIsDisappearsFromTheCache()
        {
            // Act
            var root = new A() as ITabObjectInternal;
            var b = root.On<B>();
            root.PageObjectLocator.Unregister<B>();

            // Check
            Assert.AreEqual(0, root.PageObjectLocator.CacheObjectCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenAPageObjectIsUnregistedBeforeItWasCached_ThenNoExceptionIsThrown()
        {
            // Act
            var a = new A() as ITabObjectInternal;
            a.PageObjectLocator.Unregister<B>();

            // Check
            Assert.AreEqual(0, a.PageObjectLocator.CacheObjectCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheSamePageObjectIsAccessedOnDifferentPath_ThenBothAreResultsAreCached()
        {
            // Act
            var a = new A() as ITabObjectInternal;
            var gWithParentA = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
            var gWithParentB = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

            // Check
            Assert.AreEqual(2, (a as ITabObjectInternal).PageObjectLocator.CacheObjectCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheSamePageObjectIsAccessedOnDifferentPath_ThenExplicitOnConditionIsEffective()
        {
            // Act
            var a = new A() as ITabObjectInternal;
            var gWithParentA = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
            var gWithParentB = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

            // Check
            Assert.AreEqual(typeof(A), gWithParentA.Parent.GetType());
            Assert.AreEqual(typeof(B), gWithParentB.Parent.GetType());
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheSamePageObjectIsAccessedOnDifferentPathMultipleTime_ThenDuplicatesDoNotFurtherIncreaseTheCacheCount()
        {
            // Act
            var a = new A() as ITabObjectInternal;
            var gWithParentA = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
            var gWithParentB = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));
            var gWithParentA2 = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
            var gWithParentB2 = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

            // Check
            Assert.AreEqual(2, a.PageObjectLocator.CacheObjectCount);
            Assert.AreEqual(typeof(A), gWithParentA.Parent.GetType());
            Assert.AreEqual(typeof(B), gWithParentB.Parent.GetType());
            Assert.AreEqual(typeof(A), gWithParentA2.Parent.GetType());
            Assert.AreEqual(typeof(B), gWithParentB2.Parent.GetType());
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheDotTreeIsGenerated_ThenIsContainsTheExpectedNodeAndEdgeCount()
        {
            // Act
            var tree = ((ITreeObject)new A()).Tree;

            // Check
            Assert.AreEqual(4, tree.EdgeCount);
            Assert.AreEqual(4, tree.NodeCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheDotTreeIsWrittenToDisk_ThenAFileIsCreated()
        {
            string file = null;
            try
            {
                // Act
                var root = new A();
                file = root.WriteTree();

                // Check
                Assert.IsTrue(File.Exists(file));
            }
            finally
            {
                // Clean up
                if (file != null && File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheOnMethodIsCalled_ThenStaticallyAddedChildRelationAreEffective()
        {
            // Act
            var root = new A();
            var a = root.On<A>();
            var b = root.On<B>();
            var cobject = root.On<C<object>>();
            var cint = root.On<C<int>>();
            var dobject = root.On<D<object>>();
            var dint = root.On<D<int>>();

            // Check
            Assert.AreEqual(typeof(A), root.GetType());
            Assert.AreEqual(typeof(A), a.GetType());
            Assert.AreEqual(typeof(B), b.GetType());
            Assert.AreEqual(typeof(C<object>), cobject.GetType());
            Assert.AreEqual(typeof(C<int>), cint.GetType());
            Assert.AreEqual(typeof(D<object>), dobject.GetType());
            Assert.AreEqual(typeof(D<int>), dint.GetType());
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenMultipleRootObjectAreCreated_ThenTheirCachesAreSeperated()
        {
            // Act
            var root1 = new A() as ITabObjectInternal;
            var root2 = new A() as ITabObjectInternal;
            B b = root1.On<B>(); // increase the counter by one

            // Check
            Assert.AreEqual(1, root1.PageObjectLocator.CacheTypeCount);
            Assert.AreEqual(0, root2.PageObjectLocator.CacheTypeCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheOnMethodIsCalledManyTimes_ThenTheCacheCountIncreases()
        {
            // Act
            var root = new A() as ITabObjectInternal; // create the root object, initially the cache count shall be zero
            root.On<A>(); // add all new relation, which shall increase the counter once per step
            root.On<B>();
            root.On<C<object>>();
            root.On<C<int>>();
            root.On<D<object>>();
            root.On<D<int>>();

            // Check
            Assert.AreEqual(6, root.PageObjectLocator.CacheTypeCount);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Test]
        public void WhenTheOnMethodIsCalledWithKnownObject_ThenTheCacheDoesNotIncreases()
        {
            // Act
            var root = new A() as ITabObjectInternal; // create the root object, initially the cache count shall be zero
            foreach (var i in Enumerable.Range(0, 3))
            {
                root.On<A>();
                root.On<B>();
                root.On<C<object>>();
                root.On<C<int>>();
                root.On<D<object>>();
                root.On<D<int>>();
            }

            // Check
            Assert.AreEqual(6, root.PageObjectLocator.CacheTypeCount);
        }

        /// <summary>
        /// Test method.
        /// Generic page object children of generic page objects are not supported
        /// </summary>
        [Test]
        public void WhenTheOnMethodIsCalledWithAGenericChildOfAGenericChild_ThenAnExceptionIsThrown()
        {
            // Act
            try
            {
                new A().On<E<object>>(); // E<T> is child of D<T>
            }
            catch (PageObjectNotFoundException<E<object>>)
            {
                return;
            }

            Assert.Fail();
        }

        /// <summary>
        /// Test method.
        /// Generic page object children of generic page objects are not supported
        /// </summary>
        [Test]
        public void WhenTheOnMethodIsCalledWithAGenericChildOfAGenericChildThatHasMultipleTypeParameters_ThenAnExceptionIsThrown()
        {
            try
            {
                // Act
                new A().On<F<object, object>>(); // F<T, TT> is child of D<T>
            }
            catch (PageObjectNotFoundException<F<object, object>>)
            {
                return;
            }

            Assert.Fail();
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class A : TabObject
        {
            protected override string Url => null;

            protected override Func<IWebDriver> Creator => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class B : PageObject, IChildOf<A>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class C<T> : PageObject, IChildOf<A>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class D<T> : PageObject, IChildOf<A>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class E<T> : PageObject, IChildOf<D<T>>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class F<T, TT> : PageObject, IChildOf<D<T>>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class G : PageObject, IChildOf<A>, IChildOf<B>
        {
            protected override By SearchPattern => null;
        }

        /// <summary>
        /// Helper class.
        /// </summary>
        private class B2 : B
        {
        }
    }
}
#endif