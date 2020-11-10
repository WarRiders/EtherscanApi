using System.Numerics;
using Newtonsoft.Json;

namespace EtherscanApi.Objects
{
	public class Erc721TransferEvent : TokenTransferEvent
	{
		[JsonProperty("tokenID")]
		public BigInteger TokenId { get; private set; }
		
		internal Erc721TransferEvent() { }
	}
}