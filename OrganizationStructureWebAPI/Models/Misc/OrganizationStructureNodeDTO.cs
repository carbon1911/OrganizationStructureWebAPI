using System.ComponentModel.DataAnnotations;

namespace OrganizationStructureWebAPI.Models.Misc
{
	public class OrganizationStructureNodeDTO
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public int Code { get; set; }

		[Required]
		public string Name { get; set; } = default!;
	}
}
