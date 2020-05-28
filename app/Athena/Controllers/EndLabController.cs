using Microsoft.AspNetCore.Mvc;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Athena.Data;

namespace Athena.Controllers
{
    public class EndLabController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EndLabController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string userName = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //string userName = HttpContext.Session.GetString("namespace");
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            var TemplateId = HttpContext.Session.GetInt32("TemplateId");
            var labName = _context.Template.Find(TemplateId).Lab;

            var list = client.ListNamespacedDeployment(userName, null, null, null, "lab = " + labName);
            var slist = client.ListNamespacedService(userName, null, null, null, "lab = " + labName);
            var ilist = client.ListNamespacedIngress1(userName, null, null, null, "lab = " + labName);
            var nlist = client.ListNamespacedNetworkPolicy(userName, null, null, null, "lab = " + labName);
            foreach (var item in list.Items)
            {
                client.DeleteNamespacedDeployment(item.Metadata.Name, userName);
            }
            foreach (var item in slist.Items)
            {
                client.DeleteNamespacedService(item.Metadata.Name, userName);
            }
            foreach (var item in ilist.Items)
            {
                client.DeleteNamespacedIngress1(item.Metadata.Name, userName);
            }
            foreach (var item in nlist.Items)
            {
                client.DeleteNamespacedNetworkPolicy(item.Metadata.Name, userName);
            }

            return View();
        }
    }
}