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

            return PluginSettings.LocalSettings;
        }
    }
}
