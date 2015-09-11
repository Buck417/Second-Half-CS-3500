using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS1Test
{
    class Program
    {

        static void Main(string[] args)
        {
            Tester tester = new Tester();
            tester.AddVariable("A2", 6);
            tester.AddVariable("AA32", 5);

            //Create a dictionary of a few cases that we can test
            Dictionary<string, int> cases = new Dictionary<string, int>();
            cases.Add("AA32 - 4 + (5 -2)", 4);
            cases.Add("2", 2);
            cases.Add("A2 + 2", 8);
            cases.Add("(2 *2 + 3) * 5 + 2", 37);
            cases.Add("AA32 * 2", 10);
            cases.Add("A2 / 3", 2);
            cases.Add("AA32 * A2", 30);

            foreach(KeyValuePair<string, int> testCase in cases)
            {
                try {
                    tester.testExpression(testCase.Key, testCase.Value);
                }
                catch(ArgumentException e)
                {
                    Console.WriteLine(testCase.Key + " failed - exception thrown: " + e.Message);
                }
            }

            //Now, let's do some cases that are supposed to break the calculator
            Dictionary<string, bool> fails = new Dictionary<string, bool>();
            fails.Add("()", true);
            fails.Add("A5 + 3 * 2", true);
            fails.Add("+ 3 * 2", true);
            fails.Add("3 * 0", false);

            foreach(KeyValuePair<string, bool> fail in fails)
            {
                try
                {
                    tester.testExpression(fail.Key, 0);

                    //If an exception should've been thrown...
                    if(fail.Value == true)
                    {
                        Console.WriteLine("Failure: " + fail.Key + " should have thrown an exception.");
                    }
                    //If an exception shouldn't have been thrown, it worked out
                    else
                    {
                        Console.WriteLine("Exception Passed");
                    }
                }
                catch (ArgumentException e)
                {
                    if(fail.Value == true)
                    {
                        Console.WriteLine("Exception Passed");
                    }
                    else
                    {
                        Console.WriteLine("Failure: " + fail.Key + " should not have thrown an exception.");
                    }
                }
            }
            Console.Read();
        }
    }

    public class Tester
    {
        private Dictionary<string, int> variables;
        
        public Tester()
        {
            this.variables = new Dictionary<string, int>();
        }

        public void AddVariable(string name, int value)
        {
            this.variables.Add(name, value);
        }

        public int findVariableValue(string name)
        {
            int value;
            if(this.variables.TryGetValue(name, out value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException("Variable " + name + " not found.");
            }
        }

        public Boolean testExpression(string expression, int expected)
        {
            int actual = FormulaEvaluator.Evaluator.Evaluate(expression, findVariableValue);
            if(actual == expected)
            {
                Console.WriteLine("Passed");
                return true;
            }
            else
            {
                Console.WriteLine(expression + " failed. Expected " + expected + " but got " + actual);
                return false;
            }
        }
    }
}
