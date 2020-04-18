using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Athena.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string LabGuide { get; set; }
    }
}
