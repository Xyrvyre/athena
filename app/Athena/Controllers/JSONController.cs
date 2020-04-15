using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using k8s;
using k8s.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Controllers
{
    public class JSONController : Controller
    {
        public IActionResult Index()
        {
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
                      

            var serialiseOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            // var serialisePod = JsonSerializer.Serialize<V1Pod>(pod, serialiseOptions);
            var jsonFilePod = System.IO.File.ReadAllText("C:\\Users\\s\\source\\repos\\d.json");
            var deployment = new V1Deployment();
            deployment = JsonSerializer.Deserialize<V1Deployment>(jsonFilePod, serialiseOptions);

            //File.WriteAllText("C:\\Users\\Laptop User\\pod.json", serialisePod);
            var result = client.CreateNamespacedDeployment(deployment, "default");
            ViewData["Message"] = result;
            return View();
        }
    }
}
