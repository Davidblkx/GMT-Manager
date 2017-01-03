using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core
{
    public class PluginSettings
    {
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

        public void Save(string folder)
        {
            string file = GetFilePath(folder);
            string data = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(file, data);
        }

        private static PluginSettings GetDefault()
        {
            return new PluginSettings
            {
                SaveDataToFiles = false,
                ReplaceTags = true,
                OnlyUpdateEmptyTags = false,

                GenresTagField = "Custom14",
                MoodsTagField = "Custom15",
                ThemesTagField = "Custom16",

                SearchDataByAlbum = true,
                SearchDataByArtist = true,
                SearchPriority = 0
            };
        }
        private static string GetFilePath(string folder)
        {
            string path = CoreVars.GetPluginFolderPath(folder);

            string file = Path.Combine(path, CoreVars.SettingsFile);
            return file;
        }

        public static PluginSettings LoadSettings(string folder)
        {
            string file = GetFilePath(folder);

            if (!File.Exists(file))
            {
                var settings = GetDefault();
                settings.Save(folder);
                return settings;
            }

            return JsonConvert.DeserializeObject<PluginSettings>(File.ReadAllText(file));
        }


    }
}
