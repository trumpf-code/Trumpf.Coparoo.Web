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

namespace Trumpf.Coparoo.Web.PageTests.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Trumpf.Coparoo.Web.Logging.Tree;

    /// <summary>
    /// Extensions.
    /// </summary>
    internal static class TestClassStatisticExtensions
    {
        /// <summary>
        /// Add test method statistic.
        /// </summary>
        /// <param name="source">Test class statistic to extended.</param>
        /// <param name="other">Test method statistic to add.</param>
        /// <returns>The extended test class statistics.</returns>
        internal static void Add(this TestClassStatistic source, TestMethodStatistic other)
        {
            if (source.testClassStatistics.TryGetValue(other.MethodInfo, out TestMethodStatistics value))
            {
                source.testClassStatistics[other.MethodInfo] = value + other;
            }
            else
            {
                source.testClassStatistics.Add(other.MethodInfo, new TestMethodStatistics(other.MethodInfo) + other);
            }
        }
    }


    /// <summary>
    /// Test class statistics class.
    /// </summary>
    internal class TestClassStatistic
    {
        internal Dictionary<MethodInfo, TestMethodStatistics> testClassStatistics = new Dictionary<MethodInfo, TestMethodStatistics>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassStatistic"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public TestClassStatistic(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the tree representation of the statistics.
        /// </summary>
        internal Tree Tree
        {
            get
            {
                Node node = new Node { NodeType = NodeType.PageTestClass, Id = Type.FullName, Caption = Type.Name };
                Tree result = new Tree(node);

                bool anyFailed = false;
                foreach (var testMethodStatistics in testClassStatistics.Values)
                {
                    NodeType nodeType = NodeType.PageTest;
                    Color nodeColor = testMethodStatistics.AnyFailed ? Color.Red : Color.Green;
                    anyFailed = anyFailed || testMethodStatistics.AnyFailed;

                    Node n = new Node { NodeType = nodeType, Id = Type.FullName + "." + testMethodStatistics.MethodInfo.Name, Caption = testMethodStatistics.ToString(), FrameColor = nodeColor };
                    result += n;
                    result += new Edge { To = n.Id, From = result.Root.Id };
                }

                node.FrameColor = anyFailed ? Color.Sienna2 : Color.Palegreen3;

                return result;
            }
        }

        /// <summary>
        /// Merge test class statistic.
        /// </summary>
        /// <param name="c1">Test class statistic to extended.</param>
        /// <param name="c2">Test class statistic to add.</param>
        /// <returns>The merged test class statistic.</returns>
        public static TestClassStatistic operator +(TestClassStatistic c1, TestClassStatistic c2)
        {
            Dictionary<MethodInfo, TestMethodStatistics> r = new Dictionary<MethodInfo, TestMethodStatistics>();

            foreach (var x in c1.testClassStatistics)
            {
                var matches = c2.testClassStatistics.Where(e => x.Key == e.Key).ToList();
                if (matches.Any())
                {
                    matches.ForEach(e => r.Add(x.Key, x.Value + e.Value));
                }
                else
                {
                    r.Add(x.Key, x.Value);
                }
            }

            return new TestClassStatistic(c1.Type) { testClassStatistics = r };
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() => Type.FullName + "\\n" + string.Join(Environment.NewLine, testClassStatistics.Values);
    }
}
