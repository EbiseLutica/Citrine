# Citrine

[Citrine](https://citringo.net/char.html?citrine) は、オープンソースの娘型chatbotです。

コア部分は特定のソーシャルメディアと切り離された独自のAPIで構成され、多くのソーシャルメディア上で動作させることができるよう設計されています。

## モジュール

Citrine は、 Citrine API と呼ばれる、抽象化された bot 開発の為の便利な API を介することで、様々な環境向けに bot を作り直すことなく動作させることができます。

モジュールは、実際に bot としての振る舞いを実装するものです。各機能をモジュールとして実装し、読み込むことでチャットボットとして機能します。

## 対応プラットフォーム

Citrine は、ソーシャルメディア向けの Citrine API 実装を用意するだけで、既存のモジュールを改良すること無く、多くの環境に対応させることが出来ます。この、各種ソーシャルメディアに対応した Citrine API の実装を、プラットフォームと呼びます。hubotのアダプターにあたるものです。

チェックがついていないものは対応予定のもの。

- [x] Misskey
- [x] Mastodon
- [x] Standalone
	- ターミナル上でインタラクティブに動作するバージョン
- [x] Discord
- [ ] Slack
- [ ] LINE
- [ ] Twitter
- [ ] Skype

## 必要なもの

- .NET Core 2.1

## ビルド

```shell
git clone --recursive https://github.com/Xeltica/Citrine.git

cd Citrine

# --recursive を忘れた場合
git submodule update --init

dotnet build

# Run Citrine for Misskey
cd Citrine && dotnet run

# Run Citrine for Mastodon
cd Citrine.Mastodoon && dotnet run

# Run Citrine Interactive
cd Citrine.Standalone && dotnet run
```

## Contributing

-[不具合 / 要望](//github.com/xeltica/citrine/issues/new)
-[プルリクエスト](//github.com/xeltica/citrine/compare)

### これまでの貢献者

- @u1-liquid

[その他...](//github.com/Xeltica/Citrine/graphs/contributors)

## ライセンス

[MIT License](LICENSE)
