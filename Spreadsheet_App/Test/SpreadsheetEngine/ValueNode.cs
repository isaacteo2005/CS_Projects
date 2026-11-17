/**
 * @file: ValueNode.cs
 * @brief: Node for expression tree representing a constant numerical value
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ValueNode : Node
    {
        /// <summary>
        /// constant int value of node.
        /// </summary>
        private readonly double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueNode"/> class.
        /// </summary>
        /// <param name="value">cinstant int value for node.</param>
        public ValueNode(double value = 0)
        {
            this.value = value;
        }

        /// <summary>
        /// gets the value of the node.
        /// </summary>
        /// <returns>value of node.</returns>
        public double Value()
        {
            return this.value;
        }

        /// <summary>
        /// evaluates the node to value.
        /// </summary>
        /// <param name="variableLookup">looks up variable.</param>
        /// <returns>value.</returns>
        public override double Evaluate(Func<string, double> variableLookup)
        {
            // Value node ignores varLookup and just returns whats stored (not referencing any varaibles)
            return this.value;
        }
    }
}
