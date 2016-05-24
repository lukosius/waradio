using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WARadio.Data;
using WARadio.Model;
using WMPLib;

namespace WARadio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly WindowsMediaPlayer wmp;
        private double OldVolume;

        private Station Current;

        private List<Country> ListOfCountries;
        private List<Station> ListOfStations;
        private List<Genre> ListOfGenres;

        public MainWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.StartMinimized)
            {
                WindowState = WindowState.Minimized;
            }

            wmp = new WindowsMediaPlayer();
            wmp.PlayStateChange += WmpOnPlayStateChange;
            wmp.MediaChange += WmpOnMediaChange;

            SliderVolume.Value = wmp.settings.volume;

            RoutedCommand rc = new RoutedCommand();
            rc.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(rc, ClickOnPlay));

            rc = new RoutedCommand();
            rc.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(rc, Mute));

            rc = new RoutedCommand();
            rc.InputGestures.Add(new KeyGesture(Key.Add, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(rc, VolumeUp));

            rc = new RoutedCommand();
            rc.InputGestures.Add(new KeyGesture(Key.Subtract, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(rc, VolumeDown));

            rc = new RoutedCommand();
            rc.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(rc, SearchStation));

            Current = new StationRepository().GetStation(Properties.Settings.Default.LastStation);

            if (Current != null)
            {
                if (Properties.Settings.Default.AutoplayAfterStartup)
                {
                    PlayStation(Current);
                }
                else
                {
                    ShowStation(Current);
                }
            }

            lbCountries.ItemsSource = ListOfCountries = new CountryRepository().GetCountries();
            lbGenres.ItemsSource = ListOfGenres = new GenreRepository().GetGenres();
        }

        private void PlayStationById(int id)
        {
            Station s = new StationRepository().GetStation(id);

            if (s != null)
            {
                PlayStation(s);
            }
        }

        private void ShowStation(Station s)
        {
            gWelcome.Visibility = Visibility.Hidden;
            dgStations.Visibility = Visibility.Hidden;
            gStation.Visibility = Visibility.Visible;

            if (!String.IsNullOrEmpty(Current.image))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Current.image, UriKind.Absolute);
                bitmap.EndInit();

                StationImage.Source = bitmap;
            }
            else
            {
                StationImage.Source = new BitmapImage(new Uri("pack://application:,,,/WARadio;component/Resources/nologo.png", UriKind.Absolute));
            }

            StationName.Content = Current.name;

            if (Properties.Settings.Default.FavoriteStations != null && Properties.Settings.Default.FavoriteStations.Contains(Current.id.ToString()))
            {
                ButtonFavorite.Content = FindResource("material_favorite");
            }
            else
            {
                ButtonFavorite.Content = FindResource("material_unfavorite");
            }
        }

        private void PlayStation(Station s)
        {
            Current = s;
            Properties.Settings.Default.LastStation = Current.id;

            ShowStation(Current);

            TogglePlay((new StationRepository()).GetStream(Current.id));
        }

        private void ClickOnPlay(object sender, RoutedEventArgs e)
        {
            if (dgStations.SelectedItem != null)
            {
                Station dgs = dgStations.SelectedItem as Station;

                PlayStation(dgs);
            }
            else
            {
                PlayStation(Current);
            }
        }

        private void TogglePlay(Stream s)
        {
            if (wmp.playState == WMPPlayState.wmppsPlaying)
            {
                ButtonPlay.Content = FindResource("material_play");
                wmp.controls.stop();
            }
            else
            {
                if (s != null && !String.IsNullOrEmpty(s.stream))
                {
                    ButtonPlay.Content = FindResource("material_stop");

                    wmp.URL = s.stream;
                    wmp.controls.play();
                }
            }
        }

        private void lbCountriesFilterOnTextChange(object sender, TextChangedEventArgs e)
        {
            string lower = lbCountriesFilter.Text.ToLower();

            var filtered = from c in ListOfCountries
                           let cname = c.name.ToLower()
                           let ccode = c.country_code.ToLower()
                           let cregion = c.region.ToLower()
                           where
                            ccode.StartsWith(lower) || ccode.Contains(lower) ||
                            cname.StartsWith(lower) || cname.Contains(lower) ||
                            cregion.StartsWith(lower) || cregion.Contains(lower)
                           select c;

            lbCountries.ItemsSource = filtered;
        }

        private void lbCountriesOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbCountries.SelectedItem != null)
            {
                Country ct = lbCountries.SelectedItem as Country;

                gStation.Visibility = Visibility.Hidden;
                gWelcome.Visibility = Visibility.Hidden;
                dgStations.Visibility = Visibility.Visible;

                FlyoutCountries.IsOpen = false;

                StatusbarMore.Content = "Country: " + ct.country_code;
                dgStations.ItemsSource = ListOfStations = new StationRepository().GetStationsByCountry(ct.country_code);
            }
        }

        private void lbGenresFilterOnTextChange(object sender, TextChangedEventArgs e)
        {
            string lower = lbGenresFilter.Text.ToLower();

            var filtered = from c in ListOfGenres
                           let title = c.title.ToLower()
                           where
                            title.StartsWith(lower) || title.Contains(lower) 
                           select c;

            lbGenres.ItemsSource = filtered;
        }

        private void lbGenresOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbGenres.SelectedItem != null)
            {
                Genre ct = lbGenres.SelectedItem as Genre;

                gStation.Visibility = Visibility.Hidden;
                gWelcome.Visibility = Visibility.Hidden;
                dgStations.Visibility = Visibility.Visible;

                FlyoutGenres.IsOpen = false;

                StatusbarMore.Content = "Genre: " + ct.title;
                dgStations.ItemsSource = ListOfStations = new StationRepository().GetStationsByGenre(ct.id);
            }
        }

        private async void SearchStation(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowInputAsync("Looking for station?", "Enter station:");

            if (result == null)
                return;

            gStation.Visibility = Visibility.Hidden;
            gWelcome.Visibility = Visibility.Hidden;
            dgStations.Visibility = Visibility.Visible;

            StatusbarMore.Content = "Search list";
            dgStations.ItemsSource = ListOfStations = new StationRepository().GetStationsByName(result);
        }

        private void dgStationsOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;

                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow row = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

                    Station info = row.Item as Station;

                    PlayStation(info);
                }
            }
        }

        private void ButtonPreviousOnClick(object sender, RoutedEventArgs e)
        {
            if (dgStations.SelectedIndex > 0)
            {
                dgStations.SelectedIndex = dgStations.SelectedIndex - 1;

                if (dgStations.SelectedItem != null)
                {
                    Station dgs = dgStations.SelectedItem as Station;

                    StatusbarMore.Content = "Playing previous station";

                    PlayStation(dgs);
                }
            }
            else
            {
                StatusbarMore.Content = "Cannot play previous station";
            }
        }

        private void ButtonNextOnClick(object sender, RoutedEventArgs e)
        {
            if (dgStations.SelectedIndex < dgStations.Items.Count - 1)
            {
                dgStations.SelectedIndex = dgStations.SelectedIndex + 1;

                if (dgStations.SelectedItem != null)
                {
                    Station dgs = dgStations.SelectedItem as Station;

                    StatusbarMore.Content = "Playing next station";

                    PlayStation(dgs);
                }
            }
            else
            {
                StatusbarMore.Content = "Cannot play next station";
            }
        }

        private void SliderOnVolumeChange(object sender, RoutedEventArgs e)
        {
            if (SliderVolume.Value >= 80)
            {
                ButtonVolume.Content = FindResource("material_volume_2");
            }
            else if (SliderVolume.Value > 30)
            {
                ButtonVolume.Content = FindResource("material_volume_1");
            }
            else if (SliderVolume.Value > 0)
            {
                ButtonVolume.Content = FindResource("material_volume_0");
            }
            else
            {
                ButtonVolume.Content = FindResource("material_volume_mute");
            }

            StatusbarMore.Content = "Volume changed";

            wmp.settings.volume = (int)SliderVolume.Value;
        }

        private void Mute(object sender, RoutedEventArgs e)
        {
            if (SliderVolume.Value > 0)
            {
                OldVolume = SliderVolume.Value;
                SliderVolume.Value = 0;
            }
            else
            {
                SliderVolume.Value = OldVolume;
            }
        }

        private void VolumeUp(object sender, RoutedEventArgs e)
        {
            SliderVolume.Value = (SliderVolume.Value + 10 > 100) ? 100 : SliderVolume.Value + 10;
        }

        private void VolumeDown(object sender, RoutedEventArgs e)
        {
            SliderVolume.Value = (SliderVolume.Value - 10 < 0) ? 0 : SliderVolume.Value - 10;
        }

        private void WmpOnMediaChange(object item)
        {
            StatusbarTitle.Content = string.Format("Playing: {0}", wmp.controls.currentItem.sourceURL.Trim());
        }

        private void WmpOnPlayStateChange(int state)
        {
            switch (state)
            {
                case 0:
                    StatusbarTitle.Content = "Undefined";
                    break;

                case 1:
                    StatusbarTitle.Content = "Stopped";
                    break;

                case 2:
                    StatusbarTitle.Content = "Paused";
                    break;

                case 3:
                    StatusbarTitle.Content = "Playing";
                    break;

                case 4:
                    StatusbarTitle.Content = "ScanForward";
                    break;

                case 5:
                    StatusbarTitle.Content = "ScanReverse";
                    break;

                case 6:
                    StatusbarTitle.Content = "Buffering";
                    break;

                case 7:
                    StatusbarTitle.Content = "Waiting";
                    break;

                case 8:
                    StatusbarTitle.Content = "MediaEnded";
                    break;

                case 9:
                    StatusbarTitle.Content = "Transitioning";
                    break;

                case 10:
                    StatusbarTitle.Content = "Ready";
                    break;

                case 11:
                    StatusbarTitle.Content = "Reconnecting";
                    break;

                case 12:
                    StatusbarTitle.Content = "Last";
                    break;

                default:
                    StatusbarTitle.Content = ("Unknown State: " + state.ToString());
                    break;
            }
        }

        private void MainWindowOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (wmp.playState == WMPPlayState.wmppsPlaying)
            {
                Properties.Settings.Default.Save();
                wmp.controls.stop();
            }
        }

        private void ClickOnSettings(object sender, RoutedEventArgs e)
        {
            MetroWindow s = new SettingsWindow();
            s.ShowDialog();
        }

        private void dgStationsOnAutoColumsn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "id":
                    e.Column.Header = "ID";
                    break;

                case "name":
                    e.Column.Header = "Station name";
                    break;

                default:
                    e.Cancel = true;
                    break;
            }
        }

        private void ButtonFavoriteOnClick(object sender, RoutedEventArgs e)
        {
            string cid = Current.id.ToString();

            if (Properties.Settings.Default.FavoriteStations != null && Properties.Settings.Default.FavoriteStations.Contains(cid))
            {
                ButtonFavorite.Content = FindResource("material_unfavorite");

                StatusbarMore.Content = "Removed from favorites";
                Properties.Settings.Default.FavoriteStations.Remove(cid);
            }
            else
            {
                ButtonFavorite.Content = FindResource("material_favorite");

                StatusbarMore.Content = "Added to favorites";
                Properties.Settings.Default.FavoriteStations.Add(cid);
            }

            Properties.Settings.Default.Save();
        }

        private async void ListFavoritesButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.FavoriteStations != null && Properties.Settings.Default.FavoriteStations.Count > 0)
            {
                gWelcome.Visibility = Visibility.Hidden;
                gStation.Visibility = Visibility.Hidden;
                dgStations.Visibility = Visibility.Visible;

                List<Station> favs = new List<Station>();
                StationRepository repo = new StationRepository();

                foreach (string sid in Properties.Settings.Default.FavoriteStations)
                {
                    Station sta = repo.GetStation(Int32.Parse(sid));
                    
                    if (sta != null)
                    {
                        favs.Add(sta);
                    }
                }

                StatusbarMore.Content = "The list of favorites";
                dgStations.ItemsSource = ListOfStations = favs;
            }
            else
            {
                await this.ShowMessageAsync("Information", "You have not favorited any stations!");
            }
        }
    }
}
