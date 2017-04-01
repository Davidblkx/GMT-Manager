using System.Collections.Generic;
using System.Windows.Controls;

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
        }

        public void InitializeTagFields(List<string> tagFields)
        {
            comboBoxGenres.ItemsSource = tagFields;
            comboBoxMoods.ItemsSource = tagFields;
            comboBoxThemes.ItemsSource = tagFields;

            LoadSettings();
        }

        public void LoadSettings()
        {
            var settings = PluginSettings.LocalSettings;

            comboBoxGenres.SelectedItem = settings.GenresTagField;
            comboBoxMoods.SelectedItem = settings.MoodsTagField;
            comboBoxThemes.SelectedItem = settings.ThemesTagField;

            useGenres.IsChecked = settings.HandleGenres;
            useMoods.IsChecked = settings.HandleMoods;
            useThemes.IsChecked = settings.HandleThemes;
        }

        public void SaveSettings()
        {
            GetSettings().Save();
        }

        public IPluginSettings GetSettings()
        {
            PluginSettings.LocalSettings.GenresTagField = comboBoxGenres.SelectedItem as string;
            PluginSettings.LocalSettings.MoodsTagField = comboBoxMoods.SelectedItem as string;
            PluginSettings.LocalSettings.ThemesTagField = comboBoxThemes.SelectedItem as string;

            PluginSettings.LocalSettings.HandleGenres = useGenres.IsChecked ?? false;
            PluginSettings.LocalSettings.HandleMoods = useMoods.IsChecked ?? false;
            PluginSettings.LocalSettings.HandleThemes = useThemes.IsChecked ?? false;

            return PluginSettings.LocalSettings;
        }
    }
}
