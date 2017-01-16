using MusicBeePlugin.Core.Bot;
using MusicBeePlugin.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core
{
    public interface IPluginSettings
    {
        GmtBotOptions BotOptions { get; set; }

        Dictionary<string, WindowSettings> Windows { get; set; }

        WindowSettings GetWindowSetting(string windowType);
        void SetWindowSetting(string windowType, WindowSettings setting);

        string GenresTagField { get; set; }
        string ThemesTagField { get; set; }
        string MoodsTagField { get; set; }

        List<string> Genres { get; set; }
        List<string> Moods { get; set; }
        List<string> Themes { get; set; }

        void Save();
    }
}
