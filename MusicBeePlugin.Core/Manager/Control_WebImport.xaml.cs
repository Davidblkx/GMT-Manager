using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AllMusicApi;
using AllMusicApi.Model;
using System.Threading;

namespace MusicBeePlugin.Core.Manager
{
    /// <summary>
    /// Interaction logic for Control_WebImport.xaml
    /// </summary>
    public partial class Control_WebImport : UserControl
    {
        public Control_WebImport()
        {
            InitializeComponent();
            
            _btn_search.Click += _btn_search_Click;

            btn_import.Click += Btn_import_Click;
            _listView_results.SelectionChanged += _listView_results_SelectionChanged;
            _listView_results.MouseDoubleClick += _listView_results_MouseDoubleClick;

            _cmb_suggetions.ElementButtonEnabled = false;
            _cmb_suggetions.AddOnSelection = true;
            _cmb_suggetions.ElementTextBox.KeyDown += _txt_searchQuery_KeyDown;
            _cmb_suggetions.ElementTextBox.TextChanged += _txt_searchQuery_TextChanged;
        }

        public void SetSuggetions(List<string> suggetionsSource)
        {
            _cmb_suggetions.ItemsSource = suggetionsSource;
        }

        private async void doSearch(string query, SearchResultType searchType)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _grid_cover.Visibility = Visibility.Visible;
            })).Wait();

            AllMusicApiAgent agent = new AllMusicApiAgent();
            IEnumerable<ISearchResult> results;

            if (searchType == SearchResultType.Artist)
                results = await agent.Search<ArtistResult>(query, -1);
            else
                results = await agent.Search<AlbumResult>(query, -1);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var viewStyleName = searchType == SearchResultType.Artist ?
                    "GridViewArtists" : "GridViewAlbums";

                _listView_results.View = (ViewBase)Resources[viewStyleName];
                _listView_results.ItemsSource = results;
                _grid_cover.Visibility = Visibility.Hidden;
            }));
        }

        private async void getMediaData(ISearchResult result)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _grid_cover.Visibility = Visibility.Visible;
            })).Wait();


            AllMusicApiAgent agent = new AllMusicApiAgent();

            IGmtMedia mediaData;
            if (result.ResultType == SearchResultType.Album)
                mediaData = await agent.GetAlbum(result.ID);
            else
                mediaData = await agent.GetArtist(result.ID);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _grid_cover.Visibility = Visibility.Hidden;
                OnImport?.Invoke(this, mediaData);
            }));
        }

        public void Reset()
        {
            _cmb_suggetions.InputText = string.Empty;
            _cmb_suggetions.ItemsSource = null;
            _listView_results.ItemsSource = null;
        }

        #region Events
        public event ImportHandler OnImport;
        public delegate void ImportHandler(object sender, IGmtMedia result);

        private void Btn_import_Click(object sender, RoutedEventArgs e)
        {
            var result = _listView_results.SelectedItem as ISearchResult;
            if (result == null) return;

            getMediaData(result);
        }

        private void _btn_search_Click(object sender, RoutedEventArgs e)
        {
            var searchType = _cmb_searchType.SelectedIndex == 0 ? SearchResultType.Artist : SearchResultType.Album;
            doSearch(_cmb_suggetions.InputText, searchType);
        }

        private void _txt_searchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            _btn_search.IsEnabled = _cmb_suggetions.InputText.Length > 0;
        }

        private void _listView_results_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var result = _listView_results.SelectedItem as ISearchResult;
            if (result == null) return;

            getMediaData(result);
        }

        private void _txt_searchQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var searchType = _cmb_searchType.SelectedIndex == 0 ? SearchResultType.Artist : SearchResultType.Album;
                doSearch(_cmb_suggetions.InputText, searchType);
            }
        }

        private void _listView_results_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_import.IsEnabled = _listView_results.SelectedIndex != -1;
        }
        #endregion
    }
}
