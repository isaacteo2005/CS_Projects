/**
 * @file: HW6.cs
 * @brief: test cases for HW6
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class HW6
    {
        // normal cases

        /// <summary>
        /// checks if evaluation works with precedence.
        /// </summary>
        [Test]
        public void ExpressionTreeNewPrecedence()
        {
            var tree = new ExpressionTree("(2+3*4)");
            double result = tree.Evaluate(name =>
            {
                Assert.Fail("Lookup should not be called for expression with no variables");
                return 0;
            });

            Assert.That(result, Is.EqualTo(14));
        }

        /// <summary>
        /// checks if parenthesis works in the tree.
        /// </summary>
        [Test]
        public void ExpressionTreeNewParenthesis()
        {
            var tree = new ExpressionTree("(2+3)*4");
            double result = tree.Evaluate(name =>
            {
                Assert.Fail("Lookup should not be called for expression with no variables");
                return 0;
            });

            Assert.That(result, Is.EqualTo(20));
        }

        /// <summary>
        /// checks if evaluate works with multiple higher precedence operators.
        /// </summary>
        [Test]
        public void ExpressionTreeNewMultOps()
        {
            var tree = new ExpressionTree("2+3*4-6/2");

            double result = tree.Evaluate(name =>
            {
                Assert.Fail("Lookup should not be called for expression with no variables");
                return 0;
            });

            Assert.That(result, Is.EqualTo(11));
        }

        /// <summary>
        /// checks if evaluate works with variables.
        /// </summary>
        [Test]
        public void Parentheses_WithVariables_WorkCorrectly()
        {
            var tree = new ExpressionTree("(X+Y)*Z");
            double result = tree.Evaluate(name =>
            {
                return name switch
                {
                    "X" => 2,
                    "Y" => 3,
                    "Z" => 4,
                    _ => throw new ArgumentException($"Unexpected variable: {name}")
                };
            });

            Assert.That(result, Is.EqualTo(20));
        }

        // edge cases

        /// <summary>
        /// tests if any errors occur with many nested parenthesis.
        /// </summary>
        [Test]
        public void ExpressionTreeNewNestedParenthesis()
        {
            var tree = new ExpressionTree("((((5))))");
            double result = tree.Evaluate(name =>
            {
                Assert.Fail("Lookup should not be called for expression with no variables");
                return 0;
            });

            Assert.That(result, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests evaluate works with just number in parenthesis.
        /// </summary>
        [Test]
        public void ExpressionTreeNewJustNumber()
        {
            var tree = new ExpressionTree("(42)");
            double result = tree.Evaluate(name =>
            {
                Assert.Fail("Lookup should not be called for expression with no variables");
                return 0;
            });

            Assert.That(result, Is.EqualTo(42));
        }

        /// <summary>
        /// Tests evaluate works with just variable in parenthesis.
        /// </summary>
        [Test]
        public void ExpressionTreeNewJustVariable()
        {
            var tree = new ExpressionTree("(X)");
            double result = tree.Evaluate(name =>
            {
                Assert.That(name, Is.EqualTo("X"));
                return 42;
            });

            Assert.That(result, Is.EqualTo(42));
        }

        // exception cases

        /// <summary>
        /// exception is thrown for a tree with just parenthesis.
        /// </summary>
        [Test]
        public void ExpressionTreeNewJustParenthesis()
        {
            Assert.That(() => new ExpressionTree("()"), Throws.Exception);
        }

        /// <summary>
        /// exxception is thrown when parenthesis are mismatched.
        /// </summary>
        [Test]
        public void ExpressionTreeNewMismatchParenthesis()
        {
            Assert.That(() => new ExpressionTree("(2+3"), Throws.Exception);
            Assert.That(() => new ExpressionTree("2+3)"), Throws.Exception);
        }

        [Test]
        public void ParseVariableNormal()
        {
            // Arrange
            var tree = new ExpressionTree("1+1");

            // sets position to start
            var posField = typeof(ExpressionTree).GetField(
                "position",
                BindingFlags.NonPublic | BindingFlags.Instance);
            posField.SetValue(tree, 0);

            var method = typeof(ExpressionTree).GetMethod("ParseVariable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(tree, new object[] { "B2" });

            Assert.That(result, Is.TypeOf<VarNode>());
        }

        [Test]
        public void ParseVariableEdge()
        {
            // Arrange
            var tree = new ExpressionTree("1+1");

            // sets position to start
            var posField = typeof(ExpressionTree).GetField(
                "position",
                BindingFlags.NonPublic | BindingFlags.Instance);
            posField.SetValue(tree, 0);

            var method = typeof(ExpressionTree).GetMethod("ParseVariable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(() => method.Invoke(tree, new object[] { "#" }), Throws.Exception);
        }

        [Test]
        public void ParseVariableException()
        {
            // Arrange
            var tree = new ExpressionTree("1+1");

            // sets position to start
            var posField = typeof(ExpressionTree).GetField(
                "position",
                BindingFlags.NonPublic | BindingFlags.Instance);
            posField.SetValue(tree, 0);

            var method = typeof(ExpressionTree).GetMethod("ParseNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(() => method.Invoke(tree, new object[] { null }), Throws.Exception);
        }

        [Test]
        public void ParseValueNormal()
        {
            // Arrange
            var tree = new ExpressionTree("1+1");

            // sets position to start
            var posField = typeof(ExpressionTree).GetField(
                "position",
                BindingFlags.NonPublic | BindingFlags.Instance);
            posField.SetValue(tree, 0);

            var method = typeof(ExpressionTree).GetMethod("ParseNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(tree, new object[] { "56" });

            Assert.That(result, Is.TypeOf<ValueNode>());
        }

        [Test]
        public void ParseValueEdge()
        {
            // Arrange
            var tree = new ExpressionTree("1+1");

            // sets position to start
            var posField = typeof(ExpressionTree).GetField(
                "position",
                BindingFlags.NonPublic | BindingFlags.Instance);
            posField.SetValue(tree, 0);

            var method = typeof(ExpressionTree).GetMethod("ParseNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(() => method.Invoke(tree, new object[] { "#" }), Throws.Exception);
        }

        [Test]
        public void ParseValueException()
        {
            // Arrange
            var tree = new ExpressionTree("1+1");

            // sets position to start
            var posField = typeof(ExpressionTree).GetField(
                "position",
                BindingFlags.NonPublic | BindingFlags.Instance);
            posField.SetValue(tree, 0);

            var method = typeof(ExpressionTree).GetMethod("ParseNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(() => method.Invoke(tree, new object[] { null }), Throws.Exception);
        }

        // since ParseFactor and ParseExpression depend on each no test cases would be created for them as they both get tested through the ExpressionTree consturctor tests
    }
}
