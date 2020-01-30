using Nii.JSON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ItemLookup {
    public partial class LookupWin : Form {

        private JSONObject prodData;
        private Bitmap prodImage = new Bitmap(0x4b, 0x4b);


        public LookupWin() {
            this.InitializeComponent();

            this.txtLookup.KeyPress += new KeyPressEventHandler(this.txtLookup_KeyPress);
            this.binList.DrawItem += new DrawItemEventHandler(this.binList_DrawItem);


            this.picItem.SizeMode = PictureBoxSizeMode.Zoom;

            this.ResetControls();
        }

        private void btnLookup_Click(object sender, EventArgs e) {
            this.ResetControls();
            this.txtLookup.SelectAll();
            this.txtLookup.Focus();

            if(this.txtLookup.Text.Length != 0) {
                JSONObject obj = LookupController.Lookup(this.txtLookup.Text);
                
                if(obj == null) {
                    this.txtName.Text = "PRODUCT NOT FOUND";
                    this.txtName.ForeColor = Color.Red;
                    return;
                }

                this.txtSKU.Text = obj.getString("sku");
                this.txtName.Text = obj.getString("name");
                this.txtDesc.Text = obj.getString("desc");
                this.txtMfg.Text = obj.getString("mfg");
                this.txtUPC1.Text = obj.getString("upc1");
                this.txtUPC2.Text = obj.getString("upc2");
                this.txtUPC3.Text = obj.getString("upc3");
                this.binList.Items.Add(obj.getString("bin"));
                foreach (object o in obj.getJSONArray("bins").List)
                    if ((string)o != obj.getString("bin"))
                        this.binList.Items.Add((string)o);
                this.txtQOH.Text = obj.getInt("qoh").ToString();
                this.txtMult.Text = obj.getInt("mult").ToString();
                this.txtUOM.Text = obj.getString("uom");
                this.chkDisc.Checked = obj.getBool("disc");
                this.chkSell.Checked = obj.getBool("sell");

                // get image...
                this.picItem.LoadAsync("http://b2b.monroehardware.com/products/250/" + obj.getString("sku") + ".jpg");


                this.btnApply.Enabled = true;
                this.txtNewUPC.Enabled = true;
                this.btnReturns.Enabled = true;
                this.btnRecv.Enabled = true;
                this.btnBinSmall.Enabled = true;
                this.btnBinLarge.Enabled = true;
                this.btnDNB.Enabled = true;
                this.prodData = obj;
            }
        }


        private void ResetControls() {
            this.binList.Items.Clear();
            //this.binBind.Clear();
            this.txtDesc.Text = "";
            this.txtMfg.Text = "";
            this.txtMult.Text = "";
            this.txtName.Text = "";
            this.txtQOH.Text = "";
            this.txtSKU.Text = "00000";
            this.txtUOM.Text = "";
            this.txtUPC1.Text = "";
            this.txtUPC2.Text = "";
            this.txtUPC3.Text = "";
            this.chkDisc.Checked = false;
            this.chkSell.Checked = false;
            this.btnApply.Enabled = false;
            this.txtNewUPC.Text = "";
            this.txtNewUPC.Enabled = false;
            this.txtUPCStat.Text = "";
            this.btnReturns.Enabled = false;
            this.btnRecv.Enabled = false;
            this.btnBinSmall.Enabled = false;
            this.btnBinLarge.Enabled = false;
            this.btnDNB.Enabled = false;
            this.txtName.ForeColor = Color.Black;
            Graphics.FromImage(this.prodImage).FillRectangle(Brushes.White, 0, 0, 0x4b, 0x4b);
            this.picItem.Image = this.prodImage;
        }


        // handle enter key in lookup field
        private void txtLookup_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\r') this.btnLookup_Click(sender, e);
        }

        private void binList_DrawItem(object sender, DrawItemEventArgs e) {
            e.DrawBackground();
            Brush controlText = SystemBrushes.ControlText;
            Font font = e.Font;
            if (this.binList.Items[e.Index].ToString() == this.prodData.getString("bin")) {
                controlText = Brushes.Maroon;
                font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
                if ((e.State & DrawItemState.Selected) > DrawItemState.None)
                    controlText = SystemBrushes.HighlightText;
            } else if ((e.State & DrawItemState.Selected) > DrawItemState.None) {
                controlText = SystemBrushes.HighlightText;
            }
            e.Graphics.DrawString(this.binList.Items[e.Index].ToString(), font, controlText, e.Bounds);
        }



        private void btnApply_Click(object sender, EventArgs e) {
            string upc = this.txtNewUPC.Text;
            if(upc.Length != 12) {
                MessageBox.Show("Invalid UPC length. Expecting 12 digits.", "UPC Error");
                this.txtUPCStat.Text = "Invalid UPC length.";
                return;
            }

            if(!LookupController.validateUPC(upc)) {
                this.txtUPCStat.Text = "Invalid check digit.";
                DialogResult x = MessageBox.Show("Invalid check digit. Are you sure you want to use this?", "UPC Error", MessageBoxButtons.YesNo);
                if (x == DialogResult.No) return;
            }

            try { LookupController.updateUPC(this.prodData.getString("sku"), upc); }
            catch(Exception) { 
                this.txtUPCStat.Text = "UPC update failed.";
                return;
            }

            this.txtUPCStat.Text = "UPC updated!";
        }


        private void btnRecv_Click(object sender, EventArgs e) {
            LookupController.printLabel("receiving", this.prodData);
        }

        private void btnReturns_Click(object senver, EventArgs e) {
            LookupController.printLabel("returns", this.prodData);
        }

        private void btnBinLarge_Click(object sender, EventArgs e) {
            LookupController.printLabel("binlarge", this.prodData);
        }

        private void btnBinSmall_Click(object sender, EventArgs e) {
            LookupController.printLabel("binsmall", this.prodData);
        }

        private void btnDnb_Click(object sender, EventArgs e) {
            LookupController.printLabel("donotbreak", this.prodData);
        }
    }
}
