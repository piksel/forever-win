using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace forever
{
    class SysTrayContext: ApplicationContext
    {
        private System.ComponentModel.IContainer mComponents;
        private NotifyIcon mNotifyIcon;
        private ContextMenuStrip mContextMenu;
        private ToolStripMenuItem mDisplayForm;
        private ToolStripMenuItem mExitApplication;
        private Program program;

        public SysTrayContext()
        {
            //Instantiate the component Module to hold everything
            mComponents = new System.ComponentModel.Container();
    
    
            //Instantiate the NotifyIcon attaching it to the components container and 
            //provide it an icon, note, you can imbed this resource 
            mNotifyIcon = new NotifyIcon(this.mComponents);
            mNotifyIcon.Icon = forever.icon;

            mNotifyIcon.Text = "Forever";
            mNotifyIcon.Visible = true;

            //Instantiate the context menu and items
            mContextMenu = new ContextMenuStrip();
            mDisplayForm = new ToolStripMenuItem();
            mExitApplication = new ToolStripMenuItem();

            //Attach the menu to the notify icon
            mNotifyIcon.ContextMenuStrip = mContextMenu;

            mExitApplication.Text = "Exit";
            mExitApplication.Click += new EventHandler(mExitApplication_Click);
            mContextMenu.Items.Add(mExitApplication);
        }

        void mExitApplication_Click(object sender, EventArgs e)
        {
            //Call our overridden exit thread core method!
            ExitThreadCore();
        }

        protected override void ExitThreadCore()
        {
            //Clean up any references needed
            //At this time we do not have any
            //
            Program.stop = true;
            //while(Program.done

            //Call the base method to exit the application
            base.ExitThreadCore();
        }
    }
}
