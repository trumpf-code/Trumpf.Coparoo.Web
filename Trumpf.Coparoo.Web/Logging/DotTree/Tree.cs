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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Dot tree class.
    /// </summary>
    internal class Tree
    {
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree"/> class.
        /// </summary>
        /// <param name="root">The tree root.</param>
        public Tree(Node root)
        {
            nodes.Add(root);
            Root = root;
        }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        public int NodeCount => nodes.Count();

        /// <summary>
        /// Gets the edge count.
        /// </summary>
        public int EdgeCount => edges.Count();

        /// <summary>
        /// Gets the node count.
        /// </summary>
        public IEnumerable<Node> Nodes => nodes;

        /// <summary>
        /// Gets the edge count.
        /// </summary>
        public IEnumerable<Edge> Edges => edges;

        /// <summary>
        /// Gets the root node.
        /// </summary>
        public Node Root { get; private set; }

        /// <summary>
        /// Create the union of two trees.
        /// </summary>
        /// <param name="d1">First tree (extended).</param>
        /// <param name="d2">Second tree.</param>
        /// <returns>The union of the trees.</returns>
        public static Tree operator +(Tree d1, Tree d2)
        {
            d1.nodes = d1.nodes.Union(d2.nodes).ToList();
            d1.edges.AddRange(d2.edges);
            return d1;
        }

        /// <summary>
        /// Add nodes.
        /// </summary>
        /// <param name="d1">The tree to extend.</param>
        /// <param name="n">The new nodes.</param>
        /// <returns>The extended tree.</returns>
        public static Tree operator +(Tree d1, Node n)
        {
            if (!d1.nodes.Any(e => e.Id == n.Id))
            {
                d1.nodes.Add(n);
            }
            else
            {
                // do nothing
            }

            return d1;
        }

        /// <summary>
        /// Add edges.
        /// </summary>
        /// <param name="d1">The tree to extend.</param>
        /// <param name="e">The new edges.</param>
        /// <returns>The extended tree.</returns>
        public static Tree operator +(Tree d1, Edge e)
        {
            d1.edges.Add(e);
            return d1;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return
                "digraph graphname " + Environment.NewLine +
                "{" + Environment.NewLine +
                "size=\"1,1\";" + Environment.NewLine +
                "rankdir = LR;" + Environment.NewLine +
                "splines = line;" + Environment.NewLine +
                 string.Join(Environment.NewLine, nodes) + Environment.NewLine +
                 string.Join(Environment.NewLine, edges) + Environment.NewLine +
                 "{rank=same; " + string.Join(" ", nodes.Where(f => f.NodeType == NodeType.PageTest).Select(e => e.Id)) + "}" + Environment.NewLine +
                 "{rank=same; " + string.Join(" ", nodes.Where(f => f.NodeType == NodeType.PageTestClass).Select(e => e.Id)) + "}" + Environment.NewLine +
                 "}";
        }

        /// <summary>
        /// Save the tree in the DOT or PDF format.
        /// </summary>
        /// <param name="filename">The file to save to.</param>
        /// <param name="dotBinaryPath">The path the GraphViz binary.</param>
        /// <returns></returns>
        internal string WriteGraph(string filename, string dotBinaryPath = TabObject.DEFAULT_DOT_PATH)
        {
            if (filename is null)
                throw new ArgumentNullException(nameof(filename));

            string outPdf = Path.GetFullPath(filename) + ".pdf";
            string outDot = Path.ChangeExtension(outPdf, ".dot");

            // write tree as DOT file
            File.WriteAllText(outDot, ToString());

            Process s = new Process
            {
                StartInfo =
                {
                    Arguments = " -o " + '"' + outPdf + '"' + " -Tpdf " + '"' + outDot + '"',
                    FileName = dotBinaryPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            // try to convert it to PDF
            try
            {
                // assumes dot is installed if not no output is generated
                s.Start();

                string stdOut = s.StandardOutput.ReadToEnd();
                string stdErr = s.StandardError.ReadToEnd();

                s.WaitForExit();
                if (s.ExitCode == 0)
                {
                    Trace.WriteLine($"Rendered tree written to <{outPdf}>");
                    return outPdf;
                }
                else
                {
                    Trace.WriteLine("Got non-zero return code. Rendering failed");
                    return outDot;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"No PDF generated; executable {dotBinaryPath} not found?; is http://graphviz.org/ installed?; Error text: {e.Message}");
                return outPdf;
            }
        }
    }
}
