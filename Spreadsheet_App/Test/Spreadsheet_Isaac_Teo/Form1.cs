// Isaac Teo 11887290
/**
 * @file:Form1.cs
 * @brief: Form application for spreadsheet
 * @author: Isaac Teo 11887290
 */
namespace Spreadsheet_Isaac_Teo
{
    using System.ComponentModel;
    using System.Diagnostics;
    using SpreadsheetEngine;

    public partial class Form1 : Form
    {
        /// <summary>
        /// spreadsheet for application.
        /// </summary>
        private Spreadsheet spreadsheet;

        /// <summary>
        /// current file path to save/load fromm.
        /// </summary>
        private string currentFilePath;

        public Form1()
        {
            this.InitializeComponent();
            this.InitializeDataGrid();

            this.spreadsheet = new Spreadsheet(50, 26);
            this.spreadsheet.CellPropertyChanged += this.Spreadsheet_CellPropertyChanged;
            this.dataGridView1.CellBeginEdit += this.DataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.DataGridView1_CellEndEdit;
            this.dataGridView1.RowPostPaint += this.DataGridView1_RowPostPaint;
            this.dataGridView1.SelectionChanged += this.DataGridView1_SelectionChanged;
            this.spreadsheet.OnHasUnsavedChangesChanged += this.UpdateWindowTitle;
        }

        /// <summary>
        /// gets a value indicating whether if the current file jas been modified.
        /// </summary>
        private bool IsDirty => false;

        /// <summary>
        /// initializes data grid, adds columns A-Z.
        /// </summary>
        private void InitializeDataGrid()
        {
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.RowHeadersWidth = 50;
            for (int i = 0; i < 26; i++)
            {
                char columnLetter = (char)('A' + i);
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = $"Column_{columnLetter}";
                column.HeaderText = columnLetter.ToString();
                column.Width = 100;
                this.dataGridView1.Columns.Add(column);
            }

            for (int i = 0; i < 50; i++)
            {
                this.dataGridView1.Rows.Add();
            }
        }

        /// <summary>
        /// file menu - new clears spreadsheet.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">args.</param>
        private void NewFile_Click(object sender, EventArgs e)
        {
            // checks for unsaved changes
            if (this.spreadsheet.HasUnsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save them?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // tries to save
                    if (!this.TrySaveFile())
                    {
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // cancels new operation
                }
            }

            // create new blank spreadsheet
            this.spreadsheet.ClearAllData();
            this.currentFilePath = null;

            // updates UI
            this.RefreshAllUICells();
            this.UpdateWindowTitle();
        }

        /// <summary>
        /// file menu - open loads spreadsheet.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">args.</param>
        private void OpenFile_Click(object sender, EventArgs e)
        {
            // check for unsaved changes
            if (this.spreadsheet.HasUnsavedChanges)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save them?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (!this.TrySaveFile())
                    {
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // aplies filter for xml files
                openFileDialog.Filter = "Spreadsheet Files (*.xml)|*.xml|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                        {
                            this.spreadsheet.Load(stream);
                        }

                        this.currentFilePath = openFileDialog.FileName;

                        // update UI
                        this.RefreshAllUICells();
                        this.UpdateWindowTitle();

                        MessageBox.Show("File loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// file menu - save (saves current file).
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">args.</param>
        private void SaveFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.currentFilePath))
            {
                // no file path yet use Save As instead
                this.SaveAsFile_Click(sender, e);
            }
            else
            {
                this.SaveToFile(this.currentFilePath);
            }
        }

        /// <summary>
        /// file menu - saveas (saves to new file).
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">args.</param>
        private void SaveAsFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Spreadsheet Files (*.xml)|*.xml|All Files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.SaveToFile(saveFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// draws the row header for each row.
        /// </summary>
        /// <param name="sender">spreadsheet upon load.</param>
        /// <param name="e">property to paint.</param>
        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Get the row number (add 1 because rows are 0-based internally)
            string rowNumber = (e.RowIndex + 1).ToString();

            // Create a brush with the default header text color
            using (SolidBrush brush = new SolidBrush(this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                // Draw the number in the row header
                e.Graphics.DrawString(rowNumber, e.InheritedRowStyle.Font, brush, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + 4);
            }
        }

        /// <summary>
        /// saves spreadsheet to a specified file path.
        /// </summary>
        /// <param name="filePath">path to save to.</param>
        private void SaveToFile(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    this.spreadsheet.Save(stream);
                }

                this.currentFilePath = filePath;
                this.UpdateWindowTitle();

                MessageBox.Show("File saved successfully!", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// tries to save to current file. True if saved or no save needed, false if canceled.
        /// </summary>
        /// <returns>saved or canceled.</returns>
        private bool TrySaveFile()
        {
            if (!this.spreadsheet.HasUnsavedChanges)
            {
                return true;
            }

            DialogResult result = MessageBox.Show(
                "Do you want to save your changes?",
                "Save Changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (string.IsNullOrEmpty(this.currentFilePath))
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Spreadsheet Files (*.xml)|*.xml|All Files (*.*)|*.*";
                        saveFileDialog.FilterIndex = 1;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.SaveToFile(saveFileDialog.FileName);
                            return true;
                        }

                        return false; // user cancelled save
                    }
                }
                else
                {
                    this.SaveToFile(this.currentFilePath);
                    return true;
                }
            }

            return result == DialogResult.No; // continue without saving if no
        }

        /// <summary>
        /// updates the window title with current file name and unsaved marker.
        /// </summary>
        private void UpdateWindowTitle()
        {
            string fileName = string.IsNullOrEmpty(this.currentFilePath)
                ? "Untitled"
                : Path.GetFileName(this.currentFilePath);

            string unsavedMarker = this.spreadsheet.HasUnsavedChanges ? "*" : string.Empty;

            this.Text = $"{fileName}{unsavedMarker} - Spreadsheet Application";
        }

        /// <summary>
        /// refreshes all cells in the UI grid.
        /// </summary>
        private void RefreshAllUICells()
        {
            for (int row = 0; row < 50; row++)
            {
                for (int col = 0; col < 26; col++)
                {
                    Cell cell = this.spreadsheet.GetCell(row, col);
                    this.UpdateUICell(cell);
                    this.UpdateUICellColor(cell);
                }
            }
        }

        /// <summary>
        /// handles when a cell's property changes in the spreadshee and updates UI in DataGridView.
        /// </summary>
        /// <param name="sender">cell who's property changed.</param>
        /// <param name="e">value of cell.</param>
        private void Spreadsheet_CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // sneder must be a cell
            if (sender is Cell cell)
            {
                // checks what value is being changed and calls the right method accordingly
                if (e.PropertyName == "Value" || e.PropertyName == "Text")
                {
                    if (this.dataGridView1.InvokeRequired)
                    {
                        this.dataGridView1.Invoke(new Action(() => this.UpdateUICell(cell)));
                    }
                    else
                    {
                        this.UpdateUICell(cell);
                    }
                }

                if (e.PropertyName == "BGColor")
                {
                    if (this.dataGridView1.InvokeRequired)
                    {
                        this.dataGridView1.Invoke(new Action(() => this.UpdateUICellColor(cell)));
                    }
                    else
                    {
                        this.UpdateUICellColor(cell);
                    }
                }

                // update undo/redo menu whenever a cell changes
                if (this.dataGridView1.InvokeRequired)
                {
                    this.dataGridView1.Invoke(new Action(this.UpdateUndoRedoMenuItems));
                }
                else
                {
                    this.UpdateUndoRedoMenuItems();
                }
            }
        }

        /// <summary>
        /// event hanlders for beggining editing cells.
        /// </summary>
        /// <param name="sender">cell caller.</param>
        /// <param name="e">property detected.</param>
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // get the spreadsheet cell
            Cell cell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);

            if (cell != null)
            {
                // show text property that may or may not be a formula
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Text;
            }
        }

        /// <summary>
        /// event handler for ending editing cells.
        /// </summary>
        /// <param name="sender">cell caller.</param>
        /// <param name="e">property detected.</param>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // gets value that user entered
            object value = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            string text = value?.ToString() ?? string.Empty;

            // gets spreadsheet cell
            Cell cell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);

            if (cell != null)
            {
                // updates cell's text which triggers event chain
                this.spreadsheet.SetCellText(cell, text);

                // reshows the actual cell value
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;

                // spreadsheet will evaluate and update the value which will trigger CellPropertyChanged and update the UI
            }
        }

        /// <summary>
        /// updates a specific cell in the DataGridView.
        /// </summary>
        /// <param name="cell"> cell to update.</param>
        private void UpdateUICell(Cell cell)
        {
            if (cell.RowIndex < this.dataGridView1.Rows.Count && cell.ColumnIndex < this.dataGridView1.Columns.Count)
            {
                this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
            }
        }

        /// <summary>
        /// updates undo/redo menu states.
        /// </summary>
        private void UpdateUndoRedoMenuItems()
        {
            if (this.spreadsheet.CanUndo)
            {
                this.undoMenuItem.Text = "Undo: " + this.spreadsheet.NextUndoTitle;
                this.undoMenuItem.Enabled = true;
            }
            else
            {
                this.undoMenuItem.Text = "Undo";
                this.undoMenuItem.Enabled = false;
            }

            if (this.spreadsheet.CanRedo)
            {
                this.redoMenuItem.Text = "Redo: " + this.spreadsheet.NextRedoTitle;
                this.redoMenuItem.Enabled = true;
            }
            else
            {
                this.redoMenuItem.Text = "Redo";
                this.redoMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// event handler for undo button.
        /// </summary>
        /// <param name="sender">event sender.</param>
        /// <param name="e">property detected.</param>
        private void UndoMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spreadsheet.CanUndo)
            {
                this.spreadsheet.Undo();
                this.UpdateUndoRedoMenuItems();
            }
        }

        /// <summary>
        /// event handler for redo button.
        /// </summary>
        /// <param name="sender">event sender.</param>
        /// <param name="e">property detected.</param>
        private void RedoMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spreadsheet.CanRedo)
            {
                this.spreadsheet.Redo();
                this.UpdateUndoRedoMenuItems();
            }
        }

        /// <summary>
        /// event handler for color picker button.
        /// </summary>
        /// <param name="sender">event sender.</param>
        /// <param name="e">property detected.</param>
        private void ChangeBGColorMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }

            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    uint argb = (uint)colorDialog.Color.ToArgb();
                    List<Cell> selectedCells = new List<Cell>();
                    foreach (DataGridViewCell dgvCell in this.dataGridView1.SelectedCells)
                    {
                        int r = dgvCell.RowIndex;
                        int c = dgvCell.ColumnIndex;
                        var cell = this.spreadsheet.GetCell(r, c);
                        if (cell != null)
                        {
                            selectedCells.Add(cell);
                        }
                    }

                    this.spreadsheet.SetCellColors(selectedCells, argb);
                }
            }
        }

        /// <summary>
        /// event handler for cell selection.
        /// </summary>
        /// <param name="sender">event sender.</param>
        /// <param name="e">proerpty detected.</param>
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // update undo/redo menu whenever selection changed
            this.UpdateUndoRedoMenuItems();

            // enable the "Change Background Color" menu only if a cell is selected
            this.changeBGColorMenuItem.Enabled = this.dataGridView1.SelectedCells.Count > 0;
        }

        /// <summary>
        /// updates background color in UI.
        /// </summary>
        /// <param name="cell">cell to get color.</param>
        private void UpdateUICellColor(Cell cell)
        {
            var dgvCell = this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex];
            dgvCell.Style.BackColor = Color.FromArgb((int)cell.BGColor);
        }

        /// <summary>
        /// gets the selected cells from the DtaGridView.
        /// </summary>
        /// <returns>selected cells.</returns>
        private List<Cell> GetSelectedCells()
        {
            var selectedCells = new List<Cell>();

            foreach (DataGridViewCell dgvCell in this.dataGridView1.SelectedCells)
            {
                int r = dgvCell.RowIndex;
                int c = dgvCell.ColumnIndex;
                Cell cell = this.spreadsheet.GetCell(r, c);
                if (cell != null)
                {
                    selectedCells.Add(cell);
                }
            }

            return selectedCells;
        }

        /// <summary>
        /// runs demo for project.
        /// </summary>
        private void RunDemo()
        {
            this.button1.Enabled = false;

            // sets 50 random cells to hello world
            Random rand = new Random();
            for (int i = 0; i < 50; i++)
            {
                int row = rand.Next(0, 50);
                int col = rand.Next(0, 26);
                this.spreadsheet.GetCell(row, col).Text = "Hello World!";
            }

            // sets every cell in column B to this is cell B num
            for (int row = 0; row < 50; row++)
            {
                this.spreadsheet.GetCell(row, 1).Text = $"This is cell B{row + 1}";
            }

            // every cell in column A references column B cells
            for (int row = 0; row < 50; row++)
            {
                this.spreadsheet.GetCell(row, 0).Text = $"=B{row + 1}";
            }
        }

#pragma warning disable SA1300 // Element should begin with upper-case letter
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            this.RunDemo();
        }

#pragma warning disable SA1300 // Element should begin with upper-case letter
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            if (this.spreadsheet.HasUnsavedChanges)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save them?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (!this.TrySaveFile())
                    {
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            Application.Exit();
        }
    }
}
