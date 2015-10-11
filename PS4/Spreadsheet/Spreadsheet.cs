using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Xml;

namespace SS
{
    /// <summary>
    /// This class encapsulates the behavior and logic of a spreadsheet,
    /// the Model in our MVC architecture.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph dg;
        private Dictionary<string, Cell> nonEmptyCells;

        /// <summary>
        /// Empty constructor, basically makes all validation
        /// pass and all normalization do nothing. Version is
        /// "default" with this constructor.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            dg = new DependencyGraph();
            nonEmptyCells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// This constructor chains up to the AbstractSpreadsheet class.
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            dg = new DependencyGraph();
            nonEmptyCells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// This constructor gives the option to pass in a filePath,
        /// which reads in a new spreadsheet from the filePath given.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            dg = new DependencyGraph();
            nonEmptyCells = new Dictionary<string, Cell>();
            
            int i = 0;
            try
            {
                XmlReader xr = XmlReader.Create(filePath);
                while (xr.Read())
                {
                    i++;
                }
            }
            catch(XmlException e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        private bool changed;
        /// <summary>
        /// Tells us whether the spreadsheet has changed
        /// </summary>
        public override bool Changed
        {
            get { return changed; }
            protected set { changed = value; }
        }



        /// <summary>
        /// Gets the contents of a given cell (not the value).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            if (!Cell.ValidName(name)) throw new InvalidNameException();

            //Cell does exist, let's find out what it has
            else if (nonEmptyCells.ContainsKey(name)) {
                object contents = nonEmptyCells[name].Contents;
                if (contents.GetType() == typeof(Formula)) return "=" + ((Formula)contents).ToString();
                return contents;
            }
            
            else return "";
        }

        /// <summary>
        /// Returns an IEnumerable of all of the cells that
        /// have values in them.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string name in nonEmptyCells.Keys)
            {
                yield return name;
            }
        }

        /// <summary>
        /// This sets a cell's contents to have a formula that can be evaluated.
        /// Dependencies are re-evaluated based on the variables in the formula given.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, Formula formula)
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
            foreach (string cellName in GetCellsToRecalculate(dependees))
            {
                result.Add(cellName);
            }

            return result;
        }

        /// <summary>
        /// This sets a cell's contents to just be a string.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, string text)
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
        /// This sets a cell's contents to be a static double.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (!Cell.ValidName(name)) throw new InvalidNameException();

            nonEmptyCells.AddIfNotExists(name, new Cell(name, number));
            HashSet<string> result = new HashSet<string>();
            result.Add(name);
            foreach (string cellName in GetCellsToRecalculate(name))
            {
                result.Add(cellName);
            }
            return result;
        }

        /// <summary>
        /// Gets the direct dependents of the cell 'name'. I.e., this returns
        /// all of the cells that depend on 'name'.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null) throw new ArgumentNullException("Cell name cannot be null when searching for direct dependents.");
            else if (!Cell.ValidName(name)) throw new InvalidNameException();
            //It's a bit counter-intuitive to return the dependees, but we basically want to return all the cells that depend on 'name' to be calculated
            return dg.GetDependees(name);
        }





        /***************************************** NEW FOR PS5 ********************************************/

        /// <summary>
        /// This is basically a wrapper for the protected SetCellContents methods.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string contents)
        {
            if (contents == null) throw new ArgumentNullException("contents");
            ISet<string> result;
            double number = 0;
            //If "contents" contains a formula
            if (contents.Substring(0, 1).Equals("="))
            {
                result = SetCellContents(name, new Formula(contents.Substring(1), Normalize, IsValid));
            }
            //If the "contents" are just a number
            else if (Double.TryParse(contents, out number))
            {
                result = SetCellContents(name, number);
            }
            //If the contents are a string, aka everything else
            else
            {
                result = SetCellContents(name, contents);
            }

            //Make sure we mark this as changed
            Changed = true;

            return result;
        }

        /// <summary>
        /// Gets the saved version of the spreadsheet
        /// that was saved at "filename". 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            Regex rx = new Regex(@"[a-zA-z]+\[.xml]");
            if (!rx.IsMatch(filename)) throw new SpreadsheetReadWriteException("Invalid file name " + filename);
            return "";
        }

        /// <summary>
        /// Saves the spreadsheet to file.
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            try {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();

                    //Write the spreadsheet part
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (Cell c in nonEmptyCells.Values)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", c.Name);
                        writer.WriteElementString("contents", c.Contents.ToString());
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                Changed = false;
            }
            catch(XmlException e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Gets a cell's value, based on its contents.
        /// If the contents are a string, then just return the string.
        /// If the contents comprise a formula, return the evaluation of that formula.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            //If the cell name is invalid or null, throw an InvalidNameException
            if (!Cell.ValidName(name)) throw new InvalidNameException();

            //See if the cell exists
            if (nonEmptyCells.ContainsKey(name))
            {
                Cell c = nonEmptyCells[name];
                double number = 0;
                //Return type is based on what the contents are
                if(c.Contents.GetType() == typeof(Formula))
                {
                    //TODO: Need the lookup function here
                    return ((Formula)c.Contents).Evaluate(s => 2);
                }
                else if(Double.TryParse(c.Contents.ToString(), out number))
                {
                    return number;
                }
                else
                {
                    return c.Contents;
                }
            }

            return 0;
        }
        
    }

    /// <summary>
    /// This class encapsulates some of the exctensions we want to use in
    /// existing classes.
    /// </summary>
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
        private static Regex rx = new Regex(@"(([a-zA-Z]|[_])+[0-9]*)");
        
        private string contentsType;
        
        /// <summary>
        /// This helper method just helps us see whether not a given
        /// name is a valid cell name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ValidName(string name)
        {
            if (name != null && rx.IsMatch(name) && rx.Match(name).Value.Equals(name)) return true;
            else return false;
        }

        /// <summary>
        /// The Cell constructor checks that the cell name is valid before
        /// instantiating a new Cell. If the name isn't valid, throw
        /// an InvalidNameException.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        public Cell(string name, object contents)
        {
            if (!ValidName(name)) throw new InvalidNameException();
            Name = name;
            Contents = contents;
        }

        private string name;
        /// <summary>
        /// The name for this cell. Setting the name is private, and
        /// only done through the constructor.
        /// </summary>
        public string Name
        {
            private set
            {
                name = value;
            }
            get
            {
                return name;
            }
        }
        private object contents;
        /// <summary>
        /// This just returns the cell's contents. The setter is private, and only
        /// used in the constructor.
        /// </summary>
        public object Contents
        {
            private set
            {
                contents = value;
            }
            get
            {
                return contents;
            }
        }
    }
}
