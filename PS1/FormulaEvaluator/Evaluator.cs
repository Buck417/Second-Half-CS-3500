using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// This static class evaluates mathematical expressions
    /// as an 'infix calculator.'
    /// </summary>
    public static class Evaluator
    {
        

        public delegate int Lookup(string variableName);

        public static int Evaluate(String expression, Lookup lookupVariableValue)
        {
            //Make sure there's no whitespace in our expression
            expression = expression.Replace(" ", "");
            expression = expression.Replace("\t", "");

            Stack<string> opStack = new Stack<string>();
            Stack<int> valStack = new Stack<int>();

            //Tokenize our expression
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach(string token in substrings)
            {
                //Skip over the nonsense empty tokens
                if (token == "") continue;

                //If token is an integer
                int value;
                if(Int32.TryParse(token, out value))
                {
                    valStack.ProcessInteger(value, ref opStack);
                    continue;
                }

                //If token is + or -
                if(token == "+" || token == "-")
                {
                    valStack.ProcessPlusOrMinus(token, ref opStack);
                    continue;
                }

                //If token is *, /, or left parentheses "("
                if (token == "*" || token == "/" || token == "(")
                {
                    opStack.Push(token);
                    continue;
                }

                //If token is a right parentheses
                if(token == ")")
                {
                    processRightParentheses(ref valStack, ref opStack);
                    continue;
                }

                //If we got this far, we know it's probably either a variable or invalid character
                Match match = Regex.Match(token, @"([a-zA-Z]*[0-9]*)");
                if (match.Success)
                {
                    //Make sure that the matched value is the same as the token it was given
                    if (!match.Value.Equals(token)) throw new ArgumentException("Invalid variable name: " + token);

                    valStack.ProcessVariable(token, lookupVariableValue, ref opStack);
                }
            }

            if(opStack.Count == 0)
            {
                if(valStack.Count == 1)
                {
                    return valStack.Pop();
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                if(opStack.Count == 1)
                {
                    if (valStack.Count != 2)
                        throw new ArgumentException("Need two values in the value stack if there's only one item in the operation stack.");
                    switch (opStack.Pop())
                    {
                        case "+":
                            return valStack.Pop() + valStack.Pop();
                        case "-":
                            int first = valStack.Pop();
                            int second = valStack.Pop();
                            return second - first;
                        default:
                            throw new ArgumentException();
                    }

                }
                else
                {
                    throw new ArgumentException();
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
        public static void ProcessInteger(this Stack<int> valueStack, int intValue, ref Stack<string> opStack)
        {
            //If the top of the operations stack is * or /, and assuming there's something in the value stack already
            if(opStack.onTop("*", "/"))
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
        public static void ProcessVariable(this Stack<int> valueStack, string varName, Lookup lookup, ref Stack<string> opStack)
        {
            int intValue;
            try {
                intValue = lookup(varName);
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException("Variable " + varName + " not found.");
            }
            valueStack.ProcessInteger(intValue, ref opStack);
        }

        public static void ProcessPlusOrMinus(this Stack<int> valueStack, string token, ref Stack<string> opStack)
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

        public static void processRightParentheses(ref Stack<int> valStack, ref Stack<string> opStack)
        {
            int result;
            switch (opStack.Pop())
            {
                case "+":
                    result = valStack.Pop() + valStack.Pop();
                    valStack.Push(result);
                    if(opStack.Pop() != "(") throw new ArgumentException(); //Should have had a parentheses here;
                    break;
                case "-":
                    int first = valStack.Pop();
                    int second = valStack.Pop();
                    result = second - first;
                    valStack.Push(result);
                    if (opStack.Pop() != "(") throw new ArgumentException(); //Should have had a parentheses here;
                    break;
                case "*":
                    result = valStack.Pop() * valStack.Pop();
                    valStack.Push(result);
                    break;
                case "/":
                    result = valStack.Pop() / valStack.Pop();
                    valStack.Push(result);
                    break;
                case ")":
                    throw new ArgumentException();
                default:
                    //Do nothing, the operator that was popped should've been a left paren
                    break;
            }
        }

        public static Boolean onTop(this Stack<string> opStack, string first, string second)
        {
            if(opStack.Count == 0)
            {
                return false;
            }
            else
            {
                return opStack.Peek() == first || opStack.Peek() == second;
            }
        }
        
    }

}
