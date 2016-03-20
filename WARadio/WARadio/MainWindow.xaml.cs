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

            // TODO: Remove after implementing other functions in app related with database
            Database db = new Database();

            if (db.Connection != null)
            {
                StatusTitle.Content = "Database";
                StatusDescription.Content = "Connected to database...";
            }
        }

        private void ClickSettings(object sender, RoutedEventArgs e)
        {
            MetroWindow s = new SettingsWindow();
            s.ShowDialog();
        }

        private void ClickFavorites(object sender, RoutedEventArgs e)
        {
            MetroWindow s = new FavoritesWindow();
            s.ShowDialog();
        }
    }
}
