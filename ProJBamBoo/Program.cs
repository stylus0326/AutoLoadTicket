using System;
using System.Windows.Forms;

namespace ProJBamBoo
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
                Application.Run(new frmQH(DateTime.Now.AddDays(-1)));
            else
                Application.Run(new frmQH(DateTime.Now));
        }
    }
}
