using AllMusicApi;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core.Bot
{
    /// <summary>
    /// Collection of [CacheObject] indexed by [Id]
    /// Can be serialized using ProtoBuf
    /// </summary>
    [ProtoContract]
    public class CacheBot : IEnumerable<CacheObject>, IEnumerable
    {
        [ProtoMember(10)]
        private Dictionary<string, CacheObject> _collection;

        public CacheBot()
        {
            _collection = new Dictionary<string, CacheObject>();
        }

        [Obsolete("Use [Set] instead")]
        public void Add(CacheObject newObj)
        {
            Set(newObj);
        }
        
        /// <summary>
        /// Check if id is present on collection
        /// </summary>
        /// <param name="id">The id to search</param>
        /// <returns>True if id is present</returns>
        public bool Has(string id)
        {
            return _collection.ContainsKey(id);
        }
        /// <summary>
        /// Check if [CacheObject] for specified [TrackFile] 
        /// of type [CacheType] is present in collection
        /// </summary>
        /// <param name="file">TrackFile to search</param>
        /// <param name="type">CacheType to search</param>
        /// <returns>True if is present</returns>
        public bool Has(TrackFile file, CacheType type)
        {
            return Has(file.GetCacheId(type));
        }
        /// <summary>
        /// Check if [CacheObject] for specified [TrackFile] 
        /// of type [CacheType] with a limit age is present in collection
        /// </summary>
        /// <param name="file">TrackFile to search</param>
        /// <param name="type">CacheType to search</param>
        /// <param name="maxAge">Max age allowed</param>
        /// <returns></returns>
        public bool Has(TrackFile file, CacheType type, TimeSpan maxAge)
        {
            return Get(file, type, maxAge) != null;
        }

        /// <summary>
        /// Get [CacheObject] with specified Id
        /// </summary>
        /// <param name="id">The Id of object to return</param>
        /// <returns></returns>
        public CacheObject Get(string id)
        {
            if (Has(id)) return _collection[id];
            return null;
        }
        /// <summary>
        /// Get [CacheObject] for the specified [TrackFile] of type [CacheType]
        /// </summary>
        /// <returns>The first found object, Null if not found</returns>
        public CacheObject Get(TrackFile file, CacheType type)
        {
            return Get(file.GetCacheId(type));
        }
        /// <summary>
        /// Get [CacheObject] for the specified [TrackFile] of type [CacheType]
        /// with a limit age
        /// </summary>
        /// <returns>The first found object, Null if not found</returns>
        public CacheObject Get(TrackFile file, CacheType type, TimeSpan maxAge)
        {
            CacheObject obj = Get(file, type);

            if (obj?.IsValid(maxAge) ?? false)
                return obj;

            return null;
        }

        /// <summary>
        /// Add or Update specified object
        /// </summary>
        /// <param name="obj"></param>
        public void Set(CacheObject obj)
        {
            if (Has(obj.Id))
                _collection[obj.Id] = obj;
            else
                _collection.Add(obj.Id, obj);
        }
        /// <summary>
        /// Add of Update specified object
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type"></param>
        public void Set(TrackFile file, CacheType type)
        {
            Set(new CacheObject(file, type));
        }

        public IEnumerator<CacheObject> GetEnumerator()
        {
            return _collection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.Values.GetEnumerator();
        }

        /// <summary>
        /// Serialize file to specified destination using ProtoBuf
        /// </summary>
        /// <param name="cacheCollection"></param>
        /// <param name="destinationFile"></param>
        public static void Serialize(CacheBot cacheCollection, string destinationFile)
        {
            using(var file = File.Create(destinationFile))
            {
                Serializer.Serialize(file, cacheCollection);
            }
        }
        /// <summary>
        /// Deserialize specified file, returns null if file don't exists
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public static CacheBot LoadFile(string sourceFilePath)
        {
            CacheBot cache = null;

            if (File.Exists(sourceFilePath))
            {
                using (var file = File.OpenRead(sourceFilePath))
                {
                    cache = Serializer.Deserialize<CacheBot>(file);
                }
            }

            return cache;
        }
    }
}
