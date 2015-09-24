using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass]
    public class FormulaUnitTest1
    {
        /********************************* TEST METHODS FOR FIRST CONSTRUCTOR **********************************/
        [TestMethod()]
        public void TestToString()
        {
            Formula a = new Formula("A2 + a3");
            Assert.AreEqual("A2+a3", a.ToString());
        }

        [TestMethod()]
        public void TestToStringWithNormalizer()
        {
            Formula f = new Formula("a2*A4 + A5 / B2", VarToUpper, IsValid);
            Assert.AreEqual("A2*A4+A5/B2", f.ToString());
        }

        [TestMethod()]
        public void TestEqualsNotAnObject()
        {
            Formula f = new Formula("2 + 3");
            Assert.AreEqual(false, f.Equals(new Object()));
            Assert.AreEqual(false, f.Equals("Hi there"));
        }

        [TestMethod()]
        public void TestEqualsWithNormalizer()
        {
            Formula a = new Formula("x1 + x2", VarToUpper, IsValid);
            Formula b = new Formula("X1 + X2");
            Assert.AreEqual(true, a.Equals(b));
            Assert.AreEqual(true, b.Equals(a));
        }

        [TestMethod()]
        public void TestHashCodeTrue()
        {
            Formula a = new Formula("2 + X3");
            int hashCodeA = a.GetHashCode();
            Formula b = new Formula("2+X3");
            int hashCodeB = b.GetHashCode();
            Assert.AreEqual(hashCodeA, hashCodeB);
        }

        [TestMethod()]
        public void TestHashCodeFalse()
        {
            Formula a = new Formula("2 + 3");
            int hashCodeA = a.GetHashCode();
            Formula b = new Formula("3 + 2");
            int hashCodeB = b.GetHashCode();
            Assert.AreNotEqual(hashCodeA, hashCodeB);
        }

        [TestMethod()]
        public void TestGetVariables()
        {
            Formula f = new Formula("x+y+z");
            IEnumerable<string> variables = f.GetVariables();
            List<string> expected = new List<string>();
            expected.Add("x");
            expected.Add("y");
            expected.Add("z");
            Assert.AreEqual(expected, variables);
        }

        [TestMethod()]
        public void TestGetVariablesWithNormalizer()
        {
            Formula f = new Formula("x+y+z", VarToUpper, IsValid);
            IEnumerable<string> variables = f.GetVariables();
            List<string> expected = new List<string>();
            expected.Add("X");
            expected.Add("Y");
            expected.Add("Z");
            Assert.AreEqual(expected, variables);
        }

        [TestMethod()]
        public void TestBoolOperatorTrue()
        {
            Formula a = new Formula("a");
            Formula b = new Formula("a");
            Assert.AreEqual(true, a == b);
        }

        [TestMethod()]
        public void TestBoolOperatorTrueFail()
        {
            Formula a = new Formula("a");
            Formula b = new Formula("b");
            Assert.AreEqual(false, a == b);
        }

        [TestMethod()]
        public void TestBoolOperatorFalse()
        {
            Formula a = new Formula("a");
            Formula b = new Formula("b");
            Assert.AreEqual(true, a != b);
        }

        [TestMethod()]
        public void TestBoolOperatorFalseFail()
        {
            Formula a = new Formula("a");
            Formula b = new Formula("a");
            Assert.AreEqual(false, a != b);
        }

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

        public void TestEqualsWithPrecision() {
            Formula a = new Formula("2.0");
            Formula b = new Formula("2.0000");
            Assert.AreEqual(true, a.Equals(b));
            Assert.AreEqual(true, b.Equals(a));
        }

        [TestMethod()]
        public void TestNotEqualsReversedExpressions()
        {
            Formula a = new Formula("a + b");
            Formula b = new Formula("b + a");
            Assert.AreEqual(false, a.Equals(b));
            Assert.AreEqual(false, b.Equals(a));
        }

        [TestMethod()]
        public void TestNotEqualsSameResult()
        {
            Formula a = new Formula("2 + 3 + 1");
            Formula b = new Formula("5 + 1");
            Assert.AreEqual(false, a.Equals(b));
            Assert.AreEqual(false, b.Equals(a));
        }


        [TestMethod()]
        public void TestWithENotation()
        {
            //Positive e
            Formula f = new Formula("2e3");
            Assert.AreEqual(2000, f.Evaluate(s => 0));

            //Negative e
            f = new Formula("2e-3");
            Assert.AreEqual(0.002, f.Evaluate(s => 0));
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




        /******************************************* PREVIOUS UNIT TESTS FOR f CLASS ******************************************/
        [TestMethod()]
        public void Test1()
        {
            Formula f = new Formula("5");
            Assert.AreEqual(5, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test2()
        {
            Formula f = new Formula("X5");
            Assert.AreEqual(13, f.Evaluate(s => 13));
        }

        [TestMethod()]
        public void Test3()
        {
            Formula f = new Formula("5+3");
            Assert.AreEqual(8, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test4()
        {
            Formula f = new Formula("18-10");
            Assert.AreEqual(8, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test5()
        {
            Formula f = new Formula("2 * 4");
            Assert.AreEqual(8, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test6()
        {
            Formula f = new Formula("16/2");
            Assert.AreEqual(8, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test7()
        {
            Formula f = new Formula("2+X1");
            Assert.AreEqual(6, f.Evaluate(s => 4));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Test8()
        {
            Formula f = new Formula("x + X1");
            f.Evaluate(s => { throw new ArgumentException("Unknown variable"); });
        }

        [TestMethod()]
        public void Test9()
        {
            Formula f = new Formula("2*6+3");
            Assert.AreEqual(15, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test10()
        {
            Formula f = new Formula("2+6*3");
            Assert.AreEqual(20, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test11()
        {
            Formula f = new Formula("(2+6)*3");
            Assert.AreEqual(24, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test12()
        {
            Formula f = new Formula("2*(3+5)");
            Assert.AreEqual(16, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test13()
        {
            Formula f = new Formula("2+(3+5)");
            Assert.AreEqual(10, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test14()
        {
            Formula f = new Formula("2+(3+5*9)");
            Assert.AreEqual(50, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test15()
        {
            Formula f = new Formula("2+3*(3+5)");
            Assert.AreEqual(26, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test16()
        {
            Formula f = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194, f.Evaluate(s => 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Test17()
        {
            Formula f = new Formula("5/0");
            f.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Test18()
        {
            Formula f = new Formula("+");
            f.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Test19()
        {
            Formula f = new Formula("2+5+");
            f.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Test20()
        {
            Formula f = new Formula("2+5*7)");
            f.Evaluate(s => 0);
        }
        
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Test23()
        {
            Formula f = new Formula("5+7+(5)8");
            f.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Test24()
        {
            Formula f = new Formula("");
            f.Evaluate(s => 0);
        }

        [TestMethod()]
        public void Test25()
        {
            Formula f = new Formula("y1 * 3 - 8 / 2 + 4 * (8 - 9 * 2) / 2 * x7");
            Assert.AreEqual(-12, f.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod()]
        public void Test26()
        {
            Formula f = new Formula("x1 + (x2 + (x3 + (x4 + (x5 + x6))))");
            Assert.AreEqual(6, f.Evaluate(s => 1));
        }

        [TestMethod()]
        public void Test27()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, f.Evaluate(s => 2));
        }

        [TestMethod()]
        public void Test28()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, f.Evaluate(s => 3));
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
