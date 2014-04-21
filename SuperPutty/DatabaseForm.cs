using SuperPutty.Manager;
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
    public partial class DatabaseForm : Form
    {

        public DatabaseForm()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            //button1.Enabled = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // create database
            string label = (string)textBox1.Text.Trim();
            string password = (string)textBox2.Text.Trim();
            string password2 = (string)textBox3.Text.Trim();
            if (label.Length < 5 || password.Length < 6 || password2.Length < 6)
            {
                MessageBox.Show("Fields are invalid (Label length sould be > 5 and passwords length sould be > 6");
                this.DialogResult = DialogResult.None;
                return;
            }
            if (password != password2)
            {
                MessageBox.Show("Not same passwords!");
                this.DialogResult = DialogResult.None;
                return;
            }

            string location = DatabaseManager.Instance.CreateDatabase(label, password);
            if (location != null)
            {
                MessageBox.Show("Database created " + location);
            }
            else
            {
                MessageBox.Show("Database creation failed!");
            }
        }


 
    }
}
