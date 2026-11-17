/**
 * @file: ConcreteCell.cs
 * @brief: concrete cell class for spreadsheet
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
    /// concrete implementation of cell class.
    /// </summary>
    public class ConcreteCell : Cell
    {
        public ConcreteCell(int rowIndex, int columnIndex)
            : base(rowIndex, columnIndex)
        {
        }
    }
}
