using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using k8s;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Athena.Data;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.AspNetCore.Http;

namespace Athena.Controllers
{
    public class CreateController : Controller
    {
        string p = "";
        private readonly AthenaContext _context;

        public CreateController(AthenaContext context)
        {
            _context = context;
        }
        //string userName = "default"; //get it from Login
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            //string userName = HttpContext.Session.GetString("namespace");
            string userName = HttpContext.Session.GetString("namespace");
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            //var jsonFilePod = System.IO.File.ReadAllText("C:\\Users\\s\\source\\repos\\Dep_Kub\\d.json");
            //string p = "";
            string a = "class";//key
            string b = "SecurityL7"; // value
                                     // create the namespace when logging in

            var optionsBuilder = new DbContextOptionsBuilder<AthenaContext>();
            optionsBuilder.UseSqlServer("AthenaContext");
            using (var context = new AthenaContext(optionsBuilder.Options))
            {
                var path = await _context.Template.FirstOrDefaultAsync(m => m.Id == 1);

                p = path.Path;

            }

            //YAML

            var deserializeYAML = new DeserializerBuilder()
                  .WithNamingConvention(CamelCaseNamingConvention.Instance)
                  .Build();

            foreach (string file in Directory.EnumerateFiles(p))
            {

                StreamReader fileContent = System.IO.File.OpenText(file);

                V1Deployment deployment = deserializeYAML.Deserialize<V1Deployment>(fileContent);

                var result = client.CreateNamespacedDeployment(deployment, userName);

                ViewData["Message"] = result;
            }
            return View();
        }


        public async System.Threading.Tasks.Task<IActionResult> Template(int? id)
        {


            var optionsBuilder = new DbContextOptionsBuilder<AthenaContext>();
            optionsBuilder.UseSqlServer("AthenaContext");
            using (var context = new AthenaContext(optionsBuilder.Options))
            {
                var path = await _context.Template.FirstOrDefaultAsync(m => m.Id == id);
                p = path.Path;


                //HttpContext.Session.SetString("path", p);
 
            }

            ViewBag.path = p;
            return View();
        }

        public async System.Threading.Tasks.Task<IActionResult> ChooseTemplate()
        {
            return View(await _context.Template.ToListAsync());
        }
        
    }
}
