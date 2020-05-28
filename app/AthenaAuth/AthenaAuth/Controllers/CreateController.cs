using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using k8s;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using AthenaAuth.Data;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;

namespace Athena.Controllers
{
    

    public class CreateController : Controller
    {
        string p = "";
        string userName = "testing";

        private readonly ApplicationDbContext _context;

        public CreateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<IActionResult> Index(int id)
            //get template id
        {

            userName = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);


            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("ApplicationDbContext");

            //get the path value from db
            p = _context.Template.Find(id).Path;

            //get the path value from db
            HttpContext.Session.SetInt32("TemplateId", id);
            //set session variable TemplateId

            var pDeployments = p + "Deployment";
            var pServices = p + "Service";
            var pIngress = p + "Ingress";
            string pNetPol = p + "NetworkPolicy";

            var deserializeYAML = new DeserializerBuilder()
                  .WithNamingConvention(CamelCaseNamingConvention.Instance)
                  .Build();

            try
            {

                foreach (string file in Directory.EnumerateFiles(pServices))
                {

                    StreamReader fileContent = System.IO.File.OpenText(file);

                    V1Service service = deserializeYAML.Deserialize<V1Service>(fileContent);

                    var result = client.CreateNamespacedService(service, userName);


                }

                foreach (string file in Directory.EnumerateFiles(pDeployments))
                {

                    StreamReader fileContent = System.IO.File.OpenText(file);

                    V1Deployment deployment = deserializeYAML.Deserialize<V1Deployment>(fileContent);

                    var result = client.CreateNamespacedDeployment(deployment, userName);


                }

                foreach (string file in Directory.EnumerateFiles(pIngress))
                 {
                     StreamReader fileContent = System.IO.File.OpenText(file);
                     Networkingv1beta1Ingress ingress = deserializeYAML.Deserialize<Networkingv1beta1Ingress>(fileContent);
                     AddIngress(client, ingress, userName);
                 }

                foreach (string file in Directory.EnumerateFiles(pNetPol))
                {

                    StreamReader fileContent = System.IO.File.OpenText(file);

                    V1NetworkPolicy netpolicy = deserializeYAML.Deserialize<V1NetworkPolicy>(fileContent);

                    var result = client.CreateNamespacedNetworkPolicy(netpolicy, userName);
                }
                return View();
            }
           catch {
                //CleanLab(client, userName, "lab", "juice-shop");
                // CleanLab(client,userName);
                return View();
                //return RedirectToAction("Index");
            }
            /*catch (Exception e)
            {
                return RedirectToAction("Error", e);
            }*/
        }

        /*public IActionResult Error(Exception e)
        {
            ViewBag.Exception = e;
            return View();
        }*/








        public void CleanLab(IKubernetes client, string userName)
        {
            var dList = client.ListNamespacedDeployment(userName);
            var sList = client.ListNamespacedService(userName);
            var nList = client.ListNamespacedNetworkPolicy(userName);
            var iList = client.ListNamespacedIngress1(userName);
            if (dList != null)
            {
                foreach (var item in dList.Items)
                {
                    client.DeleteNamespacedDeployment(item.Metadata.Name, userName);
                }
            }
            if (sList != null)
            {
                foreach (var item in sList.Items)
                {
                    client.DeleteNamespacedService(item.Metadata.Name, userName);
                }
            }
             if (iList != null)
             {
                 foreach (var item in iList.Items)
                 {
                     client.DeleteNamespacedIngress1(item.Metadata.Name, userName);
                 }
             }
            if (nList != null)
            {
                foreach (var item in nList.Items)
                {
                    client.DeleteNamespacedNetworkPolicy(item.Metadata.Name, userName);
                }
            }
        }
        /* public void CleanLab(IKubernetes client, string userName, string key, string value)
        {
            var dList = client.ListNamespacedDeployment(userName, null, null, null, key + " = " + value);
            var sList = client.ListNamespacedService(userName, null, null, null, key + " = " + value);
            var nList = client.ListNamespacedNetworkPolicy(userName, null, null, null, key + " = " + value);
            //var iList = client.ListNamespacedIngress1(userName, null, null, null, key + " = " + value);
            if (dList != null)
            {
                foreach (var item in dList.Items)
                {
                    client.DeleteNamespacedDeployment(item.Metadata.Name, userName);
                }
            }
            if (sList != null)
            {
                foreach (var item in sList.Items)
                {
                    client.DeleteNamespacedService(item.Metadata.Name, userName);
                }
            }
            if (iList != null)
            {
                foreach (var item in iList.Items)
                {
                    client.DeleteNamespacedIngress1(item.Metadata.Name, userName);
                }
            }
            if (nList != null)
            {
                foreach (var item in nList.Items)
                {
                    client.DeleteNamespacedNetworkPolicy(item.Metadata.Name, userName);
                }
            }
        }*/

        public void AddIngress(IKubernetes client, Networkingv1beta1Ingress ingress, string nspace)
        {
            string regex = "";
            if (ingress.Metadata.Annotations.ContainsKey("regex"))
            {
                regex = ingress.Metadata.Annotations["regex"];
            }
            // Adding rewrite annotation; getting associated regex value

            // Creating a custom path for each service. Path format is arbitrary and could be changed if needed

            foreach (var rule in ingress.Spec.Rules)
            {
                foreach (var path in rule.Http.Paths)
                {
                    path.Path = "/" + nspace + path.Path;
                    if (regex != null)
                    {
                        path.Path = path.Path + regex;
                    }
                    else if (ingress.Metadata.Annotations.ContainsKey("nginx.ingress.kubernetes.io/rewrite-target"))
                    {
                        path.Path = path.Path + "/";
                    }
                    
                }
            }
            client.CreateNamespacedIngress1(ingress, nspace);
        }

        public async System.Threading.Tasks.Task<IActionResult> ChooseTemplate(int? id)
        {
           
            var @group = _context.Group_Template
                 .Include(g => g.Group)
                 .Include(g => g.Template)
                 .Where(m => m.GroupId == id)
                 ;
            return View(@group);
        }

        public async System.Threading.Tasks.Task<IActionResult> ChooseGroup()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @group = _context.Group_User
                 .Include(g => g.Group)
                 .Include(g => g.User)
                 .Where(m => m.UserId == userId)
                 ;
            return View(@group);
        }

    }
}
