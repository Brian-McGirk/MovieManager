using MovieManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManager.ViewModels
{
    public class MyShowsViewModel
    {
        public IList<Media> Medias { get; set; }
        public int dateMonth { get; set; }
        public int dateDay { get; set; }
        public int dateYear { get; set; }
    }
}
 