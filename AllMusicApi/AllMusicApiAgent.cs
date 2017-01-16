using AllMusicApi.Model;
using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AllMusicApi
{
    public class AllMusicApiAgent
    {
        public int MaxResults { get; set; }

        public AllMusicApiAgent()
        {
            MaxResults = 40;
        }

        public async Task<List<T>> Search<T>(string query, int maxResults)
            where T : ISearchResult, new()
        {
            var urlName = HttpUtility.UrlEncode(query);

            var type = new T().ResultType.ToString().ToLower() + 's';

            var apiEndPoint = $"http://www.allmusic.com/search/{type}/{urlName}/all/";
            var results = new List<T>();
            var currentPage = 0;
            var lastCount = 0;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Referer", apiEndPoint);
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            while (results.Count < maxResults || maxResults == -1)
            {
                currentPage++;
                
                var response = await client.GetAsync(apiEndPoint + currentPage);
                if (!response.IsSuccessStatusCode) break;

                results.AddRange(GetResult<T>(await response.Content.ReadAsStringAsync()));

                if (results.Count == lastCount) break;

                lastCount = results.Count;
            }

            if (maxResults == -1) return results;

            return results.Take(maxResults).ToList();
        }

        public async Task<Artist> GetArtist(string allmusic_id)
        {
            var artist = new Artist(allmusic_id);

            var apiEndPoint = $"http://www.allmusic.com/artist/{allmusic_id}";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Referer", apiEndPoint);
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            //Get basic info
            var response = await client.GetAsync(apiEndPoint);
            if (!response.IsSuccessStatusCode) return artist;

            artist.BuildBasicInfo(await response.Content.ReadAsStringAsync());

            //Get discography
            response = await client.GetAsync(apiEndPoint + "/discography");
            if (!response.IsSuccessStatusCode) return artist;

            artist.BuildArtistAlbums(await response.Content.ReadAsStringAsync());

            //Get Relations
            response = await client.GetAsync(apiEndPoint + "/related");
            if (!response.IsSuccessStatusCode) return artist;

            artist.BuildArtistRelations(await response.Content.ReadAsStringAsync());

            return artist;
        }

        public async Task<Album> GetAlbum(string allmusic_id)
        {
            var album = new Album(allmusic_id);

            var apiEndPoint = $"http://www.allmusic.com/album/{allmusic_id}";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Referer", apiEndPoint);
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            var response = await client.GetAsync(apiEndPoint);
            if (!response.IsSuccessStatusCode) return album;

            album.BuildBasicInfo(await response.Content.ReadAsStringAsync());

            response = await client.GetAsync(apiEndPoint + "/similar");
            if (!response.IsSuccessStatusCode) return album;

            album.BuildAlbumRelations(await response.Content.ReadAsStringAsync());

            return album;
        }

        protected IEnumerable<T> GetResult<T>(string htmlDoc)
            where T : ISearchResult, new()
        {
            List<T> results = new List<T>();

            var S = CQ.Create(htmlDoc);
            var type = new T().ResultType.ToString().ToLower();

            foreach(IDomObject obj in S[$"li.{type}"])
            {
                var inst = new T();
                inst.Build(obj);
                results.Add(inst);
            }

            return results;
        }
    }
}
