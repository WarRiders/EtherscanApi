using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using EtherscanApi.Objects;
using Newtonsoft.Json.Linq;

namespace EtherscanApi
{
	public partial class Etherscan
	{
		/// <summary>
		/// Account module.
		/// </summary>
		public class AccountModule : Module
		{
			protected override string Name { get; } = "account";
			
			internal AccountModule(Etherscan client) : base(client) { }

			public Task<BigInteger> GetAddressBalance(string address, CancellationToken cancellationToken = default)
			{
				if(string.IsNullOrWhiteSpace(address))
				{
					throw new ArgumentException("Address must be non empty string.", nameof(address));
				}

				return this.SendRequest<BigInteger>("balance", new Dictionary<string, string>
				{
					{"address", address},
					{"tag", "latest"}
				}, cancellationToken);
			}

			public async Task<(string address, BigInteger balance)[]> GetAddressesBalance(string[] addresses, CancellationToken cancellationToken = default)
			{
				if(addresses is null)
				{
					throw new ArgumentNullException(nameof(addresses));
				}

				if(addresses.Length > 20)
				{
					throw new ArgumentException("No more than 20 addresses are supported in a single batch.", nameof(addresses));
				}

				var result = await this.SendRequest<JArray>(
					"balancemulti",
					new Dictionary<string, string> {{"address", string.Join(",", addresses)}, {"tag", "latest"}},
					cancellationToken
				);

				return result.Select(e => (e["account"]?.ToString() ?? string.Empty,
					e["balance"]?.ToObject<BigInteger>() ?? BigInteger.Zero)).ToArray();
			}

			public Task<T[]> GetTokenTransferEvents<T>(string address,
				string contractAddress = default,
				ulong? startBlock = default,
				ulong? endBlock = default,
				ListSortDirection? sort = default,
				uint? page = default,
				uint? perPage = default,
				CancellationToken cancellationToken = default
			) where T : TokenTransferEvent
			{
				var action = typeof(T) == typeof(Erc20TransferEvent) ? "tokentx" :
					typeof(T) == typeof(Erc721TransferEvent) ? "tokennfttx" :
					throw new InvalidOperationException("Contract type not supported.");

				if(string.IsNullOrWhiteSpace(address))
				{
					throw new ArgumentException("Address must be non empty string.", nameof(address));
				}

				var parameters = new Dictionary<string, string> {{"address", address}};

				if(!string.IsNullOrWhiteSpace(contractAddress))
				{
					parameters["contractaddress"] = contractAddress;
				}

				if(startBlock.HasValue)
				{
					parameters["startblock"] = startBlock.Value.ToString();
				}

				if(endBlock.HasValue)
				{
					parameters["endblock"] = endBlock.Value.ToString();
				}

				if(sort.HasValue)
				{
					parameters["sort"] = sort.Value == ListSortDirection.Ascending ? "asc" : "desc";
				}

				// Page navigation
				if(page.HasValue && perPage.HasValue)
				{
					parameters["page"] = page.Value.ToString();
					parameters["offset"] = page.Value.ToString();
				}

				return this.SendRequest<T[]>(action, parameters, cancellationToken);
			}
		}

		/// <summary>
		/// Account module.
		/// </summary>
		public AccountModule Account { get; }
	}
}