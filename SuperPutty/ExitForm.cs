using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty
{
    public partial class ExitForm : Form
    {
        public ExitForm()
        {
            InitializeComponent();

            CenterToScreen();
            MaximizeBox = false;
            MinimizeBox = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // quit with saving
            //this.DialogResult = DialogResult.Yes;
            //   this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // quit without saving
            //this.DialogResult = DialogResult.OK;
            //this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel;
            //this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
