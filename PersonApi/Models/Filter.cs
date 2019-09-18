using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Models
{
    /// <summary>
    /// A model representing various filters by which a GET request's results can be narrowed down.
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Limits a GET request by only fetching the first n results.
        /// </summary>
        public int? MaxNumberOfResults { get; set; }

        /// <summary>
        /// Limits a GET request by only fetching those that like chocolate or don't like chocolate.
        /// </summary>
        public bool? LikesChocolate { get; set; }

        /// <summary>
        /// Limits a GET request by only fetching Persons with a Person.Name matching Filter.Name
        /// </summary>
        public string Name { get; set; }
    }
}
