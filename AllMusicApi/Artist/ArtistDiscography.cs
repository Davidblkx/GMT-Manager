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
    public partial class Artist
    {
        public List<ArtistAlbumItem> Albums { get; set; }

        protected void InitDiscography()
        {
            Albums = new List<ArtistAlbumItem>();
        }

        public void BuildArtistAlbums(string htmlData)
        {
            var s = CQ.Create(htmlData);

            Albums = new List<ArtistAlbumItem>();
            foreach (IDomObject obj in s[".discography tbody tr"])
                Albums.Add(new ArtistAlbumItem(obj));
        }
    }

    public class ArtistAlbumItem
    {
        public ArtistAlbumItem(IDomObject htmlData)
        {
            var s = CQ.Create(htmlData);

            Title = HttpUtility.HtmlDecode(s[".title a"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "Unknown");
            Year = HttpUtility.HtmlDecode(s[".year"]?.FirstOrDefault()?.InnerHTML?.Trim() ?? "-1").ToInt();
            Url = HttpUtility.HtmlDecode(s[".title a"]?.FirstOrDefault()?.Attributes?
                .FirstOrDefault(x => x.Key == "href").Value ?? null);
            ID = Url.SubstringFromLastIndex('/', 1);
        }

        public string Title { get; set; }
        public int Year { get; set; }
        public string Url { get; set; }
        public string ID { get; set; }
    }
}
