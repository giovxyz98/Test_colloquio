using System.IO;
using System.Windows.Forms;

namespace Test_colloquio
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            IniLoader.Load(iniPath);
            Application.Run(new Form1());
        }
    }
}
