#define TESTS
using KrosWebAPI.Controllers;
using KrosWebAPI.Models;
using KrosWebAPI.Models.OrganizationStructure;
using KrosWebAPI.Repositories;
using KrosWebAPITests.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Newtonsoft.Json;


namespace KrosWebAPITests
{
	public class CompanyTests
	{
		[Fact]
		public async Task GetCompany_NonExistingId_Test()
		{
			var controller = CompanyTestData.GetThrowingControllerGet();

			var result = await controller.GetCompany(2);

			Assert.IsType<NotFoundObjectResult>(result.Result);
		}

		[Fact]

		// Post == Create
		public async Task PostCompany_CompanyBodyEmployeeBody_Test()
		{
			List<Employee> employees = EmployeeTestData.PrepareEmployees().ToList();
			var companyToPost = new Company
			{
				Id = 1,
				Code = 123,
				Director = employees[0],
				DirectorId = 1,
				Name = "Hangar 13",
			};

			var mockRepo = new Mock<IOrganizationStructureRepository<Company>>();
			mockRepo.Setup(repo => repo.Add(It.IsAny<Company>())).Returns(Task.CompletedTask);
			mockRepo.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(companyToPost);
			var controller = new CompaniesController(mockRepo.Object);

			var result = await controller.PostCompany(companyToPost);

			Assert.NotNull(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var company = Assert.IsType<Company>(createdAtActionResult.Value);
			Assert.Equal(company.Director.Id, EmployeeTestData.GetBarbucha().Id);
			Assert.Equal(1, company.Id);
			Assert.Equal("Hangar 13", company.Name);
			Assert.Equal(new List<Division>(), company.Divisions);

			controller = CompanyTestData.GetThrowingControllerAdd();

			result = await controller.PostCompany(companyToPost);

			Assert.NotNull(result);
			Assert.IsType<ConflictObjectResult>(result.Result);
		}

		[Fact]
		public async Task PostCompany_CompanyBodyEmployeeId_Test()
		{
			int companyId = 1;
			var json = $"{{\"id\": {companyId}," +
				"\r\n  \"name\": \"Hangar 13\"," +
				"\r\n  \"code\": 0," +
				"\r\n  \"directorId\": 1}";
			var company = JsonConvert.DeserializeObject<Company>(json);

			var mockRepo = new Mock<IOrganizationStructureRepository<Company>>();
			var controller = new CompaniesController(mockRepo.Object);
			mockRepo.Setup(repo => repo.Add(It.IsAny<Company>())).Returns(Task.CompletedTask);
			mockRepo.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(() =>
			{
				var modifiedCompany = company;
				modifiedCompany!.Director = EmployeeTestData.GetBarbucha();
				return modifiedCompany;
			});
			List<Employee> employees = EmployeeTestData.PrepareEmployees().ToList();

			var result = await controller.PostCompany(company!);

			Assert.NotNull(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedCompany = Assert.IsType<Company>(createdAtActionResult.Value);

			Assert.Equal(returnedCompany.Director.Id, employees[0].Id);
			Assert.Equal(1, returnedCompany.Id);
			Assert.Equal("Hangar 13", returnedCompany.Name);

			mockRepo.Setup(repo => repo.Add(It.IsAny<Company>())).Throws(new ArgumentException());

			result = await controller.PostCompany(company!);

			Assert.NotNull(result);
			Assert.IsType<ConflictObjectResult>(result.Result);
		}

		[Fact]
		// change director of the company
		public async Task PutCompany_NewEmployee_Test()
		{
			int companyId = 1;
			var company = JsonConvert.DeserializeObject<Company>(
				$"{{\"id\": {companyId}," +
				"\r\n  \"name\": \"Hangar 13\"," +
				"\r\n  \"code\": 0," +
				"\r\n  \"directorId\": 2}"
			);

			var mockRepo = new Mock<IOrganizationStructureRepository<Company>>();
			var controller = new CompaniesController(mockRepo.Object);
			mockRepo.Setup(repo => repo.Update(It.IsAny<Company>())).ReturnsAsync(() =>
			{
				var localCompany = company;
				localCompany!.Director = EmployeeTestData.PrepareEmployees().ToArray()[1];
				return localCompany;
			});
			List<Employee> employees = EmployeeTestData.PrepareEmployees().ToList();

			var result = await controller.PutCompany(companyId, company!);

			Assert.NotNull(result);
			var createdAtActionResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedCompany = Assert.IsType<Company>(createdAtActionResult.Value);
			Assert.Equal(returnedCompany.Director.Id, employees[1].Id);
			Assert.Equal(1, returnedCompany.Id);
			Assert.Equal("Hangar 13", returnedCompany.Name);
		}

		[Fact]
		// change director of the company
		public async Task PutCompany_CompanyIdEmployeeId_Test()
		{
			int companyId = 1;
			var company = JsonConvert.DeserializeObject<Company>(
				$"{{\"id\": {companyId}," +
				"\r\n  \"name\": \"Hangar 13\"," +
				"\r\n  \"code\": 0," +
				"\r\n  \"directorId\": 1}"
			);

			var mockRepo = new Mock<IOrganizationStructureRepository<Company>>();
			var controller = new CompaniesController(mockRepo.Object);
			mockRepo.Setup(repo => repo.Update(It.IsAny<Company>())).ReturnsAsync(() =>
			{ 
				var localCompany = company;
				localCompany!.Director = EmployeeTestData.PrepareEmployees().ToArray()[1];
				return localCompany; 
			}
			);
			List<Employee> employees = EmployeeTestData.PrepareEmployees().ToList();

			var companyFromJson = JsonConvert.DeserializeObject<Company>(
				$"{{\"id\": {companyId}," +
				"\r\n  \"directorId\": 2}"
			);

			var result = await controller.PutCompany(companyId, companyFromJson!);

			Assert.NotNull(result);
			var createdAtActionResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedCompany = Assert.IsType<Company>(createdAtActionResult.Value);
			Assert.Equal(returnedCompany.Director.Id, employees[1].Id);
			Assert.Equal(1, returnedCompany.Id);
			Assert.Equal("Hangar 13", returnedCompany.Name);
		}
	}
}
