﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            s.SetCellContents("a1", 20.0);
            Assert.AreEqual(20.0, s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsDoubleCellExistsTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(true, s.SetCellContents("a1", 2.0).Contains("a1"));
            Assert.AreEqual(true, s.SetCellContents("a1", 5.3).Contains("a1"));
            Assert.AreEqual(5.3, s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsStringTest()
        {
            //Gets the contents of a string-type cell
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", "Hey there");
            Assert.AreEqual("Hey there", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsStringCellExistsTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(true, s.SetCellContents("a1", "Hey there").Contains("a1"));
            Assert.AreEqual(true, s.SetCellContents("a1", "What's up?").Contains("a1"));
            Assert.AreEqual("What's up?", s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsFormulaTest()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f = new Formula("x1 + x2");
            s.SetCellContents("a1", f);
            Assert.AreEqual(new Formula("x1 + x2"), s.GetCellContents("a1"));
        }

        [TestMethod()]
        public void GetCellContentsFormulaWithOtherCellTest()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f = new Formula("a2 + a3");
            s.SetCellContents("a1", f);
            Assert.AreEqual(new Formula("a2 + a3"), s.GetCellContents("a1"));
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
            s.SetCellContents("a1", 20.0);
            s.SetCellContents("a2", "Hey there");
            IEnumerable<string> names = s.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(true, names.Contains("a1"));
            Assert.AreEqual(true, names.Contains("a2"));
        }

        [TestMethod()]
        public void SetCellContentsMixedTypeTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 2.0);
            s.SetCellContents("B1", "Title of this cell");
            IEnumerable<string> result = s.SetCellContents("C1", new Formula("A1 * 4 / 3"));
            Assert.AreEqual(true, result.Contains("A1"));
            Assert.AreEqual(true, result.Contains("C1"));
            Assert.AreEqual(false, result.Contains("B1"));
            Assert.AreEqual(2, result.Count());
        }
        
        [TestMethod()]
        public void SetCellContentsDoubleTest()
        {
            Spreadsheet s = new Spreadsheet();
            //Make sure it actually added something
            Assert.AreEqual(true, s.SetCellContents("a1", 2.0).Contains("a1"));
            //Make sure a1 has the right data
            Assert.AreEqual(2.0, s.GetCellContents("a1"));
            //Make sure a1 is in the nonempty set
            Assert.AreEqual(true, s.GetNamesOfAllNonemptyCells().Contains("a1"));
            //Make sure there's only one in the nonempty set
            Assert.AreEqual(1, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleNullNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, 20.0);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("2x", 20.0);
        }

        [TestMethod()]
        public void SetCellContentsStringTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(true, s.SetCellContents("A1", "Hey there").Contains("A1"));
        }
        
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsStringNullNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, "Hey there");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsStringInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("2_a", "Hey there");
        }

        [TestMethod()]
        public void SetCellContentsFormulaTest()
        {
            Spreadsheet s = new Spreadsheet();
            
            Assert.AreEqual(true, s.SetCellContents("B1", new Formula("A1 * A1")).Contains("A1"));
            s.SetCellContents("C1", new Formula("B1 + A1"));
            s.SetCellContents("D1", new Formula("B1 - C1"));
            IEnumerable<string> result = s.SetCellContents("A1", 3);
            Assert.AreEqual(true, result.Contains("A1"));
            Assert.AreEqual(true, result.Contains("B1"));
            Assert.AreEqual(true, result.Contains("C1"));
            Assert.AreEqual(true, result.Contains("D1"));
            
        }

        [TestMethod()]
        public void SetCellContentsFormulaCellExists()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("2 + 3"));
            Assert.AreEqual(true, s.SetCellContents("A1", new Formula("5 * 6")).Contains("A1"));
            Assert.AreEqual(new Formula("5 * 6"), s.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsFormulaCircularReferenceTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("B1"));
            s.SetCellContents("B1", new Formula("A1"));
        }
        
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaNullNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, new Formula("x + 2"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("2x", new Formula("x + 2"));
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
    }
}