using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicBeePlugin.Core.Manager
{
    /// <summary>
    /// Interaction logic for Control_BasicInfo.xaml
    /// </summary>
    public partial class Control_BasicInfo : UserControl
    {
        public Control_BasicInfo()
        {
            InitializeComponent();
            listView_FileList.SelectionChanged += OnSelectionChange;
        }

        public void SetTrackSource(List<TrackFile> files)
        {
            listView_FileList.ItemsSource = files;
        }

        private void OnSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            TrackFile file = listView_FileList.SelectedItem as TrackFile;
            if (file == null) return;

            txt_title.Text = file.Title;
            txt_artist.Text = file.Artist;
            txt_album.Text = file.Album;
        }
    }
}
