using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllMusicApi
{
    public partial class Artist
    {
        public Artist()
        {

        }
        public Artist(string id)
        {
            ID = id;

            InitBasicInfo();
        }

        public string ID { get; set; }
    }
}
