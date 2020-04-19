using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using k8s;
using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Athena.Data;

using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Athena.Controllers
{
    public class LabelController : Controller
    {

        private readonly AthenaContext _context;

        public LabelController(AthenaContext context)
        {
            _context = context;
        }
        string userName = "default";
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);
            //var jsonFilePod = System.IO.File.ReadAllText("C:\\Users\\s\\source\\repos\\Dep_Kub\\d.json");
            string p = "";
            string a = "class";//key
            string b = "SecurityL7"; // value
             // create the namespace when logging in

            var optionsBuilder = new DbContextOptionsBuilder<AthenaContext>();
            optionsBuilder.UseSqlServer("AthenaContext");
            using (var context = new AthenaContext(optionsBuilder.Options))
            {
                var label = await _context.Label.FirstOrDefaultAsync(m => m.Id == 1);
                            
                a = label.Key;
                b = label.Value;

                var path = await _context.Template.FirstOrDefaultAsync(m => m.Id == 1);

                p = path.Path;
            }

            //YAML
            
        /*var deserializeYAML = new DeserializerBuilder()
              .WithNamingConvention(CamelCaseNamingConvention.Instance)
              .Build();

            foreach (string file in Directory.EnumerateFiles(p))
            {
                
                StreamReader fileContent = System.IO.File.OpenText(file);

                V1Deployment deployment = deserializeYAML.Deserialize<V1Deployment>(fileContent);
                
                if (deployment.Metadata.Labels == null)
                {
                    deployment.Metadata.Labels = new Dictionary<string, string>();
                }

                deployment.Metadata.Labels.Add("animal", "hare");
                var result = client.CreateNamespacedDeployment(deployment, "default");

                ViewData["Message"] = result;
            }*/




            //JSON
            var serialiseOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            foreach (string file in Directory.EnumerateFiles(p))
            {
                string fileContent = System.IO.File.ReadAllText(file);

                V1Deployment deployment = JsonSerializer.Deserialize<V1Deployment>(fileContent, serialiseOptions);

                if (deployment.Metadata.Labels == null)
                {
                    deployment.Metadata.Labels = new Dictionary<string, string>();
                }

                deployment.Metadata.Labels.Add(a, b);
                var result = client.CreateNamespacedDeployment(deployment, userName);

                ViewData["Message"] = result;
            }
            
            return View();

        }
    }
}
/*
            table User
            UserId: hyerim.kwon
            pw: 
            */

/*
1) sec test lab1
2) network lab2
*/


/*
group table
classname : sec l7
members : hyerim.kwon
*/

/*string a = "key database: class";
        string b= "value database: security testing class level 7";
*/

   