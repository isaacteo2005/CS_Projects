/**
 * @file: Program.cs
 * @brief: Demo for expression tree
 * @author: Isaac Teo 11887290
 */
namespace ExpressionTreeDemo
{
    using System;
    using System.Linq.Expressions;
    using SpreadsheetEngine;

    /// <summary>
    /// demo for expression tree.
    /// </summary>
    public class Program
    {
        public static ExpressionTree ExpTree;
        public static Dictionary<string, double> VariableValues;

        public static void Main(string[] args)
        {
            string expression = "A1+B1+C1";
            ExpTree = new ExpressionTree(expression);
            VariableValues = new Dictionary<string, double>();
            bool run = true;
            while (run)
            {
                Console.WriteLine("Current Expression: " + expression);
                DisplayMenu();
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        expression = EnterExpression();
                        break;
                    case "2":
                        SetVariable();
                        break;
                    case "3":
                        EvaluateExpression();
                        break;
                    case "4":
                        run = false;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// displays meny for demo.
        /// </summary>
        private static void DisplayMenu()
        {
            Console.WriteLine("1. Enter a new expression");
            Console.WriteLine("2. set variable value");
            Console.WriteLine("3. Evaluate Tree");
            Console.WriteLine("4. Quit");
        }

        /// <summary>
        /// user enters expression then it gets read into tree.
        /// </summary>
        private static string EnterExpression()
        {
            Console.Write("Enter a new expression: ");
            string expression = Console.ReadLine()?.Trim();
            ExpTree = new ExpressionTree(expression);
            VariableValues.Clear();
            return expression;
        }

        /// <summary>
        /// asks for variable name and value then sets it.
        /// </summary>
        private static void SetVariable()
        {
            // set variable disabled since we are not using dictionary
            Console.Write("Enter variable name: ");
            string name = Console.ReadLine()?.Trim();
            Console.Write("Enter variable value: ");
            string valueInput = Console.ReadLine()?.Trim();
            if (double.TryParse(valueInput, out double value))
            {
                // stiores in dictionary that would be used for lookup function
                VariableValues[name] = value;
                Console.WriteLine($"Variable {name} set to {value}.");
            }
            else
            {
                Console.WriteLine("Invalid value. Enter a valid number.");
            }
        }

        /// <summary>
        /// evaluates the expression in the tree.
        /// </summary>
        private static void EvaluateExpression()
        {
            // creates lookup function with stored variable values
            double result = ExpTree.Evaluate(name =>
            {
                if (VariableValues.TryGetValue(name, out double value))
                {
                    return value;
                }

                // throws excpetion if no vars set, no defaults allowed
                throw new InvalidOperationException($"Variable '{name}' is not set");
            });

            Console.WriteLine($"Result: {result}");
        }
    }
}