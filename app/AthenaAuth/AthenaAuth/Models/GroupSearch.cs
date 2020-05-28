using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AthenaAuth.Models
{
    public class GroupSearch
    {
       public Group group { get; set; }
       public IEnumerable<AthenaAuth.Models.Group_User> group_user {get; set;}
       public IEnumerable<AthenaAuth.Models.Group_Template> group_template { get; set; }
    }
}
