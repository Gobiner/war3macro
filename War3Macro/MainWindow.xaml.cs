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
using System.Threading;

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
            this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,./Resources/phoenician_g.ico", UriKind.RelativeOrAbsolute));

            notificationIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,./Resources/phoenician_g.ico")).Stream);
            notificationIcon.Visible = true;
			notificationIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
			notificationIcon.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				new System.Windows.Forms.ToolStripMenuItem("Restore", null, delegate(object sender, EventArgs e)
				{
					this.Show();
               		this.WindowState = WindowState.Normal;
				}),
				new System.Windows.Forms.ToolStripMenuItem("Exit", null, delegate(object sender, EventArgs e)
				{
					this.Close();
				}),
			});
					
            notificationIcon.DoubleClick += delegate(object sender, EventArgs e)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

			foreach (TabItem tabItem in tabControl1.Items)
			{
				var textbox = (TextBox)((StackPanel)tabItem.Content).Children[0];
                textbox.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#Friz Quadrata TT");
				textbox.Text = Properties.Settings.Default[textbox.Name].ToString();
                var checkbox = (CheckBox)((StackPanel)tabItem.Content).Children[1];
                checkbox.IsChecked = (bool)Properties.Settings.Default[checkbox.Name];
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

         

        void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default[((CheckBox)e.Source).Name] = ((CheckBox)e.Source).IsChecked.Value;
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
			var tab = GetTabFromFKey(e.Key);
			if (tab == null)
				return;
            if (GetActiveWindowTitleWrapper.GetActiveWindowTitle() != "Warcraft III")
                return; 
            var lines = GetLinesFromTab(tab);
			foreach (string line in lines)
			{
				if(!string.IsNullOrEmpty(line))
				{
                    SendInputWrapper.SendString(line, e.Shift);
				}
                if (TabIsSlow(tab))
                {
                    var x = lines.Count();
                    var delay = Math.Max((int)(1000 * (Math.Pow(x, 1.4) - 4.65) / (Math.Pow(x, 1.4) + 1)), 0);
                    Thread.Sleep(delay);
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
			return ((TextBox)((StackPanel)tab.Content).Children[0]).Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		}

        private bool TabIsSlow(TabItem tab)
        {
            return ((CheckBox)((StackPanel)tab.Content).Children[1]).IsChecked.Value;
        }
    }
}
