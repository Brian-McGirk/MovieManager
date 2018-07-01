using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieManager.Data;
using MovieManager.Models;
using MovieManager.ViewModels;

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

        public static List<Media> mediaToText = new List<Media>();
         
        public IActionResult Index()
        {
            if (!UserController.session.ContainsKey("user"))
            {
                return Redirect("/User/Login");
            }

            DateTime thisDay = DateTime.Today;

            string dateString = thisDay.ToString("d");

            int index = dateString.IndexOf("/");

            string dateMonthString = dateString.Substring(0, index);
            string dateDayString = dateString.Substring(index + 1, 2);

            if (dateDayString.Contains("/"))
            {
                dateDayString = dateString.Substring(index + 1, 1);
            }

            int lastIndex = dateString.LastIndexOf("/");

            int dateMonth = Int32.Parse(dateMonthString);
            int dateDay = Int32.Parse(dateDayString);
            int dateYear = Int32.Parse(dateString.Substring(lastIndex + 1, 4));

            User user = context.Users.Include(m => m.Medias).ThenInclude(e => e.Episodes).Single(u => u.UserName == UserController.session["user"]);
            List<Media> medias = user.Medias.ToList();

            List<Media> mediasToDisplay = new List<Media>();
            
             
            bool allSeen = true;

            foreach(Media media in medias)
            {
                foreach(Episode episode in media.Episodes)
                {
                    if (!UserController.notifiedNames.Contains(media.TvShow + " Season " + episode.Season + " Episode " + episode.EpisodeNumber + " is ready to be watched!"))
                    {
                        if (Int32.Parse(episode.AirDate.Substring(0, 4)) == dateYear && Int32.Parse(episode.AirDate.Substring(5, 2)) == dateMonth && (Int32.Parse(episode.AirDate.Substring(8, 2)) == dateDay || Int32.Parse(episode.AirDate.Substring(8, 2)) == dateDay + 1))
                        {
                            mediaToText.Add(media);
                        }
                    }
                    

                    if (!episode.Seen)
                    {
                        allSeen = false;
                        break;
                    }
                    //Int32.Parse(episode.AirDate.Substring(0, 4)) > Model.dateYear || (Int32.Parse(episode.AirDate.Substring(5, 2))
                    //  == Model.dateMonth && Int32.Parse(episode.AirDate.Substring(8, 2)) > Model.dateDay)
                    //System.Diagnostics.Debug.WriteLine("TODAYS STUFF:: " + dateYear + "---" + dateMonth + "---" + dateDay);
                   // System.Diagnostics.Debug.WriteLine("EPISODE STUFF:: " + Int32.Parse(episode.AirDate.Substring(0, 4)) + "---" + Int32.Parse(episode.AirDate.Substring(5, 2)) + "---" + Int32.Parse(episode.AirDate.Substring(8, 2)));
                   
                }

                if (!allSeen)
                {
                    mediasToDisplay.Add(media);
                    allSeen = true;
                }

            }
            
            MyShowsViewModel myShowsViewModel = new MyShowsViewModel
            {
                Medias = mediasToDisplay,
                dateMonth = dateMonth,
                dateDay = dateDay,
                dateYear = dateYear

            };



            if (mediaToText.Count != 0)
            {
                return Redirect("/User/SendText");
            }

            return View(myShowsViewModel);
        }

        [HttpPost]
        public IActionResult SeenTvSeries(int id)
        {
           
            Episode episode = context.Episodes.Single(e => e.ID == id);

            episode.Seen = true;
            context.SaveChanges();

            return Redirect("/Media");
        }

        public IActionResult Copy(int id)
        {
            //User user = context.Users.Include(m => m.Medias).ThenInclude(ep => ep.Episodes).Single(u => u.UserName == UserController.session["user"]);

            //List<Media> medias = user.Medias.ToList();

            //Episode episode = context.Episodes.Single(e => e.ID == id);


            //foreach (Media media in medias)
            //{
            //    foreach (Episode episode in media.Episodes)
            //    {

            //if (!episode.Seen)
            //{
            //if (episode.Season.ToString().Length == 1 && episode.EpisodeNumber.ToString().Length == 1)
            //{
            //    Clipboard.SetText(media.TvShow + "s0" + episode.Season.ToString() + "e0" + episode.EpisodeNumber.ToString());
            //}
            //else if (episode.Season.ToString().Length == 1 && episode.EpisodeNumber.ToString().Length == 2)
            //{
            //    Clipboard.SetText(media.TvShow + "s0" + episode.Season.ToString() + "e" + episode.EpisodeNumber.ToString());
            //}
            //else if (episode.Season.ToString().Length == 2 && episode.EpisodeNumber.ToString().Length == 1)
            //{
            //    Clipboard.SetText(media.TvShow + "s" + episode.Season.ToString() + "e0" + episode.EpisodeNumber.ToString());
            //}
            //else if (episode.Season.ToString().Length == 2 && episode.EpisodeNumber.ToString().Length == 2)
            //{
            //    Clipboard.SetText(media.TvShow + "s" + episode.Season.ToString() + "e" + episode.EpisodeNumber.ToString());
            //}
            //}
            // TODO copy to clipboard based on seen status and date
            //    }
            //}
            //keyEvent.KeyCode == Keys.ControlKey &&
            //if (keyEvent.KeyCode == Keys.V)
            //{
            //    pasted = true;
            //}







            //DateTime thisDay = DateTime.Today;

            //string dateString = thisDay.ToString("d");

            //int index = dateString.IndexOf("/");

            //string dateMonthString = dateString.Substring(0, index);
            //string dateDayString = dateString.Substring(index+1, 2);

            //int dateMonth = Int32.Parse(dateMonthString);
            //int dateDay = Int32.Parse(dateDayString);
            //int dateYear = Int32.Parse(dateString.Substring(5, 4));



            //System.Diagnostics.Debug.WriteLine(dateMonthString);
            //System.Diagnostics.Debug.WriteLine(dateDay);
            //System.Diagnostics.Debug.WriteLine(dateYear);


            return Redirect("/Media");
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
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.imdb.com/find?ref_=nv_sr_fn&q=" + searchTermJoined + "&s=all");

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
                if (!s.ToLower().Equals("of"))
                {
                    searchTermCapital += s.Substring(0, 1).ToUpper() + s.Substring(1) + " ";
                }
                else
                {
                    searchTermCapital += s.ToLower() + " ";
                }
                
            }

            searchTermSplit = searchTermCapital.Split(' ');

            string searchTermJoined = String.Join("_", searchTermSplit);

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://en.wikipedia.org/wiki/List_of_" + searchTermJoined + "episodes");

            

            HtmlNode[] seasonsNode = doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/h3").ToArray();
            // For testing
            System.Diagnostics.Debug.WriteLine("SEASON NODE LENGTH" + seasonsNode.Length);

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

            // Media newMediaTvShow = context.Medias.Single(m => m.TvShow == searchTermCapital);
            User newUser = context.Users.Single(u => u.UserName == UserController.session["user"]);

            Media isMedia = context.Medias.FirstOrDefault(m => m.TvShow == searchTermCapital);

            if(isMedia == null)
            {
                Media newMedia = new Media
                {
                    TvShow = searchTermCapital,
                    User = newUser
                };

                context.Add(newMedia);
                context.SaveChanges();
            }

            if (newUser.Medias.Contains(isMedia))
            {
                ViewBag.error = searchTermCapital + " is already in your list";
                return View("Error");
            }





            //Media newMedia = new Media
            //{
            //    TvShow = searchTermCapital,
            //    User = newUser
            //};

            //context.Add(newMedia);
            //context.SaveChanges();

            List<string> seasons = new List<string>();
                List<string> episodes = new List<string>();
                List<string> airDate = new List<string>();

                foreach (HtmlNode season in seasonsNode)
                {
                    int index = season.InnerText.IndexOf("(");
                
                if (index < 0)
                {
                    break;
                }
                
                if (!seasons.Contains(season.InnerText.Substring(0, index)))
                {
                        seasons.Add(season.InnerText.Substring(0, index + 1));
                }
                else
                {
                    break;
                }

                System.Diagnostics.Debug.WriteLine("SEASON TEXT::" + season.InnerText.Substring(0, 6));
                if (season.InnerText.ToLower().Substring(0, 6) != "season")
                {
                    break;
                }

                    int episodeNumberCounter = 1;
                
                
                HtmlNode[] episodesNode = doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/table[" + (Int32.Parse(season.InnerText.Substring(7, 1)) + 1) + "]/tr[@class='vevent']").ToArray();
                    foreach (HtmlNode episode in episodesNode)
                    {


                        int firstIndex = episode.InnerText.IndexOf("\"");
                        int lastIndex = episode.InnerText.LastIndexOf("\"");
                    
                    if (lastIndex < 0)
                    {
                        break;
                    }

                    int finalIndex = episode.InnerText.Substring(firstIndex + 1, lastIndex).IndexOf("\"");

                        episodes.Add(episode.InnerText.Substring(firstIndex + 1, finalIndex));

                        int firstIndexAd = episode.InnerText.IndexOf("(");
                        if (!airDate.Contains(episode.InnerText.Substring(firstIndexAd + 1, 10)))
                        {
                            Episode newEpisode = new Episode
                            {
                                EpisodeName = episode.InnerText.Substring(firstIndex + 1, finalIndex),
                                Season = Int32.Parse(season.InnerText.Substring(7, 1)),
                                EpisodeNumber = episodeNumberCounter,
                                AirDate = episode.InnerText.Substring(firstIndexAd + 1, 10),
                                Seen = false,
                                Media = context.Medias.First(m => m.TvShow == searchTermCapital)

                            };
                            context.Add(newEpisode);
                            context.SaveChanges();
                            episodeNumberCounter++;
                            airDate.Add(episode.InnerText.Substring(firstIndexAd + 1, 10));
                        }

                    }


                }

            
            //else
            //{
            //    Media isMedia2 = context.Medias.Include(ep => ep.Episodes).Single(m => m.TvShow == searchTermCapital);
                

            //    Media newMedia = new Media
            //    {
            //        TvShow = searchTermCapital,
            //        User = newUser,
            //        Episodes = isMedia2.Episodes.ToList()


            //};

            //    context.Add(newMedia);
            //    context.SaveChanges();
            //}

            
            return Redirect("/Media");




            //Episode newEpisode = new Episode
            //{
            //    EpisodeName = "",
            //    Season = 0,
            //    EpisodeNumber = 0,
            //    AirDate = "",
            //    Seen = false,
            //    Media = newMediaTvShow

            //};

            //List<string> seasons = new List<string>();
            //List<string> episodes = new List<string>();
            //List<string> airDate = new List<string>();



            //foreach (HtmlNode season in seasonsNode)
            //{
            //    int index = season.InnerText.IndexOf("(");

            //    if (!seasons.Contains(season.InnerText.Substring(0, index)))
            //    {
            //        seasons.Add(season.InnerText.Substring(0, index));
            //    }
            //    int episodeNumberCounter = 1;

            //    HtmlNode[] episodesNode = doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/table[" + (Int32.Parse(season.InnerText.Substring(7, 1)) + 1) + "]/tr[@class='vevent']").ToArray();
            //    foreach (HtmlNode episode in episodesNode)
            //    {


            //        int firstIndex = episode.InnerText.IndexOf("\"");
            //        int lastIndex = episode.InnerText.LastIndexOf("\"");

            //        int finalIndex = episode.InnerText.Substring(firstIndex + 1, lastIndex).IndexOf("\"");

            //        episodes.Add(episode.InnerText.Substring(firstIndex + 1, finalIndex));

            //        int firstIndexAd = episode.InnerText.IndexOf("(");
            //        if(!airDate.Contains(episode.InnerText.Substring(firstIndexAd + 1, 10)))
            //        {
            //            Episode newEpisode = new Episode
            //            {
            //                EpisodeName = episode.InnerText.Substring(firstIndex + 1, finalIndex),
            //                Season = Int32.Parse(season.InnerText.Substring(7, 1)),
            //                EpisodeNumber = episodeNumberCounter,
            //                AirDate = episode.InnerText.Substring(firstIndexAd + 1, 10),
            //                Seen = false,
            //                Media = context.Medias.First(m => m.TvShow == searchTermCapital)

            //            };
            //            context.Add(newEpisode);
            //            context.SaveChanges();
            //            episodeNumberCounter++;
            //            airDate.Add(episode.InnerText.Substring(firstIndexAd + 1, 10));
            //        }

            //    }


            //}

            //return Redirect("/Media");

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

            //ViewBag.testing = airDate;



            //return View("Index");
        }



    }
}
