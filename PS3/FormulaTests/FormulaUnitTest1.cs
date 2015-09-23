using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaTests
{
    [TestClass]
    public class FormulaUnitTest1
    {
        /// <summary>
        /// Happy path - make sure it can add two decimals
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            SpreadsheetUtilities.Formula f = new SpreadsheetUtilities.Formula("2.0 + 2.5");
            
        }

        
    }
}
