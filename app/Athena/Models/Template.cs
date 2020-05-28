using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Athena.Models
{
    public partial class Template
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Template()
        {
            this.Group_Template = new HashSet<Group_Template>();
        }
        public int TemplateId { get; set; }
        [Required]
        [RegularExpression(@"[a-zA-Z0-9]{1,}[\\][\\][a-zA-Z0-9]{1,}[\\][\\]", ErrorMessage = "Template\\\\'YourFolderName'\\\\")]
        public string Path { get; set; }
        [Required]
        public string TemplateName { get; set; }
        [Required (ErrorMessage = "the same value for 'labels: lab:' in yaml files")]
        public string Lab { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Group_Template> Group_Template { get; set; }
    }

}
