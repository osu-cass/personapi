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
    /// Currently, the only difference in implementation between the two versions is that V2 has a modified GetInfo()
    /// function to demonstrate that [GET api/v2/Person/Info] calls a different function than [GET api/v1/Person/Info]
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

        // GET: api/v2/Person
        /// <summary>
        /// Gets all persons.
        /// </summary>
        /// <returns>A list of all persons.</returns>
        /// <response code="200">Successfully returned a list of all persons.</response>
        [HttpGet]
        public IEnumerable<Person> GetPersons()
        {
            return _repository.Get();
        }

        // GET: api/v2/Person/ChocolateLovers
        /// <summary>
        /// Gets all persons that like chocolate.
        /// </summary>
        /// <returns>A list of all persons that like chocolate.</returns>
        /// <response code="200">Successfully returned a list of all persons that like chocolate.</response>
        [HttpGet]
        [Route("ChocolateLovers")]
        public IEnumerable<Person> GetPersonsThatLikeChocolate()
        {
            return _repository.Get().Where(p => p.LikesChocolate == true);
        }

        // GET api/v2/Person/5
        /// <summary>
        /// Gets a single specific person by their ID.
        /// </summary>
        /// <returns>A single person.</returns>
        /// <param name="id">The ID of the requested person.</param>
        /// <response code="200">Successfully returned the person requested.</response>
        /// <response code="404">The person with the provided ID does not exist.</response>
        [HttpGet("{id}", Name = nameof(GetPerson))]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _repository.GetByIDAsync(id);

            if (person == null)
            {
                return NotFound($"Person with ID #{id} does not exist.");
            }

            return Ok(person);
        }

        // POST api/v2/Person
        /// <summary>
        /// Creates a person.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Person
        ///     {
        ///         "id" : 1,
        ///         "name" : "Margaret Thatcher",
        ///         "likesChocolate" : true
        ///     }
        ///     
        /// </remarks>
        /// <param name="person">The person to create.</param>
        /// <returns>A newly created person.</returns>
        /// <response code="201">Returns the newly created person.</response>
        /// <response code="400">If the person is null.</response>  
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            // Check that person's name has a valid character set (using regex name pattern from https://www.regextester.com/93648).
            // We do not need to test ID or LikesChocolate as they are strongly typed in the person object.
            if (!Regex.Match(person.Name, "^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$").Success)
            {
                return BadRequest($"Name \'{person.Name}\' is not valid.");
            }

            _repository.Insert(person);
            await _repository.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetPerson), new { id = person.Id }, person);
        }

        // PUT api/v2/Person/5
        /// <summary>
        /// Updates a person by their ID.
        /// </summary>
        /// <remarks>
        /// Requires the client to send the entire updated entity, not just the changes.
        /// Sample request:
        /// 
        ///     PUT /Person/5
        ///     {
        ///         "id" : 5,
        ///         "name" : "Jane Austen",
        ///         "likesChocolate" : false
        ///     }
        ///     
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <returns>No content.</returns>
        /// <response code="201">No person with the provided ID existed, so a new one was created.</response>
        /// <response code="204">Successfully updated the person. No content is returned.</response>
        /// <response code="400">Bad Request. Either the provided name is invalid, or the ID provided in the URL does not match the one in the request body.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> PutPerson(int id, Person person)
        {
            // Check that person's name has a valid character set (using regex name pattern from https://www.regextester.com/93648).
            // We do not need to test ID or LikesChocolate as they are strongly typed in the person object.
            if (!Regex.Match(person.Name, "^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$").Success)
            {
                return BadRequest($"Name \'{person.Name}\' is not valid.");
            }

            // If the id in the URL (.../Person/5) does not match the id of the Person object passed in, return a 400 status code (Bad Request).
            if (id != person.Id)
            {
                return BadRequest($"Person ID provided in URL (ID #{id}) does not match Person ID provided in PUT body (ID #{person.Id}).");
            }

            // Finally, check that the id passed in is a valid id of someone in the database - PUT can only update a person if it exists.            
            if ((await _repository.GetByIDAsync(id)) == null)
            {
                // If it isn't a valid ID, the HTTP standard allows PUT to create a new resource in its place.
                // Since POST was designed to create a new resource given a person object, we will simply route the request to the appropriate function.
                return await PostPerson(person);
            }

            // If all of the above checks passed and we didn't create a new person, we can give the go-ahead to the database to update the person.
            _repository.Update(person);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/v2/Person/5
        /// <summary>
        /// Deletes a person by their ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>No content</returns>
        /// <response code="204">Successfully deleted the person. No content is returned.</response>
        /// <response code="404">The person with the provided ID does not exist. No action was taken.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _repository.GetByIDAsync(id);

            // If no person was found with the requested ID, return a 404 with a custom error message.
            if (person == null)
            {
                return NotFound($"Person with ID #{id} does not exist.");
            }

            _repository.Delete(id);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
