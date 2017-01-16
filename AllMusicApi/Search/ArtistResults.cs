using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using System.Web;
using AllMusicApi.Extensions;

namespace AllMusicApi.Model
{
    public class ArtistResult : ISearchResult
    {
        public string ID { get; private set; }
        public string Url { get; private set; }

        public SearchResultType ResultType
        { get { return SearchResultType.Artist; } }

        public string Artist { get; private set; }
        public string Genre { get; set; }
        public string Decades { get; set; }


        public bool Build(IDomObject info)
        {
            var S = CQ.Create(info);

            Artist = HttpUtility.HtmlDecode(S[".name a"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown");

            Genre = HttpUtility.HtmlDecode(S[".genres"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown Genre");
            Decades = HttpUtility.HtmlDecode(S[".decades"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown Decade");

            Url = HttpUtility.HtmlDecode(S[".name a"]?.FirstOrDefault()?.Attributes?
                .FirstOrDefault(x => x.Key == "href").Value ?? null);
            ID = Url.SubstringFromLastIndex('/', 1);

            return ID != null && Url != null;
        }

        public int Diference(string query, string field = "Artist")
        {
            switch (field)
            {
                case "Artist":
                    return Algorithms.LevenshteinDistance.Calculate(Artist.ToLower(), query.ToLower());
                case "Genre":
                    return Algorithms.LevenshteinDistance.Calculate(Genre.ToLower(), query.ToLower());
                case "Decades":
                    return Algorithms.LevenshteinDistance.Calculate(Decades.ToLower(), query.ToLower());
                default:
                    return int.MaxValue;
            }
        }
    }
}
