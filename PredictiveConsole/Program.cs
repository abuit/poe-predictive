using System;
using System.Windows.Forms;

namespace PredictiveConsole
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (ConsoleWriter.Instance = new ConsoleWriter())
            {
                Console.SetOut(ConsoleWriter.Instance);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ConsoleForm());
            }
        }
    }
}
