using System;

namespace EtherscanApi.Exceptions
{
	public class RateLimitException : EtherscanApiException
	{
		public RateLimitException() { }
		
		public RateLimitException(string message) : base(message) { }
	}
}