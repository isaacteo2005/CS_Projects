//-----------------------------------------------------------------------
// <copyright file="FibonacciTextReader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace HW3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    // Warning is from the base class of TextReader that i am inheriting
    public class FibonacciTextReader : TextReader
    {
        /// <summary>
        /// max # of fibo numbers that can be generated.
        /// </summary>
        private int maxLines;

        /// <summary>
        /// tracks which fibo number is currently on.
        /// </summary>
        private int currentLine;

        /// <summary>
        /// both prev1 and prev2 stores the last 2 fibo numbers needed to compute the next number.
        /// </summary>
        private BigInteger prev1; // BigInteger has been used since fibonacci numbers can becom extremely large

        /// <summary>
        /// both prev1 and prev2 stores the last 2 fibo numbers needed to compute the next number.
        /// </summary>
        private BigInteger prev2;

        /// <summary>
        /// Initializes a new instance of the <see cref="FibonacciTextReader"/> class.
        /// </summary>
        /// <param name="max">Description of parameter.</param>
        public FibonacciTextReader(int max)
        {
            if (max >= 0)
            {
                this.maxLines = max;
            }
            else
            {
                this.maxLines = 0;
            }

            this.currentLine = 0;
            this.prev1 = new BigInteger(-1);
            this.prev2 = BigInteger.One;
        }

        /// <summary>
        /// overrided readLine from TextReader.
        /// </summary>
        /// <returns>string.</returns>
        public override string ReadLine()
        {
            if (this.currentLine >= this.maxLines)
            {
                return null;
            }

            BigInteger fiboNumber = this.GetNextFibonacci();
            this.currentLine++;
            return fiboNumber.ToString();
        }

        /// <summary>
        /// overrided ReadTOEnd from TExtReader.
        /// </summary>
        /// <returns>string.</returns>
        public override string ReadToEnd()
        {
            StringBuilder builder = new StringBuilder(); // stringBuilder used for efficent concatnating for strings
            string line;

            // repeatedly calls and checks if ReadLine returns null or not
            while ((line = this.ReadLine()) != null)
            {
                // appends line to the string builder
                builder.AppendLine(line);
            }

            return builder.ToString();
        }

        /// <summary>
        /// helper method to find next fibonacci number.
        /// </summary>
        /// <returns>BigInteger type.</returns>
        private BigInteger GetNextFibonacci()
        {
            BigInteger next = this.prev1 + this.prev2; // F(n) = F(n-1) + F(n-2)
            this.prev1 = this.prev2; // F(n-2) = old F(n-1)
            this.prev2 = next; // F(n-1) becomes F(n)
            return next;
        }
    }
}
