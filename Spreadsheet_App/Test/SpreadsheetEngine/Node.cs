/**
 * @file: Node.cs
 * @brief: Node for expression tree
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class Node
    {
        /// <summary>
        /// evaluates the node.
        /// </summary>
        /// <param name="variableLookup">looks up the node.</param>
        /// <returns>gets value.</returns>
        public abstract double Evaluate(Func<string, double> variableLookup);
    }
}
