using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AthenaAuth.Data;
using AthenaAuth.Models;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using k8s;
using k8s.Models;
using System;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using System.Security.Cryptography;

namespace AthenaAuth.Controllers
{
    public class TemplatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TemplatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Templates
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") == "staff")
            { return View(await _context.Template.ToListAsync()); }
            else { return View("Error"); }
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("Role") == "staff")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var template = await _context.Template
                    .FirstOrDefaultAsync(m => m.TemplateId == id);
                if (template == null)
                {
                    return NotFound();
                }

                return View(template);
            }
            else { return View("Error"); }
        }

        // GET: Templates/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") == "staff")
            {
                return View();
            }
            else { return View("Error"); }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TemplateId,Path,TemplateName,Lab")] Template template)
        {
           


            var check = from t in _context.Template
                        where t.TemplateName == template.TemplateName
                        select t;

            string path = template.Path;

            string test = CheckLabel(path, "lab");
             if (test == template.Lab)
             {
            if (ModelState.IsValid && check.Count() == 0)
                {
                    _context.Add(template);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(template);
            }
            else
            {
                return RedirectToAction("Label", new { id = test} );
            }
        }

        public IActionResult Label(string id)
        {
            ViewBag.Error=id;
            return View();
        }
            //
        public string CheckLabel(string path, string label)
        {
            var deserializeYAML = new DeserializerBuilder()
                      .WithNamingConvention(CamelCaseNamingConvention.Instance)
                      .Build();

            var config = KubernetesClientConfiguration.BuildDefaultConfig();
            IKubernetes client = new Kubernetes(config);

            //string path = @"C:\Users\Laptop User\source\repos\TestingConsole\TestingConsole\templates\juice\";
            

            
            
            string name = null;
            bool test;
            string msg ="";

            string pService = path + "Service";
            string pIngress = path + "Ingress";
            string pNetPol = path + "NetworkPolicy";
            string pDeployment = path + "Deployment";

            if (Directory.Exists(pDeployment))
            {
                if (Directory.Exists(pService) == false)
                {
                    pService = null;
                }
                if (Directory.Exists(pIngress) == false)
                {
                    pIngress = null;
                }
                if (Directory.Exists(pNetPol) == false)
                {
                    pNetPol = null;
                }
            }

            else
            {
                pDeployment = null;
            }

            if (pDeployment != null)
            {
                test = CheckDeployments(deserializeYAML, pDeployment, label, out name);
                if (test)
                {
                    if (pService != null)
                    {
                        test = CheckServices(deserializeYAML, pService, label, name);
                        if (test == false) { msg = "Not all Service resources had label " + label + "=" + name; }

                    }
                    if (pIngress != null && test == true)
                    {
                        test = CheckIngress(deserializeYAML, pIngress, label, name);
                        if (test == false) { msg = "Not all Ingress resources had label " + label + "=" + name; }
                    }
                    if (pNetPol != null && test == true)
                    {
                        test = CheckNetPol(deserializeYAML, pNetPol, label, name);
                        if (test == false) { msg = "Not all NetPol resources had label " + label + "=" + name; }
                    }
                    if (test) {
                        //msg = "All " + label + " labels present and matching value " + name;
                        msg = name;
                    }

                }
                else
                {
                    if (name != null) { msg = "Not all Deployment resources had label " + label + "=" + name; }
                    else { msg = "Label " + label + " missing from Deployment"; }

                }
            }
            else
            {
                msg = "Template must contain Deployments";
            }

            return msg;
        }

        public bool CheckDeployments(IDeserializer deserializer, string path, string key, out string value)
        {

            value = null;
            string curr;
            bool match = true;

            foreach (string file in Directory.EnumerateFiles(path))
            {
                StreamReader fileContent = System.IO.File.OpenText(file);
                V1Deployment deployment = deserializer.Deserialize<V1Deployment>(fileContent);

                // Check if service has label - if no label return false

                if (deployment.Metadata.Labels.TryGetValue(key, out curr) != true)
                {
                    match = false;
                    break;
                }
                // Matches first Deployment with label, assigns label value to labName

                else if (value == null)
                {
                    value = curr;
                }

                // If there is no match at this stage, there is a mismatch

                else if (value != curr)
                {

                    match = false;
                    break;
                }

                // Checking selectors - all deployments must select based on at least the lab name and that name must match on every template

                // Checking MatchLabels

                if (deployment.Spec.Selector.MatchLabels.TryGetValue(key, out curr))
                {
                    if (value != curr)
                    {
                        match = false;
                        break;
                    }
                }
                // Checking MatchExpressions
                else
                {
                    foreach (var expression in deployment.Spec.Selector.MatchExpressions)
                    {
                        if (expression.Key == key && expression.OperatorProperty == "In" && expression.Values.Contains(value))
                        {
                            match = true;
                            break;

                        }
                        else
                        {
                            match = false;
                        }
                    }
                    break;
                }
                // Checking Pod Spec - label should be present and correct
                if (deployment.Spec.Template.Metadata.Labels.TryGetValue(key, out curr))
                {
                    if (value != curr)
                    {
                        match = false;
                        break;
                    }
                }
                else
                {
                    match = false;
                    break;
                }

            }
            return match;
        }

        public bool CheckServices(IDeserializer deserializer, string path, string key, string value)
        {
            string curr = null;
            bool match = true;

            foreach (string file in Directory.EnumerateFiles(path))
            {
                StreamReader fileContent = System.IO.File.OpenText(file);
                V1Service service = deserializer.Deserialize<V1Service>(fileContent);

                // Check if service has label - if no label

                if (service.Metadata.Labels.TryGetValue(key, out curr) != true)
                {
                    match = false;

                    break;
                }
                // If there is no match at this stage, there is a mismatch

                else if (value != curr)
                {

                    match = false;
                    break;
                }

                service.Spec.Selector.TryGetValue(key, out curr);

                if (value != curr)
                {
                    match = false;

                    break;
                }

            }

            return match;
        }
        public bool CheckIngress(IDeserializer deserializer, string path, string key, string value)
        {

            string curr;
            bool match = true;

            foreach (string file in Directory.EnumerateFiles(path))
            {
                StreamReader fileContent = System.IO.File.OpenText(file);
                Networkingv1beta1Ingress ingress = deserializer.Deserialize<Networkingv1beta1Ingress>(fileContent);

                // Check if Ingress has label - if no label return false

                if (ingress.Metadata.Labels.TryGetValue(key, out curr) != true)
                {
                    match = false;
                    break;
                }

                // If there is no match at this stage, there is a mismatch

                else if (value != curr)
                {
                    match = false;
                    break;
                }
            }
            return match;
        }

        static bool CheckNetPol(IDeserializer deserializer, string path, string key, string value)
        {
            string curr;
            bool match = true;

            foreach (string file in Directory.EnumerateFiles(path))
            {
                StreamReader fileContent = System.IO.File.OpenText(file);
                V1NetworkPolicy netpol = deserializer.Deserialize<V1NetworkPolicy>(fileContent);

                // Check if Ingress has label - if no label return false

                if (netpol.Metadata.Labels.TryGetValue(key, out curr) != true)
                {
                    match = false;
                    break;
                }
                // Matches first NetworkPolicy with label, assigns label value to labName

                // If there is no match at this stage, there is a error of some kind

                else if (value != curr)
                {

                    match = false;
                    break;
                }

            }
            return match;
        }

        // GET: Templates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Role") == "staff")
            { 
                if (id == null)
                {
                    return NotFound();
                }

                var template = await _context.Template.FindAsync(id);
                if (template == null)
                {
                    return NotFound();
                }
                return View(template);
            }
            else { return View("Error"); }            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TemplateId,Path,TemplateName,Lab")] Template template)
        {
            if (id != template.TemplateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(template);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemplateExists(template.TemplateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(template);
        }

        // GET: Templates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("Role") == "staff")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var template = await _context.Template
                    .FirstOrDefaultAsync(m => m.TemplateId == id);
                if (template == null)
                {
                    return NotFound();
                }

                return View(template);
            }
            else { return View("Error"); }          
        }

        // POST: Templates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
   
            var template = await _context.Template.FindAsync(id);
            _context.Template.Remove(template);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemplateExists(int id)
        {
            return _context.Template.Any(e => e.TemplateId == id);
        }
    }
}
