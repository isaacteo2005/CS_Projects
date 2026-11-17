/**
 * @file: HW10.cs
 * @brief: test cases for HW10
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

    // changed methods tested in tests from before
    internal class HW10
    {
        private Spreadsheet spreadsheet;
        private MethodInfo hasCircularReferenceMethod;
        private MethodInfo clearErrorStateMethod;
        private MethodInfo updateCellValueMethod;

        [SetUp]
        public void Setup()
        {
            this.spreadsheet = new Spreadsheet(50, 26);

            this.hasCircularReferenceMethod = typeof(Spreadsheet).GetMethod("HasCircularReference", BindingFlags.NonPublic | BindingFlags.Instance);

            this.updateCellValueMethod = typeof(Spreadsheet).GetMethod("UpdateCellValue", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// uses relfection to invoke circular reference method.
        /// </summary>
        /// <param name="cell">cell to pass.</param>
        /// <param name="formula">formula to pass.</param>
        /// <returns>method invoke.</returns>
        private bool InvokeHasCircularReference(Cell cell, string formula)
        {
            return (bool)this.hasCircularReferenceMethod.Invoke(this.spreadsheet, new object[] { cell, formula, null });
        }

        /// <summary>
        /// uses reflection to invoke update cell value.
        /// </summary>
        /// <param name="cell">cell to update.</param>
        private void InvokeUpdateCellValue(Cell cell)
        {
            this.updateCellValueMethod.Invoke(this.spreadsheet, new object[] { cell });
        }

        [Test]
        /// <summary>
        /// normal case for circle ref checking if no circle ref returns false.
        /// </summary>
#pragma warning disable SA1202 // Elements should be ordered by access
        public void CircleRefNormal()
#pragma warning restore SA1202 // Elements should be ordered by access
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            var b1 = this.spreadsheet.GetCell(0, 1);
            a1.Text = "5";
            b1.Text = "=A1*2";
            bool result = this.InvokeHasCircularReference(b1, "A1*2");
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// normal case for circle ref return true.
        /// </summary>
        [Test]
        public void CircleRefNormal2()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            bool result = this.InvokeHasCircularReference(a1, "A1");
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// normal case for no circle ref with multiple cells.
        /// </summary>
        [Test]
        public void CircleRefNormal3()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            var b1 = this.spreadsheet.GetCell(0, 1);
            var c1 = this.spreadsheet.GetCell(0, 2);
            var d1 = this.spreadsheet.GetCell(0, 3);

            a1.Text = "5";
            b1.Text = "10";
            c1.Text = "=A1+B1";
            d1.Text = "=C1*2";
            bool result = this.InvokeHasCircularReference(d1, "C1*2");

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// normal case for circle ref with multiple cells return true.
        /// </summary>
        [Test]
        public void CircleRefNormal4()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            var b1 = this.spreadsheet.GetCell(0, 1);
            var c1 = this.spreadsheet.GetCell(0, 2);

            a1.Text = "=B1";
            b1.Text = "=C1";
            c1.Text = "=A1";

            bool result = this.InvokeHasCircularReference(a1, "B1");

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// edge case for circle ref checking for reference on out of bounds cell.
        /// </summary>
        [Test]
        public void CircleRefEdge()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            bool result = this.InvokeHasCircularReference(a1, "A51");
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// edge case for circle ref checking for circle ref looping back to A1.
        /// </summary>
        [Test]
        public void CircleRefEdge2()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            var b1 = this.spreadsheet.GetCell(0, 1);
            var c1 = this.spreadsheet.GetCell(0, 2);

            a1.Text = "=B1+C1";
            b1.Text = "=A1";
            c1.Text = "=D1";

            // check from A1 (has path from B1 back to A1)
            bool result = this.InvokeHasCircularReference(a1, "B1+C1");
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// excep case for circle ref checking for exception for null cell.
        /// </summary>
        [Test]
        public void CircleRefException()
        {
            Assert.That(() => this.InvokeHasCircularReference(null, "A1"), Throws.TargetInvocationException);
        }

        /// <summary>
        /// excep case for circle ref checking for exception for null formula.
        /// </summary>
        [Test]
        public void CircleRefException2()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            bool result = this.InvokeHasCircularReference(a1, null);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// excep case for circle ref checking for graceful handling of invalid formula.
        /// </summary>
        [Test]
        public void CircleRefException3()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);

            // no exception thrown for invalid formula, should handle gracefully
            Assert.DoesNotThrow(() => this.InvokeHasCircularReference(a1, "INVALID"));
        }

        /// <summary>
        /// checks if clear error state works when clearing REF.
        /// </summary>
        [Test]
        public void UpdateCellValueNormal()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            a1.Text = "=Z999"; // invalid reference
            this.InvokeUpdateCellValue(a1);
            Assert.That(a1.Value, Is.EqualTo("#REF!"));

            // fix formula
            a1.Text = "42";
            this.InvokeUpdateCellValue(a1);

            Assert.That(a1.Value, Is.EqualTo("42"));
        }

        /// <summary>
        /// checks if clear error state works when clearing circle ref.
        /// </summary>
        [Test]
        public void UpdateCellValueNormal2()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            a1.Text = "=A1"; // self reference
            this.InvokeUpdateCellValue(a1);
            Assert.That(a1.Value, Is.EqualTo("#CIRCLE!"));
            a1.Text = "5";
            this.InvokeUpdateCellValue(a1);
            Assert.That(a1.Value, Is.EqualTo("5"));
        }

        /// <summary>
        /// checks if clear error state has original value if no errors exist.
        /// </summary>
        [Test]
        public void UpdateCellValueNormal3()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            a1.Text = "42";
            this.InvokeUpdateCellValue(a1);
            string originalValue = a1.Value;
            Assert.That(a1.Value, Is.EqualTo(originalValue));
        }

        /// <summary>
        /// checks if clear error state works for error ref.
        /// </summary>
        [Test]
        public void UpdateCellValueEdge2()
        {
            var a1 = this.spreadsheet.GetCell(0, 0);
            a1.Text = "=+"; // Invalid syntax
            this.InvokeUpdateCellValue(a1);
            Assert.That(a1.Value, Is.EqualTo("#ERROR!"));
            a1.Text = "10";
            this.InvokeUpdateCellValue(a1);
            Assert.That(a1.Value, Is.EqualTo("10"));
        }

        [Test]
        public void UpdateCellValueIntegration()
        {
            // create circular ref
            var a1 = this.spreadsheet.GetCell(0, 0);
            var b1 = this.spreadsheet.GetCell(0, 1);
            var c1 = this.spreadsheet.GetCell(0, 2);

            a1.Text = "=B1";
            b1.Text = "=A1";
            c1.Text = "=A1+B1";

            this.InvokeUpdateCellValue(a1);
            this.InvokeUpdateCellValue(b1);
            this.InvokeUpdateCellValue(c1);

            // every value should be circle ref
            Assert.That(a1.Value, Is.EqualTo("#CIRCLE!"));
            Assert.That(b1.Value, Is.EqualTo("#CIRCLE!"));
            Assert.That(c1.Value, Is.EqualTo("#CIRCLE!"));

            // break circle ref
            a1.Text = "5";
            this.InvokeUpdateCellValue(a1);
            this.InvokeUpdateCellValue(b1);
            this.InvokeUpdateCellValue(c1);

            Assert.That(a1.Value, Is.EqualTo("5"));
            Assert.That(b1.Value, Is.EqualTo("5"));
            Assert.That(c1.Value, Is.EqualTo("10"));
        }
    }
}
