using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EsepteSite.Models;
using EsepteSite.Data;

namespace EsepteSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _db)
        {
            _logger = logger;
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dev()
        {
            return View();
        }

        public IActionResult CreateMessage(bool check1, bool check2, bool check3, bool check4, string ClientWants, string ClientContacs)
        {
            string types = "";
            if (check1)
            {
                types += "сайт ";
            }
            if (check2)
            {
                types += "мобильное приложение ";
            }
            if (check3)
            {
                types += "инфосистема ";
            }
            if (check4)
            {
                types += "компьютерная программа ";
            }

            Messages message = new Messages()
            {
                ClientBusiness = types,
                ClientWants = ClientWants,
                ClientContacts = ClientContacs,
                Date = DateTime.Now
            };

            db.Messages.Add(message);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Read(string p)
        {
            if (p == "porsche")
            {
                ViewBag.Messages = db.Messages.ToList();
                return View();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
