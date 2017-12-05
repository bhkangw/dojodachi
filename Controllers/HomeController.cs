using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace dojodachi.Controllers
{
    public class HomeController : Controller
    {
        [HttpGetAttribute]
        [Route("")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetObjectFromJson<DojodachiInfo>("Dojodachi") == null)
            {
                HttpContext.Session.SetObjectAsJson("Dojodachi", new DojodachiInfo());
            }

            ViewBag.Dojodachi = HttpContext.Session.GetObjectFromJson<DojodachiInfo>("Dojodachi");
            ViewBag.Message = "You got a new Dojodachi!";
            ViewBag.GameStatus = "running";
            ViewBag.Reaction = "";

            if (ViewBag.Dojodachi.fullness > 99 && ViewBag.Dojodachi.happiness > 99 && ViewBag.Dojodachi.energy > 99)
                {
                    ViewBag.Message = "Congratulations! You won!";
                }

            return View();
        }

        [HttpGetAttribute]
        [Route("feed")]
        public IActionResult Feed()
        {
            DojodachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DojodachiInfo>("Dojodachi");
            Random random = new Random();
            ViewBag.GameStatus = "running";
            int fullnessAmount = random.Next(5, 11);
            int chance = random.Next(1,5);
            if(EditDachi.meals > 0)
            {
                EditDachi.meals--;
                if (chance == 1)
                {
                    ViewBag.Reaction = ":(";
                    ViewBag.Message = "You fed your Dojodachi! But he didn't like it. Fullness +0, Meals -1";
                }
                else
                {
                    EditDachi.fullness += fullnessAmount;
                    ViewBag.Reaction = ":)";
                    ViewBag.Message = $"You fed your Dojodachi! Fullness +{fullnessAmount}, Meals -1";
                }
            }
            HttpContext.Session.SetObjectAsJson("Dojodachi", EditDachi);
            ViewBag.Dojodachi = EditDachi;
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("play")]
        public IActionResult Play()
        {
            DojodachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DojodachiInfo>("Dojodachi");
            Random random = new Random();
            ViewBag.GameStatus = "running";
            int happinessAmount = random.Next(5, 11);
            int chance = random.Next(1, 5);
            
            if(EditDachi.energy > 4)
            {
                EditDachi.energy -= 5;
                if(chance == 1)
                {
                    ViewBag.Reaction = ":(";
                    ViewBag.Message = "You played with your Dojodachi! But he didn't like it. Happiness +0, Energy -5";
                }
                else
                {
                    EditDachi.happiness += happinessAmount;
                    ViewBag.Reaction = ":)";
                    ViewBag.Message = $"You played with your Dojodachi! Happiness +{happinessAmount}, Energy -5";
                }
            }
            HttpContext.Session.SetObjectAsJson("Dojodachi", EditDachi);
            ViewBag.Dojodachi = EditDachi;
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("work")]
        public IActionResult Work()
        {
            DojodachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DojodachiInfo>("Dojodachi");
            Random random = new Random();
            ViewBag.GameStatus = "running";
            int mealsAmount = random.Next(1, 4);
            int chance = random.Next(1, 5);
            
            if(EditDachi.energy > 4)
            {
                EditDachi.energy -= 5;
                EditDachi.meals += mealsAmount;
                ViewBag.Reaction = ":)";
                ViewBag.Message = $"You sent your Dojodachi to work! Meals +{mealsAmount}, Energy -5";
            }
            HttpContext.Session.SetObjectAsJson("Dojodachi", EditDachi);
            ViewBag.Dojodachi = EditDachi;
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("sleep")]
        public IActionResult Sleep()
        {
            DojodachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DojodachiInfo>("Dojodachi");
            Random random = new Random();
            EditDachi.fullness -= 5;
            EditDachi.happiness -= 5;
            EditDachi.energy += 15;
            ViewBag.GameStatus = "running";
            ViewBag.Reaction = ":)";
            ViewBag.Message = "Your Dojodachi is sleeping! Energy +15, Happiness -5, Fullness -5";
            
            HttpContext.Session.SetObjectAsJson("Dojodachi", EditDachi);
            ViewBag.Dojodachi = EditDachi;
            if (ViewBag.Dojodachi.fullness < 1 || ViewBag.Dojodachi.happiness < 1)
            {
                ViewBag.Message = "O no! Your dojodachi is ded";
            }
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("restart")]
        public IActionResult Restart()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index"); 
        }
    }
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
