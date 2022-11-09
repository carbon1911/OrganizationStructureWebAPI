using KrosWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrosWebAPITests.Common
{
	internal static class EmployeeTestData
	{
		internal static Employee GetBarbucha()
		{
			return new Employee
			{
				Id = 1,
				FirstName = "Jan",
				LastName = "Marvanek",
				Email = "jan@marvanek.com",
				PhoneNumber = "1230",
				Salary = 60000,
				Title = "None"
			};
		}
		internal static IEnumerable<Employee> PrepareEmployees()
		{
			return new List<Employee> {
				GetBarbucha(),
				new Employee
				{
					Id = 2,
					FirstName = "Lorem",
					LastName = "Ipsum",
					Email = "lorem@ipsum.com",
					PhoneNumber = "2356523",
					Salary = 40000,
					Title = "Mgr."
				}
			};
		}
	}
}
