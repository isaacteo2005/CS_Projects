/**
 * @file: Form1.Designer..cs
 * @brief: designer class for winforms
 * @author: Isaac Teo 11887290
 */
namespace Spreadsheet_Isaac_Teo
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            A = new DataGridViewTextBoxColumn();
            B = new DataGridViewTextBoxColumn();
            C = new DataGridViewTextBoxColumn();
            D = new DataGridViewTextBoxColumn();
            button1 = new Button();
            menuStrip1 = new MenuStrip();
            editMenu = new ToolStripMenuItem();
            undoMenuItem = new ToolStripMenuItem();
            redoMenuItem = new ToolStripMenuItem();
            cellMenu = new ToolStripMenuItem();
            changeBGColorMenuItem = new ToolStripMenuItem();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saToolStripMenuItem = new ToolStripMenuItem();
            saveasToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { A, B, C, D });
            dataGridView1.Location = new Point(39, 24);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(735, 370);
            dataGridView1.TabIndex = 0;
            // 
            // A
            // 
            A.HeaderText = "A";
            A.MinimumWidth = 6;
            A.Name = "A";
            A.Width = 125;
            // 
            // B
            // 
            B.HeaderText = "B";
            B.MinimumWidth = 6;
            B.Name = "B";
            B.Width = 125;
            // 
            // C
            // 
            C.HeaderText = "C";
            C.MinimumWidth = 6;
            C.Name = "C";
            C.Width = 125;
            // 
            // D
            // 
            D.HeaderText = "D";
            D.MinimumWidth = 6;
            D.Name = "D";
            D.Width = 125;
            // 
            // button1
            // 
            button1.Location = new Point(39, 400);
            button1.Name = "button1";
            button1.Size = new Size(735, 29);
            button1.TabIndex = 1;
            button1.Text = "Run Demo";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editMenu, cellMenu });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // editMenu
            // 
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { undoMenuItem, redoMenuItem });
            editMenu.Name = "editMenu";
            editMenu.Size = new Size(49, 24);
            editMenu.Text = "Edit";
            // 
            // undoMenuItem
            // 
            undoMenuItem.Enabled = false;
            undoMenuItem.Name = "undoMenuItem";
            undoMenuItem.Size = new Size(32, 19);
            undoMenuItem.Text = "Undo";
            undoMenuItem.Click += UndoMenuItem_Click;
            // 
            // redoMenuItem
            // 
            redoMenuItem.Enabled = false;
            redoMenuItem.Name = "redoMenuItem";
            redoMenuItem.Size = new Size(32, 19);
            redoMenuItem.Text = "Redo";
            redoMenuItem.Click += RedoMenuItem_Click;
            // 
            // cellMenu
            // 
            cellMenu.DropDownItems.AddRange(new ToolStripItem[] { changeBGColorMenuItem });
            cellMenu.Name = "cellMenu";
            cellMenu.Size = new Size(48, 24);
            cellMenu.Text = "Cell";
            // 
            // changeBGColorMenuItem
            // 
            changeBGColorMenuItem.Name = "changeBGColorMenuItem";
            changeBGColorMenuItem.Size = new Size(32, 19);
            changeBGColorMenuItem.Text = "Change Background Color";
            changeBGColorMenuItem.Click += ChangeBGColorMenuItem_Click;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saToolStripMenuItem, saveasToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(224, 26);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += new System.EventHandler(this.NewFile_Click);
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(224, 26);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // saToolStripMenuItem
            // 
            saToolStripMenuItem.Name = "saToolStripMenuItem";
            saToolStripMenuItem.Size = new Size(224, 26);
            saToolStripMenuItem.Text = "Save";
            saToolStripMenuItem.Click += new System.EventHandler(this.SaveFile_Click);
            // 
            // saveasToolStripMenuItem
            // 
            saveasToolStripMenuItem.Name = "saveasToolStripMenuItem";
            saveasToolStripMenuItem.Size = new Size(224, 26);
            saveasToolStripMenuItem.Text = "Save as";
            saveasToolStripMenuItem.Click += new System.EventHandler(this.SaveAsFile_Click);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(224, 26);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Controls.Add(menuStrip1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn A;
        private DataGridViewTextBoxColumn B;
        private DataGridViewTextBoxColumn C;
        private DataGridViewTextBoxColumn D;
        private Button button1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem editMenu;
        private ToolStripMenuItem undoMenuItem;
        private ToolStripMenuItem redoMenuItem;
        private ToolStripMenuItem cellMenu;
        private ToolStripMenuItem changeBGColorMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saToolStripMenuItem;
        private ToolStripMenuItem saveasToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
    }
}
