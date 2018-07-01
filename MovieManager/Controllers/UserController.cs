using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using MovieManager.Data;
using MovieManager.Models;
using MovieManager.ViewModels;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MovieManager.Controllers
{

    
    public class UserController : Controller
    {
        // Temporary to store user in session
        public static Dictionary<string, string> session = new Dictionary<string, string>();

        public static List<string> notifiedNames= new List<string>();

        static async Task SendSms(string phoneNUmber)
        {
          
            const string accountSid = "";
            const string authToken = "";

            TwilioClient.Init(accountSid, authToken);
            
            List<Media> medias = MediaController.mediaToText;

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

            foreach (Media media in medias)
            {
                foreach (Episode episode in media.Episodes)
                {
                    if (notifiedNames.Contains(media.TvShow + " Season " + episode.Season + " Episode " + episode.EpisodeNumber + " is ready to be watched!"))
                    {
                        break;
                    }

                    if (Int32.Parse(episode.AirDate.Substring(0, 4)) == dateYear && Int32.Parse(episode.AirDate.Substring(5, 2)) == dateMonth && (Int32.Parse(episode.AirDate.Substring(8, 2)) == dateDay || Int32.Parse(episode.AirDate.Substring(8, 2)) == dateDay + 1))
                        {
                            
                            notifiedNames.Add(media.TvShow + " Season " + episode.Season + " Episode " + episode.EpisodeNumber + " is ready to be watched!");

                            var message = await MessageResource.CreateAsync(
                            body: media.TvShow + " Season " + episode.Season + " Episode " + episode.EpisodeNumber + " is ready to be watched!",
                            from: new Twilio.Types.PhoneNumber("+13145825488"),
                            to: new Twilio.Types.PhoneNumber("+1" + phoneNUmber)
                            );
                            
                        }
                    
                    
                }
            }

                
        }

        public IActionResult SetDate()
        {
            Episode episode = context.Episodes.First(e => e.ID == 1);
            
            episode.AirDate = "2018-07-01";
            context.SaveChanges();

            return Redirect("/User");
        }

        public IActionResult SendText()
        {
            User user = context.Users.Single(u => u.UserName == UserController.session["user"]);
            string phoneNumber = user.PhoneNumber;

            if (phoneNumber.Contains("-"))
            {
                phoneNumber = phoneNumber.Replace("-", "");
            }

            if(phoneNumber.Substring(0,1) == "1")
            {
                phoneNumber = phoneNumber.Substring(1);
            }
            
            SendSms(phoneNumber).Wait();
            MediaController.mediaToText.Clear();
            return Redirect("/Media");
        }

        private readonly MovieDbContext context;

        public UserController(MovieDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Logout()
        {
            session.Remove("user");
            return Redirect("/User/Login");
        }

        public IActionResult Index()
        {
            if (!session.ContainsKey("user"))
            {
                return Redirect("/User/Login");
            }

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://movieweb.com/movies/coming-soon/");

            //Get the first 4 image tags
            HtmlNode[] imgs = doc.DocumentNode.SelectNodes("//div[@class='new-movies-items']/section[@class='movie'][position()<5]/figure/a/img").ToArray();

            List<string> href = new List<string>();
            List<string> title = new List<string>();
            List<string> description = new List<string>();
            List<string> link = new List<string>();
            List<string> releaseDate = new List<string>();

            //Extract the link to the image and put it in a list
            foreach (var img in imgs)
            {
                href.Add(img.Attributes["src"].Value);
            }
             
            //Get the first 4 tags with titles
            HtmlNode[] titles = doc.DocumentNode.SelectNodes("//div[@class='new-movies-items']/section[@class='movie'][position()<5]/div[@class='movie-description']/h2/a").ToArray();

            //Extract the title names and links and add it to a list
            foreach (var t in titles)
            {
                title.Add(t.InnerText);
                link.Add(t.Attributes["href"].Value);
            }

            // Get the first 4 descriptions
            HtmlNode[] descriptions = doc.DocumentNode.SelectNodes("//div[@class='new-movies-items']/section[@class='movie'][position()<5]/div[@class='movie-description']/div[@class='movie-synopsis']/p").ToArray();

            //Extract the description and add it to a list
            foreach (var d in descriptions)
            {
                description.Add(d.InnerText);
            }

            //Get the first 4 release dates
            HtmlNode[] releaseDates = doc.DocumentNode.SelectNodes("//div[@class='new-movies-items']/section[@class='movie'][position()<5]/div[@class='movie-description']/div[@class='movie-info']/ul/li[@class='movie-release-date']/time").ToArray();

            //Extract the date and add it to a list
            foreach (var rd in releaseDates)
            {
                releaseDate.Add(rd.InnerText);
            }
            
            ViewBag.href = href;
            ViewBag.title = title;
            ViewBag.description = description;
            ViewBag.link = link;
            ViewBag.releaseDate = releaseDate;
            
            return View();
        }

        public IActionResult Register()
        {

            RegisterUserViewModel registerUserViewModel = new RegisterUserViewModel();

            return View(registerUserViewModel);
        }

        [HttpPost]
        public IActionResult Register(RegisterUserViewModel registerUserViewModel, string verifyPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(registerUserViewModel);
            }

            if (context.Users.FirstOrDefault(u => u.UserName.Equals(registerUserViewModel.UserName)) != null)
            {
                ViewBag.userExistsError = "That user already exists";
                return View();
            }

            if (registerUserViewModel.Password.Equals(verifyPassword))
            {
                User newUser = new User
                {
                    UserName = registerUserViewModel.UserName,
                    Password = registerUserViewModel.Password

                };

                context.Users.Add(newUser);
                context.SaveChanges();
                session.Add("user", registerUserViewModel.UserName);

                return Redirect("/User");
            }

            return View();
        }

        public IActionResult Login()
        {
            LoginUserViewModel loginUserViewModel = new LoginUserViewModel();

            return View(loginUserViewModel);
        }

        [HttpPost]
        public IActionResult Login(LoginUserViewModel loginUserViewModel)
        {

            User user = context.Users.FirstOrDefault(u => u.UserName.Equals(loginUserViewModel.UserName));

            if (!ModelState.IsValid)
            {
                return View(loginUserViewModel);
            }

            if (user == null)
            {
                ViewBag.nameError = "That user doesn't exist";
                return View();
            }

            if (!user.Password.Equals(loginUserViewModel.Password))
            {
                ViewBag.passwordError = "Incorrect password";
                return View();
            }

            session.Add("user", loginUserViewModel.UserName);


            return Redirect("/User");
        }

        public IActionResult Settings()
        {
            if (!session.ContainsKey("user"))
            {
                return Redirect("/User/Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Settings(string phoneNumber)
        {
            

            User user = context.Users.Single(u => u.UserName == UserController.session["user"]);
            
            user.PhoneNumber = phoneNumber;
            
            context.SaveChanges();
            
            return Redirect("/User");
        }

    }
}
