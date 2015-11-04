using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetUtilities;
using System.IO;
using System;
using System.Threading;
using System.Xml;

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

        /*
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
        }*/

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
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "3");
            s.Save("testChangedFromSpreadsheet.xml");
            Assert.AreEqual(false, s.Changed);
            s.SetContentsOfCell("a2", "a");
            Assert.AreEqual(true, s.Changed);
            File.Delete("testChangedFromSpreadsheet.xml");
        }
        
        [TestMethod()]
        public void GetSavedVersionTest()
        {
            Spreadsheet s = new Spreadsheet(TestValidToFalse, TestNormalizeToUpperCase, "test123");
            s.Save("test.xml");
            Assert.AreEqual("test123", s.GetSavedVersion("test.xml"));
            File.Delete("test.xml");
        }

        /*
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
        }*/

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

        /*
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
        }*/

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
            s.SetContentsOfCell("a1", "=b1 + 23");
            s.SetContentsOfCell("b1", "=b2 * 2");
            s.SetContentsOfCell("b2", "3");
            Assert.AreEqual(29.0, s.GetCellValue("a1"));
            Assert.AreEqual(6.0, s.GetCellValue("b1"));
            Assert.AreEqual(3.0, s.GetCellValue("b2"));

            s.SetContentsOfCell("A1", "3");
            s.SetContentsOfCell("B1", "=A1 * A1");
            s.SetContentsOfCell("D1", "=B1 - C1");
            s.SetContentsOfCell("C1", "=B1 + A1");
            Assert.AreEqual(3.0, s.GetCellValue("A1"));
            Assert.AreEqual(9.0, s.GetCellValue("B1"));
            Assert.AreEqual(12.0, s.GetCellValue("C1"));
            Assert.AreEqual(9.0-12.0, s.GetCellValue("D1"));
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
            if (!File.Exists("fourarg.xml"))
            {
                Spreadsheet old = new Spreadsheet(TestValidToTrue, TestNormalizeToUpperCase, "2.2");
                old.SetContentsOfCell("A1", "asdf");
                old.SetContentsOfCell("B2", "23");
                old.Save("fourarg.xml");
            }
            Spreadsheet s = new Spreadsheet("fourarg.xml", TestValidToTrue, TestNormalizeToUpperCase, "2.2");
            Assert.AreEqual(true, s.IsValid("a"));
            Assert.AreEqual("A", s.Normalize("a"));
            Assert.AreEqual("asdf", s.GetCellContents("A1"));
            Assert.AreEqual(23.0, s.GetCellContents("B2"));
            Assert.AreEqual(23.0, s.GetCellValue("B2"));
            Assert.AreEqual("2.2", s.Version);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetFourArgumentConstructorWrongVersionTest()
        {
            Spreadsheet s = new Spreadsheet("fourarg.xml", TestValidToFalse, TestNormalizeToUpperCase, "1.01");
        }

        /*
        [TestMethod()]
        public void CellTest()
        {
            Cell c = new Cell("a1", "2+2");
            Assert.AreEqual("2+2", c.Contents);
            Assert.AreEqual("a1", c.Name);
        }*/

        [TestMethod()]
        public void SpreadsheetReadWriteExceptionTest()
        {
            SpreadsheetReadWriteException e = new SpreadsheetReadWriteException("Hello");
            Assert.AreEqual("Hello", e.Message);
        }

        /********************** GRADER TESTS **********************/

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod()]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod()]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod()]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod()]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod()]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod()]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod()]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod()]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod()]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod()]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod()]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod()]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod()]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod()]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod()]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod()]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod()]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod()]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multiplec()
        {
            Multiple();
        }
        
        [TestMethod()]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod()]
        public void SaveTest7()
        {
            using (XmlWriter writer = XmlWriter.Create("save5.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod()]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = null;
                string contents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount);
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod()]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod()]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod()]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod()]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod()]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod()]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod()]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
        [TestMethod()]
        public void LongFormulaTest()
        {
            object result = "";
            Thread t = new Thread(() => LongFormulaHelper(out result));
            t.Start();
            t.Join(60 * 1000);
            if (t.IsAlive)
            {
                t.Abort();
                Assert.Fail("Computation took longer than 60 seconds");
            }
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 100;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

        /******************** END GRADER TESTS ********************/

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
        /// Always validates to true
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool TestValidToTrue(string str)
        {
            return true;
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
        override public void Save(string filename)
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
 