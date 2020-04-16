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
    public class YAMLController : Controller
    {

        public IActionResult Index()
        {
            string pathSource = @"C:\Users\s\source\repos\a2.yaml";
            System.IO.TextReader readFile = new StreamReader(pathSource);

            //var input = new StringReader(pathSource);
            //StreamReader input = new StreamReader(pathSource);
            //var content = readFile.ReadToEnd();
            var yaml = new YamlStream();
            yaml.Load(readFile);

            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
            StringBuilder sb = new StringBuilder();
            foreach (var entry in mapping.Children)
            {
                sb.Append(((YamlScalarNode)entry.Key).Value);
            }


            // List all the items
            //var items = (YamlSequenceNode)mapping.Children[new YamlScalarNode("aruco_bc_markers")];
            ViewData["Message"] = sb;
            return View();


            //YamlNode n = yaml.Documents.First().RootNode;

            //ViewData["Message"] = n;
            //ViewData["Message"] = m3;
            /* var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
              var client = new Kubernetes(k8SClientConfig);

                          try
              {
                  V1Deployment deployment = new V1Deployment();
                  var result = client.CreateNamespacedDeployment(deployment, "default");
                  ViewData["Message"] = result;
              }
              catch (Microsoft.Rest.HttpOperationException ex) {
                  ViewData["Message"] = ex.StackTrace;
                  //https://stackoverflow.com/questions/56640917/identifying-erroneous-field-in-kubernetes-deployment-spec-in-c-client
              }*/


            //return View();
        }
    }
}
