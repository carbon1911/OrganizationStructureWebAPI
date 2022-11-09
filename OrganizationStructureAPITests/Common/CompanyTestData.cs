using KrosWebAPI.Controllers;
using KrosWebAPI.Models;
using KrosWebAPI.Models.OrganizationStructure;
using Moq;

namespace KrosWebAPITests.Common
{
	internal class CompanyTestData
	{
		internal static _OldCompaniesController GetThrowingControllerAdd()
		{
			var mockRepo = new Mock<IOrganizationStructureRepository<Company>>();
			mockRepo.Setup(repo => repo.Add(It.IsAny<Company>())).Throws(new ArgumentException());
			return new CompaniesController(mockRepo.Object);
		}
		internal static _OldCompaniesController GetReturningControllerAdd()
		{
			var mockRepo = new Mock<IOrganizationStructureRepository<Company>>();
			mockRepo.Setup(repo => repo.Add(It.IsAny<Company>())).Returns(Task.CompletedTask);
			return new CompaniesController(mockRepo.Object);
		}
		internal static _OldCompaniesController GetThrowingControllerGet()
		{
			var mockRepo = new Mock<IOrganizationStructureRepository<Company>>();
			mockRepo.Setup(repo => repo.Get(It.IsAny<int>())).Throws(new ArgumentException());
			return new CompaniesController(mockRepo.Object);
		}
	}
}
