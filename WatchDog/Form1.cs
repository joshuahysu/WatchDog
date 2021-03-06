﻿using System;
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
using System.Net;

namespace WatchDog
{
    public partial class Form1 : Form
    {
       static bool webDown,DBDown,APDown = false;

        public Form1()
        {
            InitializeComponent();

            APIP.Text += System.Configuration.ConfigurationManager.AppSettings["APServerIP"].ToString();
            DBIP.Text += System.Configuration.ConfigurationManager.AppSettings["webServerIP"].ToString().Split('D')[0];
            webIP.Text += System.Configuration.ConfigurationManager.AppSettings["webServerIP"].ToString().Split('D')[0];

        }
        int tryTimes = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!isRunning())
                {
                    serverState.Text = "Server Down";
                    serverState.ForeColor = System.Drawing.Color.Red;

                    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ISCloseProcess"]))
                    {
                        CloseProcessAndReStart();
                    }

                    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IStrxOpen"]) && int.Parse(System.Configuration.ConfigurationManager.AppSettings["tryTimes"]) < tryTimes)//tryTimes次不成功後send trx
                    {
                        SendTrx();
                    }
                    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ISSendEmail"]) && int.Parse(System.Configuration.ConfigurationManager.AppSettings["tryTimes"]) < tryTimes)//tryTimes次不成功後send trx
                    {
                        SendEmail();
                    }

                }
                else
                {
                    serverState.Text = "Server Running";
                    serverState.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                timer1.Stop();
                errorCode.Text = ex.ToString();
                //throw;
            }
        }

        private void SendEmail()
        {
            try
            {
                var sendEmailAddress = System.Configuration.ConfigurationManager.AppSettings["sendEmailAddress"].ToString().Split(',');
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("InSiteAuto@Amkor.com");
                foreach (var item in sendEmailAddress)
                {
                    mail.To.Add(item);
                }
                mail.Subject = "T5 eRack AP Server auto-restart fail";
                mail.Body = "(This is system auto notice don't reply directly to this account) \n";

                if (APDown)
                {
                    mail.Body +="T5 eRack AP Server IP:" +
                    System.Configuration.ConfigurationManager.AppSettings["APServerIP"].ToString()
                    + " \n" +
                    "Call IT to Restart Bluetooth BridgeServerMSSQL \n";
                }
                    //var client = new SmtpClient("smtp.gmail.com", 587)//port 587
                mail.Body +=webDown?"web Down please restart IP:"+webIP.Text+" \n":"";

                mail.Body += DBDown ? "DB Down please restart IP:" + webIP.Text+" \n": "";


                var client = new SmtpClient("notes.amkor.com", 25)
                {
                    //EnableSsl = true,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    // Credentials = new NetworkCredential("happyworkertwn@gmail.com", "5iTaiwan")
                };

                client.Send(mail);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 與(MES)交換訊息
        /// </summary>
        private void SendTrx()
        {
            try
            {
                timer1.Stop();
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
                timer1.Stop();
                myProp.Start();
                tryTimes = 0;
            }
            catch (Exception ex)
            {
                errorCode.Text = ex.ToString();
                tryTimes++;
            }
            finally
            {
                timer1.Start();
            }
            timer1.Stop();
        }
        /// <summary>
        /// 程式是否執行中
        /// AppSettings["timmer"] 設定確認秒數
        /// </summary>
        /// <returns></returns>
        public bool isRunning()
        {
            var serverIsRunning = true;
            #region 獨立功能 檢查網頁是否正常
            if (!CheckWebApi(System.Configuration.ConfigurationManager.AppSettings["webServerIP"]))//不成功回傳false
            {
                webState.Text = "Web Down";
                webState.ForeColor = System.Drawing.Color.Red;
                webDown = true;
            }
            else
            {
                webState.Text = "Web Running";
                webState.ForeColor = System.Drawing.Color.Green;
                webDown = false;
            }
            #endregion

            #region 獨立功能 檢查ap是否正常

            if (!CheckWebApi(System.Configuration.ConfigurationManager.AppSettings["APServerIP"] + @":9000/api/Check", "\"Alive\"", () => { AP.Text = "AP Running"; AP.ForeColor = System.Drawing.Color.Green; }, () => { AP.Text = "AP Down"; AP.ForeColor = System.Drawing.Color.Red; }))//不成功回傳false
            {
                serverIsRunning = false;
                APDown = true;
            }
            else
            {
                APDown = false;
            }
            #endregion

            #region 獨立功能 檢查DB是否正常
            try
            {
                Model1 Model1 = new Model1();
                var info = Model1.ServerInformations.FirstOrDefault();
                if (info != null && info.LastReport.HasValue)
                {
                    DBState.Text = "DB Running";
                    DBState.ForeColor = System.Drawing.Color.Green;
                    TimeSpan ts = DateTime.Now - info.LastReport.Value;
                    label2.Text = ts.ToString();
                    if (!(ts < new TimeSpan(0, 0, Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["timmer"]))))
                        serverIsRunning = false;
                }
                DBDown = false;
            }
            catch
            {
                DBState.Text = "DB Down";
                DBState.ForeColor = System.Drawing.Color.Red;
                DBDown = true;
            }
            #endregion

            return serverIsRunning;
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
                else if (responseBody.IndexOf("HTTP 404") > 0 || responseBody.IndexOf("denied") > 0 || responseBody.IndexOf("404 Not Found") > 0)
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
                errorCode.Text = DateTime.Now.ToString() + ex.ToString();
                return false;
            }
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ISCloseProcess"]))
            {
                CloseForm form = new CloseForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Environment.Exit(Environment.ExitCode);
                }
                e.Cancel = true;
            }
            return;
        }
        int sleepTimer = 0;
        private void startTimer1_Tick(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                sleepTimer++;
                if (sleepTimer == int.Parse(System.Configuration.ConfigurationManager.AppSettings["sleep"]))
                {
                    timer1.Start();
                    sleepTimer = 0;
                }
            }
        }
    }
}
