using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Nii.JSON;


namespace ItemLookup {
    static class LookupController {
        static Progress p4gl;

        static LookupController() {
            p4gl = new Progress(Program.ProgressConfig);
        }

        public static JSONObject Lookup(string strLookup) {
            JSONObject obj = p4gl.call4GLJS("skuInfo.p", new string[] { "NFO", strLookup });
            if (obj == null) throw new Exception("Error calling progress: " + p4gl.Result);
            else if(!obj.getBool("success")) return null;
            return obj;
        }

        public static bool updateUPC(string sku, string upc) {
            JSONObject obj = p4gl.call4GLJS("skuInfo.p", new string[] { "UPC", sku, upc });
            if (obj == null) throw new Exception("Error calling progress: " + p4gl.Result);
            return obj.getBool("success");
        }

        public static bool validateUPC(string upc) {
            int num = short.Parse(upc.Substring(11, 1));
            int num2 = ((((short.Parse(upc.Substring(0, 1)) + short.Parse(upc.Substring(2, 1))) + short.Parse(upc.Substring(4, 1))) + short.Parse(upc.Substring(6, 1))) + short.Parse(upc.Substring(8, 1))) + short.Parse(upc.Substring(10, 1));
            int num3 = (((short.Parse(upc.Substring(1, 1)) + short.Parse(upc.Substring(3, 1))) + short.Parse(upc.Substring(5, 1))) + short.Parse(upc.Substring(7, 1))) + short.Parse(upc.Substring(9, 1));
            int num4 = (num2 * 3) + num3;
            int num5 = 0;

            while(num5 <= 9) {
                if ((num4 + num5) % 10 == 0)
                    break;
                num5++;
            }

            return (num5 == num);   
        }

        /*
        public static void printLabel(string format, Dictionary<string, string> data) {

        }
        */

        public static void printLabel(string format, JSONObject data) {

            if (!Program.Templates.ContainsKey(format)) {
                MessageBox.Show("No template is available for selected label.", "Missing Template");
                return;
            }

            if(!Program.Config.ContainsKey("printer."+format)) {
                MessageBox.Show("No printer is defined for selected label.", "Missing Printer");
                return;
            }

            string job = Program.Templates[format].exec(data);
            string addr = Program.Config["printer." + format];

            /*
            MessageBox.Show(data.ToString());
            MessageBox.Show(job);
            return;
            */

            sendPrintJob(addr, job);
        }

        private static void sendPrintJob(string addr, string job) {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(job);
            TcpClient client = new TcpClient();
            client.Connect(addr, 0x238c); // connect on port 9100
            client.GetStream().Write(data, 0, data.Length);
            client.Close();
        }
    }
}
