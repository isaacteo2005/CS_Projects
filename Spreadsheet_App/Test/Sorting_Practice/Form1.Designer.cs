using System;
using System.Collections.Immutable;
using System.Xml;

namespace HW2
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
        /// <summary>
        /// creates 10,000 random integers to find distinct ints, none, 
        /// </summary>
        /// param: none, returns: none, exceptions: none
        private void RunDistinctIntegers()
        {
            Random ran = new Random();
            int[] arr = new int[10000];
            for (int i = 0; i < 10000; i++)
            {
                arr[i] = ran.Next(0, 20001);
            }
            Label myLabel = new Label();
            myLabel.Size = new Size(500, 150);  
            myLabel.Text = "1. HashSet method: " + UniqueWithHashSet(arr) + " unique numbers\n";
            myLabel.Text += "The time complexity is O(n). The creation of the hashset goes through each element in the array to decide if it should be stored in the hashset so O(n) with O(1) ops, the rest of the lines are O(1) as they don't traverse any lists\n";
            myLabel.Text += "2. O(1) storage method: " + UniqueWithLowStorage(arr) + " unique numbers\n";
            myLabel.Text += "3. Sorted method: " + UniqueWithSorted(arr) + " unique numbers";
            myLabel.Location = new Point(10, 10);
            this.Controls.Add(myLabel);
        }
        /// <summary>
        /// finds all distinct integers in an array using a hashset
        /// </summary>
        /// <param name="arr"></param>
        /// <returns> # of dinstinct ints</returns>
        /// exception: none
        public int UniqueWithHashSet(int[] arr)
        {
            HashSet<int> set = new HashSet<int>(arr);
            int unique = set.Count;
            return unique;
        }
        /// <summary>
        /// finds al distincts ints in an array using a low storage method
        /// </summary>
        /// <param name="arr"></param>
        /// <returns> # of distinct ints</returns>
        /// exception: none
        public int UniqueWithLowStorage(int[] arr)
        {
            int count = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                bool is_distinct = true;
                //checks the elements before the current element and marks it if it appears more than once
                for(int j = 0; j < i; j++)
                {
                    //duplicate element found, marks the element as false then break
                    if (arr[i] == arr[j])
                    {
                        is_distinct = false;
                        break;
                    }
                }
                //mark elements don't increase count
                if (is_distinct)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// finds all distinct ints in an array using a sorting method
        /// </summary>
        /// <param name="arr"></param>
        /// <returns># of distinct ints</returns>
        /// exception: none
        public int UniqueWithSorted(int[] arr)
        {
            //return 0 if no elements in the list
            if(arr == null || arr.Length == 0)
            {
                return 0;
            }
            Array.Sort(arr);
            //always be 1 distinct element in the array with at least size 1
            int count = 1; 
            for(int i = 1; i < arr.Length; i++)
            {
                //checks if current element is different than last element
                if (arr[i] != arr[i - 1])
                {
                    count++;
                }
            }
            return count;
        }
        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 515);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion
    }
}
