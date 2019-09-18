# PersonAPI
An example ASP.NET Core Web Api implementing:
- Swashbuckle/Swagger/OpenAPI
- MOQ/unit testing
- Repository pattern
- Serilog
- Strongly-typed configurations
- Versioning

## Getting Started
Clone this repo and open it in Visual Studio. Launch using IIS Express. To access the generated documentation, navigate to `https://localhost:[####]/swagger`.

## Branches
The `master` branch contains the "workbook" version of the API, with unfinished tasks for developers to try their hand at.
The `solutions` branch supplements the solutions to unfinished tasks, and is a fully functional example API.

## Workbook Unfinished Task List
 - Implement GetPersonsByFilter() in V1.PersonController so that it passes its three unit tests: GetPersonByFilterTestNoResults(),
GetPersonByFilterTestValid(), and GetPersonByFilterTestNoFilter().
