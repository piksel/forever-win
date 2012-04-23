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
            if (args.Length == 0 || (args.Length & 1)==1)
            {
                Usage();
                return;
            }

            string[] fileName = new string[args.Length / 2];
            string[] fileArgs = new string[args.Length / 2];

            for(int i=0;i<args.Length/2;i++){
                //fileArgs[i] = args[i*2+1];
                //fileName[i] = args[i*2];
                StartAndWatch(args[i*2], args[i*2+1]);
            }

            //Console.ReadLine(); return;

            //StartAndWatch(fileName[0], fileArgs[0]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SysTrayContext stContext = new SysTrayContext();
            Application.Run(stContext);

        }

        static void Usage()
        {
            MessageBox.Show(
                "usage:\n\nforever <application> \"<arguments>\" ",
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
