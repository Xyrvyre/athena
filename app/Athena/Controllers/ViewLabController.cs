using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Controllers
{
    public class ViewLabController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}