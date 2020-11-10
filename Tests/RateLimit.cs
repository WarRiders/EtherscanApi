using System;
using System.Numerics;
using System.Threading.Tasks;
using EtherscanApi;
using EtherscanApi.Exceptions;
using NUnit.Framework;

namespace Tests
{
	[TestFixture, NonParallelizable, Order(0)]
	public class RateLimit
	{
		private const string Address = "0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae";
		
		/// <summary>
		/// To pass it must fail execution with <see cref="RateLimitException"/>.
		/// </summary>
		[Test, Order(0)]
		public async Task Fail()
		{
			var client = new Etherscan(limit: false);

			Task<BigInteger>
				task1 = client.Account.GetAddressBalance(Address),
				task2 = client.Account.GetAddressBalance(Address);

			try
			{
				await Task.WhenAll(task1, task2);
			}
			catch(RateLimitException)
			{
				Assert.Pass();
				return;
			}
			
			Assert.Fail();
		}

		[Test, Order(1)]
		public async Task Success()
		{
			var client = new Etherscan(limit: true);
			
			Task<BigInteger>
				task1 = client.Account.GetAddressBalance(Address),
				task2 = client.Account.GetAddressBalance(Address);
			
			await Task.WhenAll(task1, task2);

			Assert.AreEqual(task1.Result, task2.Result);
		}

		[TearDown]
		public async Task Wait()
		{
			await Task.Delay(TimeSpan.FromSeconds(5d));
		}
	}
}