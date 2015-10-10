using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetUtilities;

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
            Assert.AreEqual("=x1 + x2", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsFormulaWithOtherCellTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=a2 + a3");
            Assert.AreEqual("=a2 + a3", s.GetCellContents("a1"));
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
            Assert.AreEqual("=5 * 6", s.GetCellContents("A1"));
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
        public void GetSavedVersionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCellValueTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetContentsOfCellTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SpreadsheetTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SpreadsheetTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SpreadsheetTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCellContentsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetContentsOfCellTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSavedVersionTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCellValueTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AbstractSpreadsheetTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidNameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CellTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCellValueTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSavedVersionTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SpreadsheetTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SpreadsheetTest4()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SpreadsheetTest5()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SpreadsheetTest6()
        {
            Assert.Fail();
        }
        

        [TestMethod()]
        public void GetCellContentsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetContentsOfCellTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSavedVersionTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCellValueTest3()
        {
            Assert.Fail();
        }
    }
}