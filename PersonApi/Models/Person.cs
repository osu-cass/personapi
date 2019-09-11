using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PersonApi.Models
{
    public class Person
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DefaultValue(true)]
        public bool LikesChocolate { get; set; }
    }
}
