﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// (Copied from AbstractSpreadsheet for easier development)
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph dg;
        private Dictionary<string, Cell> nonEmptyCells;
        /// <summary>
        /// (Copied from AbstractSpreadsheet for easier development)
        /// Our zero-argument constructor; creates an empty spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            dg = new DependencyGraph();
            nonEmptyCells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// (Copied from AbstractSpreadsheet for easier development)
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (!Cell.ValidName(name)) throw new InvalidNameException();
            else if (nonEmptyCells.ContainsKey(name))
                return nonEmptyCells[name].Contents;
            else return "";
        }

        /// <summary>
        /// (Copied from AbstractSpreadsheet for easier development)
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach(string name in nonEmptyCells.Keys)
            {
                yield return name;
            }
        }

        /// <summary>
        /// (Copied from AbstractSpreadsheet for easier development)
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if ((object)formula == null) throw new ArgumentNullException();
            else if (!Cell.ValidName(name)) throw new InvalidNameException();

            nonEmptyCells.AddIfNotExists(name, new Cell(name, formula));

            HashSet<string> dependees = new HashSet<string>();
            //Add the dependency for each of the variables in this formula
            foreach (string dependee in formula.GetVariables())
            {
                dg.AddDependency(name, dependee);
                dependees.Add(dependee);
            }
            HashSet<string> result = new HashSet<string>();
            //Add the cell-in-question's name
            result.Add(name);
            //Get all the cells that need to be recalculated, based on the dependees in this formula
            foreach(string cellName in GetCellsToRecalculate(dependees))
            {
                result.Add(cellName);
            }
            
            return result;
        }

        /// <summary>
        /// (Copied from AbstractSpreadsheet for easier development)
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException();
            else if (name == null) throw new InvalidNameException();

            nonEmptyCells.AddIfNotExists(name, new Cell(name, text));
            HashSet<string> result = new HashSet<string>();
            result.Add(name);

            foreach (string cellName in GetCellsToRecalculate(name))
            {
                result.Add(cellName);
            }
            return result;
        }

        /// <summary>
        /// (Copied from AbstractSpreadsheet for easier development)
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (!Cell.ValidName(name)) throw new InvalidNameException();
            
            nonEmptyCells.AddIfNotExists(name, new Cell(name, number));
            HashSet<string> result = new HashSet<string>();
            result.Add(name);
            foreach(string cellName in GetCellsToRecalculate(name))
            {
                result.Add(cellName);
            }
            return result;
        }
        
        /// <summary>
        /// (Copied from AbstractSpreadsheet for easier development)
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null) throw new ArgumentNullException("Cell name cannot be null when searching for direct dependents.");
            else if (!Cell.ValidName(name)) throw new InvalidNameException();
            //It's a bit counter-intuitive to return the dependees, but we basically want to return all the cells that depend on 'name' to be calculated
            return dg.GetDependees(name);
        }

        
    }

    public static class Extensions
    {
        /// <summary>
        /// If no cell by this name already exists, add it. Otherwise, update the existing value.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="name"></param>
        /// <param name="cell"></param>
        public static void AddIfNotExists(this Dictionary<string, Cell> dict, string name, Cell cell)
        {
            if (dict.ContainsKey(name))
            {
                dict[name] = cell;
            }
            else
            {
                dict.Add(name, cell);
            }
        }
    }

    /// <summary>
    /// This guy just encapsulates what a spreadsheet cell has/does.
    /// </summary>
    public class Cell
    {
        public static Regex rx
        {
            //Read-only property
            get
            {
                return new Regex(@"(([a-zA-Z]|[_])+[0-9]*)");
            }
        }

        public static bool ValidName(string name)
        {
            if (name != null && rx.IsMatch(name) && rx.Match(name).Value.Equals(name)) return true;
            else return false;
        }
            
        public Cell(string name, object contents)
        {
            Name = name;
            Contents = contents;
        }

        private string name;
        public string Name
        {
            set
            {
                
                //Make sure it's a valid cell name
                if (rx.IsMatch(value))
                {
                    //If we got a match from the Regex, make sure it's the same as the initial input
                    if (rx.Match(value).Value.Equals(value))
                    {
                        name = value;
                    }
                    //If the regexed value isn't the same as the original value, it's not a valid variable
                    else
                    {
                        throw new InvalidNameException();
                    }
                }
                //If there's no regex match at all, it's an invalid variable
                else
                {
                    throw new InvalidNameException();
                }
            }
            get
            {
                return name;
            }
        }
        private object contents;
        public object Contents
        {
            set
            {
                //TODO: sanitize the contents setter
                contents = value;
            }
            get
            {
                return contents;
            }
        }
    }
}
