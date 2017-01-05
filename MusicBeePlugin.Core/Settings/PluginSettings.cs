using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core
{
    public class PluginSettings : IPluginSettings
    {
        private static string _folder = null;
        private static IPluginSettings _settings = null;

        public static IPluginSettings LocalSettings
        {
            get {
                if (_settings == null)
                    throw new NullReferenceException("Settings not initialized");

                return _settings;
            }
        }

        public bool SaveDataToFiles { get; set; }
        public bool OnlyUpdateEmptyTags { get; set; }
        public bool ReplaceTags { get; set; }

        public string GenresTagField { get; set; }
        public string ThemesTagField { get; set; }
        public string MoodsTagField { get; set; }
        
        public bool SearchDataByAlbum { get; set; }
        public bool SearchDataByArtist { get; set; }
        //0 - Album; 1 - Artist; 2 - Both
        public int SearchPriority { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Moods { get; set; }
        public List<string> Themes { get; set; }

        public void Save()
        {
            string file = CoreVars.GetFilePath(_folder, CoreVars.SettingsFile);
            string jsonFileContent = JsonConvert.SerializeObject(this, typeof(IPluginSettings), 
                Formatting.Indented, new JsonSerializerSettings());

            File.WriteAllText(file, jsonFileContent);
        }

        private static IPluginSettings GetDefaultSettings()
        {
            return new PluginSettings
            {
                SaveDataToFiles = false,
                ReplaceTags = true,
                OnlyUpdateEmptyTags = false,

                GenresTagField = "Genre",
                MoodsTagField = "Mood",
                ThemesTagField = "Occasion",

                SearchDataByAlbum = true,
                SearchDataByArtist = true,
                SearchPriority = 0,

                Genres = new List<string>(),
                Moods = new List<string>(),
                Themes = new List<string>()
            };
        }

        public static void InitSettings(string rootFolder)
        {
            _folder = rootFolder;
            LoadSettings();
        }
        private static void LoadSettings()
        {
            if (_folder == null)
                throw new ArgumentNullException("_folder", "Root folder not initialized!");

            string file = CoreVars.GetFilePath(_folder, CoreVars.SettingsFile);

            if (!File.Exists(file))
            {
                var defaultSettings = GetDefaultSettings();
                defaultSettings.Save();
                _settings = defaultSettings;
                return;
            }

            _settings = JsonConvert.DeserializeObject<PluginSettings>(File.ReadAllText(file));
            var def = GetDefaultSettings();

            _settings.GenresTagField = _settings.GenresTagField ?? def.GenresTagField;
            _settings.MoodsTagField = _settings.MoodsTagField ?? def.MoodsTagField;
            _settings.ThemesTagField = _settings.ThemesTagField ?? def.ThemesTagField;

            _settings.Genres = _settings.Genres ?? def.Genres;
            _settings.Moods = _settings.Moods ?? def.Moods;
            _settings.Themes = _settings.Themes ?? def.Themes;

            _settings.Save();
        }
    }
}
