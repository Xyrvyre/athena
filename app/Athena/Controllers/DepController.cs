using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using k8s.Models;
using k8s;
using System.IO;
using System.Text;


namespace Athena.Controllers
{
    public class DepController : Controller
    {
        public IActionResult Index()
        {

            string pathSource = @"C:\Users\s\source\repos\Dep_Kub\a2.yaml";            
            System.IO.TextReader readFile = new StreamReader(pathSource);
            string line = null;
            line = readFile.ReadToEnd();
            string[] words = line.Split(new Char[] { ' ', '\n', '\r' });
            //string a = "apps/v1";
            string a = words[1];
            /*
            V1Deployment deployment = new V1Deployment()
            {
                ApiVersion = words[1],
                Kind = words[3],
                Metadata = new V1ObjectMeta()
                {
                    Name = words[8],
                    Labels = new Dictionary<string, string>()
                    {
                        { words[16].Trim(':'), words[17] }
                    }
                },
                Spec = new V1DeploymentSpec
                {
                    Replicas = Convert.ToInt32(words[22]),
                    Selector = new V1LabelSelector()
                    {
                        MatchLabels = new Dictionary<string, string>
                                 {
                                     { words[37].Trim(':'), words[38]}
                                 }
                    },
                    Template = new V1PodTemplateSpec()
                    {
                        Metadata = new V1ObjectMeta()
                        {
                            CreationTimestamp = null,
                            Labels = new Dictionary<string, string>
                                     {
                                          { words[62].Trim(':'), words[63] }
                                     }
                        },
                        Spec = new V1PodSpec
                        {
                            Containers = new List<V1Container>()
                                     {
                                         new V1Container()
                                         {
                                             Name = words[84],
                                             Image = words[94],
                                             Ports = new List<V1ContainerPort> { new V1ContainerPort(Convert.ToInt32(words[114])) }
                                         }
                                     }
                        }
                    }
                }
 
               /* Status = new V1DeploymentStatus()
                {
                    Replicas = 1
                }
            };*/



            V1Deployment deployment = new V1Deployment()
            {
                ApiVersion = a,
                Kind = "Deployment",
                Metadata = new V1ObjectMeta()
                {
                    Name = "nginx-deployment11",
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

            /*var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            var result = client.CreateNamespacedDeployment(deployment, "default");*/

            ViewData["Message"] = result;
            return View();
        }
    }
}