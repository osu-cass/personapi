using PersonApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Repository
{
    public class PersonRepository : IRepository<Models.Person>
    {
        // The database context for a Person object.
        private readonly PersonContext _context;

        // The constructor uses dependency injection to inject PersonContext (the database context) into the Repository
        public PersonRepository(PersonContext context)
        {
            _context = context;
        }

        public void Create(Models.Person person)
        {
            _context.Persons.Add(person);
        }

        public void Delete(Models.Person person)
        {
            throw new NotImplementedException();
        }

        public Models.Person GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Models.Person person)
        {
            throw new NotImplementedException();
        }
    }
}
