using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MusicBeePlugin.Core.Bot;
using MusicBeePlugin.Core.Settings;

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
        public static string Folder { get { return _folder; } }

        public string GenresTagField { get; set; }
        public string ThemesTagField { get; set; }
        public string MoodsTagField { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Moods { get; set; }
        public List<string> Themes { get; set; }

        public bool HandleGenres { get; set; }
        public bool HandleMoods { get; set; }
        public bool HandleThemes { get; set; }

        public GmtBotOptions BotOptions { get; set; }

        public Dictionary<string, WindowSettings> Windows { get; set; }
        public WindowSettings GetWindowSetting(string windowType)
        {
            if (!Windows.ContainsKey(windowType))
                Windows.Add(windowType, new WindowSettings
                {
                    Height = 300,
                    LeftPosition = 40,
                    TopPosition = 40,
                    Width = 500
                });

            return Windows[windowType];
        }
        public void SetWindowSetting(string windowType, WindowSettings setting)
        {
            if (Windows.ContainsKey(windowType))
                Windows[windowType] = setting;
            else
                Windows.Add(windowType, setting);
        }

        public void Save()
        {
            string file = CoreVars.GetFilePath(_folder, CoreVars.SettingsFile);
            string jsonFileContent = JsonConvert.SerializeObject(this, typeof(IPluginSettings), 
                Formatting.Indented, new JsonSerializerSettings());

            File.WriteAllText(file, jsonFileContent);
        }

        private static IPluginSettings GetDefaultSettings()
        {
            var botOptions = new GmtBotOptions
            {
                CacheMaxAge = 30,
                CacheMaxSize = 250,
                FillTagsMode = 1,
                LimitCacheAge = true,
                LimitCacheSize = true,
                TagPriority = 0,
                UseAlbumTags = true,
                UseArtistTags = true,
                UsePersistentCache = true
            };

            return new PluginSettings
            {
                BotOptions = botOptions,
                Windows = new Dictionary<string, WindowSettings>(),

                GenresTagField = "Genre",
                MoodsTagField = "Mood",
                ThemesTagField = "Occasion",

                Genres = new List<string>(),
                Moods = new List<string>(),
                Themes = new List<string>(),
                HandleGenres = true,
                HandleMoods = true,
                HandleThemes = true
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
