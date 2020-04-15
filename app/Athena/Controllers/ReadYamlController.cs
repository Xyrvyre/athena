using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.AspNetCore.Mvc;
using k8s.Models;
using k8s;
using YamlDotNet.RepresentationModel;

namespace Athena.Controllers
{
    public class ReadYamlController : Controller
    {
        public IActionResult Index()
        {
            string pathSource = @"C:\Users\s\source\repos\a2.yaml";
             //string pathSource=@"C:\Users\s\source\repos\y.txt";
            System.IO.TextReader readFile = new StreamReader(pathSource);

                //var input = new StringReader(pathSource);
                //StreamReader input = new StreamReader(pathSource);
             //var content = readFile.ReadToEnd();
             /*var yaml = new YamlStream();
             yaml.Load(readFile);*/

            string line = null;
            line = readFile.ReadToEnd();
                if (line != null)
                ViewData["Message"] = line;
                
                //readFile.Close();
                //readFile = null;

              return View();
            }
    }
}