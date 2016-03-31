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

        private void ClickFavorites(object sender, RoutedEventArgs e)
        {
            MetroWindow s = new FavoritesWindow();
            s.ShowDialog();
        }

        private void ClickPlay(object sender, RoutedEventArgs e)
        {
            if (ButtonPlay.Tag.Equals("play"))
            {
                ButtonPlay.Content = FindResource("stop");
                ButtonPlay.Tag = "stop";
            }
            else
            {
                ButtonPlay.Content = FindResource("play");
                ButtonPlay.Tag = "play";
            }
        }

        private void ClickButtonFavorites(object sender, RoutedEventArgs e)
        {
            if (ButtonFavorites.Tag.Equals("favorite"))
            {
                ButtonFavorites.Content = FindResource("unfavorite");
                ButtonFavorites.Tag = "unfavorite";
            }
            else
            {
                ButtonFavorites.Content = FindResource("favorite");
                ButtonFavorites.Tag = "favorite";
            }
        }

        private void SliderVolumeChanged(object sender, RoutedEventArgs e)
        {
            if (SliderVolume.Value >= 100)
            {
                ButtonMute.Content = FindResource("volume_3");
            }
            else if (SliderVolume.Value >= 50)
            {
                ButtonMute.Content = FindResource("volume_2");
            }
            else if (SliderVolume.Value > 1)
            {
                ButtonMute.Content = FindResource("volume_1");
            }
            else if (SliderVolume.Value > 0)
            {
                ButtonMute.Content = FindResource("volume_0");
            }
            else
            {
                ButtonMute.Content = FindResource("volume_mute");
            }
        }
    }
}
