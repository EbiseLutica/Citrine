# Citrine

English ・ [日本語](README-ja.md)

Citrine is a open-source framework to create chat-bot.

It has an API that independent of a specific social-media platform, so Citrine can run on a lots of social media services.

## Modules

Modules are Citrine's actual brain. To reply, react, repost etc, create modules as bot's features, and let Citrine load them.

## How to write your own module

To write your module for Citrine, [read this document(TBD)](/docs/module)

## Platform Adapters

Citrine API is an abstractive API of each social media.

Platform adapters are implemented Citrine API to run Citrine on the specified platform. It's same as hubot's adapter.

[✔] is implemented, and [ ] is in plan.

- [x] Misskey v11 (and compatible servers)
- [x] Mastodon
- [x] Standalone
	- A REPL
- [x] Discord
- [x] [rinsuki/sea](https://github.com/rinsuki/sea)
- [ ] Slack
- [ ] LINE
- [ ] Twitter


### How to write your own platform adapter

To write your own platform adapter for Citrine, [read this doc(TBD)](/docs/adapter)

## Requirement

- .NET Core 2.1

## To build

```shell
git clone --recursive https://github.com/Xeltica/Citrine.git

cd Citrine

# If you forget cloning with --recursive
git submodule update --init

dotnet build

# Run Citrine for Misskey
cd Citrine.Misskey && dotnet run

# Run Citrine for Mastodon
cd Citrine.Mastodon && dotnet run

# Run Citrine for Discord
cd Citrine.Discord && dotnet run

# Run Citrine Interactive
cd Citrine.Standalone && dotnet run
```


## Contributing

- [Issues ](//github.com/xeltica/citrine/issues/new)
- [Pull Requests](//github.com/xeltica/citrine/compare)

### Contributors

- @u1-liquid

[More...](//github.com/Xeltica/Citrine/graphs/contributors)

## License

[MIT License](LICENSE)
