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
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Athena.Controllers
{
    public class MainController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MainController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string a;
            var UserEmail = this.User.Identity.Name;
            

            var result = GetUserRoles(UserEmail, "athena@it.weltec.ac.nz", out a);


            ViewBag.Tutor = a; 
            ViewBag.Admin = result; 
            if (a == "/Staff")
            { 
                HttpContext.Session.SetString("Role", "staff"); 
            }
            else 
            { 
                HttpContext.Session.SetString("Role", "student"); 
            }

            
            var UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var k8SClientConfig = KubernetesClientConfiguration.BuildDefaultConfig();
            
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
        public ServiceAccountCredential GetCredential(string[] scopes, string impEmail)
        {

            return new ServiceAccountCredential(new ServiceAccountCredential.Initializer(System.IO.File.ReadAllText("/etc/athena/.data/ce"))
            {
                Scopes = scopes,
                User = impEmail
            }.FromPrivateKey(System.IO.File.ReadAllText("/etc/athena/.data/pk")));


        }

// Takes three parameters - the user to lookup, the user to impersonate and an initialised string variable to store the OU path
public bool? GetUserRoles(string userID, string serviceEmail, out string ou)
{
// DirectoryService creation
string[] directoryScopes = { DirectoryService.Scope.AdminDirectoryUserReadonly };

var cred = GetCredential(directoryScopes, serviceEmail);

DirectoryService service = new DirectoryService(new BaseClientService.Initializer()
{
    HttpClientInitializer = cred,
    ApplicationName = "Athena",
});
var request = service.Users.Get(userID);
request.ViewType = UsersResource.GetRequest.ViewTypeEnum.AdminView;

var user = request.Execute();
ou = user.OrgUnitPath;
return user.IsAdmin;
}
public async Task<string> DeleteAll()
{
ApplicationDbContext c = _context;
var k8SClientConfig = KubernetesClientConfiguration.BuildDefaultConfig();
var client = new Kubernetes(k8SClientConfig);

var users = from a in c.Users
            select a.Id;

var deplist = client.ListDeploymentForAllNamespaces();

var namelist = new HashSet<string>();
foreach (var deployment in deplist.Items)

{
    if (users.Contains(deployment.Metadata.NamespaceProperty) && namelist.Contains(deployment.Metadata.NamespaceProperty) == false)
    {

        namelist.Add(deployment.Metadata.NamespaceProperty);
    }

}


foreach (var name in namelist)
{
    CleanLab(client, name);

}

return "Deleted Labs";

}



public void CleanLab(IKubernetes client, string userName)
{
var dList = client.ListNamespacedDeployment(userName);
V1ServiceList sList = null;
V1NetworkPolicyList nList = null;
Networkingv1beta1IngressList iList = null;

if (dList != null && dList.Items.Count > 0)
{
    sList = client.ListNamespacedService(userName);
    nList = client.ListNamespacedNetworkPolicy(userName);
    iList = client.ListNamespacedIngress1(userName);
    foreach (var item in dList.Items)
    {
        client.DeleteNamespacedDeployment(item.Metadata.Name, userName);
    }
}
if (sList != null && sList.Items.Count > 0)
{
    foreach (var item in sList.Items)
    {
        client.DeleteNamespacedService(item.Metadata.Name, userName);
    }
}
 if (iList != null && iList.Items.Count > 0)
 {
     foreach (var item in iList.Items)
     {
         client.DeleteNamespacedIngress1(item.Metadata.Name, userName);
     }
 }
if (nList != null && nList.Items.Count > 0)
{
    foreach (var item in nList.Items)
    {
        client.DeleteNamespacedNetworkPolicy(item.Metadata.Name, userName);
    }
}
}
}
}


