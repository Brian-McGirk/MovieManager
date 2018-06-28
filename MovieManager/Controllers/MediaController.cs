using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManager.Data;
using MovieManager.Models;

namespace MovieManager.Controllers
{
    public class MediaController : Controller
    {
        private static string st;

        private readonly MovieDbContext context;

        public MediaController(MovieDbContext dbContext)
        {
            context = dbContext;
        }


        public IActionResult Index()
        {
            //if (!UserController.session.ContainsKey("user"))
            //{
            //    return Redirect("/User/Login");
            //}

            // TODO Display all of the users TV Shows along with the episodes they haven't watched
            //IList<Media> Medias = context.Medias.Include(u => u.User).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            //if (!UserController.session.ContainsKey("user"))
            //{
            //    return Redirect("/User/Login");
            //}

            st = searchTerm;

            string[] searchTermSplit = searchTerm.Split(' ');

            string searchTermJoined = String.Join("+", searchTermSplit);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://www.imdb.com/find?ref_=nv_sr_fn&q=" + searchTermJoined + "&s=all");

            HtmlNode titles = doc.DocumentNode.SelectNodes("//div[@class='findSection']").First();

            HtmlNode[] searchResults = titles.SelectNodes("//table[@class='findList']/tr/td[@class='result_text']").ToArray();

            List<string> filteredSearchResults = new List<string>();

            foreach (HtmlNode searchResult in searchResults)
            {
                if (searchResult.InnerText.ToLower().Contains("tv series"))
                {
                    filteredSearchResults.Add(searchResult.InnerText);
                }              

            }

            ViewBag.filteredSearchResults = filteredSearchResults;

            return View();
        }

        [HttpPost]
        public IActionResult AddTvSeries()
        {
            string[] searchTermSplit = st.Split(' ');

            string searchTermCapital = "";

            foreach(string s in searchTermSplit)
            {
                searchTermCapital += s.Substring(0, 1).ToUpper() + s.Substring(1) + " ";
            }

            searchTermSplit = searchTermCapital.Split(' ');

            string searchTermJoined = String.Join("_", searchTermSplit);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://en.wikipedia.org/wiki/List_of_" + searchTermJoined + "episodes");

            HtmlNode[] seasonsNode = doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/h3").ToArray();


            //List<string> seasons = new List<string>();
            //List<string> episodes = new List<string>();

            //foreach (HtmlNode season in seasonsNode)
            //{
            //    int index = season.InnerText.IndexOf("(");

            //    if (!seasons.Contains(season.InnerText.Substring(0, index)))
            //    {
            //        seasons.Add(season.InnerText.Substring(0, index));
            //    }

            //}

            List<string> seasons = new List<string>();
            List<string> episodes = new List<string>();
            List<string> airDate = new List<string>();
              

            foreach (HtmlNode season in seasonsNode)
            {
                int index = season.InnerText.IndexOf("(");

                if (!seasons.Contains(season.InnerText.Substring(0, index)))
                {
                    seasons.Add(season.InnerText.Substring(0, index));
                }


                HtmlNode[] episodesNode = doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/table[" + (Int32.Parse(season.InnerText.Substring(7, 1)) + 1) + "]/tr[@class='vevent']").ToArray();
                foreach (HtmlNode episode in episodesNode)
                {
                    int firstIndex = episode.InnerText.IndexOf("\"");
                    int lastIndex = episode.InnerText.LastIndexOf("\"");

                    int finalIndex = episode.InnerText.Substring(firstIndex + 1, lastIndex).IndexOf("\"");

                    episodes.Add(episode.InnerText.Substring(firstIndex + 1, finalIndex));

                    int firstIndexAd = episode.InnerText.IndexOf("(");
                    if(!airDate.Contains(episode.InnerText.Substring(firstIndexAd + 1, 10)))
                    {
                        airDate.Add(episode.InnerText.Substring(firstIndexAd + 1, 10));
                    }
                    
                }


            }

            //HtmlNode[] episodesNode = doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/table[" + 2 + "]/tr[@class='vevent']").ToArray();
            //foreach (HtmlNode episode in episodesNode)
            //{
            //    int firstIndex = episode.InnerText.IndexOf("\"");
            //    int lastIndex = episode.InnerText.LastIndexOf("\"");

            //    int finalIndex = episode.InnerText.Substring(firstIndex + 1, lastIndex).IndexOf("\"");

            //    episodes.Add(episode.InnerText.Substring(firstIndex + 1, finalIndex));
            //}


            //foreach(HtmlNode episode in episodesNode)
            //{
            //    episodes.Add(episode.InnerText);
            //}

            ViewBag.testing = airDate;



            return View("Index");
        }



    }
}
