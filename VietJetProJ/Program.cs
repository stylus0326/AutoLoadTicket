using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VietJetProJ
{
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
            if (DateTime.Now.Hour == 3)
                Application.Run(new frmVJ(DateTime.Now.AddDays(-1)));
            else
                Application.Run(new frmVJ(DateTime.Now));
        }
    }
}
