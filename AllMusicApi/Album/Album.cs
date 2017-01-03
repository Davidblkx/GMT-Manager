using AllMusicApi.Extensions;
using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AllMusicApi
{
    public partial class Album
    {
        public Album()
        {

        }
        public Album(string id)
        {
            Genres = new List<string>();
            Moods = new List<string>();
            Themes = new List<string>();
            Artists = new List<RelatedArtist>();
            Tracks = new List<AlbumTrack>();
        }

        public string ID { get; set; }

        public int ReleaseDate { get; set; }
        public string Title { get; set; }
        public List<RelatedArtist> Artists { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Moods { get; set; }
        public List<string> Themes { get; set; }

        public List<AlbumTrack> Tracks { get; set; }

        public void BuildBasicInfo(string htmlDoc)
        {
            var s = CQ.Create(htmlDoc);

            Title = HttpUtility.HtmlDecode(s["h1.album-title"]?.FirstOrDefault()?
                .InnerHTML?.Trim() ?? "Unknown Title");
            
            Artists = s[".album-artist a"]?.Select(z => new RelatedArtist(z)).ToList() 
                ?? new List<RelatedArtist>();

            ReleaseDate = s[".release-date span"]?.FirstOrDefault()?
                .InnerHTML?.Reverse().Take(4).Reverse().Join().ToInt() ?? -1;

            Genres = s[".genre a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim())).ToList()
                ?? new List<string>();

            Genres.AddRange(s[".styles a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim()))
                ?? new List<string>());

            Moods = s[".moods a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim())).ToList()
                ?? new List<string>();

            Themes = s[".themes a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim())).ToList()
                ?? new List<string>();

            Tracks = new List<AlbumTrack>();
            var total_discs = s[".track-listing .disc"];
            for(var i = 0; i < total_discs.Count(); i++)
            {
                Tracks.AddRange(CQ.Create(total_discs[i])[".track"].Select(x => new AlbumTrack(x, i + 1)));
            }
        }
    }
}
