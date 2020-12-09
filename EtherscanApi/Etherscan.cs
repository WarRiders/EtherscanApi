using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EtherscanApi.Exceptions;
using Newtonsoft.Json;

namespace EtherscanApi
{
	/// <summary>
	/// Etherscan API client.
	/// </summary>
	public partial class Etherscan
	{
		private const string DefaultApiKey = "YourApiKeyToken";
		
		private readonly string apiKey;

		private readonly string baseUrl;

		private readonly HttpClient httpClient;

		#region Rate Limit
		
		private readonly bool limit;
		
		private DateTime lastRequest;

		/// <summary>
		/// Current rate limit.
		/// </summary>
		public TimeSpan RateLimit { get; }

		private readonly SemaphoreSlim rateLock;
		
		#endregion
		
		/// <summary>
		/// API module.
		/// </summary>
		public abstract class Module
		{
			/// <summary>
			/// Etherscan API client.
			/// </summary>
			private readonly Etherscan client;

			/// <summary>
			/// Module name that used as parameter with API call.
			/// </summary>
			protected abstract string Name { get; }
			
			/// <summary>
			/// Creates module for client.
			/// </summary>
			/// <param name="client">Client.</param>
			internal Module(Etherscan client)
			{
				this.client = client;
			}
			
			protected Task<T> SendRequest<T>(string action, IDictionary<string, string> requestParams = default,
				CancellationToken cancellationToken = default) =>
				this.client.SendRequest<T>(this.Name, action, requestParams, cancellationToken);
		}

		/// <summary>
		/// Creates new Etherscan API client.
		/// </summary>
		/// <param name="apiKey">API key.</param>
		/// <param name="limit">
		/// True to limit requests amount per unit of time to match allowed rate limits.
		/// Note that if this mode is in use, client will not allow concurrent requests to API to guarantee permitted
		/// amount of requests per unit of time.
		/// </param>
		/// <param name="timeout">
		/// Request timeout. Methods will throw <see cref="TaskCanceledException"/> if timeout is reached.
		/// </param>
		/// <param name="baseUrl">Base API URL.</param>
		/// <exception cref="ArgumentException">Empty or null base URL is specified.</exception>
		public Etherscan(string apiKey = DefaultApiKey, bool limit = true, TimeSpan? timeout = null, string baseUrl = "https://api.etherscan.io/api")
		{
			if(string.IsNullOrEmpty(baseUrl))
			{
				throw new ArgumentException("Base URL must be non empty string.", nameof(baseUrl));
			}
			
			this.apiKey = apiKey.Trim();
			this.baseUrl = baseUrl;
			this.limit = limit;

			if(limit)
			{
				// Has an API key:         5 requests per 1 second
				// Doesn't has an API key: 1 request  per 5 seconds
				this.RateLimit = !(string.IsNullOrEmpty(this.apiKey) || this.apiKey == DefaultApiKey)
					? TimeSpan.FromSeconds(1d / 5d)
					: TimeSpan.FromSeconds(5d);
				this.rateLock = new SemaphoreSlim(1, 1);
				this.lastRequest = new DateTime();
			}

			this.httpClient = new HttpClient {Timeout = timeout ?? TimeSpan.FromSeconds(30d)};

			this.Account = new AccountModule(this);
			this.Stats = new StatsModule(this);
		}

		/// <summary>
		/// Sends request to API.
		/// </summary>
		/// <param name="moduleName">Name of the API module.</param>
		/// <param name="action">Action to call.</param>
		/// <param name="requestParams">Additional request parameters.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <typeparam name="T">Type of the response data.</typeparam>
		/// <returns>Response data.</returns>
		/// <exception cref="RateLimitException">Request failed due to rate limit.</exception>
		/// <exception cref="InvalidKeyException">Request called using wrong API key.</exception>
		/// <exception cref="EtherscanApiException">Some error occured during API call or wrong arguments provided.</exception>
		/// <exception cref="TaskCanceledException">Canceled using token or request failed due to timeout.</exception>
		private async Task<T> SendRequest<T>(string moduleName, string action, IDictionary<string, string> requestParams = default,
			CancellationToken cancellationToken = default)
		{
			requestParams = requestParams ?? new Dictionary<string, string>();
			
			var requestUri = new UriBuilder(this.baseUrl)
			{
				Query = await new FormUrlEncodedContent(new Dictionary<string, string>(requestParams)
				{
					{"module", moduleName},
					{"action", action},
					{"apikey", this.apiKey}
				}).ReadAsStringAsync().ConfigureAwait(false)
			}.Uri;

			string responseText;

			try
			{
				HttpResponseMessage responseMessage;

				try
				{
					if(this.limit)
					{
						await this.rateLock.WaitAsync(cancellationToken).ConfigureAwait(false);
						
						// Time difference from no to time the next request is available
						var diff = this.lastRequest + this.RateLimit - DateTime.UtcNow;
						
						if(diff > TimeSpan.Zero)
						{
							// Wait till we can do the next request
							await Task.Delay(diff, cancellationToken).ConfigureAwait(false);
						}
					}
					
					responseMessage = await this.httpClient
						.GetAsync(requestUri, cancellationToken)
						.ConfigureAwait(false);
				}
				finally
				{
					if(this.limit)
					{
						this.lastRequest = DateTime.UtcNow;
						this.rateLock.Release();
					}
				}

				if(!responseMessage.IsSuccessStatusCode)
				{
					throw new EtherscanApiException($"API responded with {responseMessage.StatusCode} status code.");
				}
				
				responseText = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			catch(HttpRequestException e)
			{
				throw new EtherscanApiException("HTTP request exception.", e);
			}

			ResponseBody responseBody;

			try
			{
				responseBody = JsonConvert.DeserializeObject<ResponseBody>(responseText);
			}
			catch(JsonException e)
			{
				throw new EtherscanApiException("Failed to parse response.", e);
			}

			if(!responseBody.IsOk)
			{
				string resultText;

				try
				{
					resultText = responseBody.Result.ToString();
				}
				catch(Exception e)
				{
					throw new EtherscanApiException("Unknown error.", e);
				}

				var lower = resultText.ToLowerInvariant();
				
				if(lower.Contains("rate limit"))
				{
					throw new RateLimitException(resultText);
				}

				if(lower.Contains("invalid api key"))
				{
					throw new InvalidKeyException(resultText);
				}
				
				try
				{
					// Try to return empty result
					return responseBody.Result.ToObject<T>();
				}
				catch
				{
					throw new EtherscanApiException(responseBody.Message);
				}
			}

			try
			{
				return responseBody.Result.ToObject<T>();
			}
			catch(JsonException e)
			{
				throw new EtherscanApiException("Failed to parse result.", e);
			}
		}
	}
}