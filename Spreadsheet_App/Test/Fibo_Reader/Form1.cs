namespace HW3
{
    // every method here has been made by the system
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// generic load function for application, returns: none, exception: none
        /// </summary>
        /// <param TextReader name="sr"></param>
        private void LoadText(TextReader sr)
        {
            string content = sr.ReadToEnd();
            textBox1.Text = content;
            sr.Close();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu item event handler for loading from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Used AI Help for figuring out openFileDialog
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Title = "Select a text file to load";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader fileReader = new StreamReader(openFileDialog.FileName))
                    {
                        this.LoadText(fileReader);
                        this.Text = $"Text Viewer - {Path.GetFileName(openFileDialog.FileName)}"; // updates form title with filename
                    }
                }
            }
        }
        /// <summary>
        /// Menu item event handler for loading fibonacci first 50
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadFibonacciNumbersfirst50ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FibonacciTextReader fiboReader = new FibonacciTextReader(50);
            this.LoadText(fiboReader);
            this.Text = "Fibo first 50";

        }
        /// <summary>
        /// Menu item event handler for loading fibonacci first 100 (including 0)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadFibonacciNumbersfirst100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FibonacciTextReader fiboReader = new FibonacciTextReader(100);
            this.LoadText(fiboReader);
            this.Text = "Fibo first 100";
        }
        /// <summary>
        /// Menu item event handler for saving to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.Title = "Save text to file";
                saveFileDialog.DefaultExt = "txt";
                // auto adds extension for file name (.txt)
                saveFileDialog.AddExtension = true;
                // set warning when saving to a file
                saveFileDialog.OverwritePrompt = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string textSave = textBox1.Text;
                    File.WriteAllText(saveFileDialog.FileName, textSave);
                    MessageBox.Show(
                        $"File saved successfully to:{Environment.NewLine}{saveFileDialog.FileName}",
                        "Save Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
    }
}
