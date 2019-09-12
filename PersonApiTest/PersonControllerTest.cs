using System;
using Xunit;
using Moq;
using PersonApi.Repositories;
using PersonApi.Models;
using System.Collections.Generic;
using PersonApi.Controllers;

namespace PersonApiTest
{
    public class PersonControllerTest
    {
        // Our fake "database" to to test our controller against
        public List<Person> fakePersons = new List<Person>()
        {
            new Person(){ Id = 1, Name = "Margaret Thatcher", LikesChocolate = true },
            new Person(){ Id = 2, Name = "William Shakespeare", LikesChocolate = true },
            new Person(){ Id = 3, Name = "George Orwell", LikesChocolate = false },
            new Person(){ Id = 4, Name = "J.K. Rowling", LikesChocolate = false },
            new Person(){ Id = 5, Name = "Harper Lee", LikesChocolate = true },
        };

        // Set up mock repository that returns our fakePersons list upon its Get(...) function being called
        public Mock<IRepository<Person, int>> fakePersonRepository = new Mock<IRepository<Person, int>>();

        [Fact]
        public void GetPersonsTest()
        {
            // For this test, we want to be able to emulate the GetPersons() function of the PersonControllerTest.
            // In order for the GetPersons() function to work, we must define the behavior of any repository functions it calls.
            fakePersonRepository.Setup(x => x.Get(null, null, "")).Returns(fakePersons);

            // By passing the mock repository's object into our controller, we have
            // successfully used dependency injection to mock a working repository
            PersonController personController = new PersonController(fakePersonRepository.Object);

            //// Now, we confirm that the controller successfully returns all persons.
            //Assert.Equal(fakePersons, personController.GetPersons());
        }
    }
}
