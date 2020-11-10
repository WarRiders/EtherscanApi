using System.Numerics;
using Newtonsoft.Json;

namespace EtherscanApi.Objects
{
	public class Erc20TransferEvent : TokenTransferEvent
	{
		[JsonProperty("value")]
		public BigInteger Value { get; private set; }
		
		internal Erc20TransferEvent() { }
	}
}