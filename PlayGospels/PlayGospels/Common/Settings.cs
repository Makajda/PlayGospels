using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace PlayGospels.Common {
    public class Settings {
        private readonly static string fileName = Path.Combine(Given.PathToData, "settings.json");
        private static readonly Lazy<Settings> instance = new Lazy<Settings>(Load);
        public static Settings Instance { get => instance.Value; }

        public void Save() {
            try {
                var json = JsonSerializer.Serialize<Settings>(this, new JsonSerializerOptions() { IgnoreNullValues = true, WriteIndented = true });
                File.WriteAllText(fileName, json);
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        private static Settings Load() {
            try {
                var json = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<Settings>(json);
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
                return new Settings();
            }
        }

        //Settings
        public bool IsCtrpnlHide { get; set; }
        public string LangSound { get; set; }
        public string LangLeft { get; set; }
        public string LangRight { get; set; }
        public bool IsLangLeft { get; set; } = true;
        public bool IsLangRight { get; set; } = true;
        public int Times { get; set; } = 1;
        public double TextFontSize { get; set; } = 18d;
        public string Book { get; set; }
        public Dictionary<string, string[]> ChaptersSelected { get; set; }
    }
}
