using AllMusicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core
{
    public class TrackFile : IGmtMedia
    {
        public string FilePath { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Moods { get; set; } = new List<string>();
        public List<string> Themes { get; set; } = new List<string>();
    }
}
