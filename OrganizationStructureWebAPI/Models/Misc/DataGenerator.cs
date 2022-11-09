using Bogus;
using OrganizationStructureWebAPI.Models.OrganizationStructure;

namespace OrganizationStructureWebAPI.Models.Misc
{
	/// <summary>
	/// Example data creation for db testing.
	/// </summary>
	public class DataGenerator
	{
		public static void Initialize(OrganizationStructureDbContext dbContext)
		{
			dbContext.Database.EnsureCreated();

			var employees = GenerateEmployees(dbContext);

			var testCompanies = new Faker<Company>()
				.RuleFor(c => c.Name, rule => rule.Company.CompanyName())
				.RuleFor(c => c.Code, rule => new Random().Next());

			//var company = testCompanies.Generate();
			var companies = new List<Company>();
			for (int i = 1; i < 3; ++i)
			{
				companies.Add(new Company()
				{
					Name = $"Company{i}",
					Id = i,
					Code = i,
					Director = employees[0],
					DirectorId = employees[0].Id,
				});
			}

			var company = companies[0];
			company.Director = employees[0];
			company.DirectorId = employees[0].Id;

			var company2 = companies[1];

			foreach (var c in companies)
			{
				dbContext.Add(c);
			}


			dbContext.SaveChanges();

			/**********************************************************************/

			//var testDivisions = new Faker<Division>()
			//	.RuleFor(d => d.Name, rule => rule.Commerce.Department())
			//	.RuleFor(d => d.Code, rule => new Random().Next());

			//var divisions = testDivisions.Generate(2);

			var divisions = new List<Division>();
			for (int i = 1; i < 5; ++i)
			{
				divisions.Add(new Division()
				{
					Code = i,
					Company = i < 3 ? company : company2,
					DivisionLead = employees[0],
					DivisionLeadId = employees[0].Id,
					Id = i,
					Name = i.ToString()
				});
			}

			var zerothDivision = divisions[0];
			//zerothDivision.DivisionLead = employees[0];
			//zerothDivision.DivisionLeadId = employees[0].Id;
			//zerothDivision.Company = company;

			var firstDivision = divisions[1];
			//firstDivision.DivisionLead = employees[1];
			//firstDivision.DivisionLeadId = employees[1].Id;
			//firstDivision.Company = company;

			foreach (var division in divisions)
			{
				dbContext.Add(division);
			}

			dbContext.SaveChanges();

			/**********************************************************************/

			var testProjects = new Faker<Project>()
				.RuleFor(p => p.Name, rule => rule.Commerce.ProductName())
				.RuleFor(p => p.Code, rule => new Random().Next());

			var projects = testProjects.Generate(2);

			projects[0].ProjectLead = employees[0];
			projects[0].ProjectLeadId = employees[0].Id;
			projects[0].Division = zerothDivision;

			projects[1].ProjectLead = employees[0];
			projects[1].ProjectLeadId = employees[0].Id;
			projects[1].Division = zerothDivision;

			foreach (var project in projects)
			{
				dbContext.Add(project);
			}

			dbContext.SaveChanges();

			/**********************************************************************/

			var testDepartments = new Faker<Department>()
				.RuleFor(d => d.Name, rule => rule.Commerce.Department())
				.RuleFor(d => d.Code, rule => new Random().Next());

			var departments = testDepartments.Generate(2);

			departments[0].DepartmentLead = employees[0];
			departments[0].DepartmentLeadId = employees[0].Id;
			departments[0].Project = dbContext.Entry(projects[0]).Entity;

			departments[1].DepartmentLead = employees[0];
			departments[1].DepartmentLeadId = employees[0].Id;
			departments[1].Project = dbContext.Entry(projects[0]).Entity;

			foreach (var department in departments)
			{
				dbContext.Add(department);
			}

			dbContext.SaveChanges();
		}

		private static List<Employee> GenerateEmployees(OrganizationStructureDbContext dbContext)
		{
			var testEmployees = new Faker<Employee>()
				.RuleFor(e => e.FirstName, ruleFunction => ruleFunction.Name.FirstName())
				.RuleFor(e => e.LastName, ruleFunction => ruleFunction.Name.LastName())
				.RuleFor(e => e.Title, () =>
				{
					var titles = new List<string> { "Bc.", "MSc.", "Ing." };
					return titles[new Random().Next(titles.Count)];
				})
				.RuleFor(e => e.Email, ruleFunction => ruleFunction.Person.Email)
				.RuleFor(e => e.PhoneNumber, ruleFunction =>
				{
					var phoneNumbers = new List<string> { "+4215 23 456 778", "+421 123 456 789", "+420 222 064 500" };
					return phoneNumbers[new Random().Next(phoneNumbers.Count)];
				}
				)
				.RuleFor(e => e.Salary, () => new Random().Next(1000, 5000));

			var employees = testEmployees.Generate(2);
			foreach (var employee in employees)
			{
				dbContext.Add(employee);
			}
			return employees;
		}
	}
}
