using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context {get; set; }
        private PasswordHasher<User> regHasher = new PasswordHasher<User>();
        private PasswordHasher<Login> logHasher = new PasswordHasher<Login>();
        public User GetUser()
        {
            return _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId"));
            //putting the session search in a variable that we can call throughout.
        }


        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost("register")]
        public IActionResult CreateUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                //seeing if the email has been used, already in the DB.
                if(_context.Users.FirstOrDefault(usr => usr.Email == newUser.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use, try logging in!");
                    return View("Index");
                    //land here if the email is in DB.
                }
                string hash = regHasher.HashPassword(newUser, newUser.Password);
                newUser.Password = hash;
                //hashing password
                _context.Users.Add(newUser);
                _context.SaveChanges();
                //saving to database
                HttpContext.Session.SetInt32("userId", newUser.UserId);
                //adding the new user into session to be userId
                return Redirect ("/home");
                }
            return View("Index");
            //if validations aren't met.
        }


        [HttpPost("login")]
        public IActionResult Login(Login lu)
        {
            if(ModelState.IsValid)
            //if validations are met
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == lu.LoginEmail);
                //comparing the logged in email(lu) w/ a user in the DB.
                if(userInDb == null)
                //check if user is in DB.
                //if it is not in DB
                {
                    ModelState.AddModelError("LoginEmail", "Email not in database.");
                    return View("Index");
                }
                var result = logHasher.VerifyHashedPassword(lu, userInDb.Password, lu.LoginPassword);
                //comparing hashed password in DB against hashed password from logging in user
                if(result == 0)
                //if there is an error
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Email or Password!");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                //logging user in by setting in session
                return Redirect("/home");
            }
            return View("Index");
            //landed if validations aren't met.
        }


        [HttpGet("home")]
        public IActionResult Home()
        {
            User current = GetUser();
            //setting variable of current to current user in DB from the GetUser Function
            if(current == null)
            //checking to see if there is a user logged into session (logged in)
            //if not, redirect.
            {
                return Redirect("/");
            }
            List<MeetUp> AllActivities = _context.Activities
                            .Include(m => m.Coordinator)
                            .Include(m => m.Participants)
                            .ThenInclude(wp => wp.Tagalong)
                            .Where(m => m.Date >= DateTime.Now)
                            .OrderBy(m => m.Date)
                            .ToList();
            ViewBag.User = current;
            return View(AllActivities);
        }

        [HttpGet("activity/{meetupId}/status")]
        public IActionResult ToggleActivity(int meetupId, string status)
        {
            User current = GetUser();
            if(current == null)
            {
                return Redirect("/");
            }
            if(status == "join")
            {
                Association newAct = new Association();
                newAct.UserId = current.UserId;
                newAct.MeetUpId = meetupId;
                //making the join
                _context.Associations.Add(newAct);
                //creating a new watchparty which means association
            }
            else if(status == "leave")
            {
                Association backout = _context.Associations
                                    .FirstOrDefault(w => w.UserId == current.UserId && w.MeetUpId == meetupId);
                                    _context.Associations.Remove(backout);
                                    //we need the & statement to detect movie ID.
                                    //making sure they are equal.
            }
            _context.SaveChanges();
            return RedirectToAction("Home");
            }

            [HttpGet("activity/{meetupId}")]
            public IActionResult DisplayActivity(int meetupId)
            {
            User current = GetUser();
            if(current == null)
            {
                return Redirect("/");
                //checking the ID 
            }
            MeetUp showing = _context.Activities
                            .Include(m => m.Participants)
                            .ThenInclude(u => u.Tagalong)
                            .Include(m => m.Coordinator)
                            .FirstOrDefault(m => m.MeetUpId == meetupId);
                            //grab the activity that matches the route
            ViewBag.User = current;
            return View(showing);
            }

        [HttpGet("activity/{meetupId}/delete")]
        public IActionResult DeleteActivity(int meetupId)
        {
            User current = GetUser();
            if(current == null)
            {
                return Redirect("/");
                //checking the ID 
            }
            MeetUp remove = _context.Activities.FirstOrDefault(m => m.MeetUpId == meetupId);
            _context.Activities.Remove(remove);
            _context.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpGet("activity/new")]
        public IActionResult NewActivity()
        { 
            User current = GetUser();
            if(current == null)
            {
                return Redirect("/");
            }
            return View();
        }
        
        [HttpPost("activity/create")]
        public IActionResult CreateMovie(MeetUp newActivity)
        {
            User current = GetUser();
            if(current == null)
            {
                return Redirect("/");
                //checking the ID 
            }
            if(ModelState.IsValid)
            {
                newActivity.UserId = current.UserId;
                //putting the id of the person logged in, into the id of the new movie being created
                _context.Activities.Add(newActivity);
                _context.SaveChanges();
                return RedirectToAction("home");
            }
            return View("NewActivity");
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect ("/");
        }
    }
}

