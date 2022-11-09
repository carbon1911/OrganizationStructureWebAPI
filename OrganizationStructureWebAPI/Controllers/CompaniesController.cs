using OrganizationStructureWebAPI.Models;
using OrganizationStructureWebAPI.Models.Misc;
using OrganizationStructureWebAPI.Models.OrganizationStructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

using CompaniesBaseController = OrganizationStructureWebAPI.Controllers.OrganizationStructureBaseController
	<OrganizationStructureWebAPI.Models.Misc.OrganizationStructureNodeNull, 
	OrganizationStructureWebAPI.Models.OrganizationStructure.Company, 
	OrganizationStructureWebAPI.Models.OrganizationStructure.Division>;

namespace OrganizationStructureWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CompaniesController : CompaniesBaseController
	{
		public CompaniesController(OrganizationStructureDbContext context, ILogger<CompaniesBaseController> logger) 
			: base(context, logger) { }

		// GET: api/Companies
		/// <summary>
		/// Returns all companies with their directors and divisions.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Company>>> GetCompanies() => await Get();

		// GET: api/Companies/5
		/// <summary>
		/// Returns company with its director and children division or NotFound if the company
		/// with given <paramref name="id"/> does not exist.
		/// </summary>
		/// <param name="id"></param>
		[HttpGet("id")]
		public async Task<ActionResult<Company>> GetCompany(int id) => await Get(id);

		// PUT: api/Companies
		/// <summary>
		/// Facilitates modification of the name and code of given company.
		/// </summary>
		[HttpPut]
		public async Task<ActionResult<Company>> PutCompany(OrganizationStructureNodeDTO newData) => await Put(newData);

		// PUT: api/Companies/5/2
		/// <summary>
		/// Facilitates the change of company's director.
		/// </summary>
		[HttpPut("{companyId}/{directorId}")]
		public async Task<ActionResult<Company>> PutCompany(int companyId, int directorId) => await Put(companyId, directorId);

		// PUT: api/Companies/5/divisionsListIds
		/// <summary>
		/// Assigns new divisions with <paramref name="newDivisions"/> IDs to the company with ID <paramref name="companyId"/>.
		/// Removes all former divisions that are not in the <paramref name="newDivisions"/> and have no children projects associated.
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="newDivisions"></param>
		[HttpPut("{companyId}/divisionsListIds")]
		public async Task<ActionResult<Company>> PutCompany(int companyId, ImmutableList<int> newDivisions) => await Put(companyId, newDivisions);

		// POST: api/Companies/2
		/// <summary>
		/// Creates a new company with <paramref name="directorId"/> director ID.
		/// </summary>
		/// <param name="directorId"></param>
		/// <param name="newData"></param>
		[HttpPost("{directorId}")]
		public async Task<ActionResult<Company>> PostCompany(int directorId, OrganizationStructureNodeDTO newData) => await Post(directorId, newData);

		// DELETE: api/Companies/2
		/// <summary>
		/// Deletes company with given ID if it has no children divisions assigned.
		/// </summary>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		public async Task<ActionResult<Company>> DeleteCompany(int id) => await Delete(id);

		protected override void AddChild(Company parent, Division child) => parent.Divisions.Add(child);

		protected override IEnumerable<Division> GetChildrenAsEnumerable() => _children!.Include(c => c.Projects).ToList();

		protected override Task<OrganizationStructureNodeNull?> GetParent(int id) => throw new NotImplementedException();

		protected override IQueryable<Company> GetQuery() => _currents.Include(c => c.Director).Include(c => c.Divisions);

		protected override void SetLead(Company node, Employee employee)
		{
			node.Director = employee;
			node.DirectorId = employee.Id;
		}

		protected override Company SetParentNode(Company node, OrganizationStructureNodeNull parent) => throw new NotImplementedException();
	}
}
