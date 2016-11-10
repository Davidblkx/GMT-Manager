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
    public class SongResult : ISearchResult
    {
        public SongResult()
        {
            Composers = new List<SongArtistResult>();
            Performers = new List<SongArtistResult>();
        }

        public string ID { get; private set; }
        public string Url { get; private set; }

        public SearchResultType ResultType
        {
            get { return SearchResultType.Song; }
        }

        public string Title { get; set; }

        public List<SongArtistResult> Performers { get; private set; }
        public List<SongArtistResult> Composers { get; private set; }

        public bool Build(IDomObject info)
        {
            Composers = new List<SongArtistResult>();
            Performers = new List<SongArtistResult>();

            var S = CQ.Create(info);

            Title = HttpUtility.HtmlDecode(S[".title a"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown");
            Title = Title.Substring(1, Title.Length - 2); //Remove first and last char

            Url = HttpUtility.HtmlDecode(S[".title a"]?.FirstOrDefault()?.Attributes?
                .FirstOrDefault(x => x.Key == "href").Value ?? null);
            ID = Url.SubstringFromLastIndex('/', 1);

            foreach(IDomObject obj in S[".composers a"])
            {
                if (obj == null) continue;

                string name = HttpUtility.HtmlDecode(obj.InnerHTML.Trim());
                string url = HttpUtility.HtmlDecode(obj.Attributes?
                .FirstOrDefault(x => x.Key == "href").Value ?? null);

                Composers.Add(new SongArtistResult(name, url));
            }

            foreach (IDomObject obj in S[".performers a"])
            {
                if (obj == null) continue;

                string name = HttpUtility.HtmlDecode(obj.InnerHTML.Trim());
                string url = HttpUtility.HtmlDecode(obj.Attributes?
                .FirstOrDefault(x => x.Key == "href").Value ?? null);

                Performers.Add(new SongArtistResult(name, url));
            }

            return ID != null && Url != null;
        }
    }


    public class SongArtistResult
    {
        public SongArtistResult(string name, string url)
        {
            Name = name;
            Url = url;
            ID = url.SubstringFromLastIndex('/', 1);
        }

        public string Name { get; private set; }
        public string Url { get; private set; }
        public string ID { get; private set; }
    }
}
