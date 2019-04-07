# Citrine

[Citrine](https://citringo.net/char.html?citrine) は、オープンソースの嫁型chatbotです。

コア部分は特定のソーシャルメディアと切り離された独自のAPIで構成され、多くのソーシャルメディア上で動作させることができるよう設計されています。

## 対応プラットフォーム

チェックがついていないものは対応予定のもの

- [x] Misskey
- [ ] Mastodon
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
dotnet build
dotnet run --project Citrine
```

## ライセンス

[MIT License](LICENSE)
