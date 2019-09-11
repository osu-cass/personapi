using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonApi.Models;

namespace PersonApi.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/Person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        // The database context for a Person object.
        private readonly PersonContext _context;

        // The constructor uses dependency injection to inject PersonContext (the database context) into the controller
        public PersonController(PersonContext context)
        {
            _context = context;

            // *** Uncomment the below lines to automatically generate a couple people upon calling the API ***
            if (_context.Persons.Count() == 0)
            {
                // Create a bunch of new persons if collection is empty,
                // which means you can't delete all persons.
                _context.Persons.Add(new Person { Name = "J.R.R Tolkien", LikesChocolate = true });
                _context.Persons.Add(new Person { Name = "Harper Lee", LikesChocolate = true });
                _context.Persons.Add(new Person { Name = "Edgar Allen Poe", LikesChocolate = false });
                _context.Persons.Add(new Person { Name = "Arthur Conan Doyle", LikesChocolate = true });
                _context.SaveChanges();
            }
        }

        // GET: api/Person
        /// <summary>
        /// Gets all persons.
        /// </summary>
        /// <returns>A list of all persons.</returns>
        /// <response code="200">Successfully returned a list of all persons.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Persons.ToListAsync();
        }

        // GET: api/Person/ChocolateLovers
        /// <summary>
        /// Gets all persons that like chocolate.
        /// </summary>
        /// <returns>A list of all persons that like chocolate.</returns>
        /// <response code="200">Successfully returned a list of all persons that like chocolate.</response>
        [HttpGet]
        [Route("ChocolateLovers")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonsThatLikeChocolate()
        {
            return await _context.Persons.Where(p => p.LikesChocolate == true).ToListAsync();
        }

        // GET api/Person/5
        /// <summary>
        /// Gets a single specific person by their ID.
        /// </summary>
        /// <returns>A single person.</returns>
        /// <param name="id">The ID of the requested person.</param>
        /// <returns>The person requested.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // POST api/Person
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
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        // PUT api/Person/5
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
        /// <response code="204">Successfully updated the person. No content is returned.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(long id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            _context.Entry(person).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/Person/5
        /// <summary>
        /// Deletes a person by their ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>No content</returns>
        /// <response code="204">Successfully deleted the person. No content is returned.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
