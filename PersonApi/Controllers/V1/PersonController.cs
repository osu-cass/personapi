using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonApi.Configurations;
using PersonApi.Models;
using PersonApi.Repositories;

namespace PersonApi.Controllers.V1
{
    /// <summary>
    /// Version 1 of the PersonController. Contains implementations of PUT, POST, DELETE, and various GETs.
    /// </summary>
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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

            // Additionally, we inject IOptions<ProjectConfigurations> which uses (object relational) mapping to
            // take property values from appsettings.json and store them in the ProjectConfigurations object.
            _projectConfigurations = settings.Value;
        }

        // GET: api/v1/Person/Info
        /// <summary>
        /// Gets information about the API and its version.
        /// </summary>
        /// <returns>API version information.</returns>
        /// <response code="200">Successfully returned information about the API.</response>
        [HttpGet]
        [Route("Info")]
        public string GetInfo()
        {
            return "You are using PersonAPI Version 1!";
        }

        // GET: api/v1/Person
        /// <summary>
        /// Gets all persons.
        /// </summary>
        /// <returns>A list of all persons.</returns>
        /// <response code="200">Successfully returned a list of all persons.</response>
        [HttpGet]
        public async Task<IEnumerable<Person>> GetPersons()
        {
            // For those familiar with async programming, "await"ing this call may seem silly because we are essentially 
            // executing the code synchronously, however the benefit is that if someone calls the API endpoint multiple
            // times in a row very quickly, ASP.NET Core can optimize threads in the background.
            return await _repository.GetAsync();
        }

        // GET: api/v1/Person/Filter
        /// <summary>
        /// Gets all persons that match the filter, which is passed in through the query string.
        /// </summary>
        /// <remarks>
        /// Query string parameters are automatically mapped to the Filter class. For example,
        /// 
        ///     GET /api/v1/Person/Filter?LikesChocolate=true[AMPERSAND]MaxNumberOfResults=3
        ///     
        /// will produce an instance of the Filter class in which the bool LikesChocolate is set to true,
        /// and int NumberOfResults is set to 3. Since we left out the third possible variable, it is automatically 
        /// set to null, indicating we should ignore that filter trait. This object is passed into GetPersonsByFilter(...),
        /// and we can use its members to narrow down search results. In this case, we should return the first 3
        /// people who like chocolate.
        /// 
        /// Another sample query:
        /// 
        ///     GET /api/v1/Person/Filter?Name="Charles+Dickens"
        ///
        /// will produce an instance of the Filter class in which Name is set to "Charles Dickens", and the other
        /// two variables are null. In this case, we should return all people with the name Charles Dickens.
        /// </remarks>
        /// <returns>A list of all persons that match the filter.</returns>
        /// <response code="200">Successfully returned a list of all persons that match the filter.</response>
        /// <response code="400">No filter was provided.</response>
        /// <response code="404">There were no people that matched the provided filter.</response>

        // TODO: Data attributes will go here. You will need one to indicate this is a GET method, and one
        // that specifies that calls to api/v1/Person/Filter should be routed to this method. Look to
        // GetInfo() if you need an example.
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonsByFilter([FromQuery] Filter filter)
        {
            // TODO: Implement this method. The method signature has been provided, and all information about the
            // functionality of this method is provided in the above XML comments, and in the Filter.cs class.

            // The unit tests that correspond to this method are located in PersonApiTest.PersonControllerTest and are:
            // - GetPersonByFilterTestValid()
            // - GetPersonByFilterTestNoFilter()
            // - GetPersonByFilterTestNoResults()

            // A note about the return type: While Task<ActionResult<IEnumerable<Person>>> may look formidable,
            // this function accepts the following return statements:
            //      return BadRequest("Custom error text goes here"); will return a 400 Status Code
            //      return NotFound("Custom error text goes here"); will return a 404 Status Code
            //      return Ok(object of type IEnumerable<Person> containing the filter results goes here); will return a 200 Status Code.
            throw new NotImplementedException();
        }

        // GET api/v1/Person/5
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

        // POST api/v1/Person
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

        // PUT api/v1/Person/5
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
        /// <response code="400">Bad Request. Either the provided name is invalid, or 
        /// the ID provided in the URL does not match the one in the request body.</response>
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
                // Since POST creates a new resource given a person object, we will simply route the request to the appropriate function.
                return await PostPerson(person);
            }

            // If all of the above checks passed and we didn't create a new person, we can give the go-ahead to the database to update the person.
            _repository.Update(person);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/v1/Person/5
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
