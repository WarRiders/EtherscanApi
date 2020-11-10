using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EtherscanApi
{
	[JsonObject]
	internal class ResponseBody
	{
		[JsonProperty("status")]
		private string StatusText { get; set; }

		[JsonProperty("result")]
		public JToken Result { get; private set; }
		
		[JsonIgnore]
		public bool IsOk => this.StatusText == "1";
	}
}