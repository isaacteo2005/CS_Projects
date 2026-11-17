/**
 * @file: HW9.cs
 * @brief: test cases for HW9
 * @author: Isaac Teo 11887290
 */
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    internal class HW9
    {
        private Spreadsheet spreadsheet;
        private MemoryStream stream;
        private Cell cell;
        private uint whiteColor = 0xFFFFFFFF;
        private uint redColor = 0xFFFF0000;
        private uint blueColor = 0xFF0000FF;

        [SetUp]
        public void Setup()
        {
            this.spreadsheet = new Spreadsheet(50, 26);
            this.stream = new MemoryStream();
            this.cell = this.spreadsheet.GetCell(0, 0);
        }

        /// <summary>
        /// loads xml file from stream.
        /// </summary>
        /// <returns>XML doc.</returns>
        private XDocument LoadXmlFromStream()
        {
            this.stream.Position = 0;
            return XDocument.Load(this.stream);
        }

        /// <summary>
        /// creates the xml file.
        /// </summary>
        /// <param name="xmlContent">xml content to create file for.</param>
        private void CreateXmlFile(string xmlContent)
        {
            this.stream = new MemoryStream();
            using (var writer = new StreamWriter(this.stream, leaveOpen: true))
            {
                writer.Write(xmlContent);
                writer.Flush();
            }

            this.stream.Position = 0;
        }

        /// <summary>
        /// uses reflection to invoke HasNonDefaultProperties.
        /// </summary>
        /// <param name="cell">cell to check.</param>
        /// <returns>if the cell has deafult proeprties.</returns>
        private bool InvokeHasNonDefaultProperties(Cell cell)
        {
            var method = typeof(Spreadsheet).GetMethod("HasNonDefaultProperties", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(this.spreadsheet, new object[] { cell });
        }

        /// <summary>
        /// normal case for saving empty spreadsheet.
        /// </summary>
        [Test]
#pragma warning disable SA1202 // Elements should be ordered by access
        public void SaveNormal()
#pragma warning restore SA1202 // Elements should be ordered by access
        {
            this.spreadsheet.Save(this.stream);

            var doc = this.LoadXmlFromStream();
            var root = doc.Root;
            Assert.That(root.Name.LocalName, Is.EqualTo("spreadsheet"));
            Assert.That(root.Elements("cell").Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// normal case for saving with single cell with text.
        /// </summary>
        [Test]
        public void SaveNormal2()
        {
            var cell = this.spreadsheet.GetCell(0, 0);
            cell.Text = "Hello World";

            this.spreadsheet.Save(this.stream);

            var doc = this.LoadXmlFromStream();
            var cellElement = doc.Root.Elements("cell").FirstOrDefault();

            // checks that all attributes properly saved and the default color does not save
            Assert.Multiple(() =>
            {
                Assert.That(cellElement, Is.Not.Null);
                Assert.That(cellElement.Attribute("name")?.Value, Is.EqualTo("A1"));
                Assert.That(cellElement.Element("text")?.Value, Is.EqualTo("Hello World"));
                Assert.That(cellElement.Element("bgcolor"), Is.Null); // default color not saved
            });
        }

        /// <summary>
        /// normal case testing for saving single cell with color.
        /// </summary>
        [Test]
        public void SaveNormal3()
        {
            var cell = this.spreadsheet.GetCell(0, 0);
            cell.BGColor = this.redColor;

            this.spreadsheet.Save(this.stream);

            var doc = this.LoadXmlFromStream();
            var cellElement = doc.Root.Elements("cell").FirstOrDefault();

            // checks that all attributes properly saved and the default text does not save
            Assert.Multiple(() =>
            {
                Assert.That(cellElement, Is.Not.Null);
                Assert.That(cellElement.Attribute("name")?.Value, Is.EqualTo("A1"));
                Assert.That(cellElement.Element("bgcolor")?.Value, Is.EqualTo("FFFF0000"));
                Assert.That(cellElement.Element("text"), Is.Null); // default text not saved
            });
        }

        /// <summary>
        /// normal case for saving cell with both text and color.
        /// </summary>
        [Test]
        public void SaveNormal4()
        {
            var cell = this.spreadsheet.GetCell(0, 0);
            cell.Text = "Hello";
            cell.BGColor = this.redColor;

            this.spreadsheet.Save(this.stream);

            var doc = this.LoadXmlFromStream();
            var cellElement = doc.Root.Elements("cell").FirstOrDefault();

            // checks that all attributes properly saved
            Assert.Multiple(() =>
            {
                Assert.That(cellElement.Element("text")?.Value, Is.EqualTo("Hello"));
                Assert.That(cellElement.Element("bgcolor")?.Value, Is.EqualTo("FFFF0000"));
            });
        }

        /// <summary>
        /// edge case checking if last cell of spreadsheet can be saved.
        /// </summary>
        [Test]
        public void SaveEdge()
        {
            var cell = this.spreadsheet.GetCell(49, 25); // Z50
            cell.Text = "Last Cell";
            cell.BGColor = this.redColor;
            this.spreadsheet.Save(this.stream);

            var doc = this.LoadXmlFromStream();
            var cellElement = doc.Root.Elements("cell").FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(cellElement, Is.Not.Null);
                Assert.That(cellElement.Element("text")?.Value, Is.EqualTo("Last Cell"));
                Assert.That(cellElement.Element("bgcolor")?.Value, Is.EqualTo("FFFF0000"));
            });
        }

        /// <summary>
        /// expcetion case where null stream catches exception.
        /// </summary>
        [Test]
        public void SaveException()
        {
            Assert.That(() => this.spreadsheet.Save(null), Throws.ArgumentNullException);
        }

        /// <summary>
        /// exception case checking if saving after stream is closed throws exception.
        /// </summary>
        [Test]
        public void SaveException2()
        {
            this.stream.Close();
            Assert.That(() => this.spreadsheet.Save(this.stream), Throws.Exception);
        }

        /// <summary>
        /// exception case for saving after stream is disposed.
        /// </summary>
        [Test]
        public void SaveException3()
        {
            this.stream.Dispose();
            Assert.That(() => this.spreadsheet.Save(this.stream), Throws.Exception);
        }

        // AI used to create xml strings for test cases

        /// <summary>
        /// normal case for loading xml file with text.
        /// </summary>
        [Test]
        public void LoadNormal()
        {
            string xml = @"<?xml version=""1.0""?>
                    <spreadsheet>
                        <cell name=""A1"">
                            <text>Hello World</text>
                        </cell>
                    </spreadsheet>";
            this.CreateXmlFile(xml);
            this.spreadsheet.Load(this.stream);
            var cell = this.spreadsheet.GetCell(0, 0);
            Assert.That(cell.Text, Is.EqualTo("Hello World"));
            Assert.That(cell.BGColor, Is.EqualTo(this.whiteColor));
        }

        /// <summary>
        /// normal case for load testing for xml file with text and color.
        /// </summary>
        [Test]
        public void LoadNormal2()
        {
            string xml = @"<?xml version=""1.0""?>
                    <spreadsheet>
                        <cell name=""B5"">
                            <text>Test Cell</text>
                            <bgcolor>FFFF0000</bgcolor>
                        </cell>
                    </spreadsheet>";
            this.CreateXmlFile(xml);

            this.spreadsheet.Load(this.stream);

            // B5 is row 4, col 1
            var cell = this.spreadsheet.GetCell(4, 1);
            Assert.That(cell.Text, Is.EqualTo("Test Cell"));
            Assert.That(cell.BGColor, Is.EqualTo(this.redColor));
        }

        /// <summary>
        /// normal case for load testing loading multiple cells with mix attributes.
        /// </summary>
        [Test]
        public void LoadNormal3()
        {
            string xml = @"<?xml version=""1.0""?>
                    <spreadsheet>
                        <cell name=""A1"">
                            <text>First</text>
                            <bgcolor>FFFF0000</bgcolor>
                        </cell>
                        <cell name=""B2"">
                            <text>Second</text>
                            <bgcolor>FF0000FF</bgcolor>
                        </cell>
                        <cell name=""C3"">
                            <text>=A1+B2</text>
                        </cell>
                    </spreadsheet>";
            this.CreateXmlFile(xml);
            this.spreadsheet.Load(this.stream);
            var a1 = this.spreadsheet.GetCell(0, 0);
            var b2 = this.spreadsheet.GetCell(1, 1);
            var c3 = this.spreadsheet.GetCell(2, 2);

            // asserts that all cells have the right text and color
            Assert.Multiple(() =>
            {
                Assert.That(a1.Text, Is.EqualTo("First"));
                Assert.That(a1.BGColor, Is.EqualTo(this.redColor));
                Assert.That(b2.Text, Is.EqualTo("Second"));
                Assert.That(b2.BGColor, Is.EqualTo(this.blueColor));
                Assert.That(c3.Text, Is.EqualTo("=A1+B2"));
            });
        }

        /// <summary>
        /// edge case testing for loading empty spreadsheet.
        /// </summary>
        [Test]
        public void LoadEdge()
        {
            string xml = @"<?xml version=""1.0""?>
                    <spreadsheet>
                    </spreadsheet>";
            this.CreateXmlFile(xml);
            this.spreadsheet.Load(this.stream);

            // no cells should be modified and should be kept as default values.
            var cell = this.spreadsheet.GetCell(0, 0);
            Assert.That(cell.Text, Is.EqualTo(string.Empty));
            Assert.That(cell.BGColor, Is.EqualTo(this.whiteColor));
        }

        /// <summary>
        /// edge case for load testing if cells are only loaded when given a valid name.
        /// </summary>
        [Test]
        public void LoadEdge2()
        {
            string xml = @"<?xml version=""1.0""?>
                    <spreadsheet>
                        <cell>
                            <text>No Name</text>
                        </cell>
                        <cell name=""B1"">
                            <text>Has Name</text>
                        </cell>
                    </spreadsheet>";
            this.CreateXmlFile(xml);
            this.spreadsheet.Load(this.stream);

            var cellA1 = this.spreadsheet.GetCell(0, 0);
            var cellB1 = this.spreadsheet.GetCell(0, 1);

            // only cells with name loads
            Assert.That(cellA1.Text, Is.EqualTo(string.Empty)); // not loaded
            Assert.That(cellB1.Text, Is.EqualTo("Has Name"));
        }

        public void LoadEdge3()
        {
            string xml = @"<spreadsheet>
                        <cell unusedattr=""abc"" name=""B1"">
                            <text>=A1+6</text>
                            <some_tag_you_didnt_write>blah</some_tag_you_didnt_write>
                            <bgcolor>FF8000</bgcolor>
                            <another_unused_tag>data</another_unused_tag>
                        </cell>
                        <cell name=""C1"">
                            <text>Normal Cell</text>
                        </cell>
                    </spreadsheet>";
            this.CreateXmlFile(xml);
            this.spreadsheet.Load(this.stream);
            var b1 = this.spreadsheet.GetCell(0, 1);
            var c1 = this.spreadsheet.GetCell(0, 2);
            Assert.Multiple(() =>
            {
                // B1 loads correctly despite extra tags
                Assert.That(b1.Text, Is.EqualTo("=A1+6"));
                Assert.That(b1.BGColor, Is.EqualTo(0xFF8000));

                // C1 loads normally
                Assert.That(c1.Text, Is.EqualTo("Normal Cell"));
                Assert.That(c1.BGColor, Is.EqualTo(0xFFFFFFFF)); // default white
            });
        }

        /// <summary>
        /// exception case for load testing null stream passed.
        /// </summary>
        [Test]
        public void LoadException()
        {
            Assert.That(() => this.spreadsheet.Load(null), Throws.ArgumentNullException);
        }

        /// <summary>
        /// expcetion case checking loading after stream disposed.
        /// </summary>
        [Test]
        public void LoadException2()
        {
            this.stream.Dispose();
            Assert.That(() => this.spreadsheet.Load(this.stream), Throws.Exception);
        }

        /// <summary>
        /// exception case checking loading after stream closed.
        /// </summary>
        [Test]
        public void LoadException3()
        {
            this.stream.Close();
            Assert.That(() => this.spreadsheet.Load(this.stream), Throws.Exception);
        }

        /// <summary>
        /// exception case checking loading empty xml file.
        /// </summary>
        [Test]
        public void LoadException4()
        {
            string xml = string.Empty;
            this.CreateXmlFile(xml);
            Assert.That(() => this.spreadsheet.Load(this.stream), Throws.Exception);
        }

        /// <summary>
        /// exception case checking loading from an invalid xml file.
        /// </summary>
        [Test]
        public void LoadException5()
        {
            string xml = @"This is not valid XML";
            this.CreateXmlFile(xml);
            Assert.That(() => this.spreadsheet.Load(this.stream), Throws.Exception);
        }

        /// <summary>
        /// normal case for no default properties testing default cell.
        /// </summary>
        [Test]
        public void NoDefaultNormal()
        {
            bool result = this.InvokeHasNonDefaultProperties(this.cell);
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// normal case for no default props testing nondefault text.
        /// </summary>
        [Test]
        public void NoDefaultNormal2()
        {
            this.cell.Text = "Hello World";
            bool result = this.InvokeHasNonDefaultProperties(this.cell);
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// normal case for no default props testing nondefault text and color.
        /// </summary>
        [Test]
        public void NoDefaultNormal3()
        {
            this.cell.Text = "Hello World";
            this.cell.BGColor = this.redColor;
            bool result = this.InvokeHasNonDefaultProperties(this.cell);
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// edge case checking if both text and color is set to default.
        /// </summary>
        [Test]
        public void NoDefaultEdge()
        {
            this.cell.Text = string.Empty;
            this.cell.BGColor = 0xFFFFFFFF;
            bool result = this.InvokeHasNonDefaultProperties(this.cell);
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// exception case checking if null cell throws exception.
        /// </summary>
        [Test]
        public void NoDefaultException()
        {
            var spreadsheet = new Spreadsheet(50, 26);
            var method = typeof(Spreadsheet).GetMethod("HasNonDefaultProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            // checks if TargetInvocationException thrown then checks innerexception is argument null exception
            var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(spreadsheet, new object[] { null }));
            Assert.That(ex.InnerException, Is.TypeOf<ArgumentNullException>());
            Assert.That(ex.InnerException.Message, Does.Contain("Cell cannot be null"));
        }
    }
}
