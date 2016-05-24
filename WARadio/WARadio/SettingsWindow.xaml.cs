using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Reflection;

namespace WARadio
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            TitleLabel.Content = AssemblyTitle;
            CopyrightLabel.Content = AssemblyCopyright;
            DescriptionBox.Text = AssemblyDescription;

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                string AppName = Assembly.GetEntryAssembly().GetName().Name;

                if (rk.GetValue(AppName) == null)
                {
                    Properties.Settings.Default.StartWithWindows = false;
                    Properties.Settings.Default.Save();
                }
            }

            SettingStartWithWindows.IsChecked = Properties.Settings.Default.StartWithWindows;
            SettingStartMinimized.IsChecked = Properties.Settings.Default.StartMinimized;
            SettingAutoplay.IsChecked = Properties.Settings.Default.AutoplayAfterStartup;
        }

        private void SettingsOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Properties.Settings.Default.StartWithWindows != SettingStartWithWindows.IsChecked)
            {
                Properties.Settings.Default.StartWithWindows = (bool)SettingStartWithWindows.IsChecked;

                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    string AppName = Assembly.GetEntryAssembly().GetName().Name;

                    if (Properties.Settings.Default.StartWithWindows)
                    {
                        rk.SetValue(AppName, Assembly.GetEntryAssembly().Location);
                    }
                    else
                    {
                        rk.DeleteValue(AppName);
                    }
                }
            }

            Properties.Settings.Default.StartMinimized = (bool)SettingStartMinimized.IsChecked;
            Properties.Settings.Default.AutoplayAfterStartup = (bool)SettingAutoplay.IsChecked;
            Properties.Settings.Default.Save();
        }

        #region Assembly Attribute Accessors
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        #endregion
    }
}
