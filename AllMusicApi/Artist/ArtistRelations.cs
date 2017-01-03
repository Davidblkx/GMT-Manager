using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllMusicApi
{
    public partial class Artist
    {
        public List<RelatedArtist> SimiliarTo { get; set; }
        public List<RelatedArtist> InfluencedBy { get; set; }
        public List<RelatedArtist> FollowedBy { get; set; }
        public List<RelatedArtist> AssociatedWith { get; set; }

        protected void InitArtistRelations()
        {
            SimiliarTo = new List<RelatedArtist>();
            InfluencedBy = new List<RelatedArtist>();
            FollowedBy = new List<RelatedArtist>();
            AssociatedWith = new List<RelatedArtist>();
        }

        public void BuildArtistRelations(string htmlData)
        {
            var s = CQ.Create(htmlData);

            SimiliarTo = new List<RelatedArtist>();

            foreach (IDomObject obj in s[".related.similars a"])
                SimiliarTo.Add(new RelatedArtist(obj));

            foreach (IDomObject obj in s[".related.influencers a"])
                InfluencedBy.Add(new RelatedArtist(obj));

            foreach (IDomObject obj in s[".related.followers a"])
                FollowedBy.Add(new RelatedArtist(obj));

            foreach (IDomObject obj in s[".related.associatedwith a"])
                AssociatedWith.Add(new RelatedArtist(obj));
        }
    }
}
