using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Controllers
{
    public class SplitController : Controller
    {
        public IActionResult Index()
        {
            string pathSource = @"C:\Users\s\source\repos\a2.yaml";
            //string pathSource=@"C:\Users\s\source\repos\y.txt";
            System.IO.TextReader readFile = new StreamReader(pathSource);
            string line = null;
            line = readFile.ReadToEnd();
            //if (line != null)
            //string[] words = line.Split(':');
            //string word2 = null;
            string[] words = line.Split(new Char[] { ' ', '\n' });
            /*
            ViewData["Message"]
               = '#' + words[0] + '#' + words[1] + '#' + words[2] + '#' + words[3] + '#' + words[4] + '#' + words[5]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[12] + '#' + words[13] + '#' + words[14] + '#' + words[15] + '#' + words[16] + '#' + words[17]
               + '#' + words[18] + '#' + words[19] + '#' + words[20] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11]
               + '#' + words[6] + '#' + words[7] + '#' + words[8] + '#' + words[9] + '#' + words[10] + '#' + words[11];               ;

            */
                      /*
                                      StringBuilder sb = new StringBuilder();
                                  foreach (var word in words)
                                  {
                                      if (word.EndsWith(':'))
                                      {
                                          word2 = word.Trim(':');
                                          sb.Append("*" + word2 + "*");
                                      }
                                      else
                                      {
                                          sb.Append("*" + word + "*");
                                      }
                                  }
                                  ViewData["Message"] = sb;
                                  */
                      //readFile.Close();
                      //readFile = null;

            int num = 0;
            StringBuilder sb = new StringBuilder();

            foreach (var word in words)
            {
                sb.Append(word +'['+ num.ToString()+']'+ '+');
                num = num + 1;
            }
            ViewData["Message"] = sb;
            return View();
        }
    }
}