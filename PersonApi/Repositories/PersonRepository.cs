using PersonApi.Models;

namespace PersonApi.Repositories
{
    public class PersonRepository : RepositoryBase<Person, int>
    {
        public PersonRepository(PersonContext context) : base(context) { }

    }
}