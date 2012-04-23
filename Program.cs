using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

namespace forever
{
    class Program
    {
        public static volatile bool stop = false;
        public static volatile bool done = false;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Usage();
                return;
            }

            string fileName = args[0];
            string fileArgs = "";
            for(int i=1;i<args.Length;i++)
                fileArgs += " "+args[i];

            StartAndWatch(fileName, fileArgs);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SysTrayContext stContext = new SysTrayContext();
            Application.Run(stContext);

        }

        static void Usage()
        {
            MessageBox.Show(
                "usage:\n\nforever <application>",
                "forever usage",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1                
            );
        }

        static void StartAndWatch(string fileName, string fileArgs)
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
                    if (stop) {
                        done = true;
                        break;
                    }
                    try
                    {
                        if (System.Environment.HasShutdownStarted) break;
                        var p = Process.Start(psi);
                        p.WaitForExit();
                    }
                    catch(Exception x)
                    {
                        MessageBox.Show(
                            String.Format("Error while running arguments.\n\nError:\n  {0}\nFilename:\n  {1}\nArguments:\n  {2}", x.Message, fileName, fileArgs),
                            "Error while running arguments",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }).Start();
        }
    }
}
