using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllMusicApi.Model
{
    public interface ISearchResult
    {
        SearchResultType ResultType { get;  }

        string ID { get; }
        string Url { get; }

        bool Build(IDomObject info);

        int Diference(string query, string field = "Artist");
    }
}
