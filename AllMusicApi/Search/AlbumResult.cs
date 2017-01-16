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
    public class AlbumResult : ISearchResult
    {
        public string ID { get; private set; }
        public string Url { get; private set; }

        public SearchResultType ResultType
        {
            get { return SearchResultType.Album; }
        }

        public string Title { get; private set; }
        public string Artist { get; private set; }
        public string ArtistId { get; private set; }
        public string ArtistUrl { get; private set; }
        public string Year { get; private set; }
        public string Genre { get; private set; }

        public bool Build(IDomObject info)
        {
            var S = CQ.Create(info);

            Title = HttpUtility.HtmlDecode(S[".title a"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown");
            Artist = HttpUtility.HtmlDecode(S[".artist a"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown");

            Year = HttpUtility.HtmlDecode(S[".year"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown");
            Genre = HttpUtility.HtmlDecode(S[".genres"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown");

            Url = HttpUtility.HtmlDecode(S[".title a"]?.FirstOrDefault()?.Attributes?
                .FirstOrDefault(x => x.Key == "href").Value ?? null);
            ArtistUrl = HttpUtility.HtmlDecode(S[".artist a"]?.FirstOrDefault()?.Attributes?
                .FirstOrDefault(x => x.Key == "href").Value ?? null);

            ID = Url.SubstringFromLastIndex('/', 1);
            ArtistId = ArtistUrl.SubstringFromLastIndex('/', 1);

            return ID != null && Url != null;
        }

        public override string ToString()
        {
            return $"{Title} by {Artist} in {Year}[{Genre}]";
        }

        public int Diference(string query, string field = "Artist")
        {
            switch (field)
            {
                case "Artist":
                    return Algorithms.LevenshteinDistance.Calculate(Artist.ToLower(), query.ToLower());
                case "Genre":
                    return Algorithms.LevenshteinDistance.Calculate(Genre.ToLower(), query.ToLower());
                case "Title":
                    return Algorithms.LevenshteinDistance.Calculate(Title.ToLower(), query.ToLower());
                default:
                    return int.MaxValue;
            }
        }
    }
}
