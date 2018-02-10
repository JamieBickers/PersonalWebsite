using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace PersonalWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("projects/{projectName}")]
        public IActionResult Projects(string projectName)
        {
            var configurationSection = configuration.GetSection($"Projects:{projectName}");
            var project = new Project();
            configurationSection.Bind(project);

            return View(project);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
