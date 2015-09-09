using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS1Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FormulaEvaluator.Evaluator.Lookup lookup = testLookup;
            
            //Regular use case
            string expression = "(2 + 2) * 6"; //Should be 24
            int result = FormulaEvaluator.Evaluator.Evaluate<string>(expression, testLookup);

            //No close parentheses
            expression = "(2 + 2 * 6"; //Should throw an ArgumentException

            //Order of operations
            expression = "2 + 2 * 2"; //Should be 6

            //Using variable names
            expression = "x + y";
        }

        public static int testLookup(String varName)
        {

            return 0;
        }
        
    }
}
