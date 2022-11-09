using System.Runtime.Serialization;

namespace OrganizationStructureWebAPI.Exceptions
{
	public class TypeArgumentException : Exception
	{
		public TypeArgumentException()
		{
		}

		public TypeArgumentException(string? message) : base(message)
		{
		}

		public TypeArgumentException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected TypeArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
