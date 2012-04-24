using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.IO;

namespace forever
{
    static class Program
    {

        public static volatile bool stop = false;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Drop a CSV file on the executable to launch. Each line should be formatted accordingly:\n" +
                                "<application>, <argument>,");
                return;
            }
            StreamReader sr = new StreamReader(@args[0]);
            string strline = "";
            string[] values = null;

            while (!sr.EndOfStream)
            {
                strline = sr.ReadLine();
                values = strline.Split(',');
                startKeepAlive(values[0], values[1]);
            }
            sr.Close();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SysTrayContext stContext = new SysTrayContext();
            Application.Run(stContext);

        }
        static void startKeepAlive(string fileName, string fileArgs)
        {
            new Thread(delegate()
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = fileArgs
                };

                while (true)
                {
                    if (stop)
                    {
                        break;
                    }
                    try
                    {
                        if (System.Environment.HasShutdownStarted) break;
                        var p = Process.Start(psi);
                        p.WaitForExit();
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(
                            String.Format("Error while running arguments.\n\nError:\n  {0}\nFilename:\n  {1}\nArguments:\n  {2}",
                                x.Message, fileName, fileArgs),
                            "Error while running arguments",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }).Start();
        }

    }



}
