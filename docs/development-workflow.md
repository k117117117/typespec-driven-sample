# 開発ワークフロー

## npm scripts

```bash
npm run up              # docker compose up -d
npm run up:build        # docker compose up -d --build
npm run down            # docker compose down
npm run down:clean      # docker compose down -v (ボリュームも削除)

npm run tsp-and-nswag   # TypeSpec コンパイル + TS 型生成 + リソース定数生成 + NSwag コード生成 + 孤立エンティティ削除 (セット)
npm run tsp:compile     # TypeSpec コンパイル + TS 型生成 + リソース定数生成
npm run nswag:generate  # NSwag コード生成のみ
npm run clean:orphan-entities  # 孤立 *Entity.cs の検出・削除
npm run generate:resources     # OpenAPI → CRUD リソース名定数生成 (admin.frontend)

npm run build:backend   # バックエンドビルド
npm run build:frontend  # フロントエンドビルド
npm run build:all       # 全ビルド (tsp+nswag → backend → frontend)

npm run openapi:generate:ts  # OpenAPI → TypeScript 型定義生成 (admin.frontend, tsp:compile に含まれるため通常は単体実行不要)
```

## TypeSpec スキーマ (.tsp) を修正した場合

```bash
npm run tsp-and-nswag
```

TypeSpec コンパイル → TypeScript 型定義生成 → CRUD リソース名定数生成 + NSwag コード生成が実行され、生成された C# コントローラーは `dotnet watch` が自動検知して反映します。フロントエンドも `generated/resources.g.ts` が更新されるため、新しいリソースは `App.tsx` に自動的に反映されます。

NSwag が `Presentation/Generated/Controllers.g.cs` に抽象コントローラーと DTO クラスを自動生成するので、以下の順序で実装を追加・更新してください:

1. **`Application/Domain/<リソース名>/Models/<リソース名>.cs`** — ドメインモデル。ビジネスルール（バリデーション、状態遷移）をここに集約する。  
`internal` な生成メソッド (`Create`) と変更メソッド (`Update*`) 、`internal` な復元メソッド (`Reconstruct`) を持つ。
2. **`Application/Domain/<リソース名>/Repositories/I<リソース名>Repository.cs`** — リポジトリインターフェース。ドメインモデルを受け渡す CRUD 操作を定義する。
3. **`Application/Application/<リソース名>/<リソース名>ApplicationService.cs`** — アプリケーションサービス。ユースケース（登録・更新・削除等）のオーケストレーションを行う。  
ドメインモデルの `internal` メソッドを呼び出せる唯一のレイヤー。
4. **`Infrastructure/<リソース名>/<リソース名>Entity.cs`** — EF Core エンティティ (`internal`)。DB カラムと 1:1 対応するプロパティを持つ。
5. **`Infrastructure/<リソース名>/<リソース名>Repository.cs`** — リポジトリ実装 (`internal`)。Entity ↔ ドメインモデルの変換を担う。
6. **`Infrastructure/Data/AppDbContext.cs`** — `DbSet<Entity>` の登録。enum は `HasConversion<string>()` で文字列永続化。
7. **`Infrastructure/DependencyInjection.cs`** — リポジトリの DI 登録 (`AddScoped<IRepo, Repo>`)。
8. **`Presentation/<リソース名>/<リソース名>Controller.cs`** — NSwag 生成の抽象コントローラーを継承し、ApplicationService を primary constructor で DI。  
NSwag DTO ↔ ドメインモデルのマッピングを private static メソッドで行う。
9. **`Presentation/Program.cs`** — ApplicationService の DI 登録 (`AddScoped<>`)。

## NSwag コード生成の設定 (`nswag.json`)

各サービスの `Presentation/nswag.json` で NSwag のコード生成オプションを管理しています。生成されるコントローラーのクラス名・名前空間・スタイル等を変更したい場合はこのファイルを編集してください。

```
admin.backend/Presentation/nswag.json    ← admin バックエンド用
game.server/Presentation/nswag.json      ← ゲームサーバー用
```

設定変更後は `npm run nswag:generate`（または `dotnet build -t:NSwag`）で再生成されます。

### 設定可能なオプションの調べ方

NSwag の公式ドキュメントは網羅的ではないため、以下の方法でオプションを確認できます:

| 方法 | 説明 |
|---|---|
| **公式サンプル** | [NSwag.Sample.NET100/nswag.json](https://github.com/RicoSuter/NSwag/blob/master/src/NSwag.Sample.NET100/nswag.json) — 全オプションがデフォルト値付きで列挙されている（.NET 10 向けサンプル） |
| **NSwagStudio** | Windows GUI ツール。全オプションをフォームで確認・編集し `.nswag` ファイルとして保存できる ([Wiki](https://github.com/RicoSuter/NSwag/wiki/NSwagStudio)) |
| **CLI ヘルプ** | `nswag help openapi2cscontroller` で C# コントローラー生成の全パラメータを一覧表示 |
| **Wiki** | [NSwag Configuration Document](https://github.com/RicoSuter/NSwag/wiki/NSwag-Configuration-Document)、[CSharpControllerGenerator](https://github.com/RicoSuter/NSwag/wiki/CSharpControllerGenerator) |
| **ソースコード** | 最も正確。[CSharpControllerGeneratorSettings.cs](https://github.com/RicoSuter/NSwag/blob/master/src/NSwag.CodeGeneration.CSharp/CSharpControllerGeneratorSettings.cs) のプロパティがそのまま nswag.json のキーに対応 |

> **Tip:** NSwag の公式ドキュメントは網羅的ではないため、最新かつ詳細な設定例は [GitHub リポジトリの `src/` 配下にあるサンプルプロジェクト群](https://github.com/RicoSuter/NSwag/tree/master/src) (`NSwag.Sample.*`) を直接参照するのが最も確実です。NSwagStudio で設定を調整してからエクスポートした JSON を `nswag.json` にマージする方法も手軽です。

## サーバー・バックエンド (C#) を修正した場合

→ **何もしなくてOK**。`dotnet watch` がファイル変更を検知して自動的にリビルド＆再起動します。

## フロントエンド (React) を修正した場合

→ **何もしなくてOK**。Vite HMR がファイル変更を検知して自動的にブラウザに反映されます。

## 依存関係 (package.json / .csproj) を変更した場合

```bash
npm run down:clean
npm run up:build
```

Docker イメージの再ビルドとボリュームの再作成が必要です。

## DB 接続情報 (開発環境)

| 項目 | 値 |
|---|---|
| ホスト | `localhost` |
| ポート | `5432` |
| ユーザー (root) | `postgres` |
| パスワード | `postgres` |

| データベース | 用途 | 作成方法 |
|---|---|---|
| `gameserver` | game.server 用 | `scripts/init-db.sh` (コンテナ初回起動時) |
| `admin` | admin.backend 用 | `scripts/init-db.sh` (コンテナ初回起動時) |

各アプリケーションのテーブルは、EF Core の `Database.Migrate()` により起動時にマイグレーションが適用されます。マイグレーションファイルは各サービスの `Infrastructure/Migrations/` に配置されています。
