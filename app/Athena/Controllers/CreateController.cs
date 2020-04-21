﻿using System;
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

namespace Athena.Controllers
{
    public class CreateController : Controller
    {

        private readonly AthenaContext _context;

        public CreateController(AthenaContext context)
        {
            _context = context;
        }

        //get it from Login
        string userName = "default";
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            
            string p = "";
            
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
                
                if (deployment.Metadata.Labels == null)
                {
                    deployment.Metadata.Labels = new Dictionary<string, string>();
                }

                var result = client.CreateNamespacedDeployment(deployment, "default");

                ViewData["Message"] = result;
            }
                        
            return View();
        }

        public IActionResult Template()
        {


            return View();
        }

        public IActionResult ChooseTemplate()
        {


            return View();
        }
    }
}