﻿using System;
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
        public void TestNotEqualsBothNull()
        {
            Formula a = new Formula(null);
            Formula b = new Formula(null);
            Assert.AreEqual(false, a != b);
            Assert.AreEqual(false, b != a);
        }

        [TestMethod()]
        public void TestEqualsIdentity()
        {
            Formula a = new Formula("a");
            Assert.AreEqual(true, a == a);
        }

        [TestMethod()]
        public void TestEqualsFromToString()
        {
            string expression = "x + 2";
            Formula a = new Formula(expression);
            Formula b = new Formula(a.ToString());
            Assert.AreEqual(true, a.Equals(b));
            Assert.AreEqual(true, b.Equals(a));
        }

        [TestMethod()]
        public void TestEqualsBothNull()
        {
            Formula a = new Formula(null);
            Formula b = new Formula(null);
            Assert.AreEqual(true, a == b);
            Assert.AreEqual(true, b == a);
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
            List<string> variables = (List<string>)f.GetVariables();
            Assert.AreEqual(true, variables.Contains("x"));
            Assert.AreEqual(true, variables.Contains("y"));
            Assert.AreEqual(true, variables.Contains("z"));
        }

        [TestMethod()]
        public void TestGetVariablesSeparatedBySpace()
        {
            Formula f = new Formula("x y");
            List<string> variables = (List<string>)f.GetVariables();
            Assert.AreEqual(true, variables.Contains("x"));
            Assert.AreEqual(true, variables.Contains("y"));
        }

        [TestMethod()]
        public void TestGetVariablesWithNormalizer()
        {
            Formula f = new Formula("x+y+z", VarToUpper, IsValid);
            List<string> variables = (List<string>)f.GetVariables();
            Assert.AreEqual(true, variables.Contains("X"));
            Assert.AreEqual(true, variables.Contains("Y"));
            Assert.AreEqual(true, variables.Contains("Z"));
        }

        [TestMethod()]
        public void TestGetVariablesLongerFormula()
        {
            //Make sure we strip out numbers and expressions too
            Formula f = new Formula("3*a1 + b2+ c3 + x + y + _a + _2 + 6 + 7");
            List<string> vars = (List<string>)f.GetVariables();
            Assert.AreEqual(true, vars.Contains("a1"));
            Assert.AreEqual(true, vars.Contains("b2"));
            Assert.AreEqual(true, vars.Contains("c3"));
            Assert.AreEqual(true, vars.Contains("x"));
            Assert.AreEqual(true, vars.Contains("y"));
            Assert.AreEqual(true, vars.Contains("_a"));
            Assert.AreEqual(true, vars.Contains("_2"));
            Assert.AreEqual(7, vars.Count);
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
            Assert.AreEqual(2.0, f.Evaluate(s => 0));
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
            Assert.AreEqual(15.0, f.Evaluate(s => 15));
        }

        [TestMethod()]
        public void TestDoubleUnderScores()
        {
            Formula f = new Formula("__9");
            Assert.AreEqual(10.0, f.Evaluate(s => 10));
        }

        [TestMethod()]
        public void TestDoubleUnderScoresNoNumber()
        {
            Formula f = new Formula("__");
            Assert.AreEqual(2.0, f.Evaluate(s => 2));
        }

        [TestMethod()]
        public void TestOneCharOneNumber()
        {
            Formula f = new Formula("a1");
            Assert.AreEqual(10.0, f.Evaluate(s => 10));
        }

        [TestMethod()]
        public void TestUnderscoreNumber()
        {
            Formula f = new Formula("_2");
            Assert.AreEqual(20.0, f.Evaluate(s => 20));
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
            Assert.AreEqual(2000.0, f.Evaluate(s => 0));

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
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void TestValidVariableWithValidator()
        {
            Formula f = new Formula("x2", VarToUpper, IsValid);
            Assert.AreEqual(2.0, f.Evaluate(s => 2));
        }




        /******************************************* PREVIOUS UNIT TESTS FOR f CLASS ******************************************/
        [TestMethod()]
        public void Test1()
        {
            Formula f = new Formula("5");
            Assert.AreEqual(5.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test2()
        {
            Formula f = new Formula("X5");
            Assert.AreEqual(13.0, f.Evaluate(s => 13.0));
        }

        [TestMethod()]
        public void Test3()
        {
            Formula f = new Formula("5+3");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test4()
        {
            Formula f = new Formula("18-10");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test5()
        {
            Formula f = new Formula("2 * 4");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test6()
        {
            Formula f = new Formula("16/2");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test7()
        {
            Formula f = new Formula("2+X1");
            Assert.AreEqual(6.0, f.Evaluate(s => 4));
        }

        [TestMethod()]
        public void Test8()
        {
            Formula f = new Formula("x + X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test9()
        {
            Formula f = new Formula("2*6+3");
            Assert.AreEqual(15.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test10()
        {
            Formula f = new Formula("2+6*3");
            Assert.AreEqual(20.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test11()
        {
            Formula f = new Formula("(2+6)*3");
            Assert.AreEqual(24.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test12()
        {
            Formula f = new Formula("2*(3+5)");
            Assert.AreEqual(16.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test13()
        {
            Formula f = new Formula("2+(3+5)");
            Assert.AreEqual(10.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test14()
        {
            Formula f = new Formula("2+(3+5*9)");
            Assert.AreEqual(50.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test15()
        {
            Formula f = new Formula("2+3*(3+5)");
            Assert.AreEqual(26.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test16()
        {
            Formula f = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test17()
        {
            Formula f = new Formula("5/0");
            Object result = f.Evaluate(s => 0);
            Assert.IsInstanceOfType(result, typeof(FormulaError));
        }

        [TestMethod()]
        public void Test18()
        {
            Formula f = new Formula("+");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test19()
        {
            Formula f = new Formula("2+5+");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test20()
        {
            Formula f = new Formula("2+5*7)");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }
        
        [TestMethod()]
        public void Test23()
        {
            Formula f = new Formula("5+7+(5)8");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test24()
        {
            Formula f = new Formula("");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test25()
        {
            Formula f = new Formula("y1 * 3 - 8 / 2 + 4 * (8 - 9 * 2) / 2 * x7");
            Assert.AreEqual(-12.0, f.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod()]
        public void Test26()
        {
            Formula f = new Formula("x1 + (x2 + (x3 + (x4 + (x5 + x6))))");
            Assert.AreEqual(6.0, f.Evaluate(s => 1));
        }

        [TestMethod()]
        public void Test27()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12.0, f.Evaluate(s => 2));
        }

        [TestMethod()]
        public void Test28()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0.0, f.Evaluate(s => 3));
        }


        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidExpression()
        {
            Formula f = new Formula("a2", s => s, s => false);
        }



        /********************************* HELPER METHODS **********************************/
        private static string VarToUpper(string variable)
        {
            return variable.ToUpper();
        }

        private static bool IsValid(string formula)
        {
            foreach(string variable in GetTokens(formula))
            {
                if (variable == "+" || variable == "*" || variable == "/" || variable == "-") continue;
                Match match = Regex.Match(variable, @"(([a-zA-Z]|[_])+[0-9]*)");
                
                //If the variable isn't valid, return false
                if (!match.Success)
                {
                    return false;
                }
                else
                {
                    //Make sure the variable returned from the match was the same as the original variable
                    if (!variable.Equals(match.Value)) return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /************************************** GRADING TESTS ***********************************/
        // Simple tests that return FormulaErrors
        [TestMethod()]
        public void Test16a()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test17b()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test18c()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test19d()
        {
            Formula f = new Formula("+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test20e()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test21f()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test22g()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test23h()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test24i()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test25a()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test26a()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test27a()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test28a()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod()]
        public void Test29()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod()]
        public void Test30()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod()]
        public void Test31()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod()]
        public void Test32()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod()]
        public void Test33()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test34()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test35()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test36()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test37()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod()]
        public void Test38()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void Test39()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod()]
        public void Test40()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod()]
        public void Test41()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod()]
        public void Test42()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod()]
        public void Test43()
        {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod()]
        public void Test44()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        [TestMethod()]
        public void Test45()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod()]
        public void Test46()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test47()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test48()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test49()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test50()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod()]
        public void Test51a()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51b()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51c()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51d()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51e()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52a()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52b()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52c()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52d()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52e()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }
    }
}
