using Nii.JSON;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ItemLookup {

    class Template {
        private string tmpl;

        public Template(string _tmpl) {
            this.tmpl = _tmpl;
        }

        public string exec(Dictionary<string, string> data) {

            data["today"] = DateTime.Today.ToString("MM/dd/yy");

            string str = Regex.Replace(this.tmpl, @"@IF\(([A-Za-z0-9_]+)\)((.|\n)*?)@ENDIF", delegate (Match match) {
                if (data[match.Groups[1].Value] == null)
                    return "";
                if (data[match.Groups[1].Value] == "0")
                    return "";
                if (data[match.Groups[1].Value] == "false")
                    return "";

                return match.Groups[2].Value;
            });

            foreach (string key in data.Keys)
                str = str.Replace("%(" + key + ")", data[key]);

            return str;
        }

        public string exec(JSONObject obj) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            
            foreach(string key in obj.getDictionary().Keys) {
                data[key] = obj[key].ToString(); 
            }

            return this.exec(data);
        }

        public override string ToString() {
            return this.tmpl;
        }
    }
}
