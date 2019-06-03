# Citrine

[Citrine](https://citringo.net/char.html?citrine) は、オープンソースの娘型chatbotです。

コア部分は特定のソーシャルメディアと切り離された独自のAPIで構成され、多くのソーシャルメディア上で動作させることができるよう設計されています。

## 対応プラットフォーム

チェックがついていないものは対応予定のもの

- [x] Misskey
- [x] Mastodon
- [ ] Discord
- [ ] Slack
- [x] Standalone
- [ ] LINE
- [ ] Skype
- [ ] Twitter

## 必要なもの

- .NET Core 2.1

## ビルド

```
git clone https://github.com/Xeltica/Citrine.git
cd Citrine
git submodule update --init
dotnet build
dotnet run --project Citrine
```

## Contributing

-[不具合 / 要望](//github.com/xeltica/citrine/issues/new)
-[プルリクエスト](//github.com/xeltica/citrine/compare)

### これまでの貢献者

- @u1-liquid

[その他...](//github.com/Xeltica/Citrine/graphs/contributors)

## ライセンス

[MIT License](LICENSE)
