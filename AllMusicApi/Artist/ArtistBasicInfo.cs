using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AllMusicApi.Extensions;

namespace AllMusicApi
{
    public partial class Artist : IGmtMedia
    {
        protected virtual void InitBasicInfo()
        {
            Members = new List<RelatedArtist>();
            MemberOf = new List<RelatedArtist>();
            Genres = new List<string>();
            Moods = new List<string>();
            Themes = new List<string>();
        }

        public string Name { get; set; }
        public string Tagline { get; set; }

        public int Birth { get; set; }

        public List<RelatedArtist> Members { get; set; }
        public List<RelatedArtist> MemberOf { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Moods { get; set; }
        public List<string> Themes { get; set; }

        public void BuildBasicInfo(string htmlDoc)
        {
            var s = CQ.Create(htmlDoc);

            Name = HttpUtility.HtmlDecode(s[".artist-name"]?.FirstOrDefault()?
                .InnerHTML?.Trim() ?? "Unkown Artist");

            Tagline = HttpUtility.HtmlDecode(s[".biography span"]?.FirstOrDefault()?
                .InnerHTML?.Trim() ?? string.Empty);

            string birth = s[".birth div"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "-1";
            if (birth.Length > 4) birth = birth.SubstringFromLastIndex('1').Substring(0, 4);

            Birth = birth.ToInt();

            Members = getRelatedArtists(s[".group-members a"]);

            Genres = s[".genre a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim())).ToList()
                ?? new List<string>();

            Genres.AddRange(s[".styles a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim()))
                ?? new List<string>());

            Moods = s[".moods a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim())).ToList()
                ?? new List<string>();

            Themes = s[".themes a"]?.Select(x => HttpUtility.HtmlDecode(x.InnerHTML.Trim())).ToList()
                ?? new List<string>();
        }

        private List<RelatedArtist> getRelatedArtists(IEnumerable<IDomObject> collection)
        {
            var list = new List<RelatedArtist>();

            foreach(IDomObject obj in collection)
            {
                string url = obj?.Attributes?.FirstOrDefault(x => x.Key == "href").Value;
                if (url != null) url = "http://www.allmusic.com" + url;

                string name = HttpUtility.HtmlDecode(CQ.Create(obj)["span"]?
                    .FirstOrDefault()?.InnerHTML?.Trim() ?? "Unkown Artist");
                list.Add(new RelatedArtist(name, url));
            }

            return list;
        }

    }
}
