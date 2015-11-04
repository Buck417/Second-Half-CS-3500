using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace SS
{
    /// <summary>
    /// A Spreadsheet object represents the state of a simple spreadsheet.  A 
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

        private DependencyGraph dependency_graph;
        private Dictionary<String, Cell> cell;
        private HashSet<String> dependents;

        /// <summary>
        /// Constructor for Spreadsheet with default values added to
        /// delegates
        /// </summary>
        public Spreadsheet()
            : base(s => true, s => s, "default")
        {
            dependency_graph = new DependencyGraph();
            cell = new Dictionary<String, Cell>();
            Changed = false;
        }

        /// <summary>
        /// Constructor which takes in delegate values from the user
        /// </summary>
        /// <param name="isValid">Validity delegate</param>
        /// <param name="normalize">Normalization delegate</param>
        /// <param name="version">Version of spreadsheet</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            dependency_graph = new DependencyGraph();
            cell = new Dictionary<String, Cell>();
            Changed = false;            //Test
        }

        /// <summary>
        /// Constructor which takes in delegate values from the user including a file path
        /// </summary>
        /// <param name="isValid">Validity delegate</param>
        /// <param name="normalize">Normalization delegate</param>
        /// <param name="version">Version of spreadsheet</param>
        public Spreadsheet(string file_path, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            dependency_graph = new DependencyGraph();
            cell = new Dictionary<String, Cell>();

            if (version != GetSavedVersion(file_path))
                throw new SpreadsheetReadWriteException("The versions don't match.");

            if (file_path == null || file_path == "")
                throw new SpreadsheetReadWriteException("The filename cannot be null or empty");

            try
            {
                using (XmlReader reader = XmlReader.Create(file_path))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "cell":
                                    reader.Read();
                                    String temp_name = reader.ReadElementContentAsString();
                                    String temp_content = reader.ReadElementContentAsString();
                                    if (temp_name == null || temp_content == null)
                                        throw new SpreadsheetReadWriteException("Name or Contents was missing from XML");
                                    SetContentsOfCell(temp_name, temp_content);
                                    break;

                                case "spreadsheet":
                                    Version = reader["version"];
                                    break;

                                default:
                                    throw new SpreadsheetReadWriteException("Bad XML");
                            }
                        }
                    }
                }
            }
            catch (InvalidNameException)
            {
                throw new SpreadsheetReadWriteException("Error in the name of the file");
            }
            Changed = false;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cell.Keys;
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                throw new InvalidNameException();

            name = Normalize(name);
            if (!cell.ContainsKey(name))
                return "";
            return cell[name].contents;

        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        protected override ISet<string> SetCellContents(string name, double number)
        {

            if (cell.ContainsKey(name))
            {
                cell[name] = new Cell(number);
            }
            else
            {
                cell.Add(name, new Cell(number));
            }

            dependency_graph.ReplaceDependees(name, new HashSet<String>());
            HashSet<String> dependees = new HashSet<string>(GetCellsToRecalculate(name));
            return dependees;
        }


        /// <summary>
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
        protected override ISet<string> SetCellContents(string name, string text)
        {

            if (cell.ContainsKey(name))
            {
                cell[name] = new Cell(text);
            }
            else
            {
                cell.Add(name, new Cell(text));
            }
            string cell_content = (String)cell[name].contents;

            if (cell_content.Equals(""))
                cell.Remove(name);

            dependency_graph.ReplaceDependees(name, new HashSet<String>());

            HashSet<String> dependees = new HashSet<string>(GetCellsToRecalculate(name));
            return dependees;
        }

        /// <summary>
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
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {

            IEnumerable<String> dependee_list = dependency_graph.GetDependees(name);
            dependency_graph.ReplaceDependees(name, formula.GetVariables());

            try
            {
                HashSet<String> all_dependees = new HashSet<String>(GetCellsToRecalculate(name));

                if (cell.ContainsKey(name))
                {
                    cell[name] = new Cell(formula, LookupValue);
                }
                else
                {
                    cell.Add(name, new Cell(formula, LookupValue));
                }

                return all_dependees;
            }

            catch (CircularException)
            {
                dependency_graph.ReplaceDependees(name, dependee_list);
                throw new CircularException();
            }
        }

        /// <summary>
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
            if (name == null)
                throw new ArgumentNullException();

            if (!Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                throw new InvalidNameException();
            return dependency_graph.GetDependents(name);
        }

        /// <summary>
        /// Private class for representing a Cell and what a Cell contains.
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// Setter/Getter for cell contents
            /// </summary>
            public Object contents { get; private set; }
            /// <summary>
            /// Setter/Getter for cell value
            /// </summary>
            public Object value { get; private set; }

            string contents_type;
            string value_type;

            /// <summary>
            /// Constructor for a string cell
            /// </summary>
            /// <param name="name">Name of Cell</param>
            public Cell(string name)
            {
                contents = name;
                value = contents;
                contents_type = name.GetType().ToString();
                value_type = contents_type;
            }

            /// <summary>
            /// Constructor for a double cell
            /// </summary>
            /// <param name="name">Name of Cell</param>
            public Cell(double name)
            {
                contents = name;
                value = contents;
                contents_type = name.GetType().ToString();
                value_type = contents_type;

            }

            /// <summary>
            /// Constructor for a formula cell
            /// </summary>
            /// <param name="formula">Formula in Cell</param>
            /// <param name="lookup">Lookup delegate</param>
            public Cell(Formula formula, Func<string, double> lookup)
            {
                contents = formula;
                value = formula.Evaluate(lookup);
                contents_type = formula.GetType().ToString();
                value_type = value.GetType().ToString();

            }

            /// <summary>
            /// Checks if it is a formula and assigns a value to the cell determined by
            /// the lookup delegate
            /// </summary>
            /// <param name="lookup">Lookup Delegate</param>
            public void Evaluate(Func<string, double> lookup)
            {
                if (contents_type.Equals("SpreadsheetUtilities.Formula"))
                {
                    Formula temp = (Formula)contents;
                    value = temp.Evaluate(lookup);
                }
            }


        }


        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                switch (reader.Name)
                                {
                                    case "spreadsheet":
                                        return reader["version"];

                                    default:
                                        throw new SpreadsheetReadWriteException("Bad XML while loading");
                                }
                            }

                        }
                    }
                    catch (XmlException)
                    {
                        throw new SpreadsheetReadWriteException("Bad XML while parsing");
                    }

                    throw new SpreadsheetReadWriteException("The XML contained nothing");
                }

            }
            catch (XmlException)
            {
                throw new SpreadsheetReadWriteException("Wrong Path or File name");
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Error opening file");
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    String temp_cell;
                    foreach (String s in cell.Keys)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", s);

                        if (cell[s].contents is double)
                            temp_cell = cell[s].contents.ToString();

                        else if (cell[s].contents is Formula)
                            temp_cell = "=" + cell[s].contents.ToString();
                        else
                            temp_cell = (string)cell[s].contents;

                        writer.WriteElementString("contents", temp_cell);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (XmlException)
            {
                throw new SpreadsheetReadWriteException("The XML was corrupt");
            }
            catch (IOException)
            {
                throw new SpreadsheetReadWriteException("Something went wrong");
            }

            Changed = false;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                throw new InvalidNameException();

            name = Normalize(name);
            if (!cell.ContainsKey(name))
                return "";
            return cell[name].value;
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (name == null)
                throw new InvalidNameException();

            if (content == null)
                throw new ArgumentNullException();

            if (!Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*") || !IsValid(name))
                throw new InvalidNameException();

            HashSet<String> dependent_list;
            double cell_value;

            if (content.Equals(" "))
                dependent_list = new HashSet<String>(SetCellContents(name, content));

            else if (Double.TryParse(content, out cell_value))
                dependent_list = new HashSet<String>(SetCellContents(name, cell_value));

            else if (content.StartsWith("="))
                dependent_list = new HashSet<String>(SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid)));

            else
                dependent_list = new HashSet<String>(SetCellContents(name, content));

            foreach (String s in dependent_list)
            {
                Cell cell_number_value;
                if (cell.TryGetValue(s, out cell_number_value))
                    cell_number_value.Evaluate(LookupValue);
            }

            Changed = true;
            return dependent_list;
        }

        /// <summary>
        /// Helper method for returning the value of a cell
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <returns>Value of the cell</returns>
        private double LookupValue(string name)
        {
            Cell temp_cell;
            if (cell.TryGetValue(name, out temp_cell))
            {
                if (temp_cell.value is double)
                    return (double)temp_cell.value;
                else
                    throw new ArgumentException("The value was not a double");
            }

            throw new ArgumentException("The cell does not have a valid value");
        }


    }
}