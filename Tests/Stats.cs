using System;
using System.Numerics;
using System.Threading.Tasks;
using EtherscanApi;
using NUnit.Framework;

namespace Tests
{
	[TestFixture, Order(2)]
	public class Stats
	{
		private Etherscan client;
		
		[OneTimeSetUp]
		public void Setup()
		{
			this.client = new Etherscan(limit: true);
		}

		[Test]
		public async Task TotalEthSupply()
		{
			var supply = await this.client.Stats.GetTotalEthSupply();
			TestContext.WriteLine(supply);
			Assert.True(supply > BigInteger.Zero);
		}

		[Test, SetCulture("en-US")]
		public async Task EthPrice()
		{
			var dt = DateTime.Now - TimeSpan.FromDays(1);
			var price = await this.client.Stats.GetEthPrice();
			
			TestContext.WriteLine($"{price.Btc} BTC ({price.BtcTimestamp})\n{price.UsdPrice:C} ({price.UsdTimestamp})");
			
			Assert.Greater(price.Btc, decimal.Zero);
			Assert.Greater(price.UsdPrice, decimal.Zero);
			Assert.Greater(price.BtcTimestamp, dt);
			Assert.Greater(price.UsdTimestamp, dt);
		}
	}
}