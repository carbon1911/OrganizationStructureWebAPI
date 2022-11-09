namespace OrganizationStructureWebAPI.Models.OrganizationStructure
{
	public class Company : OrganizationStructureNode
	{
		public Employee? Director
		{
			get => Lead;
			set => Lead = value;
		}

		public int DirectorId
		{
			get => EmployeeId;
			set => EmployeeId = value;
		}
		public List<Division> Divisions { get; set; } = new List<Division>();

		public override IEnumerable<OrganizationStructureNode>? GetChildren() => Divisions;
	}
}
