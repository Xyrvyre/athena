﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;

using k8s;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Athena.Data;

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
        {

            userName = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var k8SClientConfig = KubernetesClientConfiguration.BuildDefaultConfig();
            var client = new Kubernetes(k8SClientConfig);


            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("ApplicationDbContext");
            p = _context.Template.Find(id).Path;
                        
            HttpContext.Session.SetInt32("TemplateId", id);
            

            var pDeployments = "/etc/athena/Templates/" + p + "/Deployment";
            var pServices = "/etc/athena/Templates/" + p + "/Service";
            var pIngress = "/etc/athena/Templates/" + p + "/Ingress";
            string pNetPol = "/etc/athena/Templates/" + p + "/NetworkPolicy";
            
            try
            {

                foreach (string file in Directory.EnumerateFiles(pServices))
                {

                    var fileContent = System.IO.File.ReadAllText(file);

                    var service = Yaml.LoadFromString<V1Service>(fileContent);
                   

                    var result = client.CreateNamespacedService(service, userName);


                }
                
                foreach (string file in Directory.EnumerateFiles(pDeployments))
                {

                   
                   
                    var fileContent = System.IO.File.ReadAllText(file);

                    var deployment = Yaml.LoadFromString<V1Deployment>(fileContent);
                    
                    var result = client.CreateNamespacedDeployment(deployment, userName);


                }

                 foreach (string file in Directory.EnumerateFiles(pIngress))
                 {
                    
                    var fileContent = System.IO.File.ReadAllText(file);

                    var ingress = Yaml.LoadFromString<Networkingv1beta1Ingress>(fileContent);
                    
                    AddIngress(client, ingress, userName);
                 }

                foreach (string file in Directory.EnumerateFiles(pNetPol))
                {

                    
                    V1NetworkPolicy netPol = null;
                    var fileContent = System.IO.File.ReadAllText(file);

                    netPol = Yaml.LoadFromString<V1NetworkPolicy>(fileContent);
                    

                    var result = client.CreateNamespacedNetworkPolicy(netPol, userName);
                }
                

                return View();
            }
           catch {
                return View();
            }
        }



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
