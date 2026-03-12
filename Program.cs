using System;
using System.IO;
using System.Windows.Forms;

namespace Test_colloquio
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Carica config.ini dalla cartella del programma (se esiste).
            // Popola le classi Predefiniti_* via reflection prima di
            // istanziare qualsiasi form.
            string iniPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "config.ini");

            IniLoader.Load(iniPath);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
