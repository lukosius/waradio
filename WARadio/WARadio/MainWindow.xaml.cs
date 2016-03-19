using System.Windows;
using MahApps.Metro.Controls;

namespace WARadio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickSettings(object sender, RoutedEventArgs e)
        {
            MetroWindow s = new SettingsWindow();
            s.ShowDialog();
        }
    }
}
