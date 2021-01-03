# Etherscan API wrapper for .NET
This library provides the asynchronous bindings for some of the [Etherscan API](https://etherscan.io/apis) calls with possibility to
rate limit requests made by the client instance.

Please note, that this lib does not convert WEI to `decimal` type and provides these values as `BigInteger`.

## Features
- Built using [TAP](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
- Embedded rate limiting to follow the Etherscan API restrictions
- Provides exceptions for 'rate limit' or 'invalid API key' errors

Tise wrapper does not cover all API calls (especially PRO). This may change soon.

## Usage examples
```c#
using EtherscanApi;

var client = new Etherscan("YourApiKeyToken");
var balance = await client.Account.GetAddressBalance("0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae");
```

For more examples see the Tests project.

Notice: all test data is taken from the Etherscan API usage examples.

## Implemented modules
Etherscan API consist of _modules_ and _actions_. For now, the wrapper has the following modules and actions implemented:
- **Account**: `balance`, `balancemulti`, `tokentx`, `tokennfttx`
- **Stats**: `ethsupply`, `ethprice`

## TODO
- [ ] NuGet package
- [ ] Implement all modules and actions
- [ ] Configurable rate limit
- [ ] PRO features
- [ ] Test eth networks support
