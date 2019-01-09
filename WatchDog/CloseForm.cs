using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WatchDog
{
    public partial class CloseForm : Form
    {

        public string Password;

        public CloseForm()
        {
            InitializeComponent();
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            Password = textBox1.Text;

            // load setting 
            if (Password.Equals(System.Configuration.ConfigurationManager.AppSettings["ClosePassword"]))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Wrong Password!");
            }
                
        }

    }
}
