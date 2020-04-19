using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using k8s;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Athena.Data;



namespace Athena.Controllers
{
    public class LoginController : Controller
    {
        private readonly AthenaContext _context;

        public LoginController(AthenaContext context)
        {
            _context = context;
        }
        string userName = "default";

        public IActionResult Index()
        {

            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            // create the namespace when logging in

            var optionsBuilder = new DbContextOptionsBuilder<AthenaContext>();
            optionsBuilder.UseSqlServer("AthenaContext");


            using (var context = new AthenaContext(optionsBuilder.Options))
            {
                var label = _context.User.FirstOrDefaultAsync(m => m.UserId == "hyerim.kwon");

                //userName = label.UserId;
            }

            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = userName
                }
            };

            var result = client.CreateNamespace(ns);
            ViewData["Message"] = result;
            return View();
        }
    }
}
