namespace OrganizationStructureWebAPI.Models.OrganizationStructure
{
	public class Project : OrganizationStructureNode
	{
		public Employee? ProjectLead
		{
			get => Lead;
			set => Lead = value;
		}
		public int ProjectLeadId
		{
			get => EmployeeId;
			set => EmployeeId = value;
		}

		/// <summary>
		/// Link to parent.
		/// </summary>
		public Division? Division { get; set; }
		public List<Department> Departments { get; set; } = new List<Department>();
		public override IEnumerable<OrganizationStructureNode>? GetChildren() => Departments;
	}
}
