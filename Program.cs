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

        public static Thread[] runningWatchers;

        public static bool WatchersRunning{
            get {
                foreach (Thread t in runningWatchers)
                    if (t.IsAlive) return true;
                return false;
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0 || (args.Length & 1)==1)
            {
                Usage();
                return;
            }

            runningWatchers = new Thread[args.Length / 2];

            for(int i=0;i<args.Length/2;i++){
                StartAndWatch(args[i*2], args[i*2+1], ref runningWatchers[i]);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SysTrayContext stContext = new SysTrayContext();
            Application.Run(stContext);

        }

        static void Usage()
        {
            MessageBox.Show(
                "usage:\n\nforever <application1> \"<arguments1>\" [<application2> \"<arguments2>\"] ..",
                "forever usage",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1                
            );
        }

        static void StartAndWatch(string fileName, string fileArgs, ref Thread thread)
        {
            thread = new Thread(delegate()
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = fileArgs
                };

                int runsPerSecond = 0;
                var rpsStowwatch = Stopwatch.StartNew();

                while (true)
                {
                    if (stop)
                    {
                        break;
                    }
                    if (runsPerSecond > 10)
                    {
                        if (MessageBox.Show(
                            String.Format("The process '{0}' with the arguments '{1}' got restarted more than 10 times in one second.\n"+
                            "This tends to be a unwanted behavior.\n\nIf you press cancel the other processes will continue to be watched.",fileName,fileArgs),
                            "something is not right",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Error) == DialogResult.Retry)
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                    try
                    {
                        if (System.Environment.HasShutdownStarted) break;
                        if (rpsStowwatch.ElapsedMilliseconds > 1000)
                        {
                            rpsStowwatch.Reset();
                            runsPerSecond = 0;
                        }
                        runsPerSecond++;
                        var p = Process.Start(psi);
                        p.WaitForExit();
                    }
                    catch(Exception x)
                    {
                        MessageBox.Show(
                            String.Format("Error while running arguments.\n\nError:\n  {0}\nFilename:\n  {1}\nArguments:\n  {2}",
                                x.Message, fileName, fileArgs),
                            "Error while running arguments",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            });
            thread.Start();
        }
    }
}
