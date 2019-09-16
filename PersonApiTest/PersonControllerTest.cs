using System;
using Xunit;
using Moq;
using PersonApi.Repositories;
using PersonApi.Models;
using System.Collections.Generic;
using PersonApi.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OkResult = Microsoft.AspNetCore.Mvc.OkResult;
using PersonApi.Controllers.V1;
using Microsoft.Extensions.Options;
using PersonApi.Configurations;

namespace PersonApiTest
{
    public class PersonControllerTest
    {
        // Our fake "database" to to test our controller against.
        public List<Person> fakePersons = new List<Person>()
        {
            new Person(){ Id = 1, Name = "Margaret Thatcher", LikesChocolate = true },
            new Person(){ Id = 2, Name = "William Shakespeare", LikesChocolate = true },
            new Person(){ Id = 3, Name = "George Orwell", LikesChocolate = false },
            new Person(){ Id = 4, Name = "J.K. Rowling", LikesChocolate = false },
            new Person(){ Id = 5, Name = "Harper Lee", LikesChocolate = true },
        };

        // Set up mock repository. Since our tests should be able to run without a live database instance,
        // we can pass it a fake repository using MOQ to emulate the functions we need.
        public Mock<IRepository<Person, int>> fakePersonRepository = new Mock<IRepository<Person, int>>();

        // Our constructor also requires a mock of IOptions.
        public Mock<IOptions<ProjectConfigurations>> fakeProjectConfigurations = new Mock<IOptions<ProjectConfigurations>>();

        /// <summary>
        /// Test for PersonController's GetPersons() function.
        /// </summary>
        [Fact]
        public void GetPersonsTest()
        {
            // For this test, we want to be able to emulate the GetPersons() function of the PersonControllerTest.
            // In order for the GetPersons() function to work, we must define the behavior of any repository functions it calls.
            fakePersonRepository.Setup(x => x.Get(null, null, "")).Returns(fakePersons);

            // By passing the mock repository's object into our controller, we have
            // successfully used dependency injection to mock a working repository.
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Confirm that the controller successfully returns all persons.
            Assert.Equal(fakePersons, personController.GetPersons().ToList());
        }

        /// <summary>
        /// Test for PersonController's GetPersonsThatLikeChocolate() function
        /// </summary>
        [Fact]
        public void GetPersonsThatLikeChocolateTest()
        {
            // Set up fake repository and inject into controller.
            fakePersonRepository.Setup(x => x.Get(null, null, "")).Returns(fakePersons);
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Confirm that the controller successfully returns all persons that like chocolate.
            Assert.Equal(fakePersons.Where(d => d.LikesChocolate == true), personController.GetPersonsThatLikeChocolate().ToList());
        }

        /// <summary>
        /// Test that PersonController's GetPerson() function returns valid requests
        /// </summary>
        /// <remarks>
        /// A valid request is defined by providing an ID that corresponds to an existing person.
        /// </remarks>
        [Fact]
        public async void GetPersonTestValid()
        {
            // Set up fake repository and inject into controller.
            fakePersonRepository.Setup(x => x.GetByIDAsync(2)).ReturnsAsync(fakePersons.FirstOrDefault(p => p.Id == 2));
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Confirm that when we request the person with ID #2, the controller returns a non-null person, a 200 (Success) status code, and the correct person.
            ActionResult<Person> personResult = await personController.GetPerson(2);
            var successResult = personResult.Result as ObjectResult;
            Assert.NotNull(personResult);
            Assert.Equal(200, successResult.StatusCode);
            Assert.Equal(fakePersons.FirstOrDefault(p => p.Id == 2), successResult.Value);
        }

        /// <summary>
        /// Test that PersonController's GetPerson() function returns errors upon invalid requests
        /// </summary>
        /// <remarks>
        /// An invalid request is defined by providing an ID that does not correspond to an existing person.
        /// </remarks>
        [Fact]
        public async void GetPersonTestInvalidId()
        {
            // Set up fake repository and inject into controller.
            fakePersonRepository.Setup(x => x.GetByIDAsync(2)).ReturnsAsync(fakePersons.FirstOrDefault(p => p.Id == 2));
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Confirm that when we request a person with ID #6 (which doesn't exist), the controller returns a null person.
            ActionResult<Person> personResult = await personController.GetPerson(6);
            Assert.Null(personResult.Value);

            // Confirm that we receive a 404 (Not Found) and a custom error message.
            var notFoundResult = personResult.Result as NotFoundObjectResult;
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Person with ID #6 does not exist.", notFoundResult.Value);
        }

        /// <summary>
        /// Test that PersonController's PostPerson() function successfully posts a valid Person.
        /// </summary>
        /// <remarks>
        /// A valid person is defined as one with a valid Name character set
        /// </remarks>
        [Fact]
        public async void PostPersonTestValid()
        {
            // Set up fake repository and inject into controller.
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Post a valid person (defined by having a valid Name) to the controller.
            ActionResult<Person> personResult = await personController.PostPerson(new Person { Id = 6, Name = "Mark Twain", LikesChocolate = false });

            // Confirm we receive a 201 (Created) status code and the ID's of the created person and supplied person match.
            var createdResult = personResult.Result as CreatedAtActionResult;
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(6, createdResult.RouteValues["id"]);
        }

        /// <summary>
        /// Test that PersonController's PostPerson() function will fail when given an invalid Person.
        /// </summary>
        /// <remarks>
        /// Here, an invalid person is defined as not having  a valid Name character set
        /// </remarks>
        [Fact]
        public async void PostPersonTestInvalidName()
        {
            // Set up fake repository and inject into controller.
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Post an invalid person (defined by having an invalid Name) to the controller.
            ActionResult<Person> personResult = await personController.PostPerson(new Person { Id = 7, Name = "Inval1d N^ame", LikesChocolate = true });

            // Confirm we do not recieve a personResult, we receive a 400 (Bad Request) status code, and we recieve a custom error message.
            var createdResult = personResult.Result as BadRequestObjectResult;
            Assert.Null(personResult.Value);
            Assert.Equal(400, createdResult.StatusCode);
            Assert.Equal("Name 'Inval1d N^ame' is not valid.", createdResult.Value);
        }

        /// <summary>
        /// Test that PersonController's PutPerson() function will succeed when given a valid Person.
        /// </summary>
        [Fact]
        public async void PutPersonTestValid()
        {
            // Set up fake repository and inject into controller.
            // Here, we want the PUT method to identify that a person with ID #8 already exists, so that it may update it.
            Person existingPerson = new Person { Id = 8, Name = "Shel Silverstein", LikesChocolate = true };
            fakePersonRepository.Setup(x => x.GetByIDAsync(8)).ReturnsAsync(existingPerson);
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Now that we have "established" (in the repository's eyes) that Person #8 already exists, PUT Person #8 with changed fields.
            ActionResult<Person> putPersonResult = await personController.PutPerson(8, new Person { Id = 8, Name = "Roald Dahl", LikesChocolate = false });

            // Upon success, PUT should return a non-null personResult and a 204 (No Content) status code, 
            // indicating that the request was successful but did not return content (or create a new person).
            var createdResult = putPersonResult.Result as NoContentResult;
            Assert.NotNull(putPersonResult);
            Assert.Equal(204, createdResult.StatusCode);
        }

        /// <summary>
        /// Test that PersonController's PutPerson() function will create a new person (instead of updating) when given an invalid Person.
        /// </summary>
        /// <remarks>
        /// Here, an invalid person is defined by providing an id which does not correspond to an existing person. Since PUT requires that we send the entire
        /// updated entity, we can go ahead and create a new person as we would in a POST.
        /// </remarks>
        [Fact]
        public async void PutPersonTestCreateNewPerson()
        {
            // Set up fake repository and inject into controller.
            // Here, we want the PUT method to identify that a person with ID #8 does NOT already exist, so that it may create a new person.
            fakePersonRepository.Setup(x => x.GetByIDAsync(8)).ReturnsAsync(() => null);
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // Now that we have "established" (in the repository's eyes) that Person #8 doesn't exist, PUT Person #8.
            ActionResult<Person> putPersonResult = await personController.PutPerson(8, new Person { Id = 8, Name = "Roald Dahl", LikesChocolate = false });

            // Upon success, PUT should return a non-null personResult and a 201 (Created) status code, indicating that the request successfully created a new person.
            var createdResult = putPersonResult.Result as CreatedAtActionResult;
            Assert.NotNull(putPersonResult);
            Assert.Equal(201, createdResult.StatusCode);
        }

        /// <summary>
        /// Test that PersonController's PutPerson() function will serve a 400 (Bad Request) if the user provides two different IDs (one in the URL, one in the request body)
        /// </summary>
        [Fact]
        public async void PutPersonTestMismatchedIds()
        {
            // Set up fake repository and inject into controller.
            // Here, we want the PUT method to identify that a person with ID #8 already exists, so that it may update it.
            Person existingPerson = new Person { Id = 8, Name = "Shel Silverstein", LikesChocolate = true };
            fakePersonRepository.Setup(x => x.GetByIDAsync(8)).ReturnsAsync(existingPerson);
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // PUT Person #8, but change the first parameter (corresponding to the URL's paramater from PUT /Person/7) to 7.
            ActionResult<Person> putPersonResult = await personController.PutPerson(7, new Person { Id = 8, Name = "Roald Dahl", LikesChocolate = false });

            // Upon request, PUT should return a non-null personResult, a 400 (Bad Request) status code, and a custom error message.
            var createdResult = putPersonResult.Result as BadRequestObjectResult;
            Assert.NotNull(putPersonResult);
            Assert.Equal(400, createdResult.StatusCode);
            Assert.Equal("Person ID provided in URL (ID #7) does not match Person ID provided in PUT body (ID #8).", createdResult.Value);
        }

        /// <summary>
        /// Test that PersonController's PutPerson() function will serve a 400 (Bad Request) if the user provides an invalid Name
        /// </summary>
        [Fact]
        public async void PutPersonTestInvalidName()
        {
            // Set up fake repository and inject into controller.
            // Here, we want the PUT method to identify that a person with ID #8 already exists, so that it may update it.
            Person existingPerson = new Person { Id = 8, Name = "Inval1d N^ame", LikesChocolate = true };
            PersonController personController = new PersonController(fakePersonRepository.Object, fakeProjectConfigurations.Object);

            // PUT Person #8, but change the first parameter (corresponding to the URL's paramater from PUT /Person/7) to 7.
            ActionResult<Person> putPersonResult = await personController.PutPerson(8, existingPerson);

            // Upon request, PUT should return a non-null personResult, a 400 (Bad Request) status code, and a custom error message.
            var createdResult = putPersonResult.Result as BadRequestObjectResult;
            Assert.NotNull(putPersonResult);
            Assert.Equal(400, createdResult.StatusCode);
            Assert.Equal("Name 'Inval1d N^ame' is not valid.", createdResult.Value);
        }
    }
}
