using KrosWebAPI.Controllers;
using KrosWebAPI.Models;
using KrosWebAPI.Models.OrganizationStructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace KrosWebAPITests
{
	public class EmployeeTests
	{
		[Fact]
		public async Task DeleteEmployee_EmployeeAssignedToCompany_Test()
		{
			var mockRepo = new Mock<IOrganizationStructureRepository<Employee>>();
			var controller = new EmployeesController((IEmployeeRepository)mockRepo.Object);
			mockRepo.Setup(repo => repo.Add(It.IsAny<Employee>())).Throws(new ArgumentException());

			var employeeJson = "\"id\": 1,\r\n" +
								"\"firstName\": \"bruno\",\r\n" +
								"\"lastName\": \"cicmanec\",\r\n" +
								"\"title\": \"senor\",\r\n" +
								"\"phoneNumber\": \"1234\",\r\n" +
								"\"email\": \"hello@world.com\",\r\n" +
								"\"salary\": 42";

			int companyId = 1;
			var companyJson = $"{{\"id\": {companyId}," +
				"\r\n  \"name\": \"Hangar 13\"," +
				"\r\n  \"code\": 0," +
				$"\r\n  \"directorId\": {employeeJson}";
			var company = JsonConvert.DeserializeObject<Company>(companyJson);

			var deleted = await controller.DeleteEmployee(company!.Director!.Id);

			Assert.IsType<BadRequestResult>(deleted);
		}
	}
}