/**
 * @file: HW4.cs
 * @brief: test cases for HW4
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using TestProject1;
    using static System.Net.Mime.MediaTypeNames;

    internal class HW4
    {
        /// <summary>
        /// normal case testing new cell with normal numbers.
        /// </summary>
        [Test]
        public void TestCellConstructorNormal()
        {
            int row = 5;
            int col = 3;
            var cell = new TestCell(row, col);

            // cell and row index are equal to parameters
            Assert.That(cell.RowIndex, Is.EqualTo(row));
            Assert.That(cell.ColumnIndex, Is.EqualTo(col));

            // text and value are empty
            Assert.That(cell.Text, Is.EqualTo(string.Empty));
            Assert.That(cell.Value, Is.EqualTo(string.Empty));

            // get methods return empty fields
            Assert.That(cell.GetTextField(), Is.EqualTo(string.Empty));
            Assert.That(cell.GetValueField(), Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// edge case testing using int.maxValue.
        /// </summary>
        [Test]
        public void TestCellConstructorEdge()
        {
            int row = int.MaxValue;
            int col = int.MaxValue;
            var cell = new TestCell(row, col);

            // row and column index should be able to take in max values
            Assert.That(cell.RowIndex, Is.EqualTo(row));
            Assert.That(cell.ColumnIndex, Is.EqualTo(col));
        }

        /// <summary>
        /// exception case testing for negative cells.
        /// </summary>
        [Test]
        public void TestCellConstructorException()
        {
            int row = -5;
            int col = -3;
            var cell = new TestCell(row, col);

            // row and column should not accept negative rows and cols and set both to 0 instead
            Assert.That(cell.RowIndex, Is.EqualTo(0));
            Assert.That(cell.ColumnIndex, Is.EqualTo(0));
            Assert.That(cell.Text, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// normal case for text setter using regular string.
        /// </summary>
        [Test]
        public void TestTextSetterNormal()
        {
            var cell = new TestCell(5, 3);
            cell.Text = "Text";
            Assert.That(cell.GetTextField(), Is.EqualTo("Text"));
        }

        /// <summary>
        /// edge case for text setter for leading and ending white space.
        /// </summary>
        [Test]
        public void TestTextSetterEdge()
        {
            var cell = new TestCell(5, 3);
            cell.Text = " Text ";
            Assert.That(cell.Text, Is.EqualTo(" Text "));
        }

        // could not find exception case as no invalid values would cause an error

        /// <summary>
        /// normal case for set value using regular ints.
        /// </summary>
        [Test]
        public void TestSetValueNormal()
        {
            var cell = new TestCell(5, 3);
            cell.ExposeSetValue("One");
            Assert.That(cell.Value, Is.EqualTo("One"));
        }

        /// <summary>
        /// Edge case for set value testing if spreadsheet application can use setValue.
        /// </summary>
        [Test]
        public void TestSetValueEdge()
        {
            Spreadsheet sheet = new Spreadsheet(3, 3);
            var cell = sheet.GetCell(1, 1) as ConcreteCell;

            // triggers onCallTextChanged
            cell.Text = "Hello";
            Assert.That(cell.Value, Is.EqualTo("Hello"));
        }

        // could not find exception case, smiliar to text test cases

        /// <summary>
        /// normal case for constructor spreadsheet.
        /// </summary>
        [Test]
        public void TestSpreadsheetConstructorNormal()
        {
            int rows = 5;
            int cols = 3;
            var sheet = new Spreadsheet(rows, cols);

            // rows and columns should match
            Assert.That(sheet.RowCount, Is.EqualTo(rows));
            Assert.That(sheet.ColumnCount, Is.EqualTo(cols));

            // each cell should not be null
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var cell = sheet.GetCell(r, c);
                    Assert.That(cell, Is.Not.Null);
                }
            }
        }

        /// <summary>
        /// edge case for spreadsheet constructor testing for 0 rows.
        /// </summary>
        [Test]
        public void TestSpreadsheetConstructorEdge()
        {
            var sheet = new Spreadsheet(0, 5);
            Assert.That(sheet.RowCount, Is.EqualTo(0));
            Assert.That(sheet.ColumnCount, Is.EqualTo(5));

            // should not have cells if rows = 0
            Assert.That(sheet.GetCell(0, 0), Is.Null);
        }

        /// <summary>
        /// testing exception case for negative rows.
        /// </summary>
        [Test]
        public void TestSpreadsheetConstructorException()
        {
            var sheet = new Spreadsheet(-5, 5);

            // negative values should set to 0
            Assert.That(sheet.RowCount, Is.EqualTo(0));
            Assert.That(sheet.ColumnCount, Is.EqualTo(5));
        }

        /// <summary>
        /// test case for getcell with normal indicides.
        /// </summary>
        [Test]
        public void TestGetCellNormal()
        {
            int rows = 5;
            int cols = 3;
            var sheet = new Spreadsheet(rows, cols);
            var cell = sheet.GetCell(2, 1);
            Assert.That(cell, Is.Not.Null);
            Assert.That(cell.RowIndex, Is.EqualTo(2));
            Assert.That(cell.ColumnIndex, Is.EqualTo(1));
        }

        /// <summary>
        /// edge case for getcell testing boundary indicies.
        /// </summary>
        [Test]
        public void TestGetCellEdge()
        {
            int rows = 5;
            int cols = 3;
            var sheet = new Spreadsheet(rows, cols);
            var cell = sheet.GetCell(4, 2);
            Assert.That(cell, Is.Not.Null);
        }

        /// <summary>
        /// exception case for getcell tetsing for negative rows.
        /// </summary>
        [Test]
        public void TestGetCellException()
        {
            int rows = 5;
            int cols = 3;
            var sheet = new Spreadsheet(rows, cols);

            // should return null as negative cells don't exist
            var cell = sheet.GetCell(-4, 2);
            Assert.That(cell, Is.Null);
        }

        /// <summary>
        /// normal case for getcellreference testing getting an ordinary reference.
        /// </summary>
        [Test]
        public void TestCellRefernceNormal()
        {
            var sheet = new Spreadsheet(5, 3);

            // AI helped on figuring out how to use reflection to access private methods
            var method = typeof(Spreadsheet).GetMethod("GetCellFromReference", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Cell result = method.Invoke(sheet, new object[] { "B2" }) as Cell;

            // should be index (1,1)
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RowIndex, Is.EqualTo(1));
            Assert.That(result.ColumnIndex, Is.EqualTo(1));
        }

        /// <summary>
        /// edge case for getcellreference testing upper boundary for cell reference.
        /// </summary>
        [Test]
        public void TestCellRefernceEdge()
        {
            var sheet = new Spreadsheet(50, 26);
            var method = typeof(Spreadsheet).GetMethod("GetCellFromReference", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Cell result = method.Invoke(sheet, new object[] { "Z50" }) as Cell;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RowIndex, Is.EqualTo(49));
            Assert.That(result.ColumnIndex, Is.EqualTo(25));
        }

        /// <summary>
        /// exception case for getcellreference testing negative column number.
        /// </summary>
        [Test]
        public void TestCellRefernceException()
        {
            var sheet = new Spreadsheet(50, 26);
            var method = typeof(Spreadsheet).GetMethod("GetCellFromReference", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Cell result = method.Invoke(sheet, new object[] { "A-5" }) as Cell;
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// normal case on cell text changed method changing a normal cell's value.
        /// </summary>
        [Test]
        public void TestCellTextChangedNormal()
        {
            var sheet = new Spreadsheet(50, 26);
            var cell = sheet.GetCell(5, 5);
            cell.Text = "Hello World";
            var method = typeof(Spreadsheet).GetMethod("OnCellTextChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // invokes OnCellTextChanged from method
            method.Invoke(sheet, new object[] { cell });
            Assert.That(cell.Value, Is.EqualTo("Hello World"));
        }

        /// <summary>
        /// edge case of OnCellTextChanged changing cell text of lower bounds of spreadsheet and getting the value.
        /// </summary>
        [Test]
        public void TestCellTextChangedEdge()
        {
            var sheet = new Spreadsheet(50, 26);
            var cell = sheet.GetCell(0, 0);
            cell.Text = "First Cell Value";
            var method = typeof(Spreadsheet).GetMethod("OnCellTextChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(sheet, new object[] { cell });
            Assert.That(cell.Value, Is.EqualTo("First Cell Value"));
        }

        /// <summary>
        /// exception case testing formula case reference outside of spreadsheet boundaries.
        /// </summary>
        [Test]
        public void TestCellTextChangedException()
        {
            var sheet = new Spreadsheet(50, 26);
            var cell = sheet.GetCell(0, 0);
            cell.Text = "=A51";
            var method = typeof(Spreadsheet).GetMethod("OnCellTextChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(sheet, new object[] { cell });
            Assert.That(cell.Value, Is.EqualTo("#REF!"));
        }
    }
}
