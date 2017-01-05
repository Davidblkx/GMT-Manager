using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core
{
    public interface IPluginSettings
    {
        bool SaveDataToFiles { get; set; }
        bool OnlyUpdateEmptyTags { get; set; }
        bool ReplaceTags { get; set; }

        string GenresTagField { get; set; }
        string ThemesTagField { get; set; }
        string MoodsTagField { get; set; }

        bool SearchDataByAlbum { get; set; }
        bool SearchDataByArtist { get; set; }
        int SearchPriority { get; set; }

        List<string> Genres { get; set; }
        List<string> Moods { get; set; }
        List<string> Themes { get; set; }

        void Save();
    }
}
