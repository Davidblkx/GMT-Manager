using AllMusicApi;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core.Bot
{
    [ProtoContract]
    public class CacheObject : IGmtMedia, IEqualityComparer<CacheObject>
    {
        public CacheObject() { }
        public CacheObject(string id, IGmtMedia tags)
        {
            Id = id;
            Genres = tags?.Genres ?? new List<string>();
            Moods = tags?.Moods ?? new List<string>();
            Themes = tags?.Themes ?? new List<string>();
            Created = DateTime.Now;
        }
        public CacheObject(TrackFile file, CacheType type)
        {
            Id = file.GetCacheId(type);
            Genres = file.Genres;
            Moods = file.Moods;
            Themes = file.Themes;
            Created = DateTime.Now;
        }

        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public List<string> Genres { get; set; }
        [ProtoMember(3)]
        public List<string> Moods { get; set; }
        [ProtoMember(4)]
        public List<string> Themes { get; set; }
        [ProtoMember(5)]
        public DateTime Created { get; set; }

        public TimeSpan GetAge()
        {
            return TimeSpan.FromTicks((DateTime.Now - Created).Ticks);
        }
        public bool IsValid(TimeSpan maxAge)
        {
            return GetAge() < maxAge;
        }

        public bool Equals(CacheObject x, CacheObject y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(CacheObject obj)
        {
            return Id.GetHashCode();
        }
    }
}
