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
    public class MainController : Controller
    {
        private readonly AthenaContext _context;

        public MainController(AthenaContext context)
        {
            _context = context;
        }
        string userName = "sky";

        public IActionResult Index()
        {

            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            // create the namespace when logging in

           // save username from google api into Database Namespace 
                                        //and assign it to variable username
                                        // take username to Createcontroller

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
