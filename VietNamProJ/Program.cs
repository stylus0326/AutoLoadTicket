using System;
using System.Windows.Forms;

namespace VietNamProJ
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
            Application.Run(new frmVN(DateTime.Now.AddDays(-6)));
            //Application.Run(new frmAGS());
        }
    }
}
