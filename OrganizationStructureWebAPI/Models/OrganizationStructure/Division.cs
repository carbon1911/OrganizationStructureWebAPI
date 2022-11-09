namespace OrganizationStructureWebAPI.Models.OrganizationStructure
{
	public class Division : OrganizationStructureNode
	{		
		public Employee? DivisionLead
		{
			get => Lead;
			set => Lead = value;
		}

		public int DivisionLeadId
		{
			get => EmployeeId;
			set => EmployeeId = value;
		}

		/// <summary>
		/// Link to parent.
		/// </summary>
		/// 
		public Company? Company { get; set; }

		public List<Project> Projects { get; set; } = new List<Project>();

		public override IEnumerable<OrganizationStructureNode> GetChildren() => Projects;
	}
}
