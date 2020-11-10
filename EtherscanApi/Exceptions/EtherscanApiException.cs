using System;

namespace EtherscanApi.Exceptions
{
	public class EtherscanApiException : Exception
	{
		public EtherscanApiException() { }
		
		public EtherscanApiException(string message) : base(message) { }
		
		public EtherscanApiException(string message, Exception innerException) : base(message, innerException) { }
	}
}