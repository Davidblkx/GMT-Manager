using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicBeePlugin.Core.Tools;

namespace MusicBeePlugin.Core.Bot
{
    /// <summary>
    /// Interaction logic for Control_BotSettings.xaml
    /// </summary>
    public partial class Control_BotSettings : UserControl
    {
        private Dictionary<string, string> _tooltips;

        public Control_BotSettings()
        {
            InitializeComponent();
            LoadToolTips();

            foreach(var ctrl in Grid_Main.GetChildren(true))
            {
                if (_tooltips.ContainsKey(ctrl.Name))
                    ctrl.MouseEnter += ShowToolTip;
            }

            Setting_CheckBox_TagType_Album.Checked += UpdateTagPriority;
            Setting_CheckBox_TagType_Artist.Checked += UpdateTagPriority;
            Setting_CheckBox_TagType_Album.Unchecked += UpdateTagPriority;
            Setting_CheckBox_TagType_Artist.Unchecked += UpdateTagPriority;

            LoadSettings();
        }

        public GmtBotOptions GetSettings()
        {
            return new GmtBotOptions
            {
                AlgorithmTolerance = (int)Setting_Slider_AlgorithmTolerance.Value,
                CacheMaxAge = Setting_TextBox_Cache_LimitTimeValue.Text.ToInt(),
                CacheMaxSize = Setting_TextBox_Cache_LimitSizeValue.Text.ToInt(),
                FillTagsMode = GetTagFillMode(),
                LimitCacheAge = Setting_CheckBox_Cache_LimitTime.IsChecked ?? false,
                LimitCacheSize = Setting_CheckBox_Cache_LimitSize.IsChecked ?? false,
                UseAlbumTags = Setting_CheckBox_TagType_Album.IsChecked ?? false,
                UseArtistTags = Setting_CheckBox_TagType_Artist.IsChecked ?? false,
                UsePersistentCache = Setting_CheckBox_Cache_Persistent.IsChecked ?? false,
                TagPriority = Setting_ComboBox_TagType_Priority.SelectedIndex
            };
        }

        public void LoadSettings()
        {
            var o = PluginSettings.LocalSettings.BotOptions;

            Setting_CheckBox_Cache_LimitSize.IsChecked = o.LimitCacheSize;
            Setting_CheckBox_Cache_LimitTime.IsChecked = o.LimitCacheAge;
            Setting_CheckBox_Cache_Persistent.IsChecked = o.UsePersistentCache;
            Setting_CheckBox_TagType_Album.IsChecked = o.UseAlbumTags;
            Setting_CheckBox_TagType_Artist.IsChecked = o.UseArtistTags;
            Setting_ComboBox_TagType_Priority.SelectedIndex = o.TagPriority;
            Setting_Slider_AlgorithmTolerance.Value = o.AlgorithmTolerance;
            Setting_TextBox_Cache_LimitSizeValue.Text = o.CacheMaxSize.ToString();
            Setting_TextBox_Cache_LimitTimeValue.Text = o.CacheMaxAge.ToString();

            switch (o.FillTagsMode)
            {
                case 1:
                    Setting_RadioButton_SaveMode_Empty.IsChecked = true;
                    break;

                case 2:
                    Setting_RadioButton_SaveMode_Add.IsChecked = true;
                    break;

                case 3:
                    Setting_RadioButton_SaveMode_Replace.IsChecked = true;
                    break;

                default:
                    Setting_RadioButton_SaveMode_Test.IsChecked = true;
                    break;
            }
        }
        private int GetTagFillMode()
        {
            if (Setting_RadioButton_SaveMode_Replace.IsChecked ?? false)
                return 3;

            if (Setting_RadioButton_SaveMode_Add.IsChecked ?? false)
                return 2;

            if (Setting_RadioButton_SaveMode_Empty.IsChecked ?? false)
                return 1;

            return 0;
        }

        private void UpdateTagPriority(object sender, RoutedEventArgs e)
        {
            Setting_ComboBox_TagType_Priority.IsEnabled =
                (Setting_CheckBox_TagType_Album.IsChecked ?? false) &&
                (Setting_CheckBox_TagType_Artist.IsChecked ?? false);
        }
        private void ShowToolTip(object sender, MouseEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem == null) return;

            TextBox_Info.Text = _tooltips[elem.Name];
        }
        private void LoadToolTips()
        {
            _tooltips = new Dictionary<string, string>();
            _tooltips.Add("Setting_RadioButton_SaveMode_Test",
                "Search, download and show results in debug window, but no tag value is updated");
            _tooltips.Add("Setting_RadioButton_SaveMode_Empty",
                "Only save data to empty tags");
            _tooltips.Add("Setting_RadioButton_SaveMode_Add", 
                "Add data to existing values");
            _tooltips.Add("Setting_RadioButton_SaveMode_Replace",
                "Replace tags with new data");

            _tooltips.Add("Setting_Slider_AlgorithmTolerance",
                "Bot string comparison tolerance, 0 - Exact Match, 30 - Extremely loosely match. Max Recommend - 5");

            _tooltips.Add("Setting_CheckBox_TagType_Album",
                "Search for GMT tags by Album");
            _tooltips.Add("Setting_CheckBox_TagType_Artist",
                "Search for GMT tags by Artist");
            _tooltips.Add("Setting_ComboBox_TagType_Priority",
                "Defines the search priority, Albums, Artists or Both");

            _tooltips.Add("Setting_CheckBox_Cache_Persistent",
                "Use cache between sessions to save bandwidth and increment bot speed");
            _tooltips.Add("Setting_CheckBox_Cache_LimitSize",
                "Defines the max storage in MegaBytes, default 250MB");
            _tooltips.Add("Setting_CheckBox_Cache_LimitTime",
                "Defines the cache expiration, value in days, default 30 Days");
        }
    }
}
