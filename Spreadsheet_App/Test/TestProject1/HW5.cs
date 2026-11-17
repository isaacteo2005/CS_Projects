/**
 * @file: HW5.cs
 * @brief: test cases for HW5
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

    internal class HW5
    {
        /// <summary>
        /// dictionary for varnode test cases.
        /// </summary>
        private Dictionary<string, double> variables = new Dictionary<string, double>();

        /// <summary>
        /// normal case for tree constructor testing with normal values.
        /// </summary>
        [Test]
        public void TestExpressionTreeNormal()
        {
            ExpressionTree tree = new ExpressionTree("A+B");

            // tree must not be null
            Assert.That(tree, Is.Not.Null);

            // test by providing lookup function with values
            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "A" => 5,
                    "B" => 3,
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            // test evaluate is correct
            Assert.That(result, Is.EqualTo(8));
        }

        /// <summary>
        /// edge case for tree constructor testing with single variable.
        /// </summary>
        [Test]
        public void TestExpressionTreeEdge()
        {
            ExpressionTree tree = new ExpressionTree("A");

            // variables not set - should throw when evaluated
            Assert.That(
                () => tree.Evaluate(name =>
            {
                throw new InvalidOperationException($"Variable {name} not set");
            }), Throws.InvalidOperationException);

            // After providing value
            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "A" => 99,
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            Assert.That(result, Is.EqualTo(99));
        }

        /// <summary>
        /// exception case for tree constructor testing with null expression.
        /// </summary>
        [Test]
        public void TestExpressionTreeException()
        {
            ExpressionTree tree = new ExpressionTree(null);

            // test with default expression "A1+B1+C1"
            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "A1" => 5,
                    "B1" => 10,
                    "C1" => 15,
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            Assert.That(result, Is.EqualTo(30));
        }

        /// <summary>
        /// normal case for set var testing with setting for single variable.
        /// </summary>
        [Test]
        public void TestSetVariableNormal()
        {
            ExpressionTree tree = new ExpressionTree("A");

            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "A" => 50,
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            Assert.That(result, Is.EqualTo(50));
        }

        /// <summary>
        /// edge case for set var testing with setting 2 duplicate vars and being case sensitive.
        /// </summary>
        [Test]
        public void TestSetVariableEdge()
        {
            // same variable used twice
            var tree = new ExpressionTree("X+x");

            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "X" => 5,   // uppercase X
                    "x" => 10,  // lowercase x - different variable!
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            // X + x = 5 + 10 = 15
            Assert.That(result, Is.EqualTo(15));
        }

        /// <summary>
        /// exception case for set var testing for setting value for null var.
        /// </summary>
        [Test]
        public void TestSetVariableException()
        {
            var tree = new ExpressionTree("A+B");

            // If a variable is missing in lookup, should throw
            Assert.That(
                () => tree.Evaluate(name =>
            {
                if (name == "A")
                {
                    return 5;
                }

                // B is missing then throw exception
                throw new InvalidOperationException($"Variable {name} not set");
            }), Throws.InvalidOperationException);
        }

        /// <summary>
        /// normal case for evaluate testing for evaluate with a valid expression.
        /// </summary>
        [Test]
        public void TestEvaluateNormal()
        {
            var tree = new ExpressionTree("A+B");

            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "A" => 3,
                    "B" => 2,
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            Assert.That(result, Is.EqualTo(5));
        }

        /// <summary>
        /// edge case for evaluate with only a +, should be 0.
        /// </summary>
        [Test]
        public void TestEvaluateEdge()
        {
            Assert.That(() => new ExpressionTree("+"), Throws.Exception);
        }

        /// <summary>
        /// exception case for evlauate with a divide by zero error.
        /// </summary>
        [Test]
        public void TestEvaluateException()
        {
            var tree = new ExpressionTree("X/Y");

            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "X" => 10,
                    "Y" => 0,
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            // divide by zero should return PositiveInfinity
            Assert.That(result, Is.EqualTo(double.PositiveInfinity));
        }

        /// <summary>
        /// normal case for opnode evaluate using regular values.
        /// </summary>
        [Test]
        public void TestOpEvaluateNormal()
        {
            var left = new ValueNode(6);
            var right = new ValueNode(3);

            var addNode = new AdditionOpNode(left, right);
            var subNode = new SubtractionOpNode(left, right);
            var mulNode = new MultiplicationOpNode(left, right);
            var divNode = new DivisionOpNode(left, right);
            Assert.Multiple(() =>
            {
                Assert.That(addNode.Evaluate(name => 0), Is.EqualTo(9));
                Assert.That(subNode.Evaluate(name => 0), Is.EqualTo(3));
                Assert.That(mulNode.Evaluate(name => 0), Is.EqualTo(18));
                Assert.That(divNode.Evaluate(name => 0), Is.EqualTo(2));
            });
        }

        /// <summary>
        /// edge case for opnode evaluate adding 5 to inf.
        /// </summary>
        [Test]
        public void TestOpEvaluateEdge()
        {
            var left = new ValueNode(double.PositiveInfinity);
            var right = new ValueNode(5);
            var node = new AdditionOpNode(left, right);

            Assert.That(node.Evaluate(name => 0), Is.EqualTo(double.PositiveInfinity));
        }

        /// <summary>
        /// exception case for opnode evaluate dividing by 0.
        /// </summary>
        [Test]
        public void TestOpEvaluateException()
        {
            var left = new ValueNode(5);
            var right = new ValueNode(0);
            var node = new DivisionOpNode(left, right);

            Assert.That(node.Evaluate(name => 0), Is.EqualTo(double.PositiveInfinity));
        }

        /// <summary>
        /// normal case for var evaluate using normal int.
        /// </summary>
        [Test]
        public void TestVarEvaluateNormal()
        {
            var node = new VarNode("X");

            double result = node.Evaluate(name =>
            {
                Assert.That(name, Is.EqualTo("X"));
                return 42;
            });

            Assert.That(result, Is.EqualTo(42));
        }

        /// <summary>
        /// edge case for var evaluate testing upper and lower case is different.
        /// </summary>
        [Test]
        public void TestVarEvaluateEdge()
        {
            var nodeUpper = new VarNode("X");
            var nodeLower = new VarNode("x");

            double resultUpper = nodeUpper.Evaluate(name =>
            {
                Assert.That(name, Is.EqualTo("X"));
                return 10;
            });

            double resultLower = nodeLower.Evaluate(name =>
            {
                Assert.That(name, Is.EqualTo("x"));
                return 20;
            });

            // Assert
            Assert.That(resultUpper, Is.EqualTo(10));
            Assert.That(resultLower, Is.EqualTo(20));
        }

        // this test case has been removed since dictionary isn't used anymore
        /*
        /// <summary>
        /// exception case for var evaluate testing null dictionary returns 0.
        /// </summary>
        [Test]
        public void TestVarEvaluateException()
        {
            // catches exception
            var error = Assert.Throws<ArgumentNullException>(() => new VarNode("X", null));

            // catches descrption of exception matches
            Assert.That(error.Message, Does.Contain("VarNode 'X' has null dictionary reference"));
        }
        */

        /// <summary>
        /// normal case testing if parse token works on value node and var node.
        /// </summary>
        [Test]
        public void TestParseTokenNormal()
        {
            ExpressionTree expression = new ExpressionTree();
            var method = typeof(ExpressionTree).GetMethod("ParseToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result1 = method.Invoke(expression, new object[] { "12" });
            var result2 = method.Invoke(expression, new object[] { "B3" });

            // asserts that both nodes are value nodes
            Assert.That(result1, Is.TypeOf<ValueNode>());
            var valueNode = result1 as ValueNode;
            Assert.That(result2, Is.TypeOf<VarNode>());
            var varNode = result2 as VarNode;
        }

        /// <summary>
        /// edge case testing if parse token returns null if invalid character detected.
        /// </summary>
        [Test]
        public void TestParseTokenEdge()
        {
            ExpressionTree expression = new ExpressionTree();
            var method = typeof(ExpressionTree).GetMethod("ParseToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result1 = method.Invoke(expression, new object[] { "#" });
            Assert.That(result1, Is.EqualTo(null));
        }

        /// <summary>
        /// exception case for test parse token returns null if null detetced.
        /// </summary>
        [Test]
        public void TestParseTokenException()
        {
            ExpressionTree expression = new ExpressionTree();
            var method = typeof(ExpressionTree).GetMethod("ParseToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result1 = method.Invoke(expression, new object[] { null });
            Assert.That(result1, Is.EqualTo(null));
        }

        [Test]
        public void TestParseExpressionNormal()
        {
            // Arrange
            var tree = new ExpressionTree("1+1");

            // sets position to start
            var posField = typeof(ExpressionTree).GetField(
                "position",
                BindingFlags.NonPublic | BindingFlags.Instance);
            posField.SetValue(tree, 0);

            var method = typeof(ExpressionTree).GetMethod("ParseExpression", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(tree, new object[] { "3+4", 0 });

            Assert.That(result, Is.TypeOf<AdditionOpNode>());
        }

        /// <summary>
        /// tests if parse expression throws an exception if expression incomplete.
        /// </summary>
        [Test]
        public void TestParseExpressionEdge()
        {
            ExpressionTree expression = new ExpressionTree();
            var method = typeof(ExpressionTree).GetMethod("ParseExpression", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(() => method.Invoke(expression, new object[] { "3+" }), Throws.Exception);
        }

        /// <summary>
        /// tests if expcetion is thrown when null passed.
        /// </summary>
        [Test]
        public void TestParseExpressionException()
        {
            ExpressionTree expression = new ExpressionTree();
            var method = typeof(ExpressionTree).GetMethod("ParseExpression", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(() => method.Invoke(expression, new object[] { null }), Throws.Exception);
        }
    }
}
