using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS1_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Regular use case
            string expression = "(2 + 2) * 6";

            //No close parentheses
            expression = "(2 + 2 * 6";
        }
    }
}
