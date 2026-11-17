/**
 * @file: VarNode.cs
 * @brief: Node for expression tree representing a variable
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class VarNode : Node
    {
        /// <summary>
        /// name of the variable stored.
        /// </summary>
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="VarNode"/> class.
        /// </summary>
        /// <param name="name">name of the var.</param>
        /// <param name="variables">references to dictionary.</param>
        public VarNode(string name)
        {
            // throws an exception if name is null
            if (name == null)
            {
                throw new ArgumentNullException($"VarNode '{nameof(name)}' has null name");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Variable name cannot be empty or whitespace", nameof(name));
            }

            this.name = name;
        }

        /// <summary>
        /// gets the name of the varnode.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// evaluates the value.
        /// </summary>
        /// <param name="variableLookup">looks up varaible.</param>
        /// <returns>evaluation.</returns>
        /// <exception cref="ArgumentNullException">throws when variable is null.</exception>
        public override double Evaluate(Func<string, double> variableLookup)
        {
            if (variableLookup == null)
            {
                throw new ArgumentNullException(nameof(variableLookup));
            }

            // Call the lookup function to get the variable's value
            return variableLookup(this.name);
        }
    }
}
