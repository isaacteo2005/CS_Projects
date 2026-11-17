/**
 * @file: ColorChangeCommand.cs
 * @brief: command for changing color.
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
    /// command for changing cell background color.
    /// </summary>
    public class ColorChangeCommand : Command
    {
        /// <summary>
        /// cell who's color have changed.
        /// </summary>
        private Cell cell;

        /// <summary>
        /// old color of cell.
        /// </summary>
        private uint oldColor;

        /// <summary>
        /// new color for cell.
        /// </summary>
        private uint newColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorChangeCommand"/> class.
        /// </summary>
        /// <param name="cell">cell's color have changed.</param>
        /// <param name="oldColor">old color.</param>
        /// <param name="newColor">new color.</param>
        public ColorChangeCommand(Cell cell, uint oldColor, uint newColor)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("Color Change command's cell is null");
            }

            this.cell = cell;
            this.oldColor = oldColor;
            this.newColor = newColor;
        }

        /// <summary>
        /// Gets title of command.
        /// </summary>
        public override string Title => "color change";

        /// <summary>
        /// executes command.
        /// </summary>
        public override void Execute()
        {
            this.cell.BGColor = this.newColor;
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

            this.cell.BGColor = this.oldColor;
        }
    }
}
