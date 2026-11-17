/**
 * @file: HW7.cs
 * @brief: test cases for HW6
 * @author: Isaac Teo 11887290
 */
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestProject1")]

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class HW7
    {
        private Spreadsheet spreadsheet = new Spreadsheet(50, 26);

        // test cases for evaluate, subclasses of OpNode in HW5 and HW6 test
        // no normal case for lookup as those are in HW5 and HW6

        /// <summary>
        /// expcetion case testing for when variable not found.
        /// </summary>
        [Test]
#pragma warning disable SA1202 // Elements should be ordered by access
        public void TestVariableNotFound_ThrowsException()
#pragma warning restore SA1202 // Elements should be ordered by access
        {
            var node = new VarNode("Y");

            Assert.That(
                () => node.Evaluate(name =>
            {
                throw new InvalidOperationException($"Variable {name} not found");
            }), Throws.InvalidOperationException);
        }

        /// <summary>
        /// exception case for lookup with null.
        /// </summary>
        [Test]
        public void TestNullLookup_ThrowsException()
        {
            var tree = new ExpressionTree("A+B");

            Assert.That(
                () => tree.Evaluate(null),
                Throws.ArgumentNullException);
        }

        /// <summary>
        /// normal case using regular index in spreadsheet.
        /// </summary>
        [Test]
        public void LookupVarNormal()
        {
            var cell = this.spreadsheet.GetCell(0, 0);
            cell.Text = "42";
            cell.SetValue("42");
            var lookupMethod = typeof(Spreadsheet).GetMethod(
                "LookupVariable",
                BindingFlags.NonPublic | BindingFlags.Instance);
            double result = (double)lookupMethod.Invoke(this.spreadsheet, new object[] { "A1" });
            Assert.That(result, Is.EqualTo(42.0));
        }

        /// <summary>
        /// edge case using last row and col as index.
        /// </summary>
        [Test]
        public void LookupVarEdge()
        {
            var cell = this.spreadsheet.GetCell(49, 25);
            cell.Text = "42";
            cell.SetValue("42");
            var lookupMethod = typeof(Spreadsheet).GetMethod(
                "LookupVariable",
                BindingFlags.NonPublic | BindingFlags.Instance);
            double result = (double)lookupMethod.Invoke(this.spreadsheet, new object[] { "Z50" });
            Assert.That(result, Is.EqualTo(42.0));
        }

        /// <summary>
        /// exception case checking for null passed through.
        /// </summary>
        [Test]
        public void LookupVarException()
        {
            var cell = this.spreadsheet.GetCell(49, 25);
            cell.Text = "42";
            cell.SetValue("42");
            var lookupMethod = typeof(Spreadsheet).GetMethod(
                "LookupVariable",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var ex = Assert.Throws<TargetInvocationException>(() =>
        lookupMethod.Invoke(this.spreadsheet, new object[] { null }));
        }

        /// <summary>
        /// normal case updating value for normal cell.
        /// </summary>
        [Test]
        public void UpdateCellNormal()
        {
            var cell = this.spreadsheet.GetCell(0, 0);
            cell.Text = "Hello World";
            var lookupMethod = typeof(Spreadsheet).GetMethod(
                "UpdateCellValue",
                BindingFlags.NonPublic | BindingFlags.Instance);
            lookupMethod.Invoke(this.spreadsheet, new object[] { cell });
            Assert.That(cell.Value, Is.EqualTo("Hello World"));
        }

        /// <summary>
        /// edge case updating value for edge cell.
        /// </summary>
        [Test]
        public void UpdateCellEdge()
        {
            var cell = this.spreadsheet.GetCell(49, 25);
            cell.Text = "Hello World";
            var lookupMethod = typeof(Spreadsheet).GetMethod(
                "UpdateCellValue",
                BindingFlags.NonPublic | BindingFlags.Instance);
            lookupMethod.Invoke(this.spreadsheet, new object[] { cell });
            Assert.That(cell.Value, Is.EqualTo("Hello World"));
        }

        /// <summary>
        /// exception case passing through null.
        /// </summary>
        [Test]
        public void UpdateCellException()
        {
            var cell = this.spreadsheet.GetCell(49, 25);
            cell.Text = "Hello World";
            var lookupMethod = typeof(Spreadsheet).GetMethod(
                "UpdateCellValue",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // checks if an exception is not thrown if null passed (just returns)
            Assert.DoesNotThrow(() =>
        lookupMethod.Invoke(this.spreadsheet, new object[] { null }));
        }

        // used AI assistance to help created recalc dependence case

        /// <summary>
        /// normal case for recalcuating dependencies using multiple cells in a dpeendence chain.
        /// </summary>
        [Test]
        public void RecalcDependNormal()
        {
            // ========== SETUP ==========
            var spreadsheet = new Spreadsheet(50, 26);

            // Get private methods
            var updateCellValueMethod = typeof(Spreadsheet).GetMethod(
                "UpdateCellValue",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var recalculateDependentsMethod = typeof(Spreadsheet).GetMethod(
                "RecalculateDependentCells",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Helper to invoke UpdateCellValue
            Action<Cell> updateCell = (cell) =>
                updateCellValueMethod.Invoke(spreadsheet, new object[] { cell });

            // Helper to invoke RecalculateDependentCells
            Action<Cell> recalcDependents = (cell) =>
                recalculateDependentsMethod.Invoke(spreadsheet, new object[] { cell, null });

            // ========== CREATE DEPENDENCY CHAIN ==========
            // A1 = 5 (base value)
            // B1 = A1 * 2  (depends on A1)
            // C1 = B1 + 3  (depends on B1)
            // D1 = A1 + C1 (depends on A1 and C1)
            var a1 = spreadsheet.GetCell(0, 0); // A1
            var b1 = spreadsheet.GetCell(0, 1); // B1
            var c1 = spreadsheet.GetCell(0, 2); // C1
            var d1 = spreadsheet.GetCell(0, 3); // D1

            // Set up formulas
            a1.Text = "5";
            updateCell(a1); // A1 = 5

            b1.Text = "=A1*2";
            updateCell(b1); // B1 = 10

            c1.Text = "=B1+3";
            updateCell(c1); // C1 = 13

            d1.Text = "=A1+C1";
            updateCell(d1); // D1 = 5 + 13 = 18

            // Verify initial values
            Assert.Multiple(() =>
            {
                Assert.That(a1.Value, Is.EqualTo("5"), "Initial A1 wrong");
                Assert.That(b1.Value, Is.EqualTo("10"), "Initial B1 wrong");
                Assert.That(c1.Value, Is.EqualTo("13"), "Initial C1 wrong");
                Assert.That(d1.Value, Is.EqualTo("18"), "Initial D1 wrong");
            });

            // ========== ACT ==========
            // Change A1 from 5 to 10
            a1.Text = "10";
            updateCell(a1); // This should trigger recalculation

            // Now explicitly call RecalculateDependentCells for A1
            recalcDependents(a1);

            // ========== ASSERT ==========
            // All dependent cells should have updated values
            Assert.Multiple(() =>
            {
                // A1 is now 10
                Assert.That(a1.Value, Is.EqualTo("10"), "A1 not updated");

                // B1 = A1 * 2 = 10 * 2 = 20
                Assert.That(b1.Value, Is.EqualTo("20"), "B1 not updated correctly");

                // C1 = B1 + 3 = 20 + 3 = 23
                Assert.That(c1.Value, Is.EqualTo("23"), "C1 not updated correctly");

                // D1 = A1 + C1 = 10 + 23 = 33
                Assert.That(d1.Value, Is.EqualTo("33"), "D1 not updated correctly");
            });
        }

        // no edge case as cells only either depend on one another or doesn't

        /// <summary>
        /// exception case for recalcuating dependencies method, should just return if null passed.
        /// </summary>
        [Test]
        public void RecalcDependException()
        {
            var recalculateDependentsMethod = typeof(Spreadsheet).GetMethod(
       "RecalculateDependentCells",
       BindingFlags.NonPublic | BindingFlags.Instance);

            // no exception, should just return if null passed
            Assert.DoesNotThrow(() =>
       recalculateDependentsMethod.Invoke(this.spreadsheet, new object[] { null, null }));
        }

        /// <summary>
        /// normal case for getting referenced cells.
        /// </summary>
        [Test]
        public void GetReferencedNormal()
        {
            var getReferencedCellsMethod = typeof(Spreadsheet).GetMethod(
         "GetReferencedCells",
         BindingFlags.NonPublic | BindingFlags.Instance);
            string formula = "A1+B2*C3-D4/E5";
            var result = (List<string>)getReferencedCellsMethod.Invoke(
                this.spreadsheet, new object[] { formula });
            var expected = new[] { "A1", "B2", "C3", "D4", "E5" };
            Assert.That(result, Is.EquivalentTo(expected));
        }

        /// <summary>
        /// edge case testing for just numbers (no referenced cells).
        /// </summary>
        [Test]
        public void GetReferencedEdge()
        {
            var getReferencedCellsMethod = typeof(Spreadsheet).GetMethod(
         "GetReferencedCells",
         BindingFlags.NonPublic | BindingFlags.Instance);
            string formula = "2+3*4-5/2";
            var result = (List<string>)getReferencedCellsMethod.Invoke(
                this.spreadsheet, new object[] { formula });

            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// exception case testing for passing through null.
        /// </summary>
        [Test]
        public void GetReferencedException()
        {
            var getReferencedCellsMethod = typeof(Spreadsheet).GetMethod(
        "GetReferencedCells",
        BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (List<string>)getReferencedCellsMethod.Invoke(
                this.spreadsheet, new object[] { null });
            Assert.That(result, Is.Empty);
        }

        // used AI for help with update dependencies normal case

        /// <summary>
        /// updates the dependencies normal case.
        /// </summary>
        [Test]
        public void UpdateDependNormal()
        { // Get private methods and fields
            var updateDependenciesMethod = typeof(Spreadsheet).GetMethod(
                "UpdateDependencies",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var dependentsField = typeof(Spreadsheet).GetField(
                "dependents",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var referencesField = typeof(Spreadsheet).GetField(
                "references",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var updateCellValueMethod = typeof(Spreadsheet).GetMethod(
                "UpdateCellValue",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Helper to invoke UpdateDependencies (just takes cell)
            Action<Cell> updateDeps = (cell) =>
                updateDependenciesMethod.Invoke(this.spreadsheet, new object[] { cell });

            Action<Cell> updateCell = (cell) =>
                updateCellValueMethod.Invoke(this.spreadsheet, new object[] { cell });

            // ========== CREATE CELLS WITH FORMULAS ==========
            var a1 = this.spreadsheet.GetCell(0, 0); // A1
            var b1 = this.spreadsheet.GetCell(0, 1); // B1
            var c1 = this.spreadsheet.GetCell(0, 2); // C1

            // Set up values and formulas
            a1.Text = "10";
            updateCell(a1);

            b1.Text = "=A1*2";
            updateCell(b1);

            c1.Text = "=A1+B1";
            updateCell(c1);

            // ========== CALL UPDATEDEPENDENCIES FOR EACH CELL ==========
            updateDeps(a1); // A1 has no formula, so no dependencies
            updateDeps(b1); // B1 depends on A1
            updateDeps(c1); // C1 depends on A1 and B1

            // ========== VERIFY DEPENDENCIES ==========
            var dependents = (Dictionary<Cell, List<Cell>>)dependentsField.GetValue(this.spreadsheet);
            var references = (Dictionary<Cell, List<Cell>>)referencesField.GetValue(this.spreadsheet);

            Assert.Multiple(() =>
            {
                // A1 should have dependents B1 and C1
                Assert.That(dependents.ContainsKey(a1), Is.True);
                Assert.That(dependents[a1], Contains.Item(b1));
                Assert.That(dependents[a1], Contains.Item(c1));
                Assert.That(dependents[a1].Count, Is.EqualTo(2));

                // B1 should have dependent C1
                Assert.That(dependents.ContainsKey(b1), Is.True);
                Assert.That(dependents[b1], Contains.Item(c1));
                Assert.That(dependents[b1].Count, Is.EqualTo(1));

                // B1 should reference A1
                Assert.That(references.ContainsKey(b1), Is.True);
                Assert.That(references[b1], Contains.Item(a1));
                Assert.That(references[b1].Count, Is.EqualTo(1));

                // C1 should reference A1 and B1
                Assert.That(references.ContainsKey(c1), Is.True);
                Assert.That(references[c1], Contains.Item(a1));
                Assert.That(references[c1], Contains.Item(b1));
                Assert.That(references[c1].Count, Is.EqualTo(2));

                // A1 should have no references (no formula)
                Assert.That(references.ContainsKey(a1), Is.False);
            });
        }

        // no edge case since cells are either dependent on each other or not

        /// <summary>
        /// expceiton case for updating dependencies.
        /// </summary>
        [Test]
        public void UpdateDependException()
        {
            var updateDependenciesMethod = typeof(Spreadsheet).GetMethod(
        "UpdateDependencies",
        BindingFlags.NonPublic | BindingFlags.Instance);
            var ex = Assert.Throws<TargetInvocationException>(() =>
        updateDependenciesMethod.Invoke(this.spreadsheet, new object[] { null }));
        }

        /// <summary>
        /// normal case for getting cell value.
        /// </summary>
        [Test]
        public void GetCellValueNormal()
        {
            var cellValueMethod = typeof(Spreadsheet).GetMethod(
        "GetCellValue",
        BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = this.spreadsheet.GetCell(0, 0);
            cell.SetValue("10");
            double cell_value = (double)cellValueMethod.Invoke(this.spreadsheet, new object[] { "A1" });
            Assert.That(cell_value, Is.EqualTo(10.0));
        }

        /// <summary>
        /// edge case for getting cell value using boundary row and col.
        /// </summary>
        [Test]
        public void GetCellValueEdge()
        {
            var cellValueMethod = typeof(Spreadsheet).GetMethod(
        "GetCellValue",
        BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = this.spreadsheet.GetCell(49, 25);
            cell.SetValue("50");
            double cell_value = (double)cellValueMethod.Invoke(this.spreadsheet, new object[] { "Z50" });
            Assert.That(cell_value, Is.EqualTo(50.0));
        }

        /// <summary>
        /// exception case passing in null.
        /// </summary>
        [Test]
        public void GetCellValueExpcetion()
        {
            var cellValueMethod = typeof(Spreadsheet).GetMethod(
        "GetCellValue",
        BindingFlags.NonPublic | BindingFlags.Instance);
            var ex = Assert.Throws<TargetInvocationException>(() =>
        cellValueMethod.Invoke(this.spreadsheet, new object[] { null }));
        }

        /// <summary>
        /// normal case for parsing cell.
        /// </summary>
        [Test]
        public void TryParseCellNormal()
        {
            var tryParseCellMethod = typeof(Spreadsheet).GetMethod(
        "TryParseCellReference",
        BindingFlags.NonPublic | BindingFlags.Instance);

            // helper function to invoke TryParseCell
            Func<string, (bool success, int row, int col)> invokeTryParse = (cellRef) =>
            {
                var parameters = new object[] { cellRef, null, null };
                var result = (bool)tryParseCellMethod.Invoke(this.spreadsheet, parameters);
                int row = (int)parameters[1];
                int col = (int)parameters[2];
                return (result, row, col);
            };
            var (success, row, col) = invokeTryParse("A1");
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.True, "A1 should parse successfully");
                Assert.That(row, Is.EqualTo(0), "A1 row should be 0");
                Assert.That(col, Is.EqualTo(0), "A1 column should be 0");
            });
        }

        /// <summary>
        /// edge case for parsing cell with boundary cell value.
        /// </summary>
        [Test]
        public void TryParseCellEdge()
        {
            var tryParseCellMethod = typeof(Spreadsheet).GetMethod(
        "TryParseCellReference",
        BindingFlags.NonPublic | BindingFlags.Instance);

            // helper function to invoke TryParseCell
            Func<string, (bool success, int row, int col)> invokeTryParse = (cellRef) =>
            {
                var parameters = new object[] { cellRef, null, null };
                var result = (bool)tryParseCellMethod.Invoke(this.spreadsheet, parameters);
                int row = (int)parameters[1];
                int col = (int)parameters[2];
                return (result, row, col);
            };
            var (success, row, col) = invokeTryParse("Z50");
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.True, "Z50 should parse successfully");
                Assert.That(row, Is.EqualTo(49), "Z50 row should be 49");
                Assert.That(col, Is.EqualTo(25), "Z50 column should be 25");
            });
        }

        /// <summary>
        /// expcetion case for pars ecell passing null.
        /// </summary>
        [Test]
        public void TryParseCellException()
        {
            var tryParseCellMethod = typeof(Spreadsheet).GetMethod(
        "TryParseCellReference",
        BindingFlags.NonPublic | BindingFlags.Instance);
            Func<string, (bool success, int row, int col)> invokeTryParse = (cellRef) =>
            {
                var parameters = new object[] { cellRef, null, null };
                var result = (bool)tryParseCellMethod.Invoke(this.spreadsheet, parameters);
                int row = (int)parameters[1];
                int col = (int)parameters[2];
                return (result, row, col);
            };
            var (success, row, col) = invokeTryParse(null);
            Assert.That(success, Is.False);
        }
    }
}
