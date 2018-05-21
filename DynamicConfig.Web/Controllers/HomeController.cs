using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using DynamicConfig.Infrastructure;
using System.Dynamic;
using DynamicConfig.Core.Interfaces;
using DynamicConfig.Core.Interfaces.Shared;
using DynamicConfig.Infrastructure.Data;

namespace DynamicConfig.Web.Controllers
{
    public class HomeController : Controller
    {
  
       private readonly IConfig _repository;

  
		public HomeController()
        {
			_repository = new ConfigirationReader(System.Reflection.Assembly.GetEntryAssembly().GetName().Name,"",TimeSpan.FromMinutes(1));
        }

        public IActionResult Index()
        {
			String aa  = _repository.GetValue<string>("SiteName");

			return View();
        }
      
    }
}
