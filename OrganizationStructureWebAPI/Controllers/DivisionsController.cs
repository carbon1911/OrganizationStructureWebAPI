using OrganizationStructureWebAPI.Models;
using OrganizationStructureWebAPI.Models.Misc;
using OrganizationStructureWebAPI.Models.OrganizationStructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

using DivisionsBaseController = OrganizationStructureWebAPI.Controllers.OrganizationStructureBaseController
	<OrganizationStructureWebAPI.Models.OrganizationStructure.Company, 
	OrganizationStructureWebAPI.Models.OrganizationStructure.Division, 
	OrganizationStructureWebAPI.Models.OrganizationStructure.Project>;

namespace OrganizationStructureWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DivisionsController : DivisionsBaseController
	{
		public DivisionsController(OrganizationStructureDbContext context, ILogger<DivisionsBaseController> logger)
			: base(context, logger) { }

		// GET: api/Divisions
		/// <summary>
		/// Returns all divisions with their leads and projects.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Division>>> GetDivisions() => await Get();

		// GET: api/Divisions/5
		/// <summary>
		/// Returns division with its lead and children projects or NotFound if the division
		/// with given <paramref name="id"/> does not exist.
		/// </summary>
		/// <param name="id"></param>
		[HttpGet("id")]
		public async Task<ActionResult<Division>> GetDivision(int id) => await Get(id);

		// PUT: api/Divisions
		/// <summary>
		/// Facilitates modification of the name and code of given division.
		/// </summary>
		[HttpPut]
		public async Task<ActionResult<Division>> PutDivision(OrganizationStructureNodeDTO newData) => await Put(newData);

		// PUT: api/Divisions/5/2
		/// <summary>
		/// Facilitates the change of division's lead.
		/// </summary>
		[HttpPut("{divisionId}/{divisionLeadId}")]
		public async Task<ActionResult<Division>> PutDivision(int divisionId, int divisionLeadId) => await Put(divisionId, divisionLeadId);

		// PUT: api/Divisions/5/projectsListIds
		/// <summary>
		/// Assigns new projects with <paramref name="projectsListIds"/> IDs to the division with ID <paramref name="divisionId"/>.
		/// Removes all former projects that are not in the <paramref name="projectsListIds"/> and have no children departments associated.
		/// </summary>
		/// <param name="divisionId"></param>
		/// <param name="projectsListIds"></param>
		[HttpPut("{divisionId}/projectsListIds")]
		public async Task<ActionResult<Division>> PutDivision(int divisionId, ImmutableList<int> projectsListIds) => await Put(divisionId, projectsListIds);

		// POST: api/Divisions/2/5
		/// <summary>
		/// Creates a new division with <paramref name="divisionLeadId"/> division lead ID and <paramref name="companyId"/> parent company ID.
		/// </summary>
		/// <param name="divisionLeadId"></param>
		/// <param name="companyId"></param>
		/// <param name="newData"></param>
		[HttpPost("{divisionLeadId}/{companyId}")]
		public async Task<ActionResult<Division>> PostDivision(int divisionLeadId, int companyId, OrganizationStructureNodeDTO newData) => await Post(divisionLeadId, newData, companyId);

		// DELETE: api/Divisions/2
		/// <summary>
		/// Deletes division with given <paramref name="id"/> if it has no projects assigned.
		/// </summary>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		public async Task<ActionResult<Division>> DeleteDivision(int id) => await Delete(id);

		protected override void AddChild(Division parent, Project child) => parent.Projects.Add(child);

		protected override async Task<Company?> GetParent(int id) => await _parents!.FirstOrDefaultAsync(p => p.Id == id);

		protected override IQueryable<Division> GetQuery()
			=> _currents.Include(d => d.DivisionLead).Include(d => d.Projects);

		protected override IEnumerable<Project> GetChildrenAsEnumerable() => _children!.Include(c => c.Departments).ToList();

		protected override void SetLead(Division node, Employee employee)
		{
			node.DivisionLead = employee;
			node.DivisionLeadId = employee.Id;
		}

		protected override Division SetParentNode(Division node, Company parent)
		{
			node.Company = parent;
			return node;
		}
	}
}
