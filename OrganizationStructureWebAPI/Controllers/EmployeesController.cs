using Microsoft.AspNetCore.Mvc;
using OrganizationStructureWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using OrganizationStructureWebAPI.Models.OrganizationStructure;

namespace OrganizationStructureWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeesController : ControllerBase
	{
		private readonly OrganizationStructureDbContext _context;

		public EmployeesController(OrganizationStructureDbContext context) => _context = context;

		// GET: api/Employees
		/// <summary>
		/// Returns all employees.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
		{
			return Ok(await _context.Employees.ToArrayAsync());
		}

		// GET: api/Employees/5
		/// <summary>
		/// Returns employee with its all organization structure nodes let by the employee or NotFound if the division
		/// with given <paramref name="id"/> does not exist.
		/// </summary>
		/// <param name="id"></param>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetEmployee(int id)
		{
			var fetchedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
			if (fetchedEmployee == null)
			{
				return NotFound($"Employee with ID {id} does not exist.");
			}

			return Ok(
				new 
				{ 
					emplyoee = fetchedEmployee, 
					ledNodes = GetLedNodes(fetchedEmployee).Select(n => Convert.ChangeType(n, n.GetType())) 
				}
			);
		}

		// PUT: api/Employees/5
		/// <summary>
		/// Updates the employee's data.
		/// </summary>
		[HttpPut]
		public async Task<ActionResult<Employee?>> PutEmployee([FromForm] Employee employee)
		{
			var fetchedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);
			if (fetchedEmployee == null)
			{
				return NotFound($"Employee with ID {employee.Id} does not exist.");
			}

			_context.Entry(fetchedEmployee).CurrentValues.SetValues(employee);

			await _context.SaveChangesAsync();
			return Ok(fetchedEmployee);
		}

		// POST: api/Employees/
		/// <summary>
		/// Creates a new employee or returns Conflict if employee with given ID already exists.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<Employee?>> PostEmployee([FromForm] Employee employee)
		{
			if (await _context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id) != null)
			{
				return Conflict($"Employee with ID {employee.Id} already exists.");
			}

			_context.Employees.Add(employee);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id, Employee = employee }, employee);
		}

		// DELETE: api/Employees/5
		/// <summary>
		/// Deletes a employee if it exists and is not a lead of any node.
		/// </summary>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		public async Task<ActionResult<Employee>> DeleteEmployee(int id)
		{
			var fetchedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
			if (fetchedEmployee == null)
			{
				return NotFound($"Employee with ID {id} does not exist.");
			}

			if (GetLedNodes(fetchedEmployee).Any())
			{
				return BadRequest($"Employee with ID {id} is a lead of some organization node. " +
					$"Change the lead of the organization node first and then try removing the employee.");
			}

			_context.Employees.Remove(fetchedEmployee);
			await _context.SaveChangesAsync();
			return Ok(fetchedEmployee);
		}

		private IEnumerable<OrganizationStructureNode> GetLedNodes(Employee employee)
		{
			var returnValue = new List<OrganizationStructureNode>();
			returnValue.AddRange(_context.Companies.Where(c => c.Id == employee.Id));
			returnValue.AddRange(_context.Divisions.Where(d => d.Id == employee.Id));
			returnValue.AddRange(_context.Projects.Where(p => p.Id == employee.Id));
			returnValue.AddRange(_context.Departments.Where(d => d.Id == d.Id));
			return returnValue;
		}
	}
}
