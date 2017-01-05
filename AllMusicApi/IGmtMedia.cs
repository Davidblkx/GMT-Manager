using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllMusicApi
{
    public interface IGmtMedia
    {
        List<string> Genres { get; set; }
        List<string> Moods { get; set; }
        List<string> Themes { get; set; }
    }
}
