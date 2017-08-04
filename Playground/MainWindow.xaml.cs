﻿namespace Playground
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using IO;
    using Playground.Core;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool isPlaying = false;
        private MediaElement mediaElement;
        private CommandInterpreter command;

        public MainWindow()
        {
            this.InitializeComponent();
            this.MediaElement = this.MediaPlayer;
            this.AudioSlider.Value = 1;
            MediaPlayer.MediaEnded += new RoutedEventHandler(LoopMediaEnded);
        }

        public MediaElement MediaElement
        {
            get { return this.mediaElement; }
            set { this.mediaElement = value; }
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            double value = slider.Value;
            this.MediaPlayer.Volume = value;
        }

        void LoopMediaEnded(object sender, RoutedEventArgs e)
        {
            command = new CommandInterpreter(mediaElement, Playlist);
            var incomingCommand = $"{((System.Windows.FrameworkElement)e.Source).Name}PlayBack";
            command.InterpretCommand(incomingCommand);
        }

        public void CommandProcessing(object sender, RoutedEventArgs e)
        {
            command = new CommandInterpreter(mediaElement, Playlist);
            var incomingCommand = ((System.Windows.FrameworkElement)e.Source).Name;
            command.InterpretCommand(incomingCommand);

        }

        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            command = new CommandInterpreter(mediaElement, Playlist);
            var incomingCommand = ((System.Windows.FrameworkElement)e.Source).Name;
            command.InterpretCommand(incomingCommand);
 
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position += TimeSpan.FromSeconds(5);
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            if (MediaElement.NaturalDuration.HasTimeSpan)
            {
                songStatus.Content = string.Format("{0} - {1}", MediaElement.Position.ToString(@"mm\:ss"), MediaElement.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
        }
    }
}