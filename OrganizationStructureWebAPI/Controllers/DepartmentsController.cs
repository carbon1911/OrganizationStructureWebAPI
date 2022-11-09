using OrganizationStructureWebAPI.Models;
using OrganizationStructureWebAPI.Models.Misc;
using OrganizationStructureWebAPI.Models.OrganizationStructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using BaseDepartmentsController = OrganizationStructureWebAPI.Controllers.OrganizationStructureBaseController
	<OrganizationStructureWebAPI.Models.OrganizationStructure.Project, 
	OrganizationStructureWebAPI.Models.OrganizationStructure.Department, 
	OrganizationStructureWebAPI.Models.Misc.OrganizationStructureNodeNull>;

namespace OrganizationStructureWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DepartmentsController : BaseDepartmentsController
	{
		public DepartmentsController(OrganizationStructureDbContext context, ILogger<BaseDepartmentsController> logger)
			: base(context, logger) { }

		// GET: api/Departments
		/// <summary>
		/// Returns all departments with their leads.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Department>>> GetDepartments() => await Get();

		// GET: api/Departments/5
		/// <summary>
		/// Returns the department with its lead or NotFound if the department
		/// with given <paramref name="id"/> does not exist.
		/// </summary>
		/// <param name="id"></param>
		[HttpGet("id")]
		public async Task<ActionResult<Department>> GetDepartment(int id) => await Get(id);

		// PUT: api/Departments
		/// <summary>
		/// Facilitates modification of the name and code of given department.
		/// </summary>
		[HttpPut]
		public async Task<ActionResult<Department>> PutDepartment(OrganizationStructureNodeDTO newData) => await Put(newData);

		// PUT: api/Departments/5/2
		/// <summary>
		/// Facilitates the change of department's lead.
		/// </summary>
		[HttpPut("{departmentId}/{departmentLeadId}")]
		public async Task<ActionResult<Department>> PutDepartment(int departmentId, int departmentLeadId) => await Put(departmentId, departmentLeadId);

		// POST: api/Departments/2/5
		/// <summary>
		/// Creates a new company with <paramref name="departmentLeadId"/> department lead ID and <paramref name="projectId"/> parent project ID.
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="departmentLeadId"></param>
		/// <param name="newData"></param>
		/// <returns></returns>
		[HttpPost("{departmentLeadId}/{projectId}")]
		public async Task<ActionResult<Department>> PostDepartment(int departmentLeadId, int projectId, OrganizationStructureNodeDTO newData) => await Post(departmentLeadId, newData, projectId);

		// DELETE: api/Departments/2
		/// <summary>
		/// Deletes department with given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		public async Task<ActionResult<Department>> DeleteDepartment(int id) => await Delete(id);

		protected override IQueryable<Department> GetQuery() => _currents.Include(d => d.DepartmentLead);

		protected override IEnumerable<OrganizationStructureNodeNull> GetChildrenAsEnumerable() => throw new NotImplementedException();

		protected override async Task<Project?> GetParent(int id) => await _parents!.FirstOrDefaultAsync(p => p.Id == id);

		protected override void SetLead(Department node, Employee employee)
		{
			node.DepartmentLead = employee;
			node.DepartmentLeadId = employee.Id;
		}

		protected override void AddChild(Department parent, OrganizationStructureNodeNull child) => throw new NotImplementedException();

		protected override Department SetParentNode(Department node, Project parent)
		{
			node.Project = parent;
			return node;
		}
	}
}
