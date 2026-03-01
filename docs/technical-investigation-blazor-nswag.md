# 技術調査: Blazor UIフレームワーク選定 & NSwagコード生成カスタマイズ

> 調査日: 2026-03-01

---

## 1. Blazor UIフレームワーク比較

ASP.NET Blazor向けの主要コンポーネントライブラリの比較。
管理画面（admin.frontend）の構築を想定し、OpenAPI連携・C#エンジニアの学習コストを重視して評価。

### 比較表

| ライブラリ | ライセンス | OpenAPI自動生成 | C#親和性 | 備考 |
|---|---|---|---|---|
| **Radzen Blazor** | 無料/OSS（IDE有料） | ✅ あり | ◎ | Radzen IDEがSwagger/OpenAPIからCRUD画面を自動生成 |
| **MudBlazor** | 無料/OSS | ❌ なし | ◎ | 最も人気。Material Design。ドキュメント優秀 |
| **FluentUI Blazor** | 無料/OSS（MS製） | ❌ なし | ◎ | Microsoft公式。Fluent Design準拠 |
| **Syncfusion Blazor** | 商用（個人無料） | ❌ なし | ○ | コンポーネント数最多。学習コスト高め |
| **Telerik UI for Blazor** | 商用 | ❌ なし | ○ | Progress製。エンタープライズ向け |
| **DevExpress Blazor** | 商用 | ❌ なし | ○ | DataGrid最強クラス |

### OpenAPI自動生成について

**Radzen が唯一の実用的な選択肢。** Radzen IDE（コンパニオンツール）で以下が可能:

- OpenAPI/Swaggerエンドポイントに接続
- データソースを認識してCRUDページを自動スキャフォールド
- DataGrid、フォーム、バリデーションまで自動生成

本リポジトリはTypeSpec駆動でOpenAPIを生成しているため、TypeSpecで生成したOpenAPIスキーマをRadzen IDEに食わせれば、admin画面のスキャフォールドが自動化できる可能性がある。

### 推奨

| 条件 | 推奨ライブラリ |
|---|---|
| OpenAPI自動生成が必須 | **Radzen Blazor** |
| OpenAPI自動生成が不要で、学習コスト重視 | **MudBlazor** |

---

## 2. NSwag型マッピング（OpenAPI型 → C#型）

### nswag.jsonで設定可能な組み込み型マッピング

既存の `nswag.json` で以下の型マッピングは設定済み:

```json
{
  "dateType": "System.DateTimeOffset",
  "dateTimeType": "System.DateTimeOffset",
  "timeType": "System.TimeSpan",
  "timeSpanType": "System.TimeSpan",
  "arrayType": "System.Collections.Generic.List",
  "dictionaryType": "System.Collections.Generic.IDictionary"
}
```

### カスタム型マッピング（例: IFormFile）

**nswag.jsonの設定ファイルだけでは `typeMappings` のような汎用的なカスタムマッピング設定は存在しない。**

カスタムマッピングの実現方法:

| 方法 | 難易度 | 柔軟性 | 備考 |
|---|---|---|---|
| プログラマティックAPI | 中 | ◎ | C#コードでNSwagを呼び出す |
| Liquidテンプレート | 高 | ◎◎ | 出力コード全体をカスタマイズ可能 |

#### プログラマティックAPIでの型マッピング例

```csharp
var settings = new CSharpControllerGeneratorSettings();
settings.CSharpGeneratorSettings.TypeMappers.Add(
    new PrimitiveTypeMapper("FileUpload", s => s.Format == "binary", "IFormFile")
);
```

### IFormFileマッピングの特殊ケース

NSwag は `type: string, format: binary` を自動的にファイル型として扱う:

- **サーバー生成（Controller）** → `IFormFile` に自動マッピング
- **クライアント生成** → `FileParameter` に自動マッピング

つまり **TypeSpec側で正しくスキーマを定義すれば、NSwagが自動的に `IFormFile` にマッピングする**はず。

#### TypeSpecでの定義例

```typespec
op uploadFile(
  @header contentType: "multipart/form-data",
  @body body: {
    file: bytes;
    description?: string;
  }
): void;
```

これにより OpenAPI で `type: string, format: binary` が生成され、NSwag が `IFormFile` に変換する。

> **結論:** ファイルアップロードに関しては型マッピングの手動設定は不要で、TypeSpecのスキーマ定義が正しければ自動で動く可能性が高い。ただし実機検証は必要。

---

## 3. NSwag Liquidテンプレートカスタマイズ

### 概要

NSwagはLiquidテンプレートエンジンによる出力カスタマイズをサポートしている。
既存の `nswag.json` にも `"templateDirectory": null` の設定が存在する（69行目）。

### セットアップ手順

1. テンプレートディレクトリを作成

```
game.server/Presentation/
  templates/
    Controller.liquid
    Controller.Method.liquid
    File.liquid
    Class.liquid
```

2. NSwagのデフォルトLiquidテンプレートをコピー
   - ソース: [NSwag GitHub - src/NSwag.CodeGeneration.CSharp/Templates/](https://github.com/RicoSuter/NSwag/tree/master/src/NSwag.CodeGeneration.CSharp/Templates)

3. テンプレートをカスタマイズ

4. `nswag.json` で `templateDirectory` を指定

```json
{
  "templateDirectory": "./templates"
}
```

### 実用的なカスタマイズ例

- 生成コントローラーに共通属性を追加（`[Authorize]` など）
- DTOに独自インターフェース実装を追加
- カスタムバリデーションロジックの注入
- 生成コードのフォーマットやコメントスタイルの変更

---

## まとめ

| やりたいこと | 実現可能性 | 推奨アプローチ |
|---|---|---|
| ファイル → IFormFileマッピング | ✅ 自動で動く可能性大 | TypeSpecで `bytes` + multipart指定 |
| 任意のカスタム型マッピング | ⚠️ nswag.jsonだけでは不可 | プログラマティックAPI or Liquidテンプレート |
| Liquidテンプレートカスタマイズ | ✅ 確実にできる | `templateDirectory` 設定を有効化 |
| OpenAPIからのUI自動構築 | ✅ Radzen IDEで可能 | TypeSpec生成OpenAPIをRadzen IDEに入力 |
