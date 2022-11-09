using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

// I also found this: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.phoneattribute?view=net-7.0
// but it seems to be in early development phase.

namespace OrganizationStructureWebAPI.Validators
{
	/// <summary>
	/// Validates telephone numbers, uses libphonenumbers-dotnet library.
	/// </summary>
	public class TelephoneNumber : ValidationAttribute
	{
		public override bool IsValid(object? value) => value is not string phoneNumber || IsValid(phoneNumber);

		private static bool IsValid(string phoneNumber)
		{
			var phoneNumberUtil = PhoneNumberUtil.GetInstance();
			try
			{
				return phoneNumberUtil.IsPossibleNumber(phoneNumberUtil.Parse(phoneNumber, null));
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
