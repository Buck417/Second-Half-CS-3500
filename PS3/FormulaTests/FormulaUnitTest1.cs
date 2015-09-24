using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace FormulaTests
{
    [TestClass]
    public class FormulaUnitTest1
    {
        /********************************* TEST METHODS FOR FIRST CONSTRUCTOR **********************************/
        [TestMethod()]
        public void TestNormalAddition()
        {
            SpreadsheetUtilities.Formula f = new SpreadsheetUtilities.Formula("2.0 + 2.5");
            Assert.AreEqual(4.5, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestNormalSubtraction()
        {
            Formula f = new Formula("3.33 - 1.33");
            Assert.AreEqual(2, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestNormalMultiplication()
        {
            Formula f = new Formula("3 * 1.25");
            Assert.AreEqual(3.75, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestNormalDivision()
        {
            Formula f = new Formula("7/2");
            Assert.AreEqual(3.5, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestSingleCharVariable()
        {
            Formula f = new Formula("x");
            Assert.Equals(15, f.Evaluate(s => 15));
        }

        [TestMethod()]
        public void TestOneCharOneNumber()
        {
            Formula f = new Formula("a1");
            Assert.Equals(10, f.Evaluate(s => 10));
        }

        [TestMethod()]
        public void TestUnderscoreNumber()
        {
            Formula f = new Formula("_2");
            Assert.Equals(20, f.Evaluate(s => 20));
        }
        
        [TestMethod()]
        public void TestEquals()
        {
            Formula a = new Formula("x + y2");
            Formula b = new Formula("x+y2");
            Assert.AreEqual(true, a.Equals(b));
            Assert.AreEqual(true, b.Equals(a));
        }




        /********************************* TEST METHODS FOR SECOND CONSTRUCTOR **********************************/
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariableWithValidator()
        {
            Formula f = new Formula("2x", VarToUpper, IsValid);
        }

        [TestMethod()]
        public void TestValidVariableWithValidator()
        {
            Formula f = new Formula("x2", VarToUpper, IsValid);
            Assert.Equals(2, f.Evaluate(s => 2));
        }



        /********************************* HELPER METHODS **********************************/
        private static string VarToUpper(string variable)
        {
            return variable.ToUpper();
        }

        private static bool IsValid(string variable)
        {
            Match match = Regex.Match(variable, @"([a-zA-Z]+[0-9]+)");
            if (match.Success)
            {
                return true;
            }
            return false;
        }

        
    }
}
