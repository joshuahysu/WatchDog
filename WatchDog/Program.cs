using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchDog
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                var processInfo = new ProcessStartInfo();
                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.FileName = Application.ExecutablePath;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    // The user did not allow the application to run as administrator
                    MessageBox.Show("Sorry, this application must be run as Administrator.\n" + ex.Message);
                }
            }
            else
            {
                Application.ThreadException += Application_ThreadException;
                System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "Error");
            Environment.Exit(1);
        }
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Error" + e.Exception.Message + e.Exception.StackTrace);
            MessageBox.Show(e.Exception.Message + e.Exception.StackTrace);
        }
    }




}
