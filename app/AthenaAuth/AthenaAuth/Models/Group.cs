using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AthenaAuth.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        [Required]
        public string GroupName { get; set; }
    }
}