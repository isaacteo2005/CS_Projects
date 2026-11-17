/**
 * @file: ExpressionTree.cs
 * @brief: expression Tree to evaluate expressions
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design.Serialization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public class ExpressionTree
    {
        /// <summary>
        /// root storing the expression.
        /// </summary>
        private Node root;

        /// <summary>
        /// position for parsing.
        /// </summary>
        private int position;

        /// <summary>
        /// number of parenthesis.
        /// </summary>
        private int parenCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression"> expression represented in tree.</param>
        public ExpressionTree(string expression = "A1+B1+C1")
        {
            this.parenCount = 0;
            this.position = 0;
            if (expression != null)
            {
                this.root = this.ParseExpression(expression);
            }

            if (this.root == null)
            {
                this.root = this.ParseExpression("A1+B1+C1");
            }

            if (this.parenCount != 0)
            {
                throw new Exception($"Unmatched opening parenthesis by {this.parenCount}");
            }
        }

        public Node Root => this.root;

        // SetVariable isn't used anymore since we don't use a dictionary
        /*
        /// <summary>
        /// sets specified variable within expression tree variable dictionary.
        /// </summary>
        /// <param name="variableName"> name of var to set.</param>
        /// <param name="variableValue">value of bar to set.</param>
        public void SetVariable(string variableName, double variableValue)
        {
            // if variableName is null, nothing is set
            if (variableName == null)
            {
                return;
            }

            this.variables[variableName] = variableValue;
        }
        */

        /// <summary>
        /// evaluates expression into doiuble.
        /// </summary>
        /// <param name="variableLookup">looks up the variable.</param>
        /// <returns>value.</returns>
        public double Evaluate(Func<string, double> variableLookup)
        {
            if (this.root == null)
            {
                throw new InvalidOperationException("Expression tree has no root node");
            }

            if (variableLookup == null)
            {
                throw new ArgumentNullException(nameof(variableLookup));
            }

            return this.root.Evaluate(variableLookup);
        }

        /// <summary>
        /// parses the expression and returns it as a node.
        /// </summary>
        /// <param name="expression">expression to parse.</param>
        /// <param name="precedence">precdence added for recursion.</param>
        /// <returns>node to insert, null if node is not valid.</returns>
        private Node ParseExpression(string expression, int precedence = 0)
        {
            Node left = this.ParseFactor(expression);

            while (this.position < expression.Length)
            {
                char op = expression[this.position];
                int opPrecedence = OpNode.GetPrecedence(op);

                if (op == ')')
                {
                    if (this.parenCount <= 0)
                    {
                        throw new Exception($"Unexpected closing parenthesis at position {this.position}");
                    }

                    break;
                }

                // breaks if this operator has a lower or equal precedence, returns the node
                if (opPrecedence <= precedence)
                {
                    break;
                }

                // consumes op
                this.position++;

                // parses right side of the op
                Node right = this.ParseExpression(expression, opPrecedence);

                // creates operator node
                left = OpNode.Create(op, left, right);
            }

            return left;
        }

        /// <summary>
        /// parses a single token (single var or single num), helper for ParseExpression.
        /// </summary>
        /// <param name="token">token to string.</param>
        /// <returns>ValueNode or VarNode or null if invalid.</returns>
        private Node ParseToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            // tries to parse as a valueNode
            if (double.TryParse(token, out double value))
            {
                return new ValueNode(value);
            }

            // var must start with a letter
            if (!char.IsLetter(token[0]))
            {
                return null;
            }

            // checks if each character is a letter or a digit
            foreach (char c in token)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return null;
                }
            }

            // creates varNode with reference to dictionary
            return new VarNode(token);
        }

        /// <summary>
        /// parses factors (numbers, variables, and parenthesis).
        /// </summary>
        /// <param name="expression">expression to parse.</param>
        /// <returns>Node that has been parsed.</returns>
        private Node ParseFactor(string expression)
        {
            // throws an exception of the position is outof bounds
            if (this.position >= expression.Length)
            {
                throw new Exception("Unexpected end of expression");
            }

            char current = expression[this.position];

            // parenthesis case
            if (current == '(')
            {
                this.parenCount++;
                this.position++; // skips the parenthesis

                // parses node with lowest precedence passed in
                Node inside = this.ParseExpression(expression, 0);

                // throws an exception if no ending parenthesis were found or out of bounds was reached
                if (this.position >= expression.Length || expression[this.position] != ')')
                {
                    throw new Exception("Missing closing parenthesis");
                }

                // skips ending parenthesis and returns the node
                this.position++;
                this.parenCount--;
                return inside;
            }

            // value case
            if (char.IsDigit(current) || current == '.')
            {
                return this.ParseNumber(expression);
            }

            // variable case
            if (char.IsLetter(current))
            {
                return this.ParseVariable(expression);
            }

            // throws exception if case not found
            throw new Exception("no cases found");
        }

        /// <summary>
        /// creates value node from number.
        /// </summary>
        /// <param name="expression">expression to get value node.</param>
        /// <returns>value node.</returns>
        private Node ParseNumber(string expression)
        {
            if (expression == null || !char.IsDigit(expression[this.position]))
            {
                throw new Exception("invalid value");
            }

            int start = this.position;

            // reads through every digit and decimal point (for double)
            while (this.position < expression.Length && (char.IsDigit(expression[this.position]) || expression[this.position] == '.'))
            {
                this.position++;
            }

            string numbStr = expression.Substring(start, this.position - start);
            if (double.TryParse(numbStr, out double value))
            {
                return new ValueNode(value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// creates variable node from expression.
        /// </summary>
        /// <param name="expression">expression with var node.</param>
        /// <returns>var node.</returns>
        private Node ParseVariable(string expression)
        {
            if (expression == null || !char.IsLetter(expression[this.position]))
            {
                throw new Exception("invalid varaible");
            }

            int start = this.position;

            // moves past first character (would always be a letter)
            this.position++;

            // reads additional letters/digits part of variable
            while (this.position < expression.Length && char.IsLetterOrDigit(expression[this.position]))
            {
                this.position++;
            }

            string name = expression.Substring(start, this.position - start);

            return new VarNode(name);
        }

        /// <summary>
        /// gets the list of variables in the expression tree. Warning ignored as these functions are paired together.
        /// </summary>
        /// <returns>list of var nodes.</returns>
#pragma warning disable SA1202 // Elements should be ordered by access
        public string[] GetVariableNames()
#pragma warning restore SA1202 // Elements should be ordered by access
        {
            var variables = new HashSet<string>();
            this.AddVariable(this.root, variables);
            return variables.ToArray();
        }

        /// <summary>
        /// adds the node to the hashset.
        /// </summary>
        /// <param name="node">node to add.</param>
        /// <param name="variables">hashset to add too.</param>
        private void AddVariable(Node node, HashSet<string> variables)
        {
            if (node is VarNode varNode)
            {
                variables.Add(varNode.Name);
            }
            else if (node is OpNode opNode)
            {
                this.AddVariable(opNode.Left, variables);
                this.AddVariable(opNode.Right, variables);
            }
        }
    }
}
