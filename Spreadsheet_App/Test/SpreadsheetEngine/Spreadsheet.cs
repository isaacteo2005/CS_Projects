/**
 * @file: Spreadsheet.cs
 * @brief: spreadsheet class for spreadsheet
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Spreadsheet application for class library.
    /// </summary>
    public class Spreadsheet
    {
        /// <summary>
        /// reference error constraint.
        /// </summary>
        private const string ERRORREF = "#REF!";

        /// <summary>
        /// circle error constraint.
        /// </summary>
        private const string ERRORCIRCLE = "#CIRCLE!";

        /// <summary>
        /// general error constraint.
        /// </summary>
        private const string ERRORGENERAL = "#ERROR!";

        /// <summary>
        /// deafult background color white.
        /// </summary>
        private const uint DefaultBGColor = 0xFFFFFFFF;

        /// <summary>
        /// default text (empty string).
        /// </summary>
        private const string DefaultText = "";

        /// <summary>
        /// 2D array of cells for spreadsheet.
        /// </summary>
        private Cell[,] cells;

        /// <summary>
        /// What points to each cell (Ex: A1. <- B1, C1).
        /// </summary>
        private Dictionary<Cell, List<Cell>> dependents;

        /// <summary>
        /// What each cell is point too (Ex: B1 -> A1).
        /// </summary>
        private Dictionary<Cell, List<Cell>> references;

        /// <summary>
        /// stack for undoing..
        /// </summary>
        private Stack<Command> undoStack;

        /// <summary>
        /// stack for redoing.
        /// </summary>
        private Stack<Command> redoStack;

        /// <summary>
        /// indicates whether spreadsheet has unsaved changes.
        /// </summary>
        private bool hasUnsavedChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows"> number of rows.</param>
        /// <param name="cols">number of columns.</param>
        public Spreadsheet(int rows, int cols)
        {
            // neither column nor row should be negative and should reset to 0 if so
            // sets the row and column count
            if (rows >= 0 && cols >= 0)
            {
                this.RowCount = rows;
                this.ColumnCount = cols;
            }
            else
            {
                if (rows < 0 && cols < 0)
                {
                    this.RowCount = 0;
                    this.ColumnCount = 0;
                }
                else if (cols < 0)
                {
                    this.RowCount = rows;
                    this.ColumnCount = 0;
                }
                else
                {
                    this.RowCount = 0;
                    this.ColumnCount = cols;
                }
            }

            this.cells = new Cell[this.RowCount, this.ColumnCount];

            int row_count = this.RowCount;
            int column_count = this.ColumnCount;

            // initializes each cell
            for (int r = 0; r < row_count; r++)
            {
                for (int c = 0; c < column_count; c++)
                {
                    // Create a new cell using concrete implementation
                    this.cells[r, c] = new ConcreteCell(r, c);

                    // calls event handler for each cell
                    this.cells[r, c].PropertyChanged += this.HandleCellPropertyChanged;
                }
            }

            // initializes references and dependencies
            this.references = new Dictionary<Cell, List<Cell>>();
            this.dependents = new Dictionary<Cell, List<Cell>>();

            // initializes redo and undo stack.
            this.redoStack = new Stack<Command>();
            this.undoStack = new Stack<Command>();
        }

        /// <summary>
        /// notifies UI for cell changes.
        /// </summary>
        public event PropertyChangedEventHandler CellPropertyChanged;

        /// <summary>
        /// event fires when unsaved change status changes.
        /// </summary>
        public event Action OnHasUnsavedChangesChanged;

        /// <summary>
        /// Gets number of rows.
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// Gets number of columns.
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// Gets a value indicating whether if undo is possible.
        /// </summary>
        public bool CanUndo => this.undoStack.Count > 0;

        /// <summary>
        /// Gets a value indicating whether if redo is possible.
        /// </summary>
        public bool CanRedo => this.redoStack.Count > 0;

        /// <summary>
        /// gets the title of nexrt undo command.
        /// </summary>
        public string NextUndoTitle => this.CanUndo ? this.undoStack.Peek().Title : string.Empty;

        /// <summary>
        /// gets the title of nexrt redo command.
        /// </summary>
        public string NextRedoTitle => this.CanRedo ? this.redoStack.Peek().Title : string.Empty;

        /// <summary>
        /// gets a value indicating whether spreadsheet has unsaved changes. Only Spreadsheet can modify this property.
        /// </summary>
        public bool HasUnsavedChanges
        {
            get => this.hasUnsavedChanges;
            private set
            {
                if (this.hasUnsavedChanges != value)
                {
                    this.hasUnsavedChanges = value;
                    this.OnHasUnsavedChangesChanged?.Invoke();
                }
            }
            }

        /// <summary>
        /// saves the spreadsheet to XML stream.
        /// </summary>
        /// <param name="stream">XMl to save too.</param>
        public void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanWrite)
            {
                throw new InvalidOperationException("Stream is not writable");
            }

            try
            {
                // create XML writer settings for proper formatting
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    ",
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false,
                };

                using (var writer = XmlWriter.Create(stream, settings))
                {
                    // write XML declaration
                    writer.WriteStartDocument();

                    // write root element
                    writer.WriteStartElement("spreadsheet");

                    // go through all cells
                    for (int row = 0; row < this.RowCount; row++)
                    {
                        for (int col = 0; col < this.ColumnCount; col++)
                        {
                            Cell cell = this.cells[row, col];

                            // only save cells with non-default properties
                            if (this.HasNonDefaultProperties(cell))
                            {
                                string cellName = this.GetCellName(row, col);
                                this.WriteCell(writer, cell, cellName);
                            }
                        }
                    }

                    // write end of root element
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    this.HasUnsavedChanges = false;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save spreadsheet");
            }
        }

        /// <summary>
        /// loads spreadsheet frim XML file.
        /// </summary>
        /// <param name="stream">XML to read from.</param>
        public void Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is not readable");
            }

            try
            {
                // clear existing data before loading
                this.ClearAllData();

                // load XML doc
                var doc = XDocument.Load(stream);

                if (doc.Root == null)
                {
                    throw new InvalidOperationException("Invalid XML: No root element found");
                }

                if (doc.Root.Name.LocalName != "spreadsheet")
                {
                    throw new InvalidOperationException("Invalid XML: Root element must be 'spreadsheet'");
                }

                // process each cell element
                foreach (var cellElement in doc.Root.Elements("cell"))
                {
                    this.ReadCell(cellElement);
                }

                // recalculates all formulas after loading
                this.RecalculateAllCells();
                this.HasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load spreadsheet");
            }
        }

        /// <summary>
        /// Gets the cell at the specified row and column.
        /// </summary>
        /// <param name="row"> specified row.</param>
        /// <param name="col"> specificed col.</param>
        /// <returns> Cell gotten.</returns>
        public Cell GetCell(int row, int col)
        {
            if (row >= 0 && row < this.RowCount && col >= 0 && col < this.ColumnCount)
            {
                return this.cells[row, col];
            }

            return null;
        }

        /// <summary>
        /// adds a command to the undo stack.
        /// </summary>
        /// <param name="command">command to add.</param>
        public void AddUndo(Command command)
        {
            if (command != null)
            {
                this.undoStack.Push(command);
            }
            else
            {
                throw new ArgumentNullException("Unknown command cannot be added to the Undo stack");
            }
        }

        /// <summary>
        /// undoes last command.
        /// </summary>
        public void Undo()
        {
            if (!this.CanUndo)
            {
                return;
            }

            Command command = this.undoStack.Pop();
            command.Undo();
            this.redoStack.Push(command);
            this.HasUnsavedChanges = true;
        }

        /// <summary>
        /// redoes last command.
        /// </summary>
        public void Redo()
        {
            if (!this.CanRedo)
            {
                return;
            }

            Command command = this.redoStack.Pop();
            command.Redo();
            this.undoStack.Push(command);
            this.HasUnsavedChanges = true;
        }

        /// <summary>
        /// sets cell text and records change in undo.
        /// </summary>
        /// <param name="cell">cell to set.</param>
        /// <param name="newText">new text.</param>
        public void SetCellText(Cell cell, string newText)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell is null in SetCellText");
            }

            string oldText = cell.Text;

            if (oldText == newText)
            {
                return;
            }

            var command = new TextChangeCommand(cell, oldText, newText);

            command.Execute();

            this.AddUndo(command);
            this.redoStack.Clear();
            this.HasUnsavedChanges = true;
        }

        /// <summary>
        /// sets cell's background color and records change in undo.
        /// </summary>
        /// <param name="cell">cell to set.</param>
        /// <param name="newColor">new color.</param>
        public void SetCellColor(Cell cell, uint newColor)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell is null in SetCellColor");
            }

            uint oldColor = cell.BGColor;

            if (oldColor == newColor)
            {
                return;
            }

            var command = new ColorChangeCommand(cell, oldColor, newColor);

            command.Execute();

            this.AddUndo(command);
            this.redoStack.Clear();
            this.HasUnsavedChanges = true;
        }

        /// <summary>
        /// sets background color for multiple cells and records change in undo.
        /// </summary>
        /// <param name="cells">cells set.</param>
        /// <param name="newColor">new color.</param>
        public void SetCellColors(List<Cell> cells, uint newColor)
        {
            if (cells == null || cells.Count == 0)
            {
                throw new ArgumentNullException("cells are null or empty in SetCellColors");
            }

            if (cells.Count == 1)
            {
                Cell singleCell = cells[0];
                this.SetCellColor(singleCell, newColor);
            }
            else
            {
                var composite = new CompositeCommand();

                foreach (var cell in cells)
                {
                    if (cell == null)
                    {
                        continue;
                    }

                    uint oldColor = cell.BGColor;

                    // Skip unchanged cells
                    if (oldColor == newColor)
                    {
                        continue;
                    }

                    composite.Add(new ColorChangeCommand(cell, oldColor, newColor));
                }

                composite.Execute();
                this.AddUndo(composite);
                this.redoStack.Clear();
                this.HasUnsavedChanges = true;
            }
        }

        /// <summary>
        /// clear all spreadsheet data.
        /// </summary>
        public void ClearAllData()
        {
            for (int row = 0; row < this.RowCount; row++)
            {
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    Cell cell = this.cells[row, col];
                    cell.Text = DefaultText;
                    cell.BGColor = DefaultBGColor;
                }
            }

            // clears dependency trackers
            this.references.Clear();
            this.dependents.Clear();

            // clears undo and redo stack
            this.undoStack.Clear();
            this.redoStack.Clear();

            // marks as having unsaved changes
            this.HasUnsavedChanges = true;
        }

        /// <summary>
        /// internal method called by cells to set their value when their text is changed. (internal for testing purposes and hidden from UI).
        /// </summary>
        /// <param name="cell"> cell to change value of.</param>
        internal void OnCellTextChanged(Cell cell)
        {
            if (cell == null)
            {
                return;
            }

            // updates dependencies for cells
            this.UpdateDependencies(cell);

            // checks if text starts with =
            if (!string.IsNullOrEmpty(cell.Text) && cell.Text.StartsWith("="))
            {
                // gets formula without =
                string formula = cell.Text.Substring(1);

                // check for circular reference before evaluating
                if (this.HasCircularReference(cell, formula))
                {
                    cell.SetValue("#CIRCLE!");
                    this.RecalculateDependentCells(cell);
                    return;
                }

                try
                {
                    // uses expressionTree to handle multiiple vars
                    var tree = new ExpressionTree(formula);
                    double result = tree.Evaluate(this.LookupVariable);
                    cell.SetValue(result.ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message == "#REF!")
                    {
                        cell.SetValue("#REF!");
                    }
                    else
                    {
                        cell.SetValue("#ERROR!");
                    }

                    Debug.WriteLine($"Error evaluating {cell.Text}: {ex.Message}");
                }
            }
            else
            {
                // copies text if no formula
                cell.SetValue(cell.Text ?? string.Empty);
            }

            // recalculate dependencies for all cells after running
            this.RecalculateDependentCells(cell);
        }

        /// <summary>
        /// determines if cell has non default properties that needs saving.
        /// </summary>
        /// <param name="cell">cell to dertmine.</param>
        /// <returns>if cell has non deafult peroperties.</returns>
        private bool HasNonDefaultProperties(Cell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("Cell cannot be null");
            }

            // checks if text is nondefault
            bool hasNonDefaultText = !string.IsNullOrEmpty(cell.Text);

            // checks if color is nondefault
            bool hasNonDefaultColor = cell.BGColor != DefaultBGColor;

            return hasNonDefaultText || hasNonDefaultColor;
        }

        /// <summary>
        /// wrties a single cell to the XML writer.
        /// </summary>
        /// <param name="writer">writer to write too.</param>
        /// <param name="cell">cell to write.</param>
        /// <param name="cellName">name of cell.</param>
        private void WriteCell(XmlWriter writer, Cell cell, string cellName)
        {
            if (writer == null || cell == null || string.IsNullOrEmpty(cellName))
            {
                return;
            }

            writer.WriteStartElement("cell");
            writer.WriteAttributeString("name", cellName);

            // write text element if non empty
            if (!string.IsNullOrEmpty(cell.Text))
            {
                writer.WriteElementString("text", cell.Text);
            }

            // writes color if nondefault
            if (cell.BGColor != DefaultBGColor)
            {
                // convert to hex string and removes 0x prefix
                string colorHex = cell.BGColor.ToString("X8");
                writer.WriteElementString("bgcolor", colorHex);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// reads a cell from XML reader.
        /// </summary>
        /// <param name="cellElement">cell element.</param>
        private void ReadCell(XElement cellElement)
        {
            if (cellElement == null)
            {
                return;
            }

            // get cell name attribute
            XAttribute nameAttr = cellElement.Attribute("name");

            if (nameAttr == null || string.IsNullOrEmpty(nameAttr.Value))
            {
                return;
            }

            string cellName = nameAttr.Value;

            if (!this.TryParseCellReference(cellName, out int row, out int col))
            {
                return;
            }

            if (row < 0 || row >= this.RowCount || col < 0 || col >= this.ColumnCount)
            {
                return;
            }

            Cell cell = this.cells[row, col];

            // reads text element
            XElement textElement = cellElement.Element("text");

            if (textElement != null)
            {
                cell.Text = textElement.Value;
            }

            // reads bgcolor element
            XElement bgColorElement = cellElement.Element("bgcolor");
            if (bgColorElement != null)
            {
                string colorHex = bgColorElement.Value.Trim();

                // parses the hex color
                if (uint.TryParse(colorHex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out uint color))
                {
                    cell.BGColor = color;
                }
            }
        }

        /// <summary>
        /// converts row and col to cellname.
        /// </summary>
        /// <param name="row">row.</param>
        /// <param name="col">column.</param>
        /// <returns>cell name.</returns>
        private string GetCellName(int row, int col)
        {
            if (row < 0 || row >= this.RowCount)
            {
                throw new ArgumentOutOfRangeException(nameof(row), "Row index out of range");
            }

            if (col < 0 || col >= this.ColumnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(col), "Column index out of range");
            }

            // converts column index to letter
            char colLetter = (char)('A' + col);

            // row number is 1 based on display
            int rowNumber = row + 1;

            // combines both to make cell name
            return $"{colLetter}{rowNumber}";
        }

        /// <summary>
        /// recalculates all cells after loading.
        /// </summary>
        private void RecalculateAllCells()
        {
            // records all dependencies
            for (int row = 0; row < this.RowCount; row++)
            {
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    Cell cell = this.cells[row, col];

                    // update dependencies for formula cells
                    if (cell.Text.StartsWith("="))
                    {
                        this.UpdateDependencies(cell);
                    }
                }
            }

            // evaluates all formula cells, seperate pass to make sure all dependencies properly setup first
            for (int row = 0; row < this.RowCount; row++)
            {
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    Cell cell = this.cells[row, col];

                    if (cell.Text.StartsWith("="))
                    {
                        // evaluation of the formula
                        this.UpdateCellValue(cell);
                    }
                }
            }
        }

        /// <summary>
        /// checks whether the cell has a circular reference.
        /// </summary>
        /// <param name="cell">cell to check.</param>
        /// <param name="formula">formula to check.</param>
        /// <param name="visited">visited cells.</param>
        /// <returns>if the cell has circular reference.</returns>
        private bool HasCircularReference(Cell cell, string formula, HashSet<Cell> visited = null)
        {
            // guard for null cell
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell));
            }

            // guard for forumula (handle gracefully)
            if (formula == null)
            {
                return false;
            }

            // intialize visited if this is first cell
            if (visited == null)
            {
                visited = new HashSet<Cell>();
            }

            // checks if cell has already been visited in current call chain
            if (visited.Contains(cell))
            {
                return true;
            }

            // new visited set for recursive call
            var newVisited = new HashSet<Cell>(visited);
            newVisited.Add(cell);

            // get call cell references in formula
            var referencedCells = this.GetReferencedCells(formula);

            foreach (string refName in referencedCells)
            {
                // try to parse cell ref
                if (!this.TryParseCellReference(refName, out int row, out int col))
                {
                    continue; // Invalid reference, skip if not circle ref
                }

                // ref out of bounds?
                if (row < 0 || row >= this.RowCount || col < 0 || col >= this.ColumnCount)
                {
                    continue; // Out of bounds, skip if not circle ref
                }

                Cell referencedCell = this.cells[row, col];

                // check for cell ref
                if (referencedCell == cell)
                {
                    return true;
                }

                // recursively check cell with formula
                if (referencedCell.Text != null && referencedCell.Text.StartsWith("="))
                {
                    string refFormula = referencedCell.Text.Substring(1);

                    if (this.HasCircularReference(referencedCell, refFormula, new HashSet<Cell>(newVisited)))
                    {
                        return true;
                    }
                }
            }

            return false;
            }

        /// <summary>
        /// looksups the variable by the name.
        /// </summary>
        /// <param name="variableName">var to lookup.</param>
        /// <returns>value of var.</returns>
        private double LookupVariable(string variableName)
        {
            // variableName cannot be null, whitespace, or unable to be parsed into a Cell location
            if (variableName == null)
            {
                throw new ArgumentNullException(nameof(variableName), "Variable name cannot be null");
            }

            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException("Variable name cannot be empty or whitespace", nameof(variableName));
            }

            if (!this.TryParseCellReference(variableName, out int row, out int col))
            {
                throw new InvalidOperationException(ERRORREF);
            }

            // check if cell is within bounds
            if (row < 0 || row >= this.RowCount || col < 0 || col >= this.ColumnCount)
            {
                throw new InvalidOperationException(ERRORREF);
            }

            Cell cell = this.cells[row, col];

            // checks if cell exists but has error messages
            if (cell.Value == ERRORREF ||
        cell.Value == ERRORCIRCLE ||
        cell.Value == ERRORGENERAL)
            {
                throw new InvalidOperationException(cell.Value);
            }

            // cell exists with numeric value or empty (treat empty as 0)
            if (string.IsNullOrEmpty(cell.Value))
            {
                return 0;
            }

            if (double.TryParse(cell.Value, out double value))
            {
                return value;
            }

            // non-numeric value treated as 0
            return 0;
        }

        /// <summary>
        /// updates the value of the cell.
        /// </summary>
        /// <param name="cell">cell to update.</param>
        private void UpdateCellValue(Cell cell)
        {
            if (cell == null)
            {
                return;
            }

            if (cell.Text.StartsWith("="))
            {
                string formula = cell.Text.Substring(1);

                var referencedCells = this.GetReferencedCells(formula);

                foreach (string refName in referencedCells)
                {
                    if (this.TryParseCellReference(refName, out int row, out int col))
                    {
                        if (this.cells[row, col] == cell)
                        {
                            cell.SetValue(ERRORCIRCLE);
                            break;
                        }
                    }
                }

                // check for circular reference
                if (this.HasCircularReference(cell, formula))
                {
                    cell.SetValue(ERRORCIRCLE);
                    return;
                }

                try
                {
                    var tree = new ExpressionTree(formula);
                    double result = tree.Evaluate(this.LookupVariable);
                    cell.SetValue(result.ToString());
                }
                catch (InvalidOperationException ex)
                {
                    // stes error message on invalid op exception
                    if (ex.Message == ERRORREF || ex.Message == ERRORCIRCLE)
                    {
                        cell.SetValue(ex.Message);
                    }
                    else
                    {
                        cell.SetValue(ERRORGENERAL);
                    }
                }
                catch (Exception)
                {
                    cell.SetValue(ERRORGENERAL);
                }
            }
            else
            {
                cell.SetValue(cell.Text ?? string.Empty);
            }
        }

        /// <summary>
        /// recalculate for dependent cells.
        /// </summary>
        /// <param name="changedCell">cell that wa schanged.</param>
        /// <param name="visited">checks visited cells to avoid infnite loop./param>.
        private void RecalculateDependentCells(Cell changedCell, HashSet<Cell> visited = null)
        {
            if (changedCell == null)
            {
                return;
            }

            if (visited == null)
            {
                visited = new HashSet<Cell>();
            }

            if (visited.Contains(changedCell))
            {
                return;
            }

            visited.Add(changedCell);

            if (this.dependents.ContainsKey(changedCell))
            {
                // creates copy of the list
                var dependentsCopy = new List<Cell>(this.dependents[changedCell]);

                // recursively updates each chained dependencies
                foreach (var dependent in dependentsCopy)
                {
                    this.UpdateCellValue(dependent);
                    this.RecalculateDependentCells(dependent, visited);
                }
            }
        }

        /// <summary>
        /// gets the cells that are referenced in the formula.
        /// </summary>
        /// <param name="formula">formula to use.</param>
        /// <returns>list of cells referenced as strings.</returns>
        private List<string> GetReferencedCells(string formula)
        {
            var references = new List<string>();
            if (string.IsNullOrEmpty(formula))
            {
                return references;
            }

            // used AI to figure out regex match for A1-Z50
            // uses regex to get the pattern matches
            string pattern = @"[A-Za-z]([1-9]|[1-4][0-9]|50)";
            var matches = Regex.Matches(formula, pattern);

            // adds each reference to the list while avoiding duplicates
            foreach (Match match in matches)
            {
                string reference = match.Value.ToUpper();
                if (!references.Contains(reference))
                {
                    references.Add(reference);
                }
            }

            return references;
        }

        /// <summary>
        /// updates the dependencies in the dictionary.
        /// </summary>
        /// <param name="cell">cell that was changed.</param>
        private void UpdateDependencies(Cell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell));
            }

            // clears old dependencies
            if (this.references.ContainsKey(cell))
            {
                // updates each dependent for each reference
                foreach (var oldRef in this.references[cell])
                {
                    if (this.dependents.ContainsKey(oldRef))
                    {
                        this.dependents[oldRef].Remove(cell);
                        if (this.dependents[oldRef].Count == 0)
                        {
                            this.dependents.Remove(oldRef);
                        }
                    }
                }

                this.references.Remove(cell);
            }

            // if the cell has a formula, add new dependencies
            if (cell.Text.StartsWith("="))
            {
                string formula = cell.Text.Substring(1);

                var referencedCells = this.GetReferencedCells(formula);

                if (referencedCells.Count > 0)
                {
                    this.references[cell] = new List<Cell>();

                    foreach (string refName in referencedCells)
                    {
                        if (this.TryParseCellReference(refName, out int row, out int col))
                        {
                            Cell referencedCell = this.cells[row, col];

                            this.references[cell].Add(referencedCell);

                            if (!this.dependents.ContainsKey(referencedCell))
                            {
                                this.dependents[referencedCell] = new List<Cell>();
                            }

                            this.dependents[referencedCell].Add(cell);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// gets the value of the cell. for formula eval.
        /// </summary>
        /// <param name="cellName">cell name to parse.</param>
        /// <returns>value of the cell.</returns>
        private double GetCellValue(string cellName)
        {
            if (!this.TryParseCellReference(cellName, out int row, out int col))
            {
                throw new ArgumentException($"Invalid cell reference: {cellName}");
            }

            Cell cell = this.cells[row, col];

            // gets the cell's value
            if (double.TryParse(cell.Value, out double result))
            {
                return result;
            }

            // throws exception if unable to get value
            throw new InvalidOperationException($"Cell {cellName} does not contain a valid number. Found: '{cell.Value}'");
        }

        /// <summary>
        /// parses the cell reference.
        /// </summary>
        /// <param name="reference">reference to parse.</param>
        /// <param name="row">row of cell.</param>
        /// <param name="col">col of cell.</param>
        /// <returns>success or not.</returns>
        private bool TryParseCellReference(string reference, out int row, out int col)
        {
            row = -1;
            col = -1;
            if (string.IsNullOrEmpty(reference) || reference.Length < 2)
            {
                return false;
            }

            // first char must be a cjaracter
            char colChar = reference[0];
            if (colChar < 'A' || (colChar > 'Z' && colChar < 'a') || colChar > 'z')
            {
                return false;
            }

            // coverts column to a valid index
            col = char.ToUpper(colChar) - 'A';
            if (col < 0 || col > 25)
            {
                return false;
            }

            // parses the row part
            string rowPart = reference.Substring(1);
            if (!int.TryParse(rowPart, out int rowNum))
            {
                return false;
            }

            // must be between 1-50
            if (rowNum < 1 || rowNum > 50)
            {
                return false;
            }

            row = rowNum - 1;
            return true;
        }

        /// <summary>
        /// handles cell property change.
        /// </summary>
        /// <param name="sender"> object input.</param>
        /// <param name="e"> event argument for property change.</param>
        private void HandleCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // makes sure sender is a cell
            if (sender is Cell changedCell)
            {
                // marks as unchanged when any cell property has changed
                if (e.PropertyName == nameof(Cell.Text) || e.PropertyName == nameof(Cell.BGColor))
                {
                    this.HasUnsavedChanges = true;
                }

                // recalculate cell's value if Text property changed
                if (e.PropertyName == nameof(Cell.Text))
                {
                    this.OnCellTextChanged(changedCell);
                }

                // call the event
                this.CellPropertyChanged?.Invoke(sender, e);
            }
        }

        /// <summary>
        /// gets a cell from a reference (B2, A1, etc).
        /// </summary>
        /// <param name="reference">Cell references in table.</param>
        /// <returns>the cell at the corresponding reference or null if invalid.</returns>
        private Cell GetCellFromReference(string reference)
        {
            // returns null if reference is empty or null
            if (string.IsNullOrEmpty(reference))
            {
                return null;
            }

            // trims and uppercase for parsing
            reference = reference.Trim().ToUpper();

            // gets the first character
            char columnLetter = reference[0];
            if (columnLetter < 'A' || columnLetter > 'Z')
            {
                return null;
            }

            // converts column letter to index for column
            int columnIndex = columnLetter - 'A';

            if (reference.Length < 2)
            {
                return null;
            }

            string rowString = reference.Substring(1);

            // if number string can't be parsed into an integer, return null
            if (!int.TryParse(rowString, out int rowNumber))
            {
                return null;
            }

            // gets the rowIndex from rowNumber
            int rowIndex = rowNumber - 1;

            // bound check for row and column
            if (rowIndex >= 0 && rowIndex < this.RowCount && columnIndex >= 0 && columnIndex < this.ColumnCount)
            {
                return this.cells[rowIndex, columnIndex];
            }
            else
            {
                // out of bounds
                return null;
            }
        }

        /// <summary>
        /// represents a serialized cell for XML save/load.
        /// </summary>
        private class SerializableCell
        {
            public string Name { get; set; }

            public string Text { get; set; }

            public uint BGColor { get; set; }
        }
    }
}
