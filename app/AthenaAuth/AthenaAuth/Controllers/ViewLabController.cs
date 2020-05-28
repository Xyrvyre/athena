using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using k8s;
using k8s.Models;
using System.Security.Claims;
using AthenaAuth.Data;
using Microsoft.AspNetCore.Http;

namespace Athena.Controllers
{

    

    
    
    public class ViewLabController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ViewLabController(ApplicationDbContext context)
        {
            _context = context;
        }






        public IActionResult Index()
        {
            // Get deployments (assuming namespace and lab name known already) - guessing this would need an 'Active Labs' table w. Lab Name and user namespace. 
            var userName = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var TemplateId = HttpContext.Session.GetInt32("TemplateId");
            var labName = _context.Template.Find(TemplateId).Lab;
            //var labName = "juice-shop";

            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);







            var labDeployments = client.ListNamespacedDeployment(userName, null, null, null, "lab = " + labName);
            var labPods = client.ListNamespacedPod(userName, null, null, null, "lab = " + labName);
            //labDeployments.Metadata.Name

            Dictionary<string, string> ipAdd = new Dictionary<string, string>();
            
            foreach (var deployment in labDeployments.Items)
            {
                var deploymentPods = labPods.Items.Where(a => a.Metadata.Name.Contains(deployment.Metadata.Name) == true);
                ipAdd.Add(deployment.Metadata.Name, deploymentPods.FirstOrDefault().Status.PodIP);
            }
            ViewBag.IPADD = ipAdd; 

            var labIngress = client.ListNamespacedIngress1(userName, null, null, null, "lab = " + labName);
            Dictionary<string, string> paths = new Dictionary<string, string>();
            foreach (var item in labIngress.Items)
            {
                if (item.Metadata.Annotations.ContainsKey("regex"))
                {
                    var regex = item.Metadata.Annotations["regex"];
                    foreach (var rule in item.Spec.Rules)
                    {
                        foreach (var path in rule.Http.Paths)
                        {
                            paths.Add(path.Backend.ServiceName, ("http://" + rule.Host + path.Path).Replace(regex, "") + "/");
                        }
                    }
                }
                else
                {
                    foreach (var rule in item.Spec.Rules)
                    {
                        foreach (var path in rule.Http.Paths)
                        {
                            paths.Add(path.Backend.ServiceName, "http://" + rule.Host + path.Path);
                        }
                    }
                }
            }
 
            ViewBag.ingress = paths;


            return View();
        }
    }
}