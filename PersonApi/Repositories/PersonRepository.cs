using PersonApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Repositories
{
    public class PersonRepository : RepositoryBase<Person, int>
    {
        public PersonRepository(PersonContext context) : base(context) { }

        public IEnumerable<Person> GetAllPersons()
        {
            return _dbSet;
        }

        public Person GetPersonById(int id)
        {
            return _dbSet.Where(p => p.Id == id).First();
        }
    }
}