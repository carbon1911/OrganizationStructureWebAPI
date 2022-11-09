using KrosWebAPI.Controllers;
using KrosWebAPI.Models;
using KrosWebAPI.Models.OrganizationStructure;
using KrosWebAPI.Repositories;
using KrosWebAPITests.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace KrosWebAPITests
{
	public class DivisionTests
	{
		[Fact]
		public async Task PostDivision_CompanyIdProjectLeadIdBothExist_Test()
		{
			var mockRepo = new Mock<IOrganizationStructureRepository<Division>>();
			var controller = new DivisionsController(mockRepo.Object);
			mockRepo.Setup(repo => repo.Add(It.IsAny<Division>())).Returns(Task.CompletedTask);

			var division = JsonConvert.DeserializeObject<Division>(
				"{\r\n  \"id\": 1," +
				"\r\n  \"name\": \"cleaning staff\"," +
				"\r\n  \"code\": 314," +
				"\r\n  \"divisionLeadId\": 1," +
				"\r\n  \"companyId\": 1}"
			);

			var divisionLead = new Employee
			{
				Id = 1,
				FirstName = "Jan",
				LastName = "Marvanek",
				Email = "jan@marvanek.com",
				PhoneNumber = "1230",
				Salary = 60000,
				Title = "None"
			};

			int companyId = 1;
			var json = $"{{\"id\": {companyId}," +
				"\r\n  \"name\": \"Hangar 13\"," +
				"\r\n  \"code\": 0," +
				"\r\n  \"directorId\": 1}";
			var company = JsonConvert.DeserializeObject<Company>(json);

			var result = await controller.PostDivision(division!);

			Assert.NotNull(result);
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
			var postedDivision = Assert.IsType<Division>(createdAtActionResult.Value);

			Assert.Equal(divisionLead, postedDivision.DivisionLead);
			Assert.Equal(company, postedDivision.Company);
		}

		[Fact]
		public async Task PostDivision_CompanyIdDivisionLeadIdEitherDoesNotExists_Test()
		{
			var controller = DivisionTestData.GetThrowingController();
			var division = JsonConvert.DeserializeObject<Division>(
				"{\r\n  \"id\": 1," +
				"\r\n  \"name\": \"cleaning staff\"," +
				"\r\n  \"code\": 314," +
				"\r\n  \"companyId\": 1}"
			);

			var result = await controller.PostDivision(division!);

			Assert.NotNull(result);
			Assert.IsType<BadRequestResult>(result);

			division = JsonConvert.DeserializeObject<Division>(
				"{\r\n  \"id\": 1," +
				"\r\n  \"name\": \"cleaning staff\"," +
				"\r\n  \"code\": 314," +
				"\r\n  \"directorId\": 1}"
			);

			result = await controller.PostDivision(division!);

			Assert.NotNull(result);
			Assert.IsType<BadRequestResult>(result);
		}
	}
}
