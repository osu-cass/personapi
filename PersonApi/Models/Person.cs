using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PersonApi.Models
{
    public class Person
    {
        /// <summary>
        /// The ID assigned to a person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the person.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// A boolean representing whether or not the person likes chocolate.
        /// </summary>
        [DefaultValue(true)]
        public bool LikesChocolate { get; set; }
    }
}
