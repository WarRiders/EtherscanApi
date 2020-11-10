using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EtherscanApi.Objects
{
	public sealed class EthPrice
	{
		[JsonProperty("ethbtc")]
		public decimal Btc { get; private set; }
		
		[JsonProperty("ethbtc_timestamp"), JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime BtcTimestamp { get; private set; }
		
		[JsonProperty("ethusd")]
		public decimal UsdPrice { get; private set; }
		
		[JsonProperty("ethusd_timestamp"), JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime UsdTimestamp { get; private set; }
		
		internal EthPrice() { }
	}
}