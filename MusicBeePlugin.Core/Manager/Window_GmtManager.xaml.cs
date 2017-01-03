using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        public void AddTrackToHandle(List<TrackFile> files)
        {
            _files = files;
            filesBasicInfo.SetTrackSource(_files);
            managerGenres.SetItemsSources(_files.Select(x => x.Genres));
        }

        public void TryToShow()
        {
            if (!IsVisible)
                Show();
        }

        public void ForceClose()
        {
            _cancelClose = false;
            Close();
        }

        private void HandleWindowClose(object sender, CancelEventArgs e)
        {
            e.Cancel = _cancelClose;
            if (e.Cancel)
                Hide();
        }
    }
}
