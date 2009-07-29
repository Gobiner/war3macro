using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Windows.Interop;

namespace War3Macro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private KeyboardHook hotkeyregistration;

		private System.Windows.Forms.NotifyIcon TippuTrayNotify;
		private System.Windows.Forms.ContextMenuStrip ctxTrayMenu;
		private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private bool isAppExiting = false; 

        public MainWindow()
        {
            InitializeComponent();

            //this.Icon = BitmapFrame.Create(new Uri("app.bmp", UriKind.Relative));

            //Create an instance of the NotifyIcon Class
            TippuTrayNotify = new System.Windows.Forms.NotifyIcon();

            // This icon file needs to be in the bin folder of the application
            //TippuTrayNotify.Icon = new System.Drawing.Icon("Application.ico");

            //show the Tray Notify Icon
            TippuTrayNotify.Visible = true;

            //Create a object for the context menu
            ctxTrayMenu = new System.Windows.Forms.ContextMenuStrip();

            //Add the Menu Item to the context menu
            System.Windows.Forms.ToolStripMenuItem mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            mnuExit.Text = "Exit";
            mnuExit.Click += new EventHandler(mnuExit_Click);
            ctxTrayMenu.Items.Add(mnuExit);

            //Add the Context menu to the Notify Icon Object
            TippuTrayNotify.ContextMenuStrip = ctxTrayMenu;

            IsVisibleChanged += new DependencyPropertyChangedEventHandler(MainWindow_IsVisibleChanged);
            StateChanged += new EventHandler(MainWindow_StateChanged);


            foreach (var something in tabControl1.Items)
            {
				foreach (TextBox textbox in ((StackPanel)((TabItem)something).Content).Children)
				{
					textbox.Text = (string) Properties.Settings.Default[textbox.Name];
				}
            }

			this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
			hotkeyregistration = new KeyboardHook();
			hotkeyregistration.KeyDown += KeyPressHandler;
        }

        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                TippuTrayNotify.ShowBalloonTip(2000);
            }
        }

        void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TippuTrayNotify.Visible = !(bool)e.NewValue;
        }

        void mnuExit_Click(object sender, EventArgs e)
        {
            isAppExiting = true;
            this.Close();
            TippuTrayNotify.Visible = false;
        } 

		void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.Save();
            this.Hide();
            TippuTrayNotify.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            TippuTrayNotify.BalloonTipTitle = "Tippu Tray Notify";
            TippuTrayNotify.BalloonTipText = "Tippu Tray Notify has been minimized to the system tray. To open the application, double-click the icon in the system tray.";
            TippuTrayNotify.ShowBalloonTip(400);


		}

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			Properties.Settings.Default[((TextBox)e.Source).Name] = ((TextBox)e.Source).Text;
        }

		private void KeyPressHandler(object sender, HookEventArgs e)
		{
			if (ManagedWinapi.Windows.SystemWindow.ForegroundWindow.Title != "Warcraft III")
				return;
			var tab = GetTabFromFKey(e.Key);
			if (tab == null)
				return;
			var lines = GetLinesFromTab(tab);
			foreach (string line in lines)
			{
				if(!string.IsNullOrEmpty(line))
				{
					SendInputWrapper.SendString(line, e.Shift);
				}
			}
		}

		private TabItem GetTabFromFKey(System.Windows.Forms.Keys key)
		{
			switch (key)
			{
				case System.Windows.Forms.Keys.F5:
					return F5Tab;
					break;
				case System.Windows.Forms.Keys.F6:
					return F6Tab;
					break;
				case System.Windows.Forms.Keys.F7:
					return F7Tab;
					break;
				case System.Windows.Forms.Keys.F8:
					return F8Tab;
					break;
				default: return null; break;
			}
		}

		private string[] GetLinesFromTab(TabItem tab)
		{
			var ret = new List<string>();
			foreach (TextBox textbox in ((StackPanel)tab.Content).Children)
			{
				ret.Add(textbox.Text);
			}
			return ret.ToArray();
		}
    }
}
