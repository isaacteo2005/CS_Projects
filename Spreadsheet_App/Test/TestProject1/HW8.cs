/**
 * @file: HW8.cs
 * @brief: test cases for HW8
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class HW8
    {
        private Spreadsheet spreadsheet;
        private Cell cell;
        private Cell cellA1;
        private Cell cellB1;
        private Cell cellC1;
        private uint whiteColor = 0xFFFFFFFF;
        private uint redColor = 0xFFFF0000;
        private uint blueColor = 0xFF0000FF;

        [SetUp]
        public void Setup()
        {
            this.spreadsheet = new Spreadsheet(5, 5);
            this.cell = this.spreadsheet.GetCell(0, 0);
            this.cellA1 = this.spreadsheet.GetCell(0, 0);
            this.cellB1 = this.spreadsheet.GetCell(0, 1);
            this.cellC1 = this.spreadsheet.GetCell(0, 2);

            // initialize cell with default values
            this.cellA1.SetValue("Initial");
            this.cellB1.SetValue("Initial");
            this.cellC1.SetValue("Initial");
            this.cell.SetValue("Initial");
            this.cell.BGColor = this.whiteColor;
            this.cellA1.BGColor = this.whiteColor;
            this.cellB1.BGColor = this.whiteColor;
            this.cellC1.BGColor = this.whiteColor;
        }

        /// <summary>
        /// gets cell's BGColor.
        /// </summary>
        /// <param name="cell">cell.</param>
        /// <returns>unit.</returns>
        private uint GetCellBGColor(Cell cell)
        {
            return cell.BGColor;
        }

        /// <summary>
        /// sets cell's BGColor.
        /// </summary>
        /// <param name="cell">cell to set.</param>
        /// <param name="color">new color.</param>
        private void SetCellBGColor(Cell cell, uint color)
        {
            cell.BGColor = color;
        }

        /// <summary>
        /// normal case for Text Change execute showing that text and value gets changed.
        /// </summary>
        [Test]
#pragma warning disable SA1202 // Elements should be ordered by access
        public void TextChangeExecuteNormal()
#pragma warning restore SA1202 // Elements should be ordered by access
        {
            string oldText = "Initial";
            string newText = "Changed";
            var command = new TextChangeCommand(this.cell, oldText, newText);
            command.Execute();
            Assert.That(this.cell.Text, Is.EqualTo(newText));
            Assert.That(this.cell.Value, Is.EqualTo(newText));
        }

        /// <summary>
        /// edge case for Execute for Text Changed setting text to same text.
        /// </summary>
        [Test]
        public void TextChangeExecuteEdge()
        {
            string currentText = "Same";
            this.cell.Text = currentText;
            var command = new TextChangeCommand(this.cell, currentText, currentText);
            command.Execute();
            Assert.That(this.cell.Text, Is.EqualTo(currentText));
        }

        /// <summary>
        /// tests that double executes gives the right value and undoing each change reverts them.
        /// </summary>
        public void TextChangeExecuteEdge2()
        {
            var command1 = new TextChangeCommand(this.cell, "Initial", "First");
            var command2 = new TextChangeCommand(this.cell, "First", "Second");
            command1.Execute();
            command2.Execute();
            Assert.That(this.cell.Text, Is.EqualTo("Second"));
            command2.Undo();
            Assert.That(this.cell.Text, Is.EqualTo("First"));
            command1.Undo();
            Assert.That(this.cell.Text, Is.EqualTo("Initial"));
        }

        /// <summary>
        /// exception case for execute for text change. Passing in null for cell should throw exception.
        /// </summary>
        [Test]
        public void TextChangeExecuteException()
        {
            Assert.That(() => new TextChangeCommand(null, "old", "new"), Throws.ArgumentNullException);
        }

        /// <summary>
        /// normal case for redo and undo.
        /// </summary>
        [Test]
        public void TextChangeRedoUndoNormal()
        {
            string oldText = "Initial";
            string newText = "Changed";
            var command = new TextChangeCommand(this.cell, oldText, newText);
            command.Execute();
            command.Undo();
            Assert.That(this.cell.Text, Is.EqualTo(oldText));
            command.Undo();
            Assert.That(this.cell.Text, Is.EqualTo(oldText));
            Assert.That(this.cell.Value, Is.EqualTo(oldText));
        }

        /// <summary>
        /// edge case testing if undoing and redoing causes the old text to be equal and redoing changes it back to new text.
        /// </summary>
        [Test]
        public void TextChangeRedoUndoEdge()
        {
            string oldText = "Initial";
            string newText = "Changed";
            var command = new TextChangeCommand(this.cell, oldText, newText);
            command.Execute();
            command.Undo();
            Assert.That(this.cell.Text, Is.EqualTo(oldText));
            command.Redo();
            Assert.That(this.cell.Text, Is.EqualTo(newText));
            Assert.That(this.cell.Value, Is.EqualTo(newText));
        }

        /// <summary>
        /// Edge case testing if undo before execute does nothing.
        /// </summary>
        [Test]
        public void TextChangeUndoEdge()
        {
            string initialText = "Initial";
            this.cell.Text = initialText;
            var command = new TextChangeCommand(this.cell, "Old", "New");
            command.Undo();
            Assert.That(this.cell.Text, Is.EqualTo(initialText));
        }

        /// <summary>
        /// Edge case testing if Redo before execute does nothing.
        /// </summary>
        [Test]
        public void TextChangeRedoEdge()
        {
            string initialText = "Initial";
            this.cell.Text = initialText;
            var command = new TextChangeCommand(this.cell, "Old", "New");
            command.Redo();
            Assert.That(this.cell.Text, Is.EqualTo(initialText));
        }

        // no exception case for redo or undo since nothing gets passed in

        /// <summary>
        /// changing the text to null or setting null to text should not throw an exception.
        /// </summary>
        [Test]
        public void TextChangeConstructorException()
        {
            Assert.DoesNotThrow(() => new TextChangeCommand(this.cell, null, "new"));
            Assert.DoesNotThrow(() => new TextChangeCommand(this.cell, "old", null));
        }

        /// <summary>
        /// normal case for color change showing update color works.
        /// </summary>
        [Test]
        public void ColorChangeNormal()
        {
            uint oldColor = this.whiteColor;
            uint newColor = this.redColor;
            var command = new ColorChangeCommand(this.cell, oldColor, newColor);
            command.Execute();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(newColor));
        }

        /// <summary>
        /// edge case checking if setting color to itself is still equal.
        /// </summary>
        [Test]
        public void ColorChangeEdge()
        {
            uint currentColor = this.redColor;
            this.SetCellBGColor(this.cell, currentColor);
            var command = new ColorChangeCommand(this.cell, currentColor, currentColor);
            command.Execute();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(currentColor));
        }

        /// <summary>
        /// checks if multiple executions work and undoing each change returns the original result.
        /// </summary>
        public void ColorChangeEdge2()
        {
            var command1 = new ColorChangeCommand(this.cell, this.whiteColor, this.redColor);
            var command2 = new ColorChangeCommand(this.cell, this.redColor, this.blueColor);
            command1.Execute();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(this.redColor));
            command2.Execute();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(this.blueColor));
            command2.Undo();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(this.redColor));
            command1.Undo();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(this.whiteColor));
        }

        /// <summary>
        /// exception case where exception thrown if cell is null.
        /// </summary>
        [Test]
        public void ColorChangeException()
        {
            Assert.That(() => new ColorChangeCommand(null, this.whiteColor, this.redColor), Throws.ArgumentNullException);
        }

        /// <summary>
        /// normal case checking if undo works.
        /// </summary>
        [Test]
        public void ColorChangeUndoNormal()
        {
            uint oldColor = this.whiteColor;
            uint newColor = this.redColor;
            var command = new ColorChangeCommand(this.cell, oldColor, newColor);
            command.Execute();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(newColor));
            command.Undo();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(oldColor));
        }

        /// <summary>
        /// normal case checking if redo works.
        /// </summary>
        [Test]
        public void ColorChangeRedoNormal()
        {
            uint oldColor = this.whiteColor;
            uint newColor = this.redColor;
            var command = new ColorChangeCommand(this.cell, oldColor, newColor);
            command.Execute();
            command.Undo();
            command.Redo();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(newColor));
        }

        /// <summary>
        /// Edge case checking if undo before execute doesn't change anything.
        /// </summary>
        [Test]
        public void ColorChangeUndoEdge()
        {
            uint initialColor = this.whiteColor;
            this.SetCellBGColor(this.cell, initialColor);
            var command = new ColorChangeCommand(this.cell, this.redColor, this.blueColor);
            command.Undo();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(initialColor));
        }

        /// <summary>
        /// Edge case checking if redo before execute doesn;t change anything.
        /// </summary>
        [Test]
        public void ColorChangeRedoEdge()
        {
            uint initialColor = this.whiteColor;
            this.SetCellBGColor(this.cell, initialColor);
            var command = new ColorChangeCommand(this.cell, this.redColor, this.blueColor);
            command.Redo();
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(initialColor));
        }

        // no exception for redo or undo since they both don't take in parameters

        /// <summary>
        /// normal case for composite command, makes sure both commands executed.
        /// </summary>
        [Test]
        public void CompositeNormal()
        {
            var composite = new CompositeCommand();
            var command1 = new TextChangeCommand(this.cell, "A", "B");
            var command2 = new ColorChangeCommand(this.cell, this.whiteColor, this.redColor);
            composite.Add(command1);
            composite.Add(command2);
            composite.Execute();
            Assert.That(this.cell.Text, Is.EqualTo("B"));
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(this.redColor));
        }

        /// <summary>
        /// edge case for composite command, checks if List is empty, nothing changes.
        /// </summary>
        public void CompositeEdge()
        {
            var composite = new CompositeCommand();
            string originalText = this.cell.Text;
            uint originalColor = this.GetCellBGColor(this.cell);
            composite.Execute();
            Assert.Multiple(() =>
            {
                Assert.That(this.cell.Text, Is.EqualTo(originalText));
                Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(originalColor));
            });
        }

        /// <summary>
        /// exception case checking if adding null to the composite command throws an exception.
        /// </summary>
        [Test]
        public void CompositeException()
        {
            var composite = new CompositeCommand();
            Assert.That(() => composite.Add(null), Throws.ArgumentNullException);
        }

        /// <summary>
        /// normal case for composite undo making sure it works.
        /// </summary>
        [Test]
        public void CompositeUndoNormal()
        {
            var composite = new CompositeCommand();
            composite.Add(new TextChangeCommand(this.cell, "A", "B"));
            composite.Add(new TextChangeCommand(this.cell, "B", "C"));
            composite.Add(new ColorChangeCommand(this.cell, this.whiteColor, this.redColor));

            composite.Execute();
            Assert.That(this.cell.Text, Is.EqualTo("C"));
            Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(this.redColor));
            composite.Undo();

            // undoes color then text
            Assert.Multiple(() =>
            {
                Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(this.whiteColor));
                Assert.That(this.cell.Text, Is.EqualTo("A"));
            });
        }

        /// <summary>
        /// edge case checking if undo on an empty command list does nothing.
        /// </summary>
        [Test]
        public void CompisteUndoEdge()
        {
            var composite = new CompositeCommand();
            string originalText = this.cell.Text;
            uint originalColor = this.GetCellBGColor(this.cell);
            composite.Undo();
            Assert.Multiple(() =>
            {
                Assert.That(this.cell.Text, Is.EqualTo(originalText));
                Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(originalColor));
            });
        }

        /// <summary>
        /// normal case for redo checking if it works.
        /// </summary>
        [Test]
        public void CompositeRedoNormal()
        {
            var composite = new CompositeCommand();
            composite.Add(new TextChangeCommand(this.cell, "A", "B"));
            composite.Add(new TextChangeCommand(this.cell, "B", "C"));

            composite.Execute();
            composite.Undo();
            Assert.That(this.cell.Text, Is.EqualTo("A"));
            composite.Redo();
            Assert.That(this.cell.Text, Is.EqualTo("C"));
        }

        /// <summary>
        /// edge case checking if redo on an empty command list does nothing.
        /// </summary>
        [Test]
        public void CompisteRedoEdge()
        {
            var composite = new CompositeCommand();
            string originalText = this.cell.Text;
            uint originalColor = this.GetCellBGColor(this.cell);
            composite.Redo();
            Assert.Multiple(() =>
            {
                Assert.That(this.cell.Text, Is.EqualTo(originalText));
                Assert.That(this.GetCellBGColor(this.cell), Is.EqualTo(originalColor));
            });
        }

        /// <summary>
        /// edge case checking if composite command can behave like a single command.
        /// </summary>
        [Test]
        public void CompositeUndoRedoEdge()
        {
            var composite = new CompositeCommand();
            composite.Add(new TextChangeCommand(this.cell, "A", "B"));
            composite.Execute();
            Assert.That(this.cell.Text, Is.EqualTo("B"));
            composite.Undo();
            Assert.That(this.cell.Text, Is.EqualTo("A"));
            composite.Redo();
            Assert.That(this.cell.Text, Is.EqualTo("B"));
        }

        // no exception case as undo and redo don't take in parameters

        /// <summary>
        /// normal case for spreadsheet undo checking if it works for text.
        /// </summary>
        [Test]
        public void UndoNormal()
        {
            string originalText = this.cellA1.Text;
            string newText = "Changed";
            this.spreadsheet.SetCellText(this.cellA1, newText);
            this.spreadsheet.Undo();
            Assert.That(this.cellA1.Text, Is.EqualTo(originalText));
            Assert.That(this.cellA1.Value, Is.EqualTo(originalText));
        }

        /// <summary>
        /// normal case for spreadsheet undo checking if it works for color.
        /// </summary>
        [Test]
        public void UndoNormal2()
        {
            uint originalColor = this.GetCellBGColor(this.cellA1);
            this.spreadsheet.SetCellColor(this.cellA1, this.redColor);
            this.spreadsheet.Undo();
            Assert.That(this.GetCellBGColor(this.cellA1), Is.EqualTo(originalColor));
        }

        /// <summary>
        /// normal case for undo checking if multiple color changes works normal.
        /// </summary>
        [Test]
        public void UndoNormal3()
        {
            var cells = new List<Cell> { this.cellA1, this.cellB1, this.cellC1 };
            uint oldColorA1 = this.GetCellBGColor(this.cellA1);
            uint oldColorB1 = this.GetCellBGColor(this.cellB1);
            uint oldColorC1 = this.GetCellBGColor(this.cellC1);
            this.spreadsheet.SetCellColors(cells, this.redColor);
            Assert.Multiple(() =>
            {
                Assert.That(this.GetCellBGColor(this.cellA1), Is.EqualTo(this.redColor));
                Assert.That(this.GetCellBGColor(this.cellB1), Is.EqualTo(this.redColor));
                Assert.That(this.GetCellBGColor(this.cellC1), Is.EqualTo(this.redColor));
            });
            this.spreadsheet.Undo();

            Assert.Multiple(() =>
            {
                Assert.That(this.GetCellBGColor(this.cellA1), Is.EqualTo(oldColorA1), "A1 should revert");
                Assert.That(this.GetCellBGColor(this.cellB1), Is.EqualTo(oldColorB1), "B1 should revert");
                Assert.That(this.GetCellBGColor(this.cellC1), Is.EqualTo(oldColorC1), "C1 should revert");
            });
            Assert.That(this.spreadsheet.CanUndo, Is.False);
        }

        /// <summary>
        /// edge case for undo checking if undo on an epty stack keeps it the same.
        /// </summary>
        [Test]
        public void UndoEdge()
        {
            string originalText = this.cellA1.Text;
            uint originalColor = this.GetCellBGColor(this.cellA1);
            this.spreadsheet.Undo();
            Assert.Multiple(() =>
            {
                Assert.That(this.cellA1.Text, Is.EqualTo(originalText));
                Assert.That(this.GetCellBGColor(this.cellA1), Is.EqualTo(originalColor));
            });
        }

        // no exception case as undo accepts no parameters

        /// <summary>
        /// checks if redo is working normally.
        /// </summary>
        [Test]
        public void RedoNormal()
        {
            string newText = "Changed";
            this.spreadsheet.SetCellText(this.cellA1, newText);
            this.spreadsheet.Undo();
            this.spreadsheet.Redo();
            Assert.That(this.cellA1.Text, Is.EqualTo(newText));
        }

        /// <summary>
        /// checks if redo works for colors.
        /// </summary>
        [Test]
        public void RedoNormal2()
        {
            uint newColor = this.redColor;
            this.spreadsheet.SetCellColor(this.cellA1, newColor);
            this.spreadsheet.Undo();
            this.spreadsheet.Redo();
            Assert.That(this.GetCellBGColor(this.cellA1), Is.EqualTo(newColor));
        }

        /// <summary>
        /// edge case for Redo checking if undo on an epty stack keeps it the same.
        /// </summary>
        [Test]
        public void RedoEdge()
        {
            string originalText = this.cellA1.Text;
            uint originalColor = this.GetCellBGColor(this.cellA1);
            this.spreadsheet.Redo();
            Assert.Multiple(() =>
            {
                Assert.That(this.cellA1.Text, Is.EqualTo(originalText));
                Assert.That(this.GetCellBGColor(this.cellA1), Is.EqualTo(originalColor));
            });
        }

        // no edge cases found for redo

        /// <summary>
        /// checks if passing in null to AddUndo throws exceptions.
        /// </summary>
        [Test]
        public void AddUndoException()
        {
            Assert.That(() => this.spreadsheet.AddUndo(null), Throws.ArgumentNullException);
        }

        // no normal or edge case since the Add is relatively similar to a stack push.
        [Test]
        public void SettersException()
        {
            Assert.That(() => this.spreadsheet.SetCellText(null, "text"), Throws.ArgumentNullException);
            Assert.That(() => this.spreadsheet.SetCellColor(null, this.redColor), Throws.ArgumentNullException);
            Assert.That(() => this.spreadsheet.SetCellColors(null, this.redColor), Throws.ArgumentNullException);
        }

        // no normal or edge case as it gets tested through the Undo and Redo tests and is similar to any setter
        // no test cases for the Next or Can functions as they are simple retrieval and boolean functions respectively
    }
}
