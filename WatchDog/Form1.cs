using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WatchDog.WebReference;
using System.Diagnostics;
using System.Threading;

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
                label1.Text = "Server Stop";
                label1.ForeColor = System.Drawing.Color.Red;
                try
                {
                    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ISCloseProcess"]))
                    {
                        CloseProcessAndReStart();
                    }
                    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IStrxOpen"]))
                    {
                        SendTrx();
                    }

                    timer1.Stop();
                }
                catch (Exception ex)
                {
                    throw;
                    //  Trace.WriteLine(ex.StackTrace);
                    //  Trace.WriteLine(message);
                }
            }
            else
            {
                label1.Text = "Server Running";
                label1.ForeColor = System.Drawing.Color.Black;
            }

        }
        /// <summary>
        /// 與(MES)交換訊息
        /// </summary>
        private static void SendTrx()
        {
            var arr = "";
            var AMSWebService = new AMSWebService();
            string message = $"<TagBaseInfoRoot><ProcessType>ServerDown</ProcessType><ProcessItem>T5 Bump,OFFLINE,exit,{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}</ProcessItem></TagBaseInfoRoot>";

            var success = AMSWebService.UpdateBlutToothTagBaseInfo(message, ref arr);

            if (success == false)
            {
                success = AMSWebService.UpdateBlutToothTagBaseInfo(message, ref arr);
                // Trace.WriteLine(string.Format("success = {0}, {1}, {2}", success, message, arr));
            }
            else
            {
                //Trace.WriteLine(string.Format("success = {0}, {1}", success, message));
            }
        }

        /// <summary>
        /// 關閉程式且重啟
        /// </summary>
        private static void CloseProcessAndReStart()
        {
            Process[] proc = Process.GetProcessesByName("BridgeServerMSSQL");
            if (proc.Count() > 0)
            {
                proc[0].Kill();
            }
            Thread.Sleep(1000);
            var myProp = new Process();
            myProp.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["appLocation"];
            myProp.Start();
        }
        /// <summary>
        /// 程式是否執行中
        /// AppSettings["timmer"] 設定確認秒數
        /// </summary>
        /// <returns></returns>
        public bool isRunning()
        {
            Model1 Model1 = new Model1();
            var info = Model1.ServerInformations.FirstOrDefault();
            if (info != null && info.LastReport.HasValue)
            {
                TimeSpan ts = DateTime.Now - info.LastReport.Value;
                label2.Text = ts.ToString();
                if (ts < new TimeSpan(0, 0, Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["timmer"])))
                    return true;
            }
            return false;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseForm form = new CloseForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Environment.Exit(Environment.ExitCode);
            }
            return;
        }
        int sleepTimer = 0;
        private void startTimer1_Tick(object sender, EventArgs e)
        {
            if (!timer1.Enabled) {
                sleepTimer++;
                if (sleepTimer ==120) {
                    timer1.Start();
                    sleepTimer = 0;
                }
            }
        }
    }
}
