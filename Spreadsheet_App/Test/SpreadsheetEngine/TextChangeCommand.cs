/**
 * @file: TextChangeCommand.cs
 * @brief: class to handle text changes.
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// command for changing cell text.
    /// </summary>
    public class TextChangeCommand : Command
    {
        /// <summary>
        /// cell who activated command.
        /// </summary>
        private Cell cell;

        /// <summary>
        /// old text in cell.
        /// </summary>
        private string oldText;

        /// <summary>
        /// new text in cell.
        /// </summary>
        private string newText;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextChangeCommand"/> class.
        /// </summary>
        /// <param name="cell">cell changed.</param>
        /// <param name="oldText">old text.</param>
        /// <param name="newText">new text.</param>
        public TextChangeCommand(Cell cell, string oldText, string newText)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell is TextChange command");
            }

            this.cell = cell;
            this.oldText = oldText;
            this.newText = newText;
        }

        /// <summary>
        /// Gets title of the command.
        /// </summary>
        public override string Title => "text change";

        /// <summary>
        /// executes command.
        /// </summary>
        public override void Execute()
        {
            this.cell.Text = this.newText;
            this.hasExecuted = true;
        }

        /// <summary>
        /// undoes command.
        /// </summary>
        public override void Undo()
        {
            if (!this.hasExecuted)
            {
                return;
            }

            this.cell.Text = this.oldText;
        }
    }
}
