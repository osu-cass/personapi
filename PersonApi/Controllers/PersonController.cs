﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonApi.Models;
using PersonApi.Repositories;

namespace PersonApi.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/Person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        // The repository for 
        private readonly IRepository<Person, int> _repository;

        // The constructor uses dependency injection to inject IRepository into the controller
        public PersonController(IRepository<Person, int> repository)
        {
            _repository = repository;
        }

        // GET: api/Person
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

        // GET: api/Person/ChocolateLovers
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

        // GET api/Person/5
        /// <summary>
        /// Gets a single specific person by their ID.
        /// </summary>
        /// <returns>A single person.</returns>
        /// <param name="id">The ID of the requested person.</param>
        /// <returns>The person requested.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _repository.GetByIDAsync(id);

            if (person == null)
            {
                return NotFound($"Person with ID #{id} does not exist.");
            }

            return Ok(person);
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
            // Check that person's name has a valid character set (using regex name pattern from https://www.regextester.com/93648).
            // We do not need to test ID or LikesChocolate as they are strongly typed in the person object.
            if (!Regex.Match(person.Name, "^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$").Success)
            {
                return BadRequest($"Name \'{person.Name}\' is not valid.");
            }

            _repository.Insert(person);
            await _repository.SaveChangesAsync();

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
                return BadRequest($"Person ID provided in URL (ID #{id}) does not match Person ID provided in PUT header (ID #{person.Id}).");
            }

            // Finally, check that the id passed in is a valid id of someone in the database - PUT can only update a person if it exists.
            if ((await _repository.GetByIDAsync(id)) == null)
            {
                // If it isn't a valid ID, the HTTP standard allows PUT to create a new resource in its place.
                // Since POST was designed to create a new resource given a person object, we will simply route the request to the appropriate function.
                return await PostPerson(person);
            }

            _repository.Update(person);
            await _repository.SaveChangesAsync();

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
