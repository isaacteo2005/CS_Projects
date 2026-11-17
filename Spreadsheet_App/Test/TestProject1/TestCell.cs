namespace TestProject1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SpreadsheetEngine;
    using static System.Net.Mime.MediaTypeNames;

    // Used AI for help creating a class that can access Cell protected fields.
    // Test helper class that derives from Cell to access protected fields.
    // Warning supressed due to file being correctly named.

    /// <summary>
    /// Class to test cell methods.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public class TestCell : Cell
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="TestCell"/> class.
        /// </summary>
        /// <param name="rowIndex"> index of row.</param>
        /// <param name="columnIndex"> index of column. </param>
        public TestCell(int rowIndex, int columnIndex)
            : base(rowIndex, columnIndex)
        {
        }

        // Expose protected fields through public methods for testing

        /// <summary>
        /// Gets the text field.
        /// </summary>
        /// <returns>text field.</returns>
        public string GetTextField()
        {
            return this.text; // Accessing
        }

        /// <summary>
        /// gets the value field.
        /// </summary>
        /// <returns>value field.</returns>
        public string GetValueField()
        {
            return this.value; // Accessing protected field
        }

        // Optional: Expose the internal SetValue method if needed for testing

        /// <summary>
        /// exposes set internal value for testing.
        /// </summary>
        /// <param name="newValue"> value to set.</param>
        public void ExposeSetValue(string newValue)
        {
            this.SetValue(newValue); // Calling internal method
        }
    }
}
