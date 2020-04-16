using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Text.Json;
using k8s;
using k8s.Models;
using System.Text.Json.Serialization;


namespace Athena.Controllers
{
    public class LabelController : Controller
    {
        public IActionResult Index()
        {
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            var path = @"C:\Users\s\source\repos\SecLAb1\Deployment";
            //var jsonFilePod = System.IO.File.ReadAllText("C:\\Users\\s\\source\\repos\\Dep_Kub\\d.json");
            string a = "key database: class";
            string b= "value database: security testing class level 7";

            /*
             table User
             UserId: hyerim.kwon
             pw: 
             */

             /*
             1) sec test lab1
             2) network lab2
             */
             
            
             /*
             group table
             classname : sec l7
             members : hyerim.kwon
             */

            string c = "Sky"; // create the namespace when logging in
            //string d = "variable from a Template table"
            //path = d;

            var serialiseOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            foreach (string file in Directory.EnumerateFiles(path))
            {
                string fileContent = System.IO.File.ReadAllText(file);

                V1Deployment deployment = JsonSerializer.Deserialize<V1Deployment>(fileContent, serialiseOptions);

                var result = client.CreateNamespacedDeployment(deployment, c);
                Console.WriteLine(result);
            }
            /*
            var serialiseOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            // var serialisePod = JsonSerializer.Serialize<V1Pod>(pod, serialiseOptions);
            //var jsonFilePod = System.IO.File.ReadAllText("C:\\Users\\s\\source\\repos\\Dep_Kub\\no_label.json");
            var jsonFilePod = System.IO.File.ReadAllText("C:\\Users\\s\\source\\repos\\Dep_Kub\\no_label3.json");
            var deployment = new V1Deployment();
            deployment = JsonSerializer.Deserialize<V1Deployment>(jsonFilePod, serialiseOptions);

           *//* if (deployment.Metadata.Labels == null)
            {
                deployment.Metadata.Labels = new Dictionary<string, string>();
            }*//*
            deployment.Metadata.Labels = new Dictionary<string, string>();
            deployment.Metadata.Labels.Add("animal", "hare");
            deployment.Metadata.Labels.Add(a, b);
            var result = client.CreateNamespacedDeployment(deployment, "default");
            */
            //ViewData["Message"] = result;
            return View();

        }
    }
}
