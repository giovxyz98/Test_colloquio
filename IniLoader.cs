using System;
using System.IO;
using System.Reflection;

namespace Test_colloquio
{
    /// <summary>
    /// Carica un file .ini con struttura a sezioni e ne assegna i valori
    /// alle classi statiche "Predefiniti_&lt;NomeSezione&gt;" tramite reflection.
    /// Il codice è completamente indipendente dai nomi delle sezioni e delle
    /// proprietà: per aggiungere nuove sezioni basta creare la classe statica
    /// corrispondente in Predefiniti.cs.
    /// </summary>
    public static class IniLoader
    {
        private const string ClassPrefix    = "Predefiniti_";
        private const string Namespace      = "Test_colloquio";

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath)) return;

            Type   currentType    = null;

            foreach (string rawLine in File.ReadLines(filePath))
            {
                string line = rawLine.Trim();

                // Riga vuota o commento
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                // --- Intestazione sezione: [NomeSezione] ---
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string sectionName = line.Substring(1, line.Length - 2).Trim();
                    string className   = $"{Namespace}.{ClassPrefix}{sectionName}";

                    // Cerca la classe statica corrispondente nell'assembly corrente
                    currentType = Assembly.GetExecutingAssembly().GetType(className);
                    // Se non esiste una classe per questa sezione, la ignoriamo
                    continue;
                }

                // --- Coppia nomevar = valore ---
                if (currentType == null) continue;

                int eqPos = line.IndexOf('=');
                if (eqPos < 1) continue;

                string nomeVar = line.Substring(0, eqPos).Trim();
                string valore  = line.Substring(eqPos + 1).Trim();

                // Rimuove delimitatori stringa se presenti
                if (valore.StartsWith("\"") && valore.EndsWith("\"") && valore.Length >= 2)
                    valore = valore.Substring(1, valore.Length - 2);

                // Cerca la proprietà statica pubblica con quel nome
                PropertyInfo prop = currentType.GetProperty(
                    nomeVar,
                    BindingFlags.Public | BindingFlags.Static);

                if (prop == null || !prop.CanWrite) continue;

                // Verifica di congruità del tipo e assegnazione
                try
                {
                    object converted = Convert.ChangeType(valore, prop.PropertyType);
                    prop.SetValue(null, converted);
                }
                catch
                {
                    // Tipo non congruente o stringa non riconoscibile: ignora la riga
                }
            }
        }
    }
}
