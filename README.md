# Etherscan API wrapper for .NET
Library provides asynchronous bindings for some of [Etherscan API](https://etherscan.io/apis) calls with possibility to
rate limit requests made by client instance.

Please note, that this lib do not convert WEI to `decimal` type and provides this values as `BigInteger`.

## Features
- Built using [TAP](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
- Embedded rate limiting to follow Etherscan API restrictions
- Provides exceptions for 'rate limit' or 'invalid API key' errors

Sadly wrapper covers not all API calls (especially PRO) but this may change someday.

## Usage examples
```c#
using EtherscanApi;

var client = new Etherscan("YourApiKeyToken");
var balance = await client.Account.GetAddressBalance("0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae");
```

For more examples see the Tests project.

Notice: all used test data is taken from Etherscan API usage examples.

## Implemented modules
Etherscan API consist of _modules_ and _actions_. For now, wrapper has next modules and actions implemented:
- **Account**: `balance`, `balancemulti`, `tokentx`, `tokennfttx`
- **Stats**: `ethsupply`, `ethprice`

## TODO
- [ ] NuGet package
- [ ] Implement all modules and actions
- [ ] Configurable rate limit
- [ ] PRO features
- [ ] Test eth networks support