using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManager.Models
{
    public class Media
    {
        public int ID { get; set; }
        public string TvShow { get; set; }


        public int UserID { get; set; }
        public User User { get; set; }

        public IList<Episode> Episodes { get; set; } 
    }
}
