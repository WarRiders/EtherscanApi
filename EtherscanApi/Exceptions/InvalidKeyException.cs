namespace EtherscanApi.Exceptions
{
	public class InvalidKeyException : EtherscanApiException
	{
		public InvalidKeyException() { }

		public InvalidKeyException(string message) : base(message) { }
	}
}