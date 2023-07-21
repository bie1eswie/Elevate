using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class HumanAPIUser
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string UserId { get; set; } 
        [Required]
        public string HumanID { get; set; } = string.Empty;
        public string PublicToken { get; set; } = string.Empty;

    }
}
