using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AllMusicApi;
using System.Threading.Tasks;
using AllMusicApi.Model;
using System.Linq;

namespace PluginTests
{
    [TestClass]
    public class ApiTests
    {

        [TestMethod]
        public async Task TestSearchArtist()
        {
            var api = new AllMusicApiAgent();
            var result = await api.Search<ArtistResults>("dido", 30);

            Assert.AreEqual(30, result.Count);

            var r0 = result[0];
            var r20 = result[19];

            Assert.AreEqual("Dido", r0.Name);
            Assert.AreEqual("http://www.allmusic.com/artist/dido-mn0000217344", r0.Url);
            Assert.AreEqual("dido-mn0000217344", r0.ID);
            Assert.AreEqual("Pop/Rock", r0.Genre);
            Assert.AreEqual("1990s - 2010s", r0.Decades);

            Assert.AreEqual("Irocc & Dido Brown", r20.Name);
            Assert.AreEqual("http://www.allmusic.com/artist/irocc-dido-brown-mn0002503861", r20.Url);
            Assert.AreEqual("irocc-dido-brown-mn0002503861", r20.ID);
            Assert.AreEqual("Reggae", r20.Genre);
            Assert.AreEqual("Unknown Decade", r20.Decades);
        }

        [TestMethod]
        public async Task TestSearchAlbum()
        {
            var api = new AllMusicApiAgent();
            var result = await api.Search<AlbumResult>("nevermind", 10);

            Assert.AreEqual(10, result.Count);

            var r = result[0];
            Assert.AreEqual("Nevermind", r.Title);
            Assert.AreEqual("http://www.allmusic.com/album/nevermind-mw0000185616", r.Url);
            Assert.AreEqual("nevermind-mw0000185616", r.ID);
            Assert.AreEqual("Nirvana", r.Artist);
            Assert.AreEqual("http://www.allmusic.com/artist/nirvana-mn0000357406", r.ArtistUrl);
            Assert.AreEqual("nirvana-mn0000357406", r.ArtistId);
            Assert.AreEqual("1991", r.Year);
            Assert.AreEqual("Pop/Rock", r.Genre);
        }

        [TestMethod]
        public async Task TestSearchSong()
        {
            var api = new AllMusicApiAgent();
            var result = await api.Search<SongResult>("the Great gig in the sky", 10);

            Assert.AreEqual(10, result.Count);

            var r = result[1];
            Assert.AreEqual("The Great Gig in the Sky", r.Title);
            Assert.AreEqual("http://www.allmusic.com/song/the-great-gig-in-the-sky-mt0036036156", r.Url);
            Assert.AreEqual("the-great-gig-in-the-sky-mt0036036156", r.ID);

            Assert.AreEqual(3, r.Performers.Count);
            var p = r.Performers[0];

            Assert.AreEqual("The Flaming Lips", p.Name);
            Assert.AreEqual("http://www.allmusic.com/artist/the-flaming-lips-mn0000065590", p.Url);
            Assert.AreEqual("the-flaming-lips-mn0000065590", p.ID);

            Assert.AreEqual(2, r.Composers.Count);
            var c = r.Composers[0];

            Assert.AreEqual("Roger Waters", c.Name);
            Assert.AreEqual("http://www.allmusic.com/artist/roger-waters-mn0000254556", c.Url);
            Assert.AreEqual("roger-waters-mn0000254556", c.ID);
        }

        [TestMethod]
        public async Task TestGetArtist()
        {
            var agent = new AllMusicApiAgent();
            var artist = await agent.GetArtist("pink-floyd-mn0000346336");

            Assert.IsNotNull(artist);

            Assert.AreEqual("Pink Floyd", artist.Name);

            string bio = "One of the most predominant and celebrated rock" +
                " bands of all time, prog- and space-rock legends, known " +
                "for superlative musicianship.";
            Assert.AreEqual(bio, artist.Tagline);
            Assert.AreEqual(1965, artist.Birth);

            Assert.AreEqual(6, artist.Members.Count);
            Assert.AreEqual(0, artist.MemberOf.Count);

            var m = artist.Members[0];
            Assert.AreEqual("http://www.allmusic.com/artist/david-gilmour-mn0000582930", m.Url);
            Assert.AreEqual("david-gilmour-mn0000582930", m.ID);
            Assert.AreEqual("David Gilmour", m.Name);

            Assert.AreEqual(9, artist.Genres.Count);
            Assert.AreEqual("Avant-Garde", artist.Genres[1]);
            Assert.AreEqual("Prog-Rock", artist.Genres[5]);

            Assert.AreEqual(44, artist.Moods.Count);
            Assert.IsTrue(artist.Moods.Contains("Enigmatic"));
            Assert.IsTrue(artist.Moods.Contains("Angst-Ridden"));
            Assert.IsTrue(artist.Moods.Contains("Sad"));
            Assert.IsTrue(artist.Moods.Contains("Bleak"));

            Assert.AreEqual(17, artist.Themes.Count);
            Assert.IsTrue(artist.Themes.Contains("Myths & Legends"));
            Assert.IsTrue(artist.Themes.Contains("Starry Sky"));
            Assert.IsTrue(artist.Themes.Contains("Open Road"));
            Assert.IsTrue(artist.Themes.Contains("Politics/Society"));

            Assert.AreEqual(18, artist.Albums.Count);

            var album = artist.Albums.Last();
            Assert.AreEqual(2014, album.Year);
            Assert.AreEqual("The Endless River", album.Title);
            Assert.AreEqual("http://www.allmusic.com/album/the-endless-river-mw0002758626", album.Url);
            Assert.AreEqual("the-endless-river-mw0002758626", album.ID);
        }
    }
}
