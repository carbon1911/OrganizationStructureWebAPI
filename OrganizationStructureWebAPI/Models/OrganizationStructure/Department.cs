namespace OrganizationStructureWebAPI.Models.OrganizationStructure
{
	public class Department : OrganizationStructureNode
	{
		public Employee? DepartmentLead
		{
			get => Lead;
			set => Lead = value;
		}

		public int DepartmentLeadId
		{
			get => EmployeeId;
			set => EmployeeId = value;
		}

		/// <summary>
		/// Link to parent.
		/// </summary>
		public Project? Project { get; set; }

		public override IEnumerable<OrganizationStructureNode>? GetChildren() => null;
	}
}
