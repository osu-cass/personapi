using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Repository
{
    public class Person : EntityBase
    {
        [Required]
        public string Name { get; set; }
        [DefaultValue(true)]
        public bool LikesChocolate { get; set; }
    }
}
