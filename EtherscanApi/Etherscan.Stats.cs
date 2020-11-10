using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using EtherscanApi.Objects;

namespace EtherscanApi
{
	public partial class Etherscan
	{
		/// <summary>
		/// Stats module.
		/// </summary>
		public class StatsModule : Module
		{
			protected override string Name { get; } = "stats";
			
			internal StatsModule(Etherscan client) : base(client) { }

			/// <summary>
			/// Queries for total Ethereum supply in WEI format.
			/// </summary>
			/// <param name="cancellationToken">Cancellation token.</param>
			/// <returns>Total Ethereum supply.</returns>
			/// <exception cref="TaskCanceledException">If cancelled using token.</exception>
			public Task<BigInteger> GetTotalEthSupply(CancellationToken cancellationToken = default) =>
				this.SendRequest<BigInteger>("ethsupply", cancellationToken: cancellationToken);

			/// <summary>
			/// Queries for current ETH price in BTC and USD.
			/// </summary>
			/// <param name="cancellationToken">Cancellation token.</param>
			/// <returns>ETH price.</returns>
			public Task<EthPrice> GetEthPrice(CancellationToken cancellationToken = default) =>
				this.SendRequest<EthPrice>("ethprice", cancellationToken: cancellationToken);
		}
		
		/// <summary>
		/// Stats module.
		/// </summary>
		public StatsModule Stats { get; }
	}
}