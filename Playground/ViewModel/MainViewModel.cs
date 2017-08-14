﻿namespace Playground.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Model;
    using NAudio.Wave;
    using Playground.Enums;
    using Playground.IO.Command;
    using Playground.Utility;

    public class MainViewModel
    {
        private MediaElement mediaElement;
        private Song currentSong;

        public MainViewModel(MediaElement mediaElement, ListBox listBox)
        {
            this.Playlist = new ObservableCollection<Song>();
            this.MediaElement = mediaElement;
            this.ListBox = listBox;
            this.LoadCommands();
        }

        public bool Repeat { get; set; }

        public ICommand ForwardCommand { get; set; }

        public ICommand StopCommand { get; set; }

        public ICommand PlayCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand ExitCommand { get; set; }

        public ICommand RewindCommand { get; set; }

        public ICommand FullScreenCommand { get; set; }

        public ListBox ListBox { get; set; }

        public ObservableCollection<Song> Playlist { get; set; }

        public Song CurrentSong
        {
            get
            {
                return this.currentSong;
            }
            set
            {
                this.currentSong = value;
            }
        }

        public MediaElement MediaElement
        {
            get { return this.mediaElement; }
            set { this.mediaElement = value; }
        }

        public void PlaySong(object obj)
        {
            this.MediaElement.Source = new Uri(this.currentSong.Path);
            this.MediaElement.Play();
        }

        private void LoadCommands()
        {
            this.PlayCommand = new CustomCommand(this.PlaySong, this.CanPlaySong);
            this.OpenCommand = new CustomCommand(this.LoadNewSong, this.CanLoadNewSong);
            this.ExitCommand = new CustomCommand(this.CloseApp, this.CanCloseApp);
            this.StopCommand = new CustomCommand(this.StopSong, this.CanStopSong);
            this.RewindCommand = new CustomCommand(this.RewindLoop, this.CanRewindLoop);
            this.ForwardCommand = new CustomCommand(this.ForwardLoop, this.CanForwardLoop);
            this.FullScreenCommand = new CustomCommand(this.FullScreen, this.CanFullScreen);
        }

        private bool CanFullScreen(object obj)
        {
            return true;
        }

        private void FullScreen(object obj)
        {
            var currentWin = obj as Window;
            currentWin.WindowStyle = WindowStyle.None;
            this.MediaElement.MaxWidth = SystemParameters.PrimaryScreenWidth;
            this.MediaElement.MaxHeight = SystemParameters.PrimaryScreenHeight;
            this.MediaElement.Width = SystemParameters.PrimaryScreenWidth;
            this.MediaElement.Height = SystemParameters.PrimaryScreenHeight;
            currentWin.Width = SystemParameters.PrimaryScreenWidth;
            currentWin.Height = SystemParameters.PrimaryScreenHeight;
            currentWin.WindowState = WindowState.Maximized;
            this.MediaElement.Margin = new Thickness(0, 0, 0, 0);
        }

        private bool CanForwardLoop(object obj)
        {
            return (this.ListBox.SelectedIndex < this.Playlist.Count - 1 || this.Repeat) && this.Playlist.Count != 0;
        }

        private void ForwardLoop(object obj)
        {
            if (this.ListBox.SelectedIndex == this.Playlist.Count - 1 && this.Repeat)
            {
                this.ListBox.SelectedIndex = 0;
            }
            else
            {
                this.ListBox.SelectedIndex++;
            }
        }

        private bool CanRewindLoop(object obj)
        {
            return (this.ListBox.SelectedIndex != 0 || this.Repeat) && this.Playlist.Count > 0;
        }

        private void RewindLoop(object obj)
        {
            if (this.ListBox.SelectedIndex == 0 && this.Repeat)
            {
                this.ListBox.SelectedIndex = this.Playlist.Count - 1;
            }
            else
            {
                this.ListBox.SelectedIndex--;
            }
        }

        private bool CanStopSong(object obj)
        {
            return true;
        }

        private void StopSong(object obj)
        {
            this.MediaElement.Stop();
        }

        private bool CanCloseApp(object obj)
        {
            if (Application.Current != null)
            {
                return true;
            }

            return false;
        }

        private void CloseApp(object obj)
        {
            Application.Current.Shutdown();
        }

        private bool CanLoadNewSong(object obj)
        {
            return true;
        }

        private void LoadNewSong(object obj)
        {
            bool firstLoad = false;
            if (this.Playlist.Count == 0)
            {
                firstLoad = true;
            }

            OpenCommand open = new OpenCommand(this.Playlist, this.currentSong);
            open.Execute();
            if (firstLoad && this.Playlist.Count > 0)
            {
                this.CurrentSong = this.Playlist[0];
                this.ListBox.SelectedIndex = 0;
                this.mediaElement.Pause();
            }
        }

        private bool CanPlaySong(object obj)
        {
            return this.Playlist.Count > 0;
        }

        private string AudioFormater()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("All Supported Audio | ");
            foreach (MediaFormats type in Enum.GetValues(typeof(MediaFormats)))
            {
                sb.Append($"*.{type}; ");
            }

            foreach (MediaFormats type in Enum.GetValues(typeof(MediaFormats)))
            {
                sb.Append($"|{type}s |*.{type}");
            }

            return sb.ToString().Trim();
        }

        private TimeSpan GetSongDurationInSeconds(string filePath)
        {
            MediaFoundationReader audioReader = new MediaFoundationReader(filePath);

            return audioReader.TotalTime;
        }
    }
}