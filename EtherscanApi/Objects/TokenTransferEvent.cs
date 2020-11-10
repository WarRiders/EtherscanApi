using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EtherscanApi.Objects
{
	public abstract class TokenTransferEvent
	{
		[JsonProperty("blockNumber")]
		public ulong BlockNumber { get; private set; }
		
		[JsonProperty("timeStamp"), JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime TimeStamp { get; private set; }
		
		[JsonProperty("hash")]
		public string TransactionId { get; private set; }
		
		[JsonProperty("nonce")]
		public uint Nonce { get; private set; }
		
		[JsonProperty("blockHash")]
		public string BlockHash { get; private set; }
		
		[JsonProperty("from")]
		public string FromAddress { get; private set; }
		
		[JsonProperty("to")]
		public string ToAddress { get; private set; }
		
		[JsonProperty("contractAddress")]
		public string ContractAddress { get; private set; }
		
		[JsonProperty("tokenName")]
		public string TokenName { get; private set; }
		
		[JsonProperty("tokenSymbol")]
		public string TokenSymbol { get; private set; }
		
		[JsonProperty("tokenDecimal")]
		public short TokenDecimal { get; private set; }
		
		/// <summary>
		/// Index of transaction within the block.
		/// </summary>
		[JsonProperty("transactionIndex")]
		public uint TransactionIndex { get; private set; } 
		
		[JsonProperty("gas")]
		public BigInteger Gas { get; private set; }
		
		[JsonProperty("gasPrice")]
		public BigInteger GasPrice { get; private set; }
		
		[JsonProperty("gasUsed")]
		public BigInteger GasUsed { get; private set; }
		
		[JsonProperty("cumulativeGasUsed")]
		public BigInteger CumulativeGasUsed { get; private set; }
		
		[JsonProperty("input")]
		public string Input { get; private set; }
		
		[JsonProperty("confirmations")]
		public ulong Confirmations { get; private set; }
		
		internal TokenTransferEvent() { }
	}
}