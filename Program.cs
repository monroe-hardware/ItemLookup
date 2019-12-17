using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ItemLookup {
    static class Program {

        public static ConfigFile Config;
        public static Dictionary<string, string> ProgressConfig;

        public static TemplateManager Templates;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {

            Config = new ConfigFile("itemLookup.cfg");

            ProgressConfig = new Dictionary<string, string>();
            foreach (string key in Config.Keys) {
                if (key.StartsWith("progress."))
                    ProgressConfig[key.Replace("progress.", "")] = Config[key];
            }

            Templates = new TemplateManager();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LookupWin());
        }
    }
}
