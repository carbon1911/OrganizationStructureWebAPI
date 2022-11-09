using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace OrganizationStructureWebAPI.Controllers
{
	public static class Common
	{
		/// <summary>
		///	Collects errors after model validation failed.
		/// </summary>
		/// <returns>Bad Request with reason strings</returns>
		public static BadRequestObjectResult CollectErrors(this ControllerBase controller)
		{
			var errors = controller.ModelState.Select(x => x.Value?.Errors)
						.Where(y => y?.Count > 0);

			StringBuilder sb = new();
			foreach (var collection in errors)
			{
				if (collection == null)
				{
					continue;
				}

				foreach (var e in collection)
				{
					sb.Append(e.ErrorMessage);
				}
				sb.AppendLine();
			}

			return controller.BadRequest(sb.ToString());
		}
	}
}
