using OrganizationStructureWebAPI.Models;
using OrganizationStructureWebAPI.Models.Misc;
using OrganizationStructureWebAPI.Models.OrganizationStructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

using ProjectsBaseController = OrganizationStructureWebAPI.Controllers.OrganizationStructureBaseController
	<OrganizationStructureWebAPI.Models.OrganizationStructure.Division, 
	OrganizationStructureWebAPI.Models.OrganizationStructure.Project, 
	OrganizationStructureWebAPI.Models.OrganizationStructure.Department>;

namespace OrganizationStructureWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectsController : ProjectsBaseController
	{
		public ProjectsController(OrganizationStructureDbContext context, ILogger<ProjectsBaseController> logger) :
			base(context, logger) { }

		// GET: api/Projects
		/// <summary>
		/// Returns all projects with their leads and departments.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Project>>> GetProjects() => await Get();

		// GET: api/Projects/5
		/// <summary>
		/// Returns project with its lead and children departments or NotFound if the project
		/// with given <paramref name="id"/> does not exist.
		/// </summary>
		/// <param name="id"></param>
		[HttpGet("id")]
		public async Task<ActionResult<Project>> GetProject(int id) => await Get(id);

		// PUT: api/Projects
		/// <summary>
		/// Facilitates modification of the name and code of given project.
		/// </summary>
		[HttpPut]
		public async Task<ActionResult<Project>> PutProject(OrganizationStructureNodeDTO newData) => await Put(newData);

		// PUT: api/Projects/5/2
		/// <summary>
		/// Facilitates the change of project's lead.
		/// </summary>
		[HttpPut("{projectId}/{projectLeadId}")]
		public async Task<ActionResult<Project>> PutProject(int projectId, int projectLeadId) => await Put(projectId, projectLeadId);

		// PUT: api/Projects/5/departmentListIds
		/// <summary>
		/// Assigns new departments with <paramref name="departmentListIds"/> IDs to the project with ID <paramref name="projectId"/>.
		/// Removes all former departments that are not in the <paramref name="departmentListIds"/>.
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="departmentListIds"></param>
		[HttpPut("{projectId}/departmentListIds")]
		public async Task<ActionResult<Project>> PutProject(int projectId, ImmutableList<int> departmentListIds) => await Put(projectId, departmentListIds);

		/// <summary>
		/// Creates a new project with <paramref name="projectLeadId"/> project lead ID and <paramref name="divisionId"/> parent division ID.
		/// </summary>
		/// <param name="projectLeadId"></param>
		/// <param name="divisionId"></param>
		/// <param name="newData"></param>
		/// <returns></returns>
		// POST: api/Projects/2/5
		[HttpPost("{projectLeadId}/{divisionId}")]
		public async Task<ActionResult<Project>> PostProject(int projectLeadId, int divisionId, OrganizationStructureNodeDTO newData) => await Post(projectLeadId, newData, divisionId);

		// DELETE: api/Projects/2
		/// <summary>
		/// Deletes project with given <paramref name="id"/> if it has no division assigned.
		/// </summary>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		public async Task<ActionResult<Project>> DeleteProject(int id) => await Delete(id);
		
		protected override void AddChild(Project parent, Department child) => parent.Departments.Add(child);

		protected override async Task<Division?> GetParent(int id) => await _parents!.FirstOrDefaultAsync(p => p.Id == id);

		protected override IQueryable<Project> GetQuery() => _currents.Include(p => p.ProjectLead).Include(p => p.Departments);

		protected override IEnumerable<Department> GetChildrenAsEnumerable() => _children!.ToList();

		protected override void SetLead(Project node, Employee employee)
		{
			node.ProjectLead = employee;
			node.ProjectLeadId = employee.Id;
		}

		protected override Project SetParentNode(Project node, Division parent)
		{
			node.Division = parent;
			return node;
		}
	}
}
