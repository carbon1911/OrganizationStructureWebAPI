using KrosWebAPI.Controllers;
using KrosWebAPI.Models.OrganizationStructure;
using KrosWebAPI.Repositories;
using Moq;

namespace KrosWebAPITests.Common
{

	internal class DivisionTestData
	{
		internal static readonly string DIVISION_1 = "{" +
			"\r\n  \"id\": 0," +
			"\r\n  \"name\": \"string\"," +
			"\r\n  \"code\": 0," +
			"\r\n  \"divisionLead\": " +
			"{\r\n    \"id\": 0,\r\n    \"firstName\": \"string\",\r\n    \"lastName\": \"string\",\r\n    \"title\": \"string\",\r\n    \"phoneNumber\": \"string\",\r\n    \"email\": \"user@example.com\",\r\n    \"salary\": 0\r\n  }," +
			"\r\n  \"divisionLeadId\": 0," +
			"\r\n  \"company\": {\r\n    \"id\": 0,\r\n    \"name\": \"string\",\r\n    \"code\": 0,\r\n    \"director\": {\r\n      \"id\": 0,\r\n      \"firstName\": \"string\",\r\n      \"lastName\": \"string\",\r\n      \"title\": \"string\",\r\n      \"phoneNumber\": \"string\",\r\n      \"email\": \"user@example.com\",\r\n      \"salary\": 0\r\n    },\r\n    \"directorId\": 0,\r\n    \"divisions\": [\r\n      \"string\"\r\n    ]\r\n  },\r\n  " +
			"\"projects\": [\r\n    {\r\n      \"id\": 0,\r\n      \"name\": \"string\",\r\n      \"code\": 0,\r\n      \"projectLead\": {\r\n        \"id\": 0,\r\n        \"firstName\": \"string\",\r\n        \"lastName\": \"string\",\r\n        \"title\": \"string\",\r\n        \"phoneNumber\": \"string\",\r\n        \"email\": \"user@example.com\",\r\n        \"salary\": 0\r\n      },\r\n      \"projectLeadId\": 0,\r\n      \"division\": \"string\"\r\n    }\r\n  ]\r\n" +
			"}";

		internal static OldDivisionsController GetThrowingController()
		{
			var mockRepo = new Mock<IOrganizationStructureRepository<Division>>();
			mockRepo.Setup(repo => repo.Add(It.IsAny<Division>())).Throws(new ArgumentException());
			return new DivisionsController(mockRepo.Object);
		}
	}
}
