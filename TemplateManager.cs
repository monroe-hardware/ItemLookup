using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ItemLookup {
    class TemplateManager : Dictionary<string, Template> {

        public TemplateManager() {
            foreach(string key in Program.Config.Keys) {
                if (key.StartsWith("label."))
                    initTemplate(key.Replace("label.", ""), AppDomain.CurrentDomain.BaseDirectory + Program.Config[key]);
            }
        }

        public string execTemplate(string template, Dictionary<string, string> fields) {
            foreach (string str in fields.Keys) {
                template = template.Replace("%(" + str + ")", fields[str]);
            }
            return template;
        }

        private void initTemplate(string id, string fileName) {
            this[id] = new Template(File.ReadAllText(fileName));
        }
    }
}
