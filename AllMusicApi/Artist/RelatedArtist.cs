using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllMusicApi.Extensions;
using CsQuery;
using System.Web;

namespace AllMusicApi
{
    public class RelatedArtist
    {
        public RelatedArtist(string name, string url)
        {
            Name = name;
            Url = url;
            ID = Url.SubstringFromLastIndex('/', 1);
        }

        public RelatedArtist(IDomObject htmlData)
        {
            Name = HttpUtility.HtmlDecode(htmlData.InnerHTML);
            Url = htmlData.Attributes.FirstOrDefault(x => x.Key == "href").Value;
            ID = Url.SubstringFromLastIndex('/', 1);
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string ID { get; set; }
    }
}
