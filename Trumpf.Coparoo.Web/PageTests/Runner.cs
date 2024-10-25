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
    using System.Reflection;
    using System.Runtime.ExceptionServices;

    using Exceptions;
    using Statistics;
    using Trumpf.Coparoo.Web.Internal;
    using Trumpf.Coparoo.Web.Logging.Tree;

    /// <summary>
    /// Page object test class runner.
    /// </summary>
    public static class TestRunners
    {
        private static Type[] testClassTypes;

        /// <summary>
        /// Gets the page object test class types of the entire APP-Domain.
        /// </summary>
        private static Type[] TestClassTypes => testClassTypes = testClassTypes ?? Locate.Types.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPageObjectTestClass<>))).ToArray();

        /// <summary>
        /// Get the root a page object.
        /// </summary>
        /// <param name="source">The page object.</param>
        /// <returns>The root.</returns>
        private static ITabObject Root(IPageObject source) => ((IUIObjectInternal)source).Root;

        /// <summary>
        /// Run tests for this page object.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="writeTree">Whether to write the result tree.</param>
        /// <param name="filter">The test method filter predicate.</param>
        /// <param name="treeBaseFilename">Tree file name.</param>
        /// <returns>This page object.</returns>
        public static void Test(this IPageObject source, bool writeTree = false, Func<MethodInfo, Type, bool> filter = null, string treeBaseFilename = null) => Test(source, filter, writeTree, new PageObjectStatistics(), treeBaseFilename);

        /// <summary>
        /// Run tests for this page object.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="writeTree">Whether to write the result tree.</param>
        /// <param name="filter">The test method filter predicate.</param>
        /// <param name="treeBaseFilename">Tree file name.</param>
        /// <returns>This page object.</returns>
        internal static Tree TestInternal(this IPageObject source, bool writeTree = false, Func<MethodInfo, Type, bool> filter = null, string treeBaseFilename = null) => Test(source, filter, writeTree, new PageObjectStatistics(), treeBaseFilename);

        /// <summary>
        /// Run all tests with the <c>PageTestAttribute</c>.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="writeTree">Whether to write the result tree.</param>
        /// <param name="filter">The test method filter predicate.</param>
        /// <param name="treeBaseFilename">Tree file name.</param>
        /// <returns>This page object.</returns>
        public static void TestBottomUp(this IPageObject source, bool writeTree = false, Func<MethodInfo, Type, bool> filter = null, string treeBaseFilename = null) => TestBottomUp(source, filter, writeTree, new PageObjectStatistics(), treeBaseFilename);

        /// <summary>
        /// Run all tests with the <c>PageTestAttribute</c>.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="filter">The test method filter predicate.</param>
        /// <param name="writeTree">Whether to write the result tree.</param>
        /// <param name="treeBaseFilename">Tree file name.</param>
        /// <returns>This page object.</returns>
        internal static Tree TestBottomUpInternal(this IPageObject source, Func<MethodInfo, Type, bool> filter = null, bool writeTree = false, string treeBaseFilename = null) => TestBottomUp(source, filter, writeTree, new PageObjectStatistics(), treeBaseFilename);

        /// <summary>
        /// Run all tests with the <c>PageTestAttribute</c>.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="pageObjectStatistics">Whether to write the result tree.</param>
        /// <param name="filter">The test method filter predicate.</param>
        /// <param name="writeTree">Whether to write the result tree.</param>
        /// <param name="treeBaseFilename">Tree file name.</param>
        /// <returns>This page object.</returns>
        private static Tree TestBottomUp(this IPageObject source, Func<MethodInfo, Type, bool> filter, bool writeTree, PageObjectStatistics pageObjectStatistics, string treeBaseFilename)
        {
            Trace.WriteLine($"Run tests for {source.GetType().FullName}");

            // run tests for every child
            foreach (var child in source.Children())
            {
                TestBottomUp(child, filter, false, pageObjectStatistics, treeBaseFilename);
            }

            // run tests for this page object
            return Test(source, filter, writeTree, pageObjectStatistics, treeBaseFilename);
        }

        /// <summary>
        /// Run tests for this page object.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="filter">The test method filter predicate.</param>
        /// <param name="writeTree">Whether to write the result tree.</param>
        /// <param name="pageObjectStatistics">The page object statistics.</param>
        /// <param name="treeBaseFilename">Tree file name.</param>
        /// <returns>This page object.</returns>
        private static Tree Test(this IPageObject source, Func<MethodInfo, Type, bool> filter, bool writeTree, PageObjectStatistics pageObjectStatistics, string treeBaseFilename)
        {
            filter = filter ?? ((e, f) => true);
            Tree result = null;
            try
            {
                foreach (Type classWithTests in source.TestClasses())
                {
                    Trace.WriteLine($"Found page test class for current class {source.GetType()}: {classWithTests}");

                    // create and initialize page test class
                    IPageObjectTestsInternal instance;
                    try
                    {
                        instance = (IPageObjectTestsInternal)Root(source)
                            .Configuration
                            .DependencyRegistrator
                            .Register(classWithTests)
                            .Resolve(classWithTests);
                    }
                    catch (Stashbox.Exceptions.ResolutionFailedException exception)
                    {
                        throw new TypeResolutionFailedException(exception, $"Configure the resolver via property '{nameof(Configuration.DependencyRegistrator)}' in class '{Root(source).GetType().FullName}'.");
                    }

                    instance.Init(source);

                    // check if tests should be executed
                    if (instance.Runnable)
                    {
                        Trace.WriteLine("Runnable returned true; running tests...");
                    }
                    else
                    {
                        Trace.WriteLine("Runnable returned false; skipping tests");
                        continue;
                    }

                    // get page test methods, and order them from top to bottom
                    // ordering does not respect inheritance
                    var methods = classWithTests.GetMethods().Where(m => m.GetCustomAttributes(typeof(PageTestAttribute), false).Length > 0);
                    var methodsByLine = methods.OrderBy(asd => asd.GetCustomAttribute<PageTestAttribute>(true).Line);
                    var matchingMethodsByLine = methodsByLine.Where(f => filter(f, classWithTests));

                    int testMethodsCount = matchingMethodsByLine.Count();
                    if (testMethodsCount > 0)
                    {
                        Trace.WriteLine($"Found {testMethodsCount} test methods");
                    }
                    else
                    {
                        Trace.WriteLine("No test method found; skipping test class");
                        continue;
                    }

                    Trace.WriteLine($"Execute function {nameof(IPageObjectTests.BeforeFirstTest)}...");
                    instance.BeforeFirstTest();

                    // check if tests should be executed
                    if (!instance.ReadyToRun)
                    {
                        throw new TestNotReadyToRunException(classWithTests.FullName);
                    }
                    else
                    {
                        Trace.WriteLine("Tests are ready to run...");
                    }

                    // create class statistics object
                    TestClassStatistic testClassStatistic = new TestClassStatistic(classWithTests);

                    // execute test methods
                    try
                    {
                        foreach (var testMethod in matchingMethodsByLine)
                        {
                            Test(testClassStatistic, instance, testMethod);
                        }

                        Trace.WriteLine($"Execute function {nameof(IPageObjectTests.AfterLastTest)}...");
                        instance.AfterLastTest();
                    }
                    finally
                    {
                        pageObjectStatistics.Add(source, testClassStatistic);
                    }
                }
            }
            finally
            {
                result = ((ITreeObject)Root(source)).Tree;
                foreach (var pair in pageObjectStatistics.Stats)
                {
                    // merge trees
                    result += pair.Value;
                    result += new Edge { To = pair.Value.Root.Id, From = new Node() { Id = pair.Key.FullName }.Id };
                }

                if (writeTree)
                {
                    var treeFilePath = result.WriteGraph(treeBaseFilename);
                    Trace.WriteLine($"Graph written to '{treeFilePath}'.");
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the test classes associated with this page object.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <returns>The test classes associated with this page object.</returns>
        internal static IEnumerable<Type> TestClasses(this IPageObject source)
        {
            var interfaces = source.GetType().GetInterfaces();
            var pageObjectInterfaces = interfaces.Where(e => e.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IPageObject)));
            var matchingTypes = pageObjectInterfaces.Union(new[] { source.GetType() });
            var result = TestClassTypes.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPageObjectTestClass<>) && matchingTypes.Contains(i.GenericTypeArguments.First()))).ToArray();
            return result;
        }

        /// <summary>
        /// Run tests and collect statistics.
        /// </summary>
        /// <param name="testClassStatistic">The test class statistics.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="testMethod">The test method.</param>
        private static void Test(TestClassStatistic testClassStatistic, IPageObjectTests instance, MethodInfo testMethod)
        {
            Trace.WriteLine($"Executing page test <{testMethod.Name}>");

            var startTimeForCurrentIteration = DateTime.Now;
            try
            {
                // execute test method
                testMethod.Invoke(instance, null);

                // add method statistics
                testClassStatistic.Add(new TestMethodStatistic() { MethodInfo = testMethod, Info = null, Start = startTimeForCurrentIteration, Success = true });
            }
            catch (Exception e)
            {
                Exception exceptionForTrace = e;
                if (e is TargetInvocationException && e.InnerException != null)
                {
                    exceptionForTrace = e.InnerException;
                }

                Trace.WriteLine($"Exception in page test {testMethod.Name}: {e.Message}{Environment.NewLine}{exceptionForTrace.StackTrace}{Environment.NewLine}{exceptionForTrace}");

                // add method statistics
                testClassStatistic.Add(new TestMethodStatistic() { MethodInfo = testMethod, Info = e.GetType().Name, Start = startTimeForCurrentIteration, Success = false });

                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }

            Trace.WriteLine($"Page test finished: {testMethod.Name}{Environment.NewLine}------------------------------------------------------------");
        }
    }
}