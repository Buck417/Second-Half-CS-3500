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
            string expression = "(2 *2 + 3) * 5 + 2"; //Should be 27
            int expected = 37;
            int actual = FormulaEvaluator.Evaluator.Evaluate(expression, testLookup);
            if (expected != actual)
            {
                printFailedExpression(expression, expected, actual);
            }

            //No close parentheses
            try {
                expression = "(2 + 2 * 6"; //Should throw an ArgumentException
                FormulaEvaluator.Evaluator.Evaluate(expression, testLookup);
                Console.WriteLine("Unclosed parentheses check failed");
            }
            catch( Exception e)
            {
                //Do nothing, we wanted to catch it here
            }
            //Multiple operations in parentheses
            expression = "(2 + 3 + 4)";

            //Order of operations
            expression = "2 + 2 * 2"; //Should be 6

            //Using variable names
            expression = "x + y";
        }

        public static int testLookup(String varName)
        {

            return 0;
        }

        public static void printFailedExpression(string expression, int expected, int actual)
        {
            Console.WriteLine(expression + " failed, " + expected + " but got " + actual + " instead.");
        }
        
    }
}
