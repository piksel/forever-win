using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace forever
{
    class SysTrayContext: ApplicationContext
    {
        private System.ComponentModel.IContainer container;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem menuItemExit;
        private Timer timer;

        public SysTrayContext()
        {
            container = new System.ComponentModel.Container();
    
            notifyIcon = new NotifyIcon(this.container);
            notifyIcon.Icon = forever.icon;

            notifyIcon.Text = "Forever";
            notifyIcon.Visible = true;

            //Instantiate the context menu and items
            contextMenu = new ContextMenuStrip();
            menuItemExit = new ToolStripMenuItem();

            //Attach the menu to the notify icon
            notifyIcon.ContextMenuStrip = contextMenu;

            menuItemExit.Text = "Exit";
            menuItemExit.Click += new EventHandler(menuItemExit_Click);
            contextMenu.Items.Add(menuItemExit);

            timer = new Timer(this.container);
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!Program.WatchersRunning) ExitThreadCore();
        }

        void menuItemExit_Click(object sender, EventArgs e)
        {
            ExitThreadCore();
        }

        protected override void ExitThreadCore()
        {
            Program.stop = true;

            notifyIcon.Visible = false;

            base.ExitThreadCore();
        }
    }
}
