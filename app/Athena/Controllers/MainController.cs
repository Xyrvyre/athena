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
using Microsoft.AspNetCore.Http;
using System.Collections;

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

        [HttpPost]
        public IActionResult Index(string UserId)
        {
            userName = UserId;
            HttpContext.Session.SetString("namespace", UserId);

            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile("./config");
            var client = new Kubernetes(k8SClientConfig);
            // create the namespace when logging in

            // save username from google api into Database Namespace 
            //and assign it to variable username
            // take username to Createcontroller

            // checking whether there is already a namespace
            var namespaces = client.ListNamespace();
            foreach (var n in namespaces.Items)
            {
                if (n.Metadata.Name == UserId) 
                { //ViewData["Message"] = HttpContext.Session.GetString("namespace");
                    return View(); } 
            }
            
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = UserId
                }
            };

            var result = client.CreateNamespace(ns);
            ViewData["Message"] = result;

            return View();
        }
    }
}
