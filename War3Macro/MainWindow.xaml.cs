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
        System.Windows.Forms.NotifyIcon notificationIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            this.Icon = BitmapFrame.Create(new Uri("phoenician_g.ico", UriKind.Relative));

            notificationIcon.Icon = new System.Drawing.Icon("phoenician_g.ico");
            notificationIcon.Visible = true;
            notificationIcon.DoubleClick += delegate(object sender, EventArgs e)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            foreach (var something in tabControl1.Items)
            {
				foreach (TextBox textbox in ((StackPanel)((TabItem)something).Content).Children)
				{
					textbox.Text = (string) Properties.Settings.Default[textbox.Name];
				}
            }

			hotkeyregistration = new KeyboardHook();
			hotkeyregistration.KeyDown += KeyPressHandler;
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.StateChanged += delegate(object sender, EventArgs e)
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.Hide();
                }
            };

        }

		void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.Save();
            notificationIcon.Visible = false;
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
				case System.Windows.Forms.Keys.F6:
					return F6Tab;
				case System.Windows.Forms.Keys.F7:
					return F7Tab;
				case System.Windows.Forms.Keys.F8:
					return F8Tab;
				default: return null;
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
