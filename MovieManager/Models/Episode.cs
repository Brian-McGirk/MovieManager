using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManager.Models
{
    public class Episode
    {
        public int ID { get; set; }
        public string EpisodeName { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public string AirDate { get; set; }
        public bool Seen { get; set; }

        public int MediaID { get; set; }
        public Media Media { get; set; }


    }
}
