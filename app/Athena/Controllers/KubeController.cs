using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using k8s;
using k8s.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Athena.Controllers
{
    public class KubeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);

            var pod = new V1Pod()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta()
                {
                    Name = "test-pod",
                    Labels = new Dictionary<string, string>()
                {
                    { "app", "nepomucen" }
                }
                },
                Spec = new V1PodSpec()
                {
                    Containers = new List<V1Container>()
                    {
                        new V1Container()
                        {
                            Name = "test-container",
                            Image = "nginx",
                            Env = new List<V1EnvVar>(){
                                new V1EnvVar(){
                                    Name="ENV", Value="dev"
                                }
                            }
                        }
                    }
                }
            };
            
            var result = client.CreateNamespacedPod(pod, "default");
            //pod.Metadata.Labels
           /* var n = new Dictionary<string, string>()
                {
                    { "lab", "security" }
                };*/
            //V1ObjectMeta.Labels
            ViewData["Message"] = result;
            return View(); 
        }
    }
}