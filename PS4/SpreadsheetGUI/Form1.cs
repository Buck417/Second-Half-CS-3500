using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        Spreadsheet ss;
        public Form1()
        {
            InitializeComponent();
            ss = new Spreadsheet(Validator, UppercaseString, "");
        }

        private void cellContentsBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Just functions as a normalizer - all cell names need to be uppercase.
        /// </summary>
        /// <param name="s"></param>
        private string UppercaseString(string s)
        {
            if (s == null) throw new InvalidNameException();
            return s.ToUpper();
        }

        /// <summary>
        /// Cell names must start with a letter and end with a number, and that number
        /// must be between 1 and 99.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool Validator(string s)
        {
            //Cell name must be a capital letter, followed by a number between 1 and 99
            Regex rx = new Regex(@"[A-Z]([1-9][0-9]|[1-9])");
            if (rx.IsMatch(s))
            {
                //If there was a match, make sure it returns the same as the input (not just a substring match)
                return rx.Match(s).Value.Equals(s);
            }

            //If there was no match, the cell name isn't valid.
            return false;
        }
        
    }
}
