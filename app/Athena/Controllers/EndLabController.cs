using Microsoft.AspNetCore.Mvc;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Http;

namespace Athena.Controllers
{
    public class EndLabController : Controller
    {
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("namespace");
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile("./config");
            var client = new Kubernetes(k8SClientConfig);

            var list = client.ListNamespacedDeployment(userName, null, null, null, "lab = initial");
            var slist = client.ListNamespacedService(userName, null, null, null, "lab = initial");
            foreach (var item in list.Items)
            {
                client.DeleteNamespacedDeployment(item.Metadata.Name, userName);
            }
            foreach (var item in slist.Items)
            {
                client.DeleteNamespacedService(item.Metadata.Name, userName);
            }
            
            return View();
        }
    }
}