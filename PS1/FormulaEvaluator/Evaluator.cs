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

        public static int Evaluate<T>(String expression, Lookup variableFinder)
        {
            //Make sure there's no whitespace in our expression
            expression = expression.Replace(" ", string.Empty);

            Stack<T> opStack = new Stack<T>();
            Stack<T> valStack = new Stack<T>();

            //Tokenize our expression
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return 0;
        }
    }
}
