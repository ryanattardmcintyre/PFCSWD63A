using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.Domain
{
    public class Blog
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get; set; }
        public string Url { get; set; }
        public List<Post> Posts { get; set; }

        //[ForeignKey("User")]
        //public string Owner { get; set; }
        
        //public IdentityUser User { get; set; }
    }

  
}
