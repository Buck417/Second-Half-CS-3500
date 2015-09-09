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

        public static int Evaluate(String expression, Lookup variableFinder)
        {
            //Make sure there's no whitespace in our expression
            expression = expression.Replace(" ", string.Empty);

            Stack<string> opStack = new Stack<string>();
            Stack<int> valStack = new Stack<int>();

            //Tokenize our expression
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach(string token in substrings)
            {
                //If token is an integer
                int value;
                if(Int32.TryParse(token, out value))
                {
                    valStack.ProcessInteger(value, ref opStack);
                }

                //If token is + or -

            }

            return 0;
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
            if(valueStack.Count == 0)
            {
                throw new ArgumentException();
            }

            //If the top of the operations stack is * or /, and assuming there's something in the value stack already
            if(opStack.Peek() == "*" || opStack.Peek() == "/")
            {
                int value = valueStack.Pop();
                string op = opStack.Pop();
                switch (op)
                {
                    case "*":
                        valueStack.Push(intValue * value);
                        break;
                    case "/":
                        valueStack.Push(intValue / value);
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
            int intValue = lookup(varName);
            valueStack.ProcessInteger(intValue, ref opStack);
        }

        public static void ProcessPlusOrMinus(this Stack<int> valueStack, string token, ref Stack<string> opStack)
        {
            int result;
            if(valueStack.Count < 2)
            {
                throw new ArgumentException();
            }
            switch (opStack.Peek())
            {
                case "+":
                    result = valueStack.Pop() + valueStack.Pop();
                    opStack.Pop();
                    valueStack.Push(result);
                    opStack.Push(token);
                    break;
                case "-":
                    result = valueStack.Pop() - valueStack.Pop();
                    opStack.Pop();
                    valueStack.Push(result);
                    opStack.Push(token);
                    break;
            }
        }
        
    }

}
