using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using k8s.Models;
using k8s;
using System.Text;

namespace Athena.Controllers
{
    public class Yaml2Controller : Controller
    {
        public IActionResult Index()
        {
            /*var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
           
            var namespaces = client.ListNamespace();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var ns in namespaces.Items)
            {
                Console.WriteLine(ns.Metadata.Name);
                var list = client.ListNamespacedPod(ns.Metadata.Name);

                foreach (var item in list.Items)
                {
                    ViewData["Message"] = stringBuilder.Append(item.Metadata.Name);
                    Console.WriteLine(item.Metadata.Name);
                }
            } 
                return View();*/

             V1Deployment deployment = new V1Deployment()
             {
                 ApiVersion = "apps/v1",
                 Kind = "Deployment",
                 Metadata = new V1ObjectMeta()
                 {
                     Name = "nginx-deployment5",
                     Labels = new Dictionary<string, string>()
                             {
                                 { "app", "nginx" }
                             }
                 },
                 Spec = new V1DeploymentSpec
                 {
                     Replicas = 1,
                     Selector = new V1LabelSelector()
                     {
                         MatchLabels = new Dictionary<string, string>
                                 {
                                     { "app", "nginx" }
                                 }
                     },
                     Template = new V1PodTemplateSpec()
                     {
                         Metadata = new V1ObjectMeta()
                         {
                             CreationTimestamp = null,
                             Labels = new Dictionary<string, string>
                                     {
                                          { "app", "nginx" }
                                     }
                         },
                         Spec = new V1PodSpec
                         {
                             Containers = new List<V1Container>()
                                     {
                                         new V1Container()
                                         {
                                             Name = "nginx",
                                             Image = "nginx:1.14.2",
                                             Ports = new List<V1ContainerPort> { new V1ContainerPort(80) }
                                         }
                                     }
                         }
                     }
                 }
                 /*Status = new V1DeploymentStatus()
                 {
                     Replicas = 1
                 }*/
             };
             
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            var result = client.CreateNamespacedDeployment(deployment, "default");

            ViewData["Message"] = result;
            return View();
        }
    }
}
