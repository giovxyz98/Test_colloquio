using System;
using System.IO;
using System.Reflection;

namespace Test_colloquio
{
    public static class IniLoader
    {
        private const string ClassPrefix = "Predefiniti_";
        private const string Namespace   = "Test_colloquio";

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath)) return;

            Type? currentType = null;

            foreach (string rawLine in File.ReadLines(filePath))
            {
                string line = rawLine.Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
                    continue;

                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    string sectionName = line[1..^1].Trim();
                    string className   = $"{Namespace}.{ClassPrefix}{sectionName}";
                    currentType = Assembly.GetExecutingAssembly().GetType(className);
                    continue;
                }

                if (currentType == null) continue;

                int eqPos = line.IndexOf('=');
                if (eqPos < 1) continue;

                string nomeVar = line[..eqPos].Trim();
                string valore  = line[(eqPos + 1)..].Trim();

                if (valore.StartsWith('"') && valore.EndsWith('"') && valore.Length >= 2)
                    valore = valore[1..^1];

                PropertyInfo? prop = currentType.GetProperty(
                    nomeVar,
                    BindingFlags.Public | BindingFlags.Static);

                if (prop == null || !prop.CanWrite) continue;

                try
                {
                    object converted = Convert.ChangeType(valore, prop.PropertyType);
                    prop.SetValue(null, converted);
                }
                catch { }
            }
        }
    }
}
