using System;
using System.IO;
using System.Windows.Forms;

namespace GlucoseTray
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
            var configFile = Application.ExecutablePath + ".config";
            if (!File.Exists(configFile))
                MessageBox.Show("ERROR: Configuration File is missing.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Run(new AppContext());
        }
    }
}
