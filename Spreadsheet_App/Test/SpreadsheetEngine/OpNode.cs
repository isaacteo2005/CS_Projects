/**
 * @file: OpNode.cs
 * @brief: Node for expression tree representing binary operator
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class OpNode : Node
    {
        /// <summary>
        /// storage of operators.
        /// </summary>
        private static Dictionary<char, Type> operators = new Dictionary<char, Type>();

        /// <summary>
        /// checks if operator is loaded.
        /// </summary>
        private static bool operatorsLoaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpNode"/> class.
        /// </summary>
        /// <param name="left">Left child node.</param>
        /// <param name="right">Right child node.</param>
        public OpNode(Node left, Node right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        private delegate void OnOperator(char op, Type type);

        /// <summary>
        /// gets list of operators.
        /// </summary>
        public static char[] Operators
        {
            get
            {
                LoadOperatorsIfNeeded();
                return operators.Keys.ToArray();
            }
        }

        /// <summary>
        /// gets precendece.
        /// </summary>
        public abstract int Precedence { get; }

        public Node Left { get; }

        public Node Right { get; }

        /// <summary>
        /// checks if op is an operator.
        /// </summary>
        /// <param name="op">operator.</param>
        /// <returns>if op is an op.</returns>
        public static bool IsOperator(char op)
        {
            // returns first occurence of op in Operators
            return Array.IndexOf(Operators, op) >= 0;
        }

        /// <summary>
        /// gets the precendece of the operator.
        /// </summary>
        /// <param name="op">operator.</param>
        /// <returns>precedence.</returns>
        public static int GetPrecedence(char op)
        {
            LoadOperatorsIfNeeded();

            if (operators.TryGetValue(op, out Type operatorType))
            {
                Node dummyLeft = new ValueNode(0);
                Node dummyRight = new ValueNode(0);
                var tempNode = (OpNode)Activator.CreateInstance(operatorType, dummyLeft, dummyRight);
                return tempNode.Precedence;
            }

            return 0;  // Unknown operator has lowest precedence
        }

        /// <summary>
        /// creates the operator node.
        /// </summary>
        /// <param name="op">op to create.</param>
        /// <param name="left">left node.</param>
        /// <param name="right">right node.</param>
        /// <returns>creation of opnode.</returns>
        public static OpNode Create(char op, Node left, Node right)
        {
            LoadOperatorsIfNeeded();

            if (operators.TryGetValue(op, out Type operatorType))
            {
                return (OpNode)Activator.CreateInstance(operatorType, left, right);
            }

            throw new NotSupportedException($"Operator '{op}' not supported");
        }

        /// <summary>
        /// evaluates the operator node.
        /// </summary>
        /// <param name="variableLookup">function to lookup variable values.</param>
        /// <returns>result of the operation.</returns>
        public abstract override double Evaluate(Func<string, double> variableLookup);

        /// <summary>
        /// loads all operator types via reflection.
        /// </summary>
        private static void LoadOperatorsIfNeeded()
        {
            if (operatorsLoaded)
            {
                return;
            }

            operators.Clear();
            TraverseAvailableOperators((op, type) =>
            {
                operators[op] = type;
            });

            operatorsLoaded = true;
        }

        /// <summary>
        /// traverses all availible operators.
        /// </summary>
        /// <param name="onOperator">delegate passed.</param>
        private static void TraverseAvailableOperators(OnOperator onOperator)
        {
            // get the type declaration of OperatorNode
            Type operatorNodeType = typeof(OpNode);

            // Iterate over all loaded assemblies:
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types that inherit from our OperatorNode class using LINQ
                IEnumerable<Type> operatorTypes =
                assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));

                // Iterate over those subclasses of OperatorNode
                foreach (var type in operatorTypes)
                {
                    // for each subclass, retrieve the Operator property
                    PropertyInfo operatorField = type.GetProperty("Operator");
                    if (operatorField != null)
                    {
                        // Get the character of the Operator
                        object value = operatorField.GetValue(null); // pass null for static properties

                        // If “Operator” property is not static, you will need to create
                        // an instance first and use the following code instead (or similar):
                        // object value = operatorField.GetValue(Activator.CreateInstance(type,
                        // new ConstantNode(0)));
                        if (value is char)
                        {
                            char operatorSymbol = (char)value;

                            // And invoke the function passed as parameter
                            // with the operator symbol and the operator class
                            onOperator(operatorSymbol, type);
                        }
                    }
                }
            }
        }
    }
}
