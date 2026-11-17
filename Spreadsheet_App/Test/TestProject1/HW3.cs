//-----------------------------------------------------------------------
// <copyright file="HW3.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace HW3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static System.Windows.Forms.LinkLabel;

    // all elements are documented
    internal class HW3
    {
        /// <summary>
        /// normal case for readline that tests for 5 fibo numbers.
        /// </summary>
        [Test]
        public void ReadLine_Normal()
        {
            // var is used for reduced reptition of the class name, type should be obvious
            var reader = new FibonacciTextReader(5);
            string[] expected = { "0", "1", "1", "2", "3" };

            // Asserts that each value is equal too the corresponding number in the sequence.
            for (int i = 0; i < expected.Length; i++)
            {
                string result = reader.ReadLine();
                Assert.That(result, Is.EqualTo(expected[i]));
            }
        }

        /// <summary>
        /// edge case for readline that tests for just one fibo number and another call to test that null would be returned.
        /// </summary>
        [Test]
        public void ReadLine_Edge()
        {
            var reader = new FibonacciTextReader(1);

            // first call is the first and only number in the sequence
            string firstResult = reader.ReadLine();
            Assert.That(firstResult, Is.EqualTo("0"));

            // 2nd call should return null since the max lines is 1
            string secondResult = reader.ReadLine();
            Assert.That(secondResult, Is.Null);
        }

        /// <summary>
        /// exception case for readline that tests that a max length of 0 would return null.
        /// </summary>
        [Test]
        public void ReadLine_Exception()
        {
            FibonacciTextReader reader = new FibonacciTextReader(0);
            string result = reader.ReadLine();

            // tests that 0 max lines means null sequence
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// normal test case where ReadToEnd is used to scan fibo numbers from start to finish.
        /// </summary>
        [Test]
        public void ReadToEnd_Normal()
        {
            var reader = new FibonacciTextReader(5);
            string expected = "0\n1\n1\n2\n3";

            // handles trailing new lines
            string result = reader.ReadToEnd().Trim();

            // AI HELPED WITH THIS LINE, splits on both \n and \r as well as removing empty strings
            string[] resultLines = result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // splits expected on \n
            string[] expectedLines = expected.Split('\n');

            // both result and expected should be equal
            Assert.That(resultLines, Is.EqualTo(expectedLines));
        }

        /// <summary>
        /// edge test case where ReadToEnd reads too the end of max line after a partial read.
        /// </summary>
        [Test]
        public void ReadToEnd_Edge()
        {
            var reader = new FibonacciTextReader(10);
            reader.ReadLine(); // read 0
            reader.ReadLine(); // reads 1

            string remaining = reader.ReadToEnd().Trim();
            string[] lines = remaining.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // length of lines should be 8 as there are 8 remaining entries too be read
            Assert.That(lines.Length, Is.EqualTo(8));

            // next 2 digits after partial read should be 1 and 2
            Assert.That(lines[0], Is.EqualTo("1"));
            Assert.That(lines[1], Is.EqualTo("2"));
        }

        /// <summary>
        /// exception test case testing large number of lines for a performance check.
        /// </summary>
        [Test]
        public void ReadToEnd_Exception()
        {
            var reader = new FibonacciTextReader(101);
            string result = reader.ReadToEnd();
            string[] lines = result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Check first few numbers
            for (int i = 0; i < lines.Length; i++)
            {
                TestContext.WriteLine($"Line {i}: {lines[i]}");
            }

            // number of lines should be 100 (100 numbers for max length)
            Assert.That(lines.Length, Is.EqualTo(101));

            // 100th fibo number should match
            Assert.That(lines[100], Is.EqualTo("354224848179261915075"));
        }

        /// <summary>
        /// normal test case for fibonacci text reder constructor.
        /// </summary>
        [Test]
        public void FiboConstructorNormal()
        {
            var reader = new FibonacciTextReader(5);

            // newly created objects should not be null
            Assert.That(reader, Is.Not.Null);
        }

        /// <summary>
        /// edge case for fibonacci text reader contructor that tests for a not null object when size = 0.
        /// </summary>
        [Test]
        public void FiboConstructorEdge()
        {
            var reader = new FibonacciTextReader(0);

            // object with a size of 0 should also not be null
            Assert.That(reader, Is.Not.Null);
        }

        /// <summary>
        /// expcetion case for fibonacci text reader consturtor that tests that negative numbers as max length won't cause an error.
        /// </summary>
        [Test]
        public void FiboConstructorExpception()
        {
            var reader = new FibonacciTextReader(-1);
            string result = reader.ReadLine();

            // negative numbers for max line should behave like the 0 case
            Assert.That(result, Is.Null);
        }
    }
}
