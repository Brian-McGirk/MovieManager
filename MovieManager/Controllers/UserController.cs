using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
