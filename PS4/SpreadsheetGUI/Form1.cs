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
using System.IO;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        private Spreadsheet ss;
        private string version = "ps6";
        /// <summary>
        /// Constructor for Empty Spreadsheet Form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            ss = new Spreadsheet(Validator, UppercaseString, version);

            updateTextBox(spreadsheetPanel1);
        }

        public Form1(string filename)
        {
            InitializeComponent();
            ss = new Spreadsheet(filename, Validator, UppercaseString, version);
            updateTextBox(spreadsheetPanel1);

            //Make sure all the existing cells show up
            foreach (string name in ss.GetNamesOfAllNonemptyCells())
            {
                updateCellValue(name);
            }
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
            cellValueTextBox.Focus();

            //Assigns the cell name textbox to name for the selected cell
            name = getCellName();
            cellContentsBox.Text = name;

            //Checks the cell if its a formula so it knows to add a '=' or not in front of the value
            if (ss.GetCellContents(name).GetType() == typeof(Formula))
                cellValueTextBox.Text = "=" + ss.GetCellContents(name).ToString();
            else
            {
                cellValueTextBox.Text = ss.GetCellContents(name).ToString();
               
            }
            cellValueWindow.Text = ss.GetCellValue(name).ToString();
                
        }

        /// <summary>
        /// Helper method that assigns a value to the selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solveButtonClick(object sender, EventArgs e)
        {
            //returns current cell name
            String name = getCellName();
            //IEnumerable set needed to store what setcontents returns
            ISet<String> solved;

            //Tries adding cells to the set, catching formula errors and circular exceptions
            try
            {
                solved = ss.SetContentsOfCell(name, cellValueTextBox.Text);
            }
            catch(CircularException)
            {
                cellValueTextBox.Text = "Circular Exception Thrown Referring To Itself";
                return;
            }
            catch(FormulaFormatException f)
            {
                cellValueTextBox.Text = f.Message;
                return;
            }
            catch (Exception e1)
            {
                cellValueTextBox.Text = e1.Message;
                return;
            }
            //TODO Did we handle all possible exceptions?
            updateTextBox(spreadsheetPanel1);

            //Update each cell with the new value
            foreach (String s in solved)
                updateCellValue(s);
        }

        /// <summary>
        /// Helper method that takes the grid number for the cell and inserts the value
        /// of the cell into the grid, displaying the value in the spreadsheet
        /// </summary>
        /// <param name="name"></param>
        private void updateCellValue(String name)
        {
            //Need location of cell selected
            int cell_row;
            int.TryParse(name.Substring(1), out cell_row);
            cell_row--;
            int cell_col = name.First<char>() - 'A';

            //Set the cell to the new value
            if (ss.GetCellValue(name).GetType() == typeof(FormulaError))  //TODO needs to return a message?
            {
                return;
            }
            else
            {
                spreadsheetPanel1.SetValue(cell_col, cell_row, ss.GetCellValue(name).ToString());
            }

        }

        /// <summary>
        /// Keyboard listener when the enter key is pressed, use the solve button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cellValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                solveButtonClick(sender, e);
        }

        /// <summary>
        /// Menu item that saves a spreadsheet to a spreadsheet file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveMenuItemClickHandler(object sender, EventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "Spreadsheet File (*.sprd)|*.sprd|All Files (*.*)|*.*";
            d.AddExtension = true;
            
            DialogResult result = d.ShowDialog();
            
            //User chose to save
            if(result == DialogResult.OK)
            {
                ss.Save(d.FileName);
            }

        }

        /// <summary>
        /// Menu item that closes the spreadsheet, making sure the user saves a changed spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeMenuItemClickHandler(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Menu item that opens a spreadsheet file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openMenuItemClickHandler(object sender, EventArgs e)
        {
            
            OpenFileDialog d = new OpenFileDialog();
            d.CheckFileExists = true;
            d.Filter = "Spreadsheet File (*.sprd)|*.sprd|All Files (*.*)|*.*";
            DialogResult result = d.ShowDialog();
            string filename = "";

            if(result == DialogResult.OK)
            {
                filename = d.FileName;
                SpreadsheetProgram.GetAppContext().RunForm(new Form1(filename));
            }
            
        }

        /// <summary>
        /// Menu item that creates a new spreadsheet file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newMenuItemClickHandler(object sender, EventArgs e)
        {
            SpreadsheetProgram.GetAppContext().RunForm(new Form1());
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show("\tTo use this spreadsheet application, select a cell in the grid and enter either a name, number or formula." +
                "\n\tThe format for a formula begins with an '=' sign and can only use cells that exist on the grid A1-Z99. The operations " +
                "that can be used are +, -, *, and divide. \n\tYou can change a cell by selecting the cell and editing the value in the " +
                "value box. All other cells that depend on the changed cell will also update to their new value. For example, you can either type 2, =2*A1, or 'Hello' in the text box labeled 'Edit Cell'.", "How To Use The Spreadsheet");
        }
        
        /// <summary>
        /// Event for when the form closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeSpreadsheetHandler(object sender, FormClosingEventArgs e)
        {
            //Ask if they want to save their changes, if they need to
            if (ss.Changed)
            {
                DialogResult result = MessageBox.Show("Would you like to save?", "Close Spreadsheet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    this.Close();
                }
                    
                else if (result == DialogResult.Yes)
                {
                    saveMenuItemClickHandler(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            //Close the window
            else
            {
                this.Close();
            }
        }
        
    }
}

