using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using Nii.JSON;

namespace ItemLookup {
    class Progress {

        // Configuration defaults
        private Dictionary<string, string> config = new Dictionary<string, string> {
            { "db", "apprise" },
            { "H", "192.168.1.130" },
            { "inp", "8096" },
            { "ld", "Apprise" },
            { "mmax", "10000" },
            { "N", "TCP" },
            { "P", "xyz" },
            { "s", "60" },
            { "S", "test-4GL" },
            { "T",  @"c:\temp" },
            { "U", "xyz" },
            { "dlc", @"d:\progress\" },
            { "ini", @"d:\apps\4gl\progress.ini" }
        };

        private string result;

        public Progress(Dictionary<string,string> dict) {
            if (dict != null) this.configure(dict);
        }

        public void configure(Dictionary<string,string> dict) {
            foreach (string key in dict.Keys)
                this.config[key] = dict[key];
        }

        public void configureJSON(JSONObject obj) {
            // just merge the incoming obj with the dictionary for now
            IDictionary dict = obj.getDictionary();
            foreach(string key in dict) {
                config[key] = dict[key].ToString();
            }
        }


        public string call4GL(string proc, string[] args) {
            this.Proc = proc;
            this.Param = string.Join("^", args);
            return this.Exec();
        }

        public JSONObject call4GLJS(string proc, string[] args) {
            string sJSON = this.call4GL(proc, args);
            JSONObject obj = null;
            try { obj = new JSONObject(sJSON); }
            catch (Exception) { }
            return obj;
        }

        public string Exec() {
            Process process = new Process();
            process.StartInfo.FileName = this.config["dlc"] + @"\bin\_progres.exe";
            process.StartInfo.EnvironmentVariables.Add("PROPATH", AppDomain.CurrentDomain.BaseDirectory + "4gl");

            string cmd = " -db " + this.config["db"] + " -ld " + this.config["ld"] + " -N " + this.config["N"] + " -H " + this.config["H"] + " -S " + this.config["S"];
            cmd += " -p \"" + AppDomain.CurrentDomain.BaseDirectory + @"4gl\" + this.config["p"] + "\"" + " -T " + this.config["T"] + " -mmax " + this.config["mmax"] + " -s " + this.config["s"] + " -inp " + this.config["inp"];
            if (this.config.ContainsKey("param") && this.config["param"] != null)
                cmd += " -param \"" + this.config["param"] + "\"";
            cmd += " -U " + this.config["U"] + " -P " + this.config["P"] + " -b";

            process.StartInfo.Arguments = cmd;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            StreamReader standardOutput = process.StandardOutput;
            string output = "";
            while (!process.HasExited) output += standardOutput.ReadToEnd();

            process.Close();
            this.result = output;
            return output;
        }


        public void setLogin(string user, string pass) {
            this.config["U"] = user;
            this.config["P"] = pass;
        }

        public string Proc {
            get { return this.config["p"]; }
            set { this.config["p"] = value; }
        }

        public string Param {
            get { return this.config["param"]; }
            set { this.config["param"] = value; }
        }

        public string Result {
            get { return this.result; }
        }

    }
}
