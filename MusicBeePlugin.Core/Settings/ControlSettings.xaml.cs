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

namespace MusicBeePlugin.Core.Settings
{
    /// <summary>
    /// Interaction logic for ControlSettings.xaml
    /// </summary>
    public partial class ControlSettings : UserControl
    {
        public ControlSettings()
        {
            InitializeComponent();

            checkSaveData.Checked += (sender, e) => UpdateControls();
            checkSaveData.Unchecked += (sender, e) => UpdateControls();
            checkSearchByAlbum.Checked += (sender, e) => UpdateControls();
            checkSearchByAlbum.Unchecked += (sender, e) => UpdateControls();
            checkSearchByArtist.Checked += (sender, e) => UpdateControls();
            checkSearchByArtist.Unchecked += (sender, e) => UpdateControls();
        }

        public void InitializeTagFields(List<string> tagFields)
        {
            comboBoxGenres.ItemsSource = tagFields;
            comboBoxMoods.ItemsSource = tagFields;
            comboBoxThemes.ItemsSource = tagFields;

            LoadSettings();
        }

        public void UpdateControls()
        {
            bool priority = (checkSearchByAlbum?.IsChecked ?? false) && (checkSearchByArtist?.IsChecked ?? false);
            comboBoxSearchPriority.IsEnabled = priority;

            bool save = checkSaveData.IsChecked ?? false;
            checkOnlyEmpty.IsEnabled = save;
            checkReplace.IsEnabled = save;
        }

        public void LoadSettings()
        {
            var settings = PluginSettings.LocalSettings;

            checkOnlyEmpty.IsChecked = settings.OnlyUpdateEmptyTags;
            checkReplace.IsChecked = settings.ReplaceTags;
            checkSaveData.IsChecked = settings.SaveDataToFiles;
            checkSearchByAlbum.IsChecked = settings.SearchDataByAlbum;
            checkSearchByArtist.IsChecked = settings.SearchDataByArtist;

            comboBoxSearchPriority.SelectedIndex = settings.SearchPriority;
            comboBoxGenres.SelectedItem = settings.GenresTagField;
            comboBoxMoods.SelectedItem = settings.MoodsTagField;
            comboBoxThemes.SelectedItem = settings.ThemesTagField;

            UpdateControls();
        }

        public void SaveSettings()
        {
            GetSettings().Save();
        }

        public IPluginSettings GetSettings()
        {
            var settings = PluginSettings.LocalSettings;

            settings.OnlyUpdateEmptyTags = checkOnlyEmpty.IsChecked ?? false;
            settings.ReplaceTags = checkReplace.IsChecked ?? false;
            settings.SaveDataToFiles = checkSaveData.IsChecked ?? false;
            settings.SearchDataByAlbum = checkSearchByAlbum.IsChecked ?? false;
            settings.SearchDataByArtist = checkSearchByArtist.IsChecked ?? false;

            settings.SearchPriority = comboBoxSearchPriority.SelectedIndex;
            settings.GenresTagField = comboBoxGenres.SelectedItem as string;
            settings.MoodsTagField = comboBoxMoods.SelectedItem as string;
            settings.ThemesTagField = comboBoxThemes.SelectedItem as string;

            return settings;
        }
    }
}
