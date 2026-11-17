namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// subtract operator.
    /// </summary>
    public class SubtractionOpNode : OpNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractionOpNode"/> class.
        /// </summary>
        /// <param name="left"> left node.</param>
        /// <param name="right">right node.</param>
        public SubtractionOpNode(Node left, Node right)
            : base(left, right)
        {
        }

        /// <summary>
        /// gets the operator.
        /// </summary>
        public static char Operator
        {
            get { return '-'; }
        }

        /// <summary>
        /// gets precedence.
        /// </summary>
        public override int Precedence => 1;

        /// <summary>
        /// evaluates subtraction.
        /// </summary>
        /// <param name="variableLookup">var lookup.</param>
        /// <returns>value.</returns>
        public override double Evaluate(Func<string, double> variableLookup)
        {
            double leftVal = this.Left.Evaluate(variableLookup);
            double rightVal = this.Right.Evaluate(variableLookup);
            return leftVal - rightVal;
        }
    }
}