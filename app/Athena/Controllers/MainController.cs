using Microsoft.AspNetCore.Mvc;
using k8s;
using k8s.Models;
using Athena.Data;
using System.Security.Claims;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Athena.Controllers
{
    public class MainController : Controller
    {
                
        public IActionResult Index()
        {
            string a;
            //var UserEmail = this.User.Identity.Name;
            var UserEmail = "paul.bryant@it.weltec.ac.nz";

            var result = GetUserRoles("./sa.json", UserEmail, "athena@it.weltec.ac.nz", out a);


            ViewBag.Tutor = a; 
            ViewBag.Admin = result; 
            if (a == "/Staff")
            //if (result is true)
            { HttpContext.Session.SetString("Role", "staff"); }
            else { HttpContext.Session.SetString("Role", "student"); }

            //create namespace
            var UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            
                var client = new Kubernetes(k8SClientConfig);

                var namespaces = client.ListNamespace(null, null, "metadata.name=" + UserId);

                if (namespaces != null && namespaces.Items.Count > 0)
                {
                    return View();
                }


                var ns = new V1Namespace
                {
                    Metadata = new V1ObjectMeta
                    {
                        Name = UserId
                    }
                };

                var result2 = client.CreateNamespace(ns);

                var netPolFile = "default-network-policy.yaml";
                if (System.IO.File.Exists(netPolFile))
                {
                    var fileContent = System.IO.File.ReadAllText(netPolFile);
                    var netPol = Yaml.LoadFromString<V1NetworkPolicy>(fileContent);
                    client.CreateNamespacedNetworkPolicy(netPol, UserId);
                }
                
                ViewData["Message"] = "";
                return View();

            }
            catch (Exception e)
            {
                return RedirectToAction("Error", e);
            }
   
        }

        public IActionResult Error(Exception e)
        {
            ViewBag.Exception = e;
            return View();
        }
        public ServiceAccountCredential GetCredential(string path, string[] scopes, string impEmail)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(path));
            return new ServiceAccountCredential(new ServiceAccountCredential.Initializer(values["client_email"])
            {
                Scopes = scopes,
                User = impEmail
            }.FromPrivateKey(values["private_key"]));

        }

        // Takes four parameters - the path of the credential, the user to lookup, the user to impersonate and an initialised string variable to store the OU path
        public bool? GetUserRoles(string path, string userID, string serviceEmail, out string ou)
        {
            // DirectoryService creation
            string[] directoryScopes = { DirectoryService.Scope.AdminDirectoryUserReadonly };

            var cred = GetCredential(path, directoryScopes, serviceEmail);

            DirectoryService service = new DirectoryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = "DirectoryTest",
            });
            var request = service.Users.Get(userID);
            request.ViewType = UsersResource.GetRequest.ViewTypeEnum.AdminView;

            var user = request.Execute();
            ou = user.OrgUnitPath;
            return user.IsAdmin;
        }
    }
}


