﻿using System;
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
            var result = await api.Search<ArtistResult>("dido", 30);

            Assert.AreEqual(30, result.Count);

            var r0 = result[0];
            var r20 = result[19];

            Assert.AreEqual("Dido", r0.Artist);
            Assert.AreEqual("http://www.allmusic.com/artist/dido-mn0000217344", r0.Url);
            Assert.AreEqual("dido-mn0000217344", r0.ID);
            Assert.AreEqual("Pop/Rock", r0.Genre);
            Assert.AreEqual("1990s - 2010s", r0.Decades);

            Assert.AreEqual("Irocc & Dido Brown", r20.Artist);
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

            Assert.AreEqual(42, artist.SimiliarTo.Count);
            Assert.AreEqual(20, artist.InfluencedBy.Count);
            Assert.AreEqual(142, artist.FollowedBy.Count);
            Assert.AreEqual(4, artist.AssociatedWith.Count);


        }

        [TestMethod]
        public async Task TestGetAlbum()
        {
            var agent = new AllMusicApiAgent();
            var album = await agent.GetAlbum("mule-variations-mw0000048101");

            Assert.IsNotNull(album);

            Assert.AreEqual(1999, album.ReleaseDate);
            Assert.AreEqual("Mule Variations", album.Title);

            Assert.IsTrue(album.Artists.Count == 1);

            var artist = album.Artists.FirstOrDefault();

            Assert.IsNotNull(artist);
            Assert.AreEqual("http://www.allmusic.com/artist/tom-waits-mn0000615119", artist.Url);
            Assert.AreEqual("tom-waits-mn0000615119", artist.ID);
            Assert.AreEqual("Tom Waits", artist.Name);

            Assert.IsTrue(album.Genres.Count == 4);
            Assert.IsTrue(album.Genres.Contains("Experimental Rock"));

            Assert.IsTrue(album.Moods.Count == 2);
            Assert.IsTrue(album.Moods.Contains("Somber"));

            Assert.IsTrue(album.Themes.Count == 0);

            Assert.IsTrue(album.Tracks.Count == 16);
            var track = album.Tracks.FirstOrDefault(x => x.TrackIndex == 11);

            Assert.IsNotNull(track);
            Assert.AreEqual(1, track.DiscIndex);
            Assert.AreEqual("Picture in a Frame", track.Title);
            Assert.AreEqual(2, track.Composers.Count);

            var composer = track.Composers[0];
            Assert.IsNotNull(composer);
            Assert.AreEqual("http://www.allmusic.com/artist/kathleen-brennan-mn0000306103", composer.Url);
            Assert.AreEqual("kathleen-brennan-mn0000306103", composer.ID);
            Assert.AreEqual("Kathleen Brennan", composer.Name);

            Assert.AreEqual(1, track.Performers.Count);
            var performer = track.Performers[0];
            Assert.IsNotNull(performer);
            Assert.AreEqual("http://www.allmusic.com/artist/tom-waits-mn0000615119", performer.Url);
            Assert.AreEqual("tom-waits-mn0000615119", performer.ID);
            Assert.AreEqual("Tom Waits", performer.Name);

            var time = TimeSpan.FromMinutes(3) + TimeSpan.FromSeconds(39);
            Assert.AreEqual(time, track.Time);
        }
    }
}
