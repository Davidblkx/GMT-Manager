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
    public class RelatedAlbum
    {
        public RelatedAlbum() { }
        public RelatedAlbum(IDomObject htmlData)
        {
            var s = CQ.Create(htmlData);

            Url = htmlData?.Attributes?.FirstOrDefault(x => x.Key == "href").Value ?? null;
            if (Url != null) Url = "http://www.allmusic.com" + Url;

            ID = Url.SubstringFromLastIndex('/', 1);

            Title = HttpUtility.HtmlDecode(s["div.title"]?.FirstOrDefault()?
                .InnerHTML?.Trim() ?? "Unknown Title");

            Artist = HttpUtility.HtmlDecode(s["div.artist"]?.FirstOrDefault()?
                .InnerHTML?.Trim() ?? "Unknown Artist");

            ImageLink = s["img"]?.FirstOrDefault()?.Attributes?
                .FirstOrDefault(x => x.Key == "src").Value ?? null;
        }

        public string Title { get; set; }
        public string ID { get; set; }
        public string Url { get; set; }
        public string Artist { get; set; }

        public string ImageLink { get; set; }
    }
}
