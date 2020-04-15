/*using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Controllers
{
    public class FileController : Controller
    {
        public IActionResult Index()
        {

            // Specify a file to read from and to create.
            string pathSource = @"C:\Users\s\source\repos\a.yaml";
            // string pathNew = @"C:\Users\s\source\repos\a1.yaml";

            try
            {

                using (FileStream fsSource = new FileStream(pathSource,
                    FileMode.Open, FileAccess.Read))
                {

                    // Read the source file into a byte array.
                    byte[] bytes = new byte[fsSource.Length];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    numBytesToRead = bytes.Length;

                    ViewData["Message"] = fsSource;

                    *//* // Write the byte array to the other FileStream.
                     using (FileStream fsNew = new FileStream(pathNew,
                             FileMode.Create, FileAccess.Write))
                         {
                             fsNew.Write(bytes, 0, numBytesToRead);
                         }
                     }*//*
                }
            }
            catch (FileNotFoundException ioEx)
            {
                ViewData["Message"] = ioEx.Message;
            }


            return View();
        }
    }
}*/

/*using SharpYaml.Serialization;
using System.IO;
using System.Text;

class Program
{
    private class Config
    {
        public string IndexDir { get; set; }
        public string LogDir { get; set; }
        public string[] FilterPattern { get; set; }
        public string[] SomeOptionalProp { get; set; }

        public override string ToString()
        {
            var bldr = new StringBuilder();

            bldr.AppendLine(string.Format("IndexDir - {0}", IndexDir));
            bldr.AppendLine(string.Format("LogDir - {0}", LogDir));
            if (FilterPattern != null)
            {
                bldr.AppendLine(string.Format("FilterPattern:"));
                foreach (var fp in FilterPattern)
                    bldr.AppendLine(string.Format("  - {0}", fp));
            }

            if (SomeOptionalProp != null)
            {
                bldr.AppendLine(string.Format("SomeOptionalProp:"));
                foreach (var fp in SomeOptionalProp)
                    bldr.AppendLine(string.Format("  - {0}", fp));
            }

            return bldr.ToString();
        }
    }

    static void Main(string[] args)
    {
        var yamlPath = "config.yml";
        var input = new StreamReader(yamlPath);

        var deserializer = new Serializer();
        var config = (Config)deserializer.Deserialize(input, typeof(Config));

        Console.WriteLine(config.ToString());
        Console.ReadKey();
    }
}*/