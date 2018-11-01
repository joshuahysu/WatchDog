using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchDog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!isRunning())
            {

            }
        }


        public static bool isRunning()
        {
            Model1 Model1 = new Model1();
            //ServerInfo info = db.Systems.FirstOrDefault();
            var info = Model1.ServerInformations.FirstOrDefault();
            if (info != null && info.LastReport.HasValue)
            {
                TimeSpan ts = DateTime.Now - info.LastReport.Value;
                if (ts < new TimeSpan(0, 0, 15))
                    return true;
            }

            return false;
        }
    }
}
