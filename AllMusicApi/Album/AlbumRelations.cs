using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllMusicApi
{
    public partial class Album
    {
        public List<RelatedAlbum> SimiliarAlbums { get; set; }

        public string CoverLink { get; set; }

        public void BuildAlbumRelations(string htmlData)
        {
            var s = CQ.Create(htmlData);

            SimiliarAlbums = new List<RelatedAlbum>();
            foreach (IDomObject obj in s[".similar-albums a"])
                SimiliarAlbums.Add(new RelatedAlbum(obj));

            CoverLink = s[".album-cover img"]?.FirstOrDefault()?.Attributes?
                .FirstOrDefault(x => x.Key == "src").Value ?? null;
        }
    }
}
