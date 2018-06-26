using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using MovieManager.Data;
using MovieManager.Models;
using MovieManager.ViewModels;

namespace MovieManager.Controllers
{
    public class UserController : Controller
    {

        private MovieDbContext context;

        public UserController(MovieDbContext dbContext)
        {
            context = dbContext;
        }


        public IActionResult Index()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://movieweb.com/movies/coming-soon/");

            // Gets names
            //HtmlNode[] node = doc.DocumentNode.SelectNodes("//div[@class='lister-item-content']/h3/a").ToArray();

            //List<string> test = new List<string>();

            //Gets link text
            //foreach(var n in node)
            //{
            //   test.Add(n.Attributes["href"].Value);
            //}

            //Get the first 4 image tags
            HtmlNode[] imgs = doc.DocumentNode.SelectNodes("//div[@class='new-movies-items']/section[@class='movie'][position()<5]/figure/a/img").ToArray();

            List<string> href = new List<string>();
            List<string> title = new List<string>();
            List<string> description = new List<string>();

            //Extract the link to the image and put it in a list
            foreach (var img in imgs)
            {
                href.Add(img.Attributes["src"].Value);
            }

            //Get the first 4 tags with titles
            HtmlNode[] titles = doc.DocumentNode.SelectNodes("//div[@class='new-movies-items']/section[@class='movie'][position()<5]/div[@class='movie-description']/h2/a").ToArray();

            //Extract the title names and add it to a list
            foreach (var t in titles)
            {
                title.Add(t.InnerText);
            }


            HtmlNode[] descriptions = doc.DocumentNode.SelectNodes("//div[@class='new-movies-items']/section[@class='movie'][position()<5]/div[@class='movie-description']/div[@class='movie-synopsis']/p").ToArray();

            foreach (var d in descriptions)
            {
                description.Add(d.InnerText);
            }

            ViewBag.href = href;
            ViewBag.title = title;
            ViewBag.description = description;

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
           

            return Redirect("/User");
        }

    }
}
