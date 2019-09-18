using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonApi.Configurations;
using PersonApi.Models;
using PersonApi.Repositories;

namespace PersonApi.Controllers.V2
{
    /// <summary>
    /// Version 2 of the PersonController.
    /// </summary>
    /// <remarks>
    /// This controller only has one function to demonstrate that [GET api/v2/Person/Info] calls a different function than [GET api/v1/Person/Info]
    /// </remarks>
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IRepository<Person, int> _repository;
        private ProjectConfigurations _projectConfigurations { get; set; }

        public PersonController(IRepository<Person, int> repository, IOptions<ProjectConfigurations> settings)
        {
            // The constructor uses dependency injection to inject an IRepository into the controller.
            // When actually running the API, this is a "real" repository that interacts with a local database.
            // When running test cases, this is a "fake" repository that only emulates the key functions needed.
            _repository = repository;

            // Additionally, we inject IOptions<ProjectConfigurations> which maps properties from
            // appsettings.json to the ProjectConfigurations object via the .Value property.
            _projectConfigurations = settings.Value;
        }

        // GET: api/v2/Person/Info
        /// <summary>
        /// Gets information about the API and its version.
        /// </summary>
        /// <returns>API version information.</returns>
        /// <response code="200">Successfully returned information about the API.</response>
        [HttpGet]
        [Route("Info")]
        public string GetInfo()
        {
            return "You are using PersonAPI Version 2!";
        }
    }
}
