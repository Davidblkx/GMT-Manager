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
    public class AlbumTrack
    {
        public AlbumTrack()
        {
            Composers = new List<RelatedArtist>();
            Performers = new List<RelatedArtist>();
        }

        public AlbumTrack(IDomObject htmlData, int discIndex)
        {
            var s = CQ.Create(htmlData);

            DiscIndex = discIndex;
            TrackIndex = s[".tracknum"]?.FirstOrDefault()?.InnerHTML?.Trim()?.ToInt() ?? -1;

            Title = HttpUtility.HtmlDecode(s[".title-composer .title a"]?.FirstOrDefault()?
                .InnerHTML?.Trim() ?? "Unknown Title");

            Composers = s[".title-composer .composer a"]?.Select(x => new RelatedArtist(x)).ToList()
                ?? new List<RelatedArtist>();

            Performers = s[".performer a"]?.Select(x => new RelatedArtist(x)).ToList()
                ?? new List<RelatedArtist>();

            try
            {
                string strTime = (HttpUtility.HtmlDecode(s[".time"]?.FirstOrDefault()?
                    .InnerHTML?.Trim()) ?? "0:00");

                if (strTime.IndexOf(' ') != -1)
                    strTime = strTime.Substring(0, strTime.IndexOf(' '));

                var time = strTime.Split(':');

                Time = TimeSpan.FromMinutes(double.Parse(time[0]));
                Time += TimeSpan.FromSeconds(double.Parse(time[1]));
            }
            catch
            {
                Time = TimeSpan.FromSeconds(0);
            }

        }

        public  int DiscIndex { get; set; }
        public int TrackIndex { get; set; }
        public string Title { get; set; }

        public TimeSpan Time { get; set; }
        public List<RelatedArtist> Composers { get; set; }
        public List<RelatedArtist> Performers { get; set; }
    }
}
