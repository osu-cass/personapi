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

        public PersonController()
        {
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
