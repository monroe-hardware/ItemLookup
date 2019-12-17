using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ItemLookup {
    class ConfigFile : Dictionary<string, string> {
        private string fileName;

        public ConfigFile(string _fileName) {
            this.fileName = _fileName;
            try { readConfig();  }
            catch(Exception) { }


        }

        private void readConfig() {
            if (!File.Exists(this.fileName))
                throw new Exception("Config file not found!");

            StreamReader reader = File.OpenText(this.fileName);
            while(!reader.EndOfStream) {
                string line = reader.ReadLine();
                string[] pair = line.Split('=');
                if(!line.StartsWith("#") && pair.Length == 2 && pair[1].Length > 0)
                    this[pair[0].Trim()] = pair[1].Trim();
            }
            reader.Close();
        }

        private void writeConfig() {
            StreamWriter writer = File.CreateText(this.fileName);

            foreach(string key in this.Keys)
                writer.WriteLine(key.Trim() + "=" + this[key].Trim());

            writer.Close();
        }

        public int getInt(string key) { return Convert.ToInt32(this[key]); }
        public bool getBool(string key) { return Convert.ToBoolean(this[key]); }
        public decimal getDecimal(string key) { return Convert.ToDecimal(this[key]); }
        public double getDouble(string key) { return Convert.ToDouble(this[key]); }
        public DateTime GetDateTime(string key) { return Convert.ToDateTime(this[key]); }
    }
}
