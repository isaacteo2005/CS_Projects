namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// division operator.
    /// </summary>
    public class DivisionOpNode : OpNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionOpNode"/> class.
        /// </summary>
        /// <param name="left"> left node.</param>
        /// <param name="right">right node.</param>
        public DivisionOpNode(Node left, Node right)
            : base(left, right)
        {
        }

        /// <summary>
        /// gets the operator.
        /// </summary>
        public static char Operator
        {
            get { return '/'; }
        }

        /// <summary>
        /// gets precedence.
        /// </summary>
        public override int Precedence => 2;

        /// <summary>
        /// evaluates division.
        /// </summary>
        /// <param name="variableLookup">var lookup.</param>
        /// <returns>value.</returns>
        public override double Evaluate(Func<string, double> variableLookup)
        {
            double leftVal = this.Left.Evaluate(variableLookup);
            double rightVal = this.Right.Evaluate(variableLookup);
            return this.SafeDivide(leftVal, rightVal);
        }

        /// <summary>
        /// safely handles dvision.
        /// </summary>
        /// <param name="leftVal">left value.</param>
        /// <param name="rightVal">rught value.</param>
        /// <returns>division case.</returns>
        private double SafeDivide(double leftVal, double rightVal)
        {
            if (rightVal == 0)
            {
                return double.PositiveInfinity;
            }
            else
            {
                return leftVal / rightVal;
            }
        }
    }
}