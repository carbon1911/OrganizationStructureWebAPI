using DataAnnotationsExtensions;
using OrganizationStructureWebAPI.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationStructureWebAPI.Models
{
	public class Employee
	{
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		// credits: https://stackoverflow.com/questions/23939738/how-can-i-use-data-annotations-attribute-classes-to-fail-empty-strings-in-forms

		// Seems like the best validation is no validation at all.
		// https://stackoverflow.com/questions/6999772/how-to-validate-names-in-asp-net-mvc-so-accents-are-allowed-%C3%A9-%C3%A1
		// and a subsequent source https://blog.jgc.org/2010/06/your-last-name-contains-invalid.html
		[Required(AllowEmptyStrings = false, ErrorMessage = "Empty first name.")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string FirstName { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false, ErrorMessage = "Empty last name.")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string LastName { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false, ErrorMessage = "Empty title.")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Title { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false, ErrorMessage = "Empty telephone number.")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		[TelephoneNumber(ErrorMessage = "Invalid telephone number. Provide number with international dial code please.")]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false, ErrorMessage = "Empty email.")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; } = string.Empty;

		[Min(0.1)]
		public decimal Salary { get; set; } = 0.1M;
	}
}
