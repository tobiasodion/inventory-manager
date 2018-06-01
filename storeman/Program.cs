using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace storeman
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                int timeoutMilliseconds = 5000;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                ServiceController myService = new ServiceController();
                myService.ServiceName = "MSSQLServer";
                string svcStatus = myService.Status.ToString();

                if (svcStatus == "Running")
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new LoginForm());
                }

                else if (svcStatus == "Stopped")
                {
                    myService.Start();
                    myService.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new LoginForm());
                }

                else
                {
                    myService.Stop();
                }
            }

            catch (Exception eX)
            {
                MessageBox.Show("Oops! Something went wrong. Try starting App as ADMIN " + eX.Message);
            }
           
        }
    }
}
