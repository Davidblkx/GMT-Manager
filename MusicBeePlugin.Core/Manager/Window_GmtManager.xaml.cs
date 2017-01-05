using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;
using AllMusicApi;

namespace MusicBeePlugin.Core.Manager
{
    /// <summary>
    /// Interaction logic for Window_GmtManager.xaml
    /// </summary>
    public partial class Window_GmtManager : Window
    {
        private bool _cancelClose = true;
        private List<TrackFile> _files;

        public Window_GmtManager()
        {
            InitializeComponent();
            Closing += HandleWindowClose;

            _manager_genres.SetSelectionSource(PluginSettings.LocalSettings.Genres);
            _manager_moods.SetSelectionSource(PluginSettings.LocalSettings.Moods);
            _manager_themes.SetSelectionSource(PluginSettings.LocalSettings.Themes);

            _webImport.OnImport += _webImport_OnImport;
            _btn_save_metadata.Click += _btn_save_metadata_Click;
        }

        private void _btn_save_metadata_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < _files.Count; i++)
            {
                _files[i].Genres = _manager_genres.GetItems();
                _files[i].Moods = _manager_moods.GetItems();
                _files[i].Themes = _manager_themes.GetItems();
            }

            OnSave?.Invoke(this, _files);

            Close();
        }

        private void _webImport_OnImport(object sender, IGmtMedia result)
        {
            _manager_genres.AddRange(result.Genres);
            _manager_moods.AddRange(result.Moods);
            _manager_themes.AddRange(result.Themes);

            tabControl.SelectedIndex = 0;
        }

        public void SetTrackToHandle(List<TrackFile> files)
        {
            _files = files;
            _filesBasicInfo.SetTrackSource(_files);

            _manager_genres.SetItemsSources(_files.Select(x => x.Genres));
            _manager_moods.SetItemsSources(_files.Select(x => x.Moods));
            _manager_themes.SetItemsSources(_files.Select(x => x.Themes));

            List<string> suggetions = new List<string>();
            foreach(var f in files)
            {
                suggetions.Add(f.Artist);
                suggetions.Add(f.Album);
                suggetions.Add($"{f.Album} {f.Artist}");
            }
            suggetions = suggetions.Distinct().ToList();
            _webImport.Reset();
            _webImport.SetSuggetions(suggetions);
        }

        public void TryToShow()
        {
            if (!IsVisible)
            {
                //Accept keyboard input
                ElementHost.EnableModelessKeyboardInterop(this);
                Show();
            }
        }

        public void ForceClose()
        {
            _cancelClose = false;
            Close();
        }

        public event SaveChangesHandler OnSave;
        public delegate void SaveChangesHandler(object sender, IEnumerable<TrackFile> files);

        private void HandleWindowClose(object sender, CancelEventArgs e)
        {
            e.Cancel = _cancelClose;
            if (e.Cancel)
                Hide();
        }
    }
}
