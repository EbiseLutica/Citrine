# Citrine

[English](README.md) ・ 日本語

Citrine は、オープンソースな bot フレームワークです。

特定のソーシャルメディアから独立した API を持ち、多くのソーシャルメディアで動作するように設計されています。


## モジュール

モジュールは、Citrine の脳にあたります。リプライ, リアクションなどを行う為には、モジュールを bot の機能として作成し、 Citrine に読み込ませます。

## モジュールを自作する方法

シトリンに対応するモジュールを自作するためには、 [これを読んでください(工事中)](/docs/ja/module)

## プラットフォームアダプター

Citrine API は 各ソーシャルメディアの持つ API を抽象化したものです。

プラットフォームアダプターは Citrine API の実装であり、特定のプラットフォーム上で Citrine を動作させる為のものです。 hubot のアダプターのような役割を持ちます。

[✔] は実装済み, [ ] は計画中のもの。

- [x] Misskey
- [x] Mastodon
- [x] Standalone
	- 対話型シェル
- [x] Discord
- [ ] Slack
- [ ] LINE
- [ ] Twitter


### プラットフォームアダプターを自作する方法

シトリンに対応するプラットフォームアダプターを自作するためには、 [これを読んでください(工事中)](/docs/ja/adapter)

## 必要なソフトウェア

- .NET Core 2.1

## ビルド方法

```shell
git clone --recursive https://github.com/Xeltica/Citrine.git

cd Citrine

# --recursive を忘れた場合
git submodule update --init

dotnet build

# Citrine for Misskey を実行
cd Citrine.Misskey && dotnet run

# Citrine for Mastodon を実行
cd Citrine.Mastodon && dotnet run

# Citrine for Discord を実行
cd Citrine.Discord && dotnet run

# Citrine Interactive を実行
cd Citrine.Standalone && dotnet run
```


## 貢献

- [バグ報告 / 要望](//github.com/xeltica/citrine/issues/new)
- [プルリクエスト](//github.com/xeltica/citrine/compare)

### 以前のコントリビューター

- @u1-liquid

[その他...](//github.com/Xeltica/Citrine/graphs/contributors)

## ライセンス

[MIT License](LICENSE)
