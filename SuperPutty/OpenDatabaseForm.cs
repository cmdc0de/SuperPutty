using SuperPuTTY.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperPuTTY
{
    public partial class OpenDatabaseForm : Form
    {
        public OpenDatabaseForm()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Database Files (*.db)|*.db|All files|*.*";
            // openDialog.FileName = "Sessions.XML";
            openDialog.CheckFileExists = true;
            openDialog.InitialDirectory = DatabaseManager.getFolderPath();
            if (openDialog.ShowDialog(this) == DialogResult.OK)
            {
                string password = passwordBox.Text;

                try { 
                    SuperPuTTY.OpenDatabase(openDialog.FileName, password);
                } catch (Exception ex)
                {
                    MessageBox.Show("Connexion failed !");
                    passwordBox.Text = "";
                    this.DialogResult = DialogResult.None;
                }
                this.DialogResult = DialogResult.OK;
                return;
            }
        }
    }
}
