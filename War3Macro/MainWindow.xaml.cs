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
			if (   e.Key == System.Windows.Forms.Keys.F5
				|| e.Key == System.Windows.Forms.Keys.F6 
				|| e.Key == System.Windows.Forms.Keys.F7
				|| e.Key == System.Windows.Forms.Keys.F8
				)//&& ManagedWinapi.Windows.SystemWindow.ForegroundWindow.Title == "Warcraft III"
			{
				SendInputWrapper.SendString(F5Line1.Text, false);
			}
		}
    }
}
