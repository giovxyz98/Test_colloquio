using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

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

            // ----------------------------------------------------------------
            // TEST AddEsame — rimuovere o commentare quando non serve
            // ----------------------------------------------------------------
            TestAddEsame(Predefiniti_Database.ConnectionString);
            return;
            // ----------------------------------------------------------------

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }

        private static void TestAddEsame(string connectionString)
        {
            try
            {
                var repo = new Repository(connectionString);

                var esame = new Esame
                {
                    CodiceMinisteriale = "TST001",
                    CodiceInterno      = "INT999",
                    DescrizioneEsame   = "Esame di test",
                    IdParteCorpo       = 1,
                    DurataMinuti       = 30,
                    Attivo             = true
                };

                int newId = repo.AddEsame(esame);

                MessageBox.Show(
                    $"Esame inserito con successo!\nId generato: {newId}",
                    "AddEsame Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERRORE AddEsame:\n" + ex.Message,
                    "AddEsame Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
