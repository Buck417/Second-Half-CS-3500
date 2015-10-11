using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetUtilities;
using System.IO;
using System;

namespace SS.Tests
{
    [TestClass()]
    public class SpreadsheetTests
    {
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("2x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsNullTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        public void GetCellContentsEmptyCellTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsDoubleTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "20.0");
            Assert.AreEqual(20.0, s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsDoubleCellExistsTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(true, s.SetContentsOfCell("a1", "2.0").Contains("a1"));
            Assert.AreEqual(true, s.SetContentsOfCell("a1", "5.3").Contains("a1"));
            Assert.AreEqual(5.3, s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsStringTest()
        {
            //Gets the contents of a string-type cell
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "Hey there");
            Assert.AreEqual("Hey there", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsStringCellExistsTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(true, s.SetContentsOfCell("a1", "Hey there").Contains("a1"));
            Assert.AreEqual(true, s.SetContentsOfCell("a1", "What's up?").Contains("a1"));
            Assert.AreEqual("What's up?", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsFormulaTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=x1 + x2");
            Assert.AreEqual("=x1+x2", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsFormulaWithOtherCellTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=a2 + a3");
            Assert.AreEqual("=a2+a3", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsNoCellsTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(0, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "20.0");
            s.SetContentsOfCell("a2", "Hey there");
            IEnumerable<string> names = s.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(true, names.Contains("a1"));
            Assert.AreEqual(true, names.Contains("a2"));
        }

        [TestMethod()]
        public void SetContentsOfCellMixedTypeTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "2.0");
            s.SetContentsOfCell("B1", "Title of this cell");
            IEnumerable<string> result = s.SetContentsOfCell("C1", "=A1 * 4 / 3");
            Assert.AreEqual(true, result.Contains("A1"));
            Assert.AreEqual(true, result.Contains("C1"));
            Assert.AreEqual(false, result.Contains("B1"));
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod()]
        public void SetContentsOfCellDoubleTest()
        {
            Spreadsheet s = new Spreadsheet();
            //Make sure it actually added something
            Assert.AreEqual(true, s.SetContentsOfCell("a1", "2.0").Contains("a1"));
            //Make sure a1 has the right data
            Assert.AreEqual(2.0, s.GetCellContents("a1"));
            //Make sure a1 is in the nonempty set
            Assert.AreEqual(true, s.GetNamesOfAllNonemptyCells().Contains("a1"));
            //Make sure there's only one in the nonempty set
            Assert.AreEqual(1, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellDoubleNullNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "20.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellDoubleInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2x", "20.0");
        }

        [TestMethod()]
        public void SetContentsOfCellStringTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(true, s.SetContentsOfCell("A1", "Hey there").Contains("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellStringNullNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "Hey there");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellStringInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2_a", "Hey there");
        }

        [TestMethod()]
        public void SetContentsOfCellFormulaTest()
        {
            Spreadsheet s = new Spreadsheet();

            Assert.AreEqual(true, s.SetContentsOfCell("B1", "=A1 * A1").Contains("A1"));
            s.SetContentsOfCell("C1", "=B1 + A1");
            s.SetContentsOfCell("D1", "=B1 - C1");
            IEnumerable<string> result = s.SetContentsOfCell("A1", "3");
            Assert.AreEqual(true, result.Contains("A1"));
            Assert.AreEqual(true, result.Contains("B1"));
            Assert.AreEqual(true, result.Contains("C1"));
            Assert.AreEqual(true, result.Contains("D1"));

        }

        [TestMethod()]
        public void SetContentsOfCellFormulaCellExists()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=2 + 3");
            Assert.AreEqual(true, s.SetContentsOfCell("A1", "=5 * 6").Contains("A1"));
            Assert.AreEqual("=5*6", s.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetContentsOfCellFormulaCircularReferenceTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1");
            s.SetContentsOfCell("B1", "=A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellFormulaNullNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=x + 2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellFormulaInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2x", "=x + 2");
        }

        [TestMethod()]
        public void CellValidNameTest()
        {
            Assert.AreEqual(true, Cell.ValidName("A1"));
            Assert.AreEqual(true, Cell.ValidName("A"));
            Assert.AreEqual(false, Cell.ValidName("1"));
            Assert.AreEqual(true, Cell.ValidName("_"));
            Assert.AreEqual(true, Cell.ValidName("_A"));
            Assert.AreEqual(false, Cell.ValidName(""));
            Assert.AreEqual(false, Cell.ValidName(null));
            Assert.AreEqual(true, Cell.ValidName("_1"));
            Assert.AreEqual(true, Cell.ValidName("A_2"));
            Assert.AreEqual(true, Cell.ValidName("A_A"));
            Assert.AreEqual(false, Cell.ValidName("2A"));
            Assert.AreEqual(true, Cell.ValidName("x1"));
            Assert.AreEqual(true, Cell.ValidName("x"));
        }

        [TestMethod()]
        public void ChangedFromNewSpreadsheetTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "2");
            Assert.AreEqual(true, s.Changed);
        }

        [TestMethod()]
        public void ChangedFromSavedSpreadsheetTest()
        {
            MockSpreadsheet s = new MockSpreadsheet();
            s.SetContentsOfCell("a1", "3");
            s.Save("test");
            Assert.AreEqual(false, s.Changed);
            s.SetContentsOfCell("a2", "a");
            Assert.AreEqual(true, s.Changed);
        }
        
        [TestMethod()]
        public void GetSavedVersionTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("test123", s.GetSavedVersion("test.xml"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionNonexistentFileTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetSavedVersion("234.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionNotXMLTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetSavedVersion("Spreadsheet.cs");
        }

        [TestMethod()]
        public void SaveTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "2");
            s.SetContentsOfCell("B1", "2 + 2");
            s.SetContentsOfCell("C1", "=3 * 3");
            s.Save("saveTest.xml");
            Assert.AreEqual(true, File.Exists("saveTest.xml"));
            File.Delete("saveTest.xml");
            Spreadsheet s2 = new Spreadsheet();
            s2.Save("saveTest2.xml");
            Assert.AreEqual(true, File.Exists("saveTest2.xml"));
            File.Delete("saveTest2.xml");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveNotXMLTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "2");
            s.Save("asdf.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveNoExtensionTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.Save("hello");
        }

        [TestMethod()]
        public void SaveExistingSpreadsheetTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "2");
            s.Save("existingTest.xml");
            Assert.AreEqual(true, File.Exists("existingTest.xml"));
            s.SetContentsOfCell("a1", "35");
            s.Save("existingTest.xml");
            Assert.AreEqual(true, File.Exists("existingTest.xml"));
        }

        [TestMethod()]
        public void GetCellValueFromFormulaTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=2+2");
            Assert.AreEqual(4.0, s.GetCellValue("a1"));
        }

        [TestMethod()]
        public void GetCellValueFromFormulaWithVariablesTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=23");
            s.SetContentsOfCell("b1", "=a1 * 2");
            Assert.AreEqual(23.0, s.GetCellValue("a1"));
            Assert.AreEqual(46.0, s.GetCellValue("b1"));
        }

        [TestMethod()]
        public void GetCellValueFromStringTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "Hello");
            Assert.AreEqual("Hello", s.GetCellValue("a1"));
        }

        [TestMethod()]
        public void GetCellValueFromDoubleTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "2.0");
            Assert.AreEqual(2.0, s.GetCellValue("a1"));
        }
        
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsNullContentsTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("11a", "asdf");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsNullNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "asdf");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetCellContentsInvalidFormulaTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=a + b + c)");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsCircularDependencyTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=b2 + 3");
            s.SetContentsOfCell("b2", "=a1 + 4");
        }

        /// <summary>
        /// Tests the empty constructor
        /// </summary>
        [TestMethod()]
        public void SpreadsheetTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("a", s.Normalize("a"));
            Assert.AreEqual(true, s.IsValid("a"));
            Assert.AreEqual("default", s.Version);
        }

        /// <summary>
        /// Tests the three argument constructor
        /// </summary>
        [TestMethod()]
        public void SpreadsheetThreeArgumentConstructorTest()
        {
            Spreadsheet s = new Spreadsheet(TestValidToFalse, TestNormalizeToUpperCase, "1.1");
            Assert.AreEqual(false, s.IsValid("a"));
            Assert.AreEqual("A1", s.Normalize("a1"));
            Assert.AreEqual("1.1", s.Version);
        }

        [TestMethod()]
        public void SpreadsheetFourArgumentConstructorTest()
        {
            Spreadsheet s = new Spreadsheet("fourarg.xml", TestValidToFalse, TestNormalizeToUpperCase, "2.2");
            Assert.AreEqual(false, s.IsValid("a"));
            Assert.AreEqual("A", s.Normalize("a"));
            Assert.AreEqual("2.2", s.Version);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetFourArgumentConstructorWrongVersionTest()
        {
            Spreadsheet s = new Spreadsheet("fourarg.xml", TestValidToFalse, TestNormalizeToUpperCase, "1.01");
        }

        [TestMethod()]
        public void CellTest()
        {
            Cell c = new Cell("a1", "2+2");
            Assert.AreEqual("2+2", c.Contents);
            Assert.AreEqual("a1", c.Name);
        }

        [TestMethod()]
        public void SpreadsheetReadWriteExceptionTest()
        {
            SpreadsheetReadWriteException e = new SpreadsheetReadWriteException("Hello");
            Assert.AreEqual("Hello", e.Message);
        }
        
        /// <summary>
        /// Just using this to test the constructors
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool TestValidToFalse(string str)
        {
            return false;
        }

        /// <summary>
        /// Just using this to test the constructors
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string TestNormalizeToUpperCase(string str)
        {
            return str.ToUpper();
        }
    }

    public class MockSpreadsheet : Spreadsheet
    {
        /// <summary>
        /// Saves the file if the filename isn't "test"
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            if (filename.Equals("test"))
            {
                //Do nothing
            }
            else
            {
                base.Save(filename);
            }
        }
    }
}