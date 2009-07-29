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

        public MainWindow()
        {
            InitializeComponent();

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

		void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.Save();
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
