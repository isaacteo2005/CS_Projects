/**
 * @file: Command.cs
 * @brief: abstract class for each Command
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
    /// abstract class for undo/redo commands.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// checks if a command has executed before.
        /// </summary>
        protected bool hasExecuted = false;

        /// <summary>
        /// gets the title/desc of command for UI display.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// executes command.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// undo command.
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// redoes command.
        /// </summary>
        public virtual void Redo()
        {
            if (!this.hasExecuted)
            {
                return;
            }

            this.Execute();
        }
    }
}
