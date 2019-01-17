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
using System.Net.Http;
using System.Net.Mail;

namespace WatchDog
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        int tryTimes = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!isRunning())
                {
                    serverState.Text = "Server Stop";
                    serverState.ForeColor = System.Drawing.Color.Red;

                    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ISCloseProcess"]))
                    {
                        CloseProcessAndReStart();
                    }

                    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IStrxOpen"]) && int.Parse(System.Configuration.ConfigurationManager.AppSettings["tryTimes"]) < tryTimes)//tryTimes次不成功後send trx
                    {
                        SendTrx();
                        SendEmail();
                    }
                    timer1.Stop();
                }
                else
                {
                    serverState.Text = "Server Running";
                    serverState.ForeColor = System.Drawing.Color.Black;
                }
            }
            catch (Exception ex)
            {
                timer1.Stop();
                errorCode.Text = ex.StackTrace;
                //throw;
            }
        }

        private static void SendEmail()
        {
            MailMessage mail = new MailMessage("BridgeServerMSSQL@watchdog.com", "user@hotmail.com");
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            mail.Subject = "Bluetooth BridgeServerMSSQL Server down";
            mail.Body = "Call IT to Restart Bluetooth BridgeServerMSSQL";
            client.Send(mail);
        }

        /// <summary>
        /// 與(MES)交換訊息
        /// </summary>
        private void SendTrx()
        {
            try
            {
                var arr = "";
                var AMSWebService = new AMSWebService();
                string message = $"<TagBaseInfoRoot><ProcessType>ServerDown</ProcessType><ProcessItem>T5 Bump,OFFLINE,exit,{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}</ProcessItem></TagBaseInfoRoot>";

                var success = AMSWebService.UpdateBlutToothTagBaseInfo4ServerDown(message, ref arr);

                if (success == false)
                {
                    errorCode.Text = "AMSWebService false " + DateTime.Now.ToString();
                    success = AMSWebService.UpdateBlutToothTagBaseInfo4ServerDown(message, ref arr);
                    //Trace.WriteLine(string.Format("success = {0}, {1}, {2}", success, message, arr));
                }
                else
                {
                    errorCode.Text = "AMSWebService success " + DateTime.Now.ToString();
                    //Trace.WriteLine(string.Format("success = {0}, {1}", success, message));
                }

            }
            catch
            {
                errorCode.Text = "AMSWebService false " + DateTime.Now.ToString();
                throw;
            }
        }

        /// <summary>
        /// 關閉程式且重啟
        /// </summary>
        private void CloseProcessAndReStart()
        {
            try
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
                tryTimes = 0;
            }
            catch (Exception ex)
            {
                errorCode.Text = ex.ToString();
                tryTimes++;
            }
        }
        /// <summary>
        /// 程式是否執行中
        /// AppSettings["timmer"] 設定確認秒數
        /// </summary>
        /// <returns></returns>
        public bool isRunning()
        {
            #region 獨立功能 檢查網頁是否正常
            if (!CheckWebApi(System.Configuration.ConfigurationManager.AppSettings["webServerIP"]))//不成功回傳false
            {
                webState.Text = "Web Down";
                webState.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                webState.Text = "Web Running";
                webState.ForeColor = System.Drawing.Color.Black;
            }
            #endregion

            #region 獨立功能 檢查ap是否正常

            if (!CheckWebApi(System.Configuration.ConfigurationManager.AppSettings["APServerIP"], "\"Alive\"", () => { AP.Text = "AP Start"; }, () => { AP.Text = "AP Stop"; }))//不成功回傳false
            {
                return false;
            }
            #endregion

            #region 獨立功能 檢查DB是否正常
            Model1 Model1 = new Model1();
            var info = Model1.ServerInformations.FirstOrDefault();
            if (info != null && info.LastReport.HasValue)
            {
                TimeSpan ts = DateTime.Now - info.LastReport.Value;
                label2.Text = ts.ToString();
                if (ts < new TimeSpan(0, 0, Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["timmer"])))
                    return true;
            }
            #endregion

            return false;
        }
        /// <summary>
        /// 傳送web api確認是否回傳即回傳值是否正確
        /// </summary>
        /// <param name="url"></param>
        /// <param name="assert">responseBody 正確的回傳值</param>
        /// <returns></returns>
        private bool CheckWebApi(string url, string assert = null, Action assertSuccess = null, Action assertFail = null)
        {
            HttpClient client = new HttpClient();
            try
            {
                var response = client.GetAsync(url).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;
                if (assert != null)
                {
                    if (responseBody == assert)
                        assertSuccess?.Invoke();
                    else
                    {
                        assertFail?.Invoke();
                        return false;
                    }
                }
                else if (responseBody.IndexOf("HTTP 404") > 0)
                {
                    return false;
                }

            }
            //catch (System.AggregateException ex)
            //{
            //    errorCode.Text = ex.ToString();
            //    return false;
            //}
            catch (Exception ex)
            {
                assertFail?.Invoke();
                errorCode.Text = ex.ToString();
                return false;
            }
            return true;
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
            if (!timer1.Enabled)
            {
                sleepTimer++;
                if (sleepTimer == 120)
                {
                    timer1.Start();
                    sleepTimer = 0;
                }
            }
        }
    }
}
