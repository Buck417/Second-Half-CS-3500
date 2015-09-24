// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalize and a validator.  The
    /// normalize is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private Func<string, string> normalize;
        private Func<string, bool> validator;
        private string formulaString;
        public delegate double VarLookup(string varName);

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalize is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
            formulaString = formula;
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalize and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            formulaString = formula;
            this.normalize = normalize;
            validator = isValid;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalize that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            foreach(string token in GetTokens(formulaString))
            {
                //Go through each token and decide what to do with it
                switch (token)
                {

                }
            }
            return null;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            List<string> vars = new List<string>();
            foreach(string token in GetTokens(formulaString))
            {

            }
            return vars;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return this.normalize(formulaString.Replace(" ", "")); ;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 == null && f2 == null) return true;
            else if (f1 == null && f2 != null || f1 != null && f2 == null) return false;
            return false;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1 == null && f2 == null) return false;
            else if (f1 == null && f2 != null || f1 != null && f2 == null) return true;
            return false;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            string theFormula = formulaString.Replace(" ", "");
            int hash = theFormula.GetHashCode();
            return hash;
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

        /// <summary>
        /// This encapsulates the behavior to expect when
        /// the intValue being processed is an integer.
        /// If * or / is at the top of the operator stack, 
        /// pop the value stack, pop the operator stack, 
        /// and apply the popped operator to t and the 
        /// popped number. Push the result onto the value stack.
        /// Otherwise, push t onto the value stack.
        /// </summary>
        /// <param name="valueStack"></param>
        /// <param name="intValue">The integer value we're 
        /// passing in to be calculated</param>
        /// <param name="opStack">Passed by reference so we make sure
        /// it's updated as needed</param>
        public static void ProcessInteger(ref Stack<int> valueStack, int intValue, ref Stack<string> opStack)
        {
            //If the top of the operations stack is * or /, and assuming there's something in the value stack already
            if (opStack.OnTop("*", "/"))
            {
                if (valueStack.Count == 0)
                {
                    throw new ArgumentException("Value stack empty, need at least one number to multiply");
                }
                int value = valueStack.Pop();
                switch (opStack.Pop())
                {
                    case "*":
                        valueStack.Push(intValue * value);
                        break;
                    case "/":
                        valueStack.Push(value / intValue);
                        break;
                }
            }
            else
            {
                valueStack.Push(intValue);
            }
        }

        /// <summary>
        /// This function is basically a wrapper for the 
        /// ProcessInteger function, except we need to look
        /// up the value of the intValue we're passing in first.
        /// </summary>
        /// <param name="valueStack"></param>
        /// <param name="varName">The name of the variable we're looking for</param>
        /// <param name="lookup">The lookup function to find the value
        /// of our variable</param>
        /// <param name="opStack">The operator stack</param>
        public static void ProcessVariable(ref Stack<int> valueStack, string varName, Lookup lookup, ref Stack<string> opStack)
        {
            int intValue;
            try
            {
                intValue = lookup(varName);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("Variable " + varName + " not found.");
            }
            ProcessInteger(ref valueStack, intValue, ref opStack);
        }

        /// <summary>
        /// Processes a plus or minus token. 
        /// If + or - is at the top of the operator stack, 
        /// pop the value stack twice and the operator stack 
        /// once. Apply the popped operator to the popped 
        /// numbers. Push the result onto the value stack. 
        /// Next, push the token onto the operator stack.
        /// </summary>
        /// <param name="valueStack"></param>
        /// <param name="token"></param>
        /// <param name="opStack"></param>
        public static void ProcessPlusOrMinus(ref Stack<int> valueStack, string token, ref Stack<string> opStack)
        {
            int result;
            if (opStack.Count > 0)
            {
                switch (opStack.Peek())
                {
                    case "+":
                        result = valueStack.Pop() + valueStack.Pop();
                        opStack.Pop();
                        valueStack.Push(result);
                        break;
                    case "-":
                        int first = valueStack.Pop();
                        int second = valueStack.Pop();
                        result = second - first;
                        opStack.Pop();
                        valueStack.Push(result);
                        break;
                }
            }

            opStack.Push(token);
        }

        /// <summary>
        /// Processes the right parentheses token.
        /// If + or - is at the top of the operator stack,
        /// pop the value stack twice and the operator 
        /// stack once. Apply the popped operator to the 
        /// popped numbers. Push the result onto the value 
        /// stack. Next, the top of the operator stack 
        /// should be a(. Pop it. Finally, if * or / is 
        /// at the top of the operator stack, pop the 
        /// value stack twice and the operator stack once. 
        /// Apply the popped operator to the popped numbers. 
        /// Push the result onto the value stack.
        /// </summary>
        /// <param name="valStack"></param>
        /// <param name="opStack"></param>
        public static void processRightParentheses(ref Stack<int> valStack, ref Stack<string> opStack)
        {
            int result, first, second;
            switch (opStack.Pop())
            {
                case "+":
                    result = valStack.Pop() + valStack.Pop();
                    valStack.Push(result);
                    if (!opStack.OnTop("(", "(")) throw new ArgumentException("Missing open parentheses");
                    else opStack.Pop();
                    break;
                case "-":
                    //Make sure this is done in reverse, since stacks pop things in reverse of how they were inserted
                    first = valStack.Pop();
                    second = valStack.Pop();
                    result = second - first;
                    valStack.Push(result);
                    if (!opStack.OnTop("(", "(")) throw new ArgumentException("Missing open parentheses");
                    else opStack.Pop();
                    break;
                case "*":
                    result = valStack.Pop() * valStack.Pop();
                    valStack.Push(result);
                    break;
                case "/":
                    //Make sure this is done in reverse, since stacks pop things in reverse of how they were inserted
                    first = valStack.Pop();
                    second = valStack.Pop();
                    result = second / first;
                    valStack.Push(result);
                    break;
                case ")":
                case "(":
                    throw new ArgumentException("Need an operator and not a parentheses here.");
                default:
                    //Do nothing, the operator that was popped should've been a left paren
                    break;
            }
        }

        /// <summary>
        /// This serves as a convenience method for testing
        /// which operators are on top of the operator stack.
        /// </summary>
        /// <param name="opStack"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean OnTop(this Stack<string> opStack, string first, string second)
        {
            if (opStack.Count == 0)
            {
                return false;
            }
            else
            {
                return opStack.Peek() == first || opStack.Peek() == second;
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}