using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EWS_Config_Tool
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            string EWSFileName = "";

            if (args.Length != 0)
            {
                EWSFileName = Convert.ToString(args[0]);
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EWS_MainForm(EWSFileName));
        }
    }
}
