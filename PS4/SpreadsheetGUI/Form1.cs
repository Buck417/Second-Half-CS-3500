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
using SpreadsheetUtilities;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        private Spreadsheet ss;
        /// <summary>
        /// Constructor for Empty Spreadsheet Form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            ss = new Spreadsheet(Validator, UppercaseString, "");

            updateTextBox(spreadsheetPanel1);
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

        /// <summary>
        /// Helper method that gets and creates the cell name that was selected
        /// by the user.
        /// </summary>
        /// <returns>Returns a string of the selected cell name</returns>
        private String getCellName()
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            int cell_number = row + 1;
            char cell_letter = (Char)('A' + col);

            return cell_letter + "" + cell_number;
        }

        /// <summary>
        /// Helper method that updates all text boxes when something changes
        /// </summary>
        /// <param name="s"></param>
        private void updateTextBox(SpreadsheetPanel s)
        {
            String name;

            name = getCellName();
            cellContentsBox.Text = name;
        }



        
    }
}

