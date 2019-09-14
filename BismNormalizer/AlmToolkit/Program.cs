using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmToolkit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //with args(user open file with the program)
            if (args != null && args.Length > 0)
            {
                string fileName = args[0];
                //Check file exists
                if (File.Exists(fileName))
                {
                    ComparisonForm MainFrom = new ComparisonForm();
                    MainFrom.LoadFile(fileName);
                    Application.Run(MainFrom);
                }
                //The file does not exist
                else
                {
                    Application.Run(new ComparisonForm());
                }
            }
            //without args
            else
            {
                Application.Run(new ComparisonForm());
            }
        }
    }
}
