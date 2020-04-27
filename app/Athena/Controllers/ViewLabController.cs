using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using k8s;
using k8s.Models;


namespace Athena.Controllers
{
    public class ViewLabController : Controller
    {
        public IActionResult Index()
        {
            // Get deployments (assuming namespace and lab name known already) - guessing this would need an 'Active Labs' table w. Lab Name and user namespace. 
            var userName = "theo";
            var labName = "initial";
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);

            var labDeployments = client.ListNamespacedDeployment(userName, null, null, null, "lab = " + labName);

            // This list could be displayed to user to choose a lab to view. 
            // If this was stored in a database table (eg. 'Active Deployments' ) it could have a 'pretty name' associated with it
            // eg. instead of printing 'kali-ssh-server' you could look up 'kali-ssh-server' in table and  print 'Remote Client Device', its 'pretty name'
            
            IList<string> ipAdd = new List<string>();
            foreach (var item in labDeployments.Items)
            {
                var deploymentPod = client.ListNamespacedPod(userName, null, null, null, "lab = " + labName + ",app = " + item.Metadata.Labels["app"]);
                
                ipAdd.Add((deploymentPod.Items.First()).Status.PodIP);

                //ViewData["Message"] =(deploymentPod.Items.First()).Status.PodIP;
            }
            ViewBag.IPADD = ipAdd;

            var viewerService = client.ListNamespacedService(userName, null, null, null, "lab = " + labName).Items.First();

            
            // Hardcoded toggle (would not normally be present)
            string userChoice;
            if (viewerService.Spec.Selector["app"] == "ssh-server")
            {
                userChoice = "ssh-client";
            }

            else
            {
                userChoice = "ssh-server";
            }
           
            //to be added
            if (viewerService.Spec.Selector.ContainsKey("app")) { viewerService.Spec.Selector["app"] = userChoice; }
            else { viewerService.Spec.Selector.Add("app", userChoice); }
            //

            // Changing the selector value which determines which pod the Service targets

            //viewerService.Spec.Selector["app"] = userChoice;

            // Patching the running Service

            var patch = new JsonPatchDocument<V1Service>();
            patch.Replace(e => e.Spec.Selector, viewerService.Spec.Selector);
            client.PatchNamespacedService(new V1Patch(patch), viewerService.Metadata.Name, userName);
            
            return View();
        }
    }
}