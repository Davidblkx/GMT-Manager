﻿using MusicBeePlugin.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;

namespace MusicBeePlugin.Core.Bot
{
    //TODO: Better log display window
    /// <summary>
    /// Interaction logic for Window_LaunchBot.xaml
    /// </summary>
    public partial class Window_LaunchBot : PluginWindow
    {
        private List<TrackFile> _files;
        private bool _isBotRunning;
        private GmtBot _bot;

        public bool IsBotRunning
        {
            get
            {
                return _isBotRunning;
            }
            private set
            {
                _isBotRunning = value;


                _tab_fileList.IsEnabled = !_isBotRunning;
                _tab_progress.IsEnabled = _isBotRunning;
                _tab_settings.IsEnabled = !_isBotRunning;

                _btn_startBot.Content = _isBotRunning ? "Cancel" : "Start Bot";
            }
        }

        public Window_LaunchBot()
        {
            InitializeComponent();
            _tabs.SelectedIndex = 1;
            _btn_startBot.Click += _btn_startBot_Click;
        }


        private void _btn_startBot_Click(object sender, RoutedEventArgs e)
        {
            if (_isBotRunning)
                _bot.CancelProgress();
            else
                StartBot();
        }

        private void StartBot()
        {
            IsBotRunning = true;

            var options = _settings.GetSettings();
            PluginSettings.LocalSettings.BotOptions = options;

            PluginSettings.LocalSettings.Save();

            RunBot(options);
        }

        private void RunBot(GmtBotOptions options)
        {
            _bot = new GmtBot(_files, options);

            _bot.OnProgress += Bot_OnProgress;
            _bot.OnComplete += Bot_OnComplete;

            IsBotRunning = true;

            _listView_progress.ItemsSource = _bot.Logger.LogEntries;
            _tabs.SelectedIndex = 2;

            _bot.Run(Dispatcher);
        }

        private void Bot_OnComplete(List<TrackFile> Files)
        {
            _bot.Logger.SaveToFile(CoreVars.GetFilePath(PluginSettings.Folder, CoreVars.BotLogFile));

            if(MessageBox.Show(
                $"Searching for tags completed!\n{Files.Where(x => x != null).Count()}" +
                " files need to be updated, this could take a while, please be patient",
                "Process Completed", MessageBoxButton.OK) == MessageBoxResult.OK)
            InvokeOnSave(this, Files);
            Close();
        }

        private void Bot_OnProgress(TrackFile current, int currentIndex, int total)
        {
            var counted = _progressBar.Value + currentIndex;
            _progressBar.Maximum = total;
            _progressBar.Value = counted;
            _textBlock_progress.Text = $"Files completed: {counted}/{total}";
        }


        public override void Reset()
        {
            _info?.SetTrackSource(new List<TrackFile>());
            IsBotRunning = false;
            _listView_progress.ItemsSource = null;
            _progressBar.Value = 0;
            _textBlock_progress.Text = "";
            _bot = null;
            _tabs.SelectedIndex = 1;
        }

        public override void Initialize(params object[] init_params)
        {
            if (init_params.Length == 0) return;

            var tracks = init_params[0] as List<TrackFile>;
            if (tracks == null) return;

            _info.SetTrackSource(tracks);
            _files = tracks;
        }

        protected override void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _bot?.CancelProgress();

            base.OnWindowClosing(sender, e);
        }
    }
}
