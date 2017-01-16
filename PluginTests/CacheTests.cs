using System;
using MusicBeePlugin.Core.Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PluginTests
{
    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public void TestSerialization()
        {
            var obj = new CacheObject
            {
                Id = "some_id",
                Created = DateTime.Now,
                Genres = new List<string> { "Genre1", "Genre2", "Genre3" },
                Moods = new List<string> { "Mood1", "Mood2", "Mood3" },
                Themes = new List<string> { "Theme1", "Theme2", "Theme3" }
            };

            CacheBot cache = new CacheBot();
            cache.Set(obj);

            CacheBot.Serialize(cache, "test.bin");

            var targetCache = CacheBot.LoadFile("test.bin");

            Assert.IsNotNull(targetCache);
            Assert.IsTrue(targetCache.Has(obj.Id));

            var targetObj = targetCache.Get(obj.Id);
            Assert.IsNotNull(targetObj);
            Assert.AreEqual(obj.Genres[0], targetObj.Genres[0]);
            Assert.AreEqual(obj.Genres[1], targetObj.Genres[1]);
            Assert.AreEqual(obj.Genres[2], targetObj.Genres[2]);
            Assert.AreEqual(obj.Moods[0], targetObj.Moods[0]);
            Assert.AreEqual(obj.Moods[1], targetObj.Moods[1]);
            Assert.AreEqual(obj.Moods[2], targetObj.Moods[2]);
            Assert.AreEqual(obj.Themes[0], targetObj.Themes[0]);
            Assert.AreEqual(obj.Themes[1], targetObj.Themes[1]);
            Assert.AreEqual(obj.Themes[2], targetObj.Themes[2]);
        }
    }
}
