/**
 * @file: Cellcs
 * @brief: Cell class for spreadsheet
 * @author: Isaac Teo 11887290
 */
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestProject1")]

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Cell class for spreadsheet.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// text shown in cell.
        /// </summary>
        protected string text;

        /// <summary>
        /// evaluated value of a cell.
        /// </summary>
        protected string value;

        /// <summary>
        /// color of the cell.
        /// </summary>
        protected uint bgColor;

        /// <summary>
        /// row index of cell.
        /// </summary>
        private readonly int rowIndex;

        /// <summary>
        /// column index of cell.
        /// </summary>
        private readonly int columnIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="rowIndex"> row of cell.</param>
        /// <param name="columnIndex"> column of cell.</param>
        public Cell(int rowIndex, int columnIndex)
        {
            // neither column nor row should be negative and should reset to 0 if so
            if (rowIndex >= 0 && columnIndex >= 0)
            {
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
            }
            else
            {
                if (rowIndex < 0 && columnIndex < 0)
                {
                    this.rowIndex = 0;
                    this.columnIndex = 0;
                }
                else if (columnIndex < 0)
                {
                    this.rowIndex = rowIndex;
                    this.columnIndex = 0;
                }
                else
                {
                    this.rowIndex = 0;
                    this.columnIndex = columnIndex;
                }
            }

            this.text = string.Empty;
            this.value = string.Empty;
            this.bgColor = 0xFFFFFFFF;
        }

        /// <summary>
        /// property changed event handler to notify UI layer when something changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets and sets background color property.
        /// </summary>
        public uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;
                    this.OnPropertyChanged(nameof(this.BGColor));
                }
            }
        }

        /// <summary>
        /// gets the row index of cell.
        /// </summary>
        public int RowIndex => this.rowIndex;

        /// <summary>
        /// gets column index of cell.
        /// </summary>
        public int ColumnIndex => this.columnIndex;

        /// <summary>
        /// Gets or sets text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                // calls event if the text is different
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged(nameof(this.Text));
                }
            }
        }

        /// <summary>
        /// gets the value of the cell.
        /// </summary>
        public string Value => this.value;

        /// <summary>
        /// sets the value of the cellthat only allows the Spreadsheet class to change it. (can only be accessed if used in derived class or same project).
        /// </summary>
        /// <param name="newValue"> value to set for cell.</param>
        protected internal void SetValue(string newValue)
        {
                this.value = newValue;
                this.OnPropertyChanged(nameof(this.Value));
        }

        // virtual allows derived classes to extend behavior

        /// <summary>
        /// raises property changed event.
        /// </summary>
        /// <param name="propertyName"> name of the property to change.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            // invokes if not null (?)
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
