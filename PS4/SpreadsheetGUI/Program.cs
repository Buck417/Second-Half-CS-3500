using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    class SpreadsheetProgram : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

        private static SpreadsheetProgram appContext;

        public static SpreadsheetProgram GetAppContext()
        {
            if(appContext == null)
            {
                appContext = new SpreadsheetProgram();
            }

            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private SpreadsheetProgram()
        {

        }

        
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SpreadsheetProgram appContext = SpreadsheetProgram.GetAppContext();
            appContext.RunForm(new Form1());
            Application.Run(appContext);
        }
    }
}
