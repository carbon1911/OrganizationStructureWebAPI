using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationStructureWebAPI.Models.OrganizationStructure
{
	/// <summary>
	/// Represents an organization strucure node, that is every model except employee.
	/// </summary>
	public class OrganizationStructureNode
	{
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; } = 0;

		[Required(AllowEmptyStrings = false)]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Name { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public int Code { get; set; } = 0;

		[ForeignKey(nameof(EmployeeId))]
		protected Employee Lead { get; set; } = default!;

		[Required]
		protected int EmployeeId { get; set; }

		public virtual IEnumerable<OrganizationStructureNode>? GetChildren() => throw new NotImplementedException();
	}
}
