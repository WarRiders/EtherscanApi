using System.Numerics;
using System.Threading.Tasks;
using EtherscanApi;
using EtherscanApi.Objects;
using NUnit.Framework;

namespace Tests
{
	[TestFixture, NonParallelizable, Order(1)]
	public class Account
	{
		private Etherscan client;
		
		[OneTimeSetUp]
		public void Setup()
		{
			this.client = new Etherscan(limit: true);
		}

		[Test, Order(0)]
		public async Task Balance()
		{
			var balance = await this.client.Account.GetAddressBalance("0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae");
			TestContext.WriteLine(balance);
			Assert.Pass();
		}

		[Test, Order(1)]
		public async Task BalanceMultiple()
		{
			var addresses = new[]
			{
				"0xddbd2b932c763ba5b1b7ae3b362eac3e8d40121a",
				"0x63a9975ba31b0b9626b34300f7f627147df1f526",
				"0x198ef1ec325a96cc354c7266a038be8b5c558f67"
			};

			var balances = await this.client.Account.GetAddressesBalance(addresses);
			
			Assert.AreEqual(addresses.Length, balances.Length);

			for(var i = 0; i < addresses.Length; i++)
			{
				Assert.AreEqual(addresses[i], balances[i].address);
				Assert.GreaterOrEqual(balances[i].balance, BigInteger.Zero);
				TestContext.WriteLine($"{balances[i].address}: {balances[i].balance}");
			}
		}

		[Test, Order(2)]
		public async Task Erc20TransferEvents()
		{
			var events =
				await this.client.Account.GetTokenTransferEvents<Erc20TransferEvent>(
					"0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae");
			TestContext.WriteLine(events.Length);
			Assert.Greater(events.Length, 0);
		}

		[OneTimeTearDown]
		public async Task Wait()
		{
			await Task.Delay(this.client.RateLimit);
		}
	}
}