using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using MaterialDesignThemes.Wpf;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        private List<string> playlist = new List<string>();
        private int currentTrackIndex = -1;
        private bool isPlaying = false;
        private bool isRepeat = false;
        private bool isShuffle = false;

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan totalTime = mediaElement.NaturalDuration.TimeSpan;
                TimeSpan currentTime = mediaElement.Position;

                timeText.Text = $"{currentTime:mm\\:ss} / {totalTime:mm\\:ss}";
            }
        }

        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Audio files (*.mp3, *.wav, *.m4a)|*.mp3;*.wav;*.m4a|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                playlist.Clear();
                foreach (string file in openFileDialog.FileNames)
                {
                    playlist.Add(file);
                }

                currentTrackIndex = 0;
                PlayTrack();
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                mediaElement.Pause();
                isPlaying = false;
            }
            else
            {
                mediaElement.Play();
                isPlaying = true;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex > 0)
            {
                currentTrackIndex--;
                PlayTrack();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex < playlist.Count - 1)
            {
                currentTrackIndex++;
                PlayTrack();
            }
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            isRepeat = !isRepeat;
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            isShuffle = !isShuffle;
            if (isShuffle)
            {
                Random rng = new Random();
                playlist = playlist.OrderBy(x => rng.Next()).ToList();
            }
            else
            {
                playlist = playlist.OrderBy(x => x).ToList();
            }
            currentTrackIndex = 0;
            PlayTrack();
        }

        private void PlayTrack()
        {
            if (currentTrackIndex >= 0 && currentTrackIndex < playlist.Count)
            {
                mediaElement.Source = new Uri(playlist[currentTrackIndex]);
                mediaElement.Play();
                isPlaying = true;

                historyListBox.Items.Add(playlist[currentTrackIndex]);
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan totalTime = mediaElement.NaturalDuration.TimeSpan;
                mediaElement.Position = TimeSpan.FromSeconds(totalTime.TotalSeconds * positionSlider.Value);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = volumeSlider.Value;
        }

        private void HistoryListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (historyListBox.SelectedItem != null)
            {
                string selectedTrack = historyListBox.SelectedItem.ToString();
                currentTrackIndex = playlist.IndexOf(selectedTrack);
                PlayTrack();
            }
        }
        private bool isDarkTheme = false;

        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isDarkTheme)
            { 
                var darkTheme = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml")
                };

                var primaryColor = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml")
                };

                var accentColor = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Lime.xaml")
                };

                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(darkTheme);
                Application.Current.Resources.MergedDictionaries.Add(primaryColor);
                Application.Current.Resources.MergedDictionaries.Add(accentColor);

                isDarkTheme = true;
                DarkTheme.Content = "Light Theme"; 
            }
            else 
            {
                var lightTheme = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml")
                };

                var primaryColor = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml")
                };

                var accentColor = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Lime.xaml")
                };

                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(lightTheme);
                Application.Current.Resources.MergedDictionaries.Add(primaryColor);
                Application.Current.Resources.MergedDictionaries.Add(accentColor);

                isDarkTheme = false;
                DarkTheme.Content = "Dark Theme"; 
            }
        }
    }
}