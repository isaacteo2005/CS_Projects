/**
 * @file: CompositeCommand.cs
 * @brief: command for grouped commands.
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
    /// composite command that groups multiple commands together. Used for batch operations like changing multiple cells at once.
    /// </summary>
    public class CompositeCommand : Command
    {
        /// <summary>
        /// list of commands to execute.
        /// </summary>
        private List<Command> commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeCommand"/> class.
        /// </summary>
        public CompositeCommand()
        {
            this.commands = new List<Command>();
        }

        /// <summary>
        /// gets title of command.
        /// </summary>
        public override string Title => "multiple changes";

        /// <summary>
        /// gets the number of commands.
        /// </summary>
        public int Count => this.commands.Count;

        /// <summary>
        /// Adds a command to the list.
        /// </summary>
        /// <param name="command">command to add.</param>
        public void Add(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("Command added to composite command is null");
            }

            this.commands.Add(command);
        }

        /// <summary>
        /// executes command.
        /// </summary>
        public override void Execute()
        {
            foreach (var command in this.commands)
            {
                command.Execute();
            }

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

            // undoes commands in reverse order
            for (int i = this.commands.Count - 1; i >= 0; i--)
            {
                this.commands[i].Undo();
            }
        }

        /// <summary>
        /// redoes command.
        /// </summary>
        public override void Redo()
        {
            if (!this.hasExecuted)
            {
                return;
            }

            foreach (var command in this.commands)
            {
                command.Redo();
            }
        }
    }
}
