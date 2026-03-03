# 概要

TypeSpec を軸にしたスキーマ駆動開発の概念実証・サンプルプロジェクトです。  
運営型ゲームを題材に、ゲームとその管理ツールの両方を単一の TypeSpec スキーマ駆動で開発します。  

- TypeSpec で API スキーマを定義し、OpenAPI 仕様 (`openapi.json`) を出力
- NSwag で OpenAPI仕様 から ASP.NET Core コントローラーを自動生成（バックエンド）
- API Platform Admin (`OpenApiAdmin`) が OpenAPI 仕様をパースして管理ツールの CRUD UI を自動構築（フロントエンド）

## アーキテクチャ

### 概要

```
TypeSpec (.tsp)  ──tsp compile──▶  OpenAPI (openapi.json)
                                        │
                        ┌───────────────┼───────────────┐
                        ▼               ▼               ▼
                  NSwag            openapi-typescript  API Platform Admin
                  (コード生成)     + generate-resources (OpenApiAdmin)
                        │               │               │
                        ▼               ▼               │
              ASP.NET Core        TypeScript 型定義      │
              コントローラー      + リソース定数          │
              (バックエンド API) ◀──── HTTP ──── (フロントエンド CRUD UI)
```

**OpenAPI 仕様 (`openapi.json`) が唯一の信頼できる情報源（Single Source of Truth）** として、管理ツールバックエンド・フロントエンド・APIクライアント、ゲームサーバー・APIクライアントなどのコード生成・動的構築を駆動します。

### 詳細

OpenAPI 仕様は以下の 3 つの役割を担っています。

**1. 管理ツールバックエンド: コード生成（ビルド時）**

```
typespec/admin/*.tsp
  ↓ tsp compile
typespec/tsp-output/schema/openapi.admin.json
  ↓ NSwag
admin.backend/Presentation/Generated/Controllers.g.cs    (抽象コントローラー自動生成)
admin.backend/Presentation/<Resource>/*Controller.cs     (開発者が実装を記述)
```

NSwag が OpenAPI 仕様から C# の抽象コントローラーを生成し、開発者はそれを継承して実装を書きます。

**2. ゲームサーバー: コード生成（ビルド時）**

```
typespec/game-server/*.tsp
  ↓ tsp compile
typespec/tsp-output/schema/openapi.gameserver.json
  ↓ NSwag
game.server/Presentation/Generated/Controllers.g.cs      (抽象コントローラー自動生成)
game.server/Presentation/<Resource>/*Controller.cs       (開発者が実装を記述)
```

管理ツールと同じ仕組みですが、ゲームサーバー専用の OpenAPI 仕様から独立してコード生成されます。共通モデル（Player 等）は `typespec/shared/model.tsp` で定義し、各サービスが必要に応じて拡張します。

**3. 管理ツールフロントエンド: コード生成（ビルド時）+ ランタイム解析（実行時）**

```
typespec/admin/*.tsp
  ↓ tsp compile
typespec/tsp-output/schema/openapi.admin.json
  ↓ openapi-typescript
admin.frontend/src/generated/admin.d.ts          (TypeScript 型定義自動生成)
  ↓ generate-resources
admin.frontend/src/generated/resources.g.ts      (CRUD リソース名定数自動生成)
```

`openapi-typescript` が OpenAPI 仕様から TypeScript の型定義を生成し、`generate-resources` がCRUD リソース（`/{path}` と `/{path}/{id}` の両方を持つパス）を検出してリソース名定数を生成します。`App.tsx` はこのリソース名定数を動的に参照し、新しいリソースを追加するたびに自動的に `ResourceGuesser` を描画します。

```
admin.frontend (App.tsx)
  └─ OpenApiAdmin
       ├── docEntrypoint から openapi.admin.json を取得
       ├── OpenAPI の paths をパースしてリソースを検出
       ├── リソース名 ↔ API エンドポイント ↔ HTTP メソッドのマッピングを構築
       └── ResourceGuesser が各リソースの CRUD 画面を自動構築
```

`OpenApiAdmin` は OpenAPI 仕様の `paths` を読み取り、リソースの検出・API エンドポイントとの対応・入出力フィールドのスキーマを自動的に解析します。React Admin 単体は URL と API エンドポイントの対応を知りませんが、API Platform Admin の `openApiDataProvider` がこのマッピングを担います。

## プロジェクト構造

```
├── admin.frontend/          # React フロントエンド
│   ├── src/
│   │   ├── App.tsx          # メインアプリ (OpenApiAdmin, ResourceGuesser 動的レンダリング)
│   │   ├── config.ts        # 環境変数 (API_URL, SCHEMA_URL)
│   │   ├── routes.ts        # フロントエンド内部ルーティングパス
│   │   ├── api-client.ts    # openapi-fetch 型付き API クライアント
│   │   ├── approval-request/ # 承認リクエスト画面 (カスタム)
│   │   ├── hello-world/     # カスタムページ例
│   │   ├── layout/          # カスタムレイアウト・メニュー
│   │   ├── i18n/            # 国際化 (日本語ローカライズ)
│   │   ├── types/           # 生成型を元にした手書きの型定義
│   │   └── generated/       # 自動生成ファイル (編集不要)
│   │       ├── admin.d.ts       # openapi-typescript 型定義
│   │       └── resources.g.ts   # CRUD リソース名定数
│   └── Dockerfile
│
├── admin.backend/           # ASP.NET Core バックエンド (admin)
│   ├── Presentation/        # プレゼンテーション層 (ASP.NET Core Web)
│   │   ├── AdminToolUsers/  # API コントローラー実装
│   │   ├── ApprovalRequests/
│   │   ├── Health/
│   │   ├── Generated/       # NSwag 自動生成コード (編集不要)
│   │   └── Program.cs       # DI 登録・起動設定
│   ├── Application/         # ドメイン + アプリケーション層
│   │   ├── Domain/          # ドメインモデル・リポジトリIF
│   │   └── Application/     # アプリケーションサービス
│   ├── Infrastructure/      # インフラ層 (EF Core, DB)
│   │   ├── Data/            # DbContext
│   │   ├── AdminToolUsers/  # エンティティ・リポジトリ実装
│   │   └── ApprovalRequests/
│   ├── Dockerfile
│   └── Dockerfile
│
├── game.server/             # ASP.NET Core バックエンド (game-server)
│   ├── Presentation/        # プレゼンテーション層
│   │   ├── Players/         # API コントローラー実装
│   │   ├── Health/
│   │   ├── Generated/       # NSwag 自動生成コード (編集不要)
│   │   └── Program.cs
│   ├── Application/         # ドメイン + アプリケーション層
│   │   ├── Domain/          # ドメインモデル・リポジトリIF
│   │   └── Application/     # アプリケーションサービス
│   ├── Infrastructure/      # インフラ層 (EF Core, DB)
│   │   ├── Data/
│   │   └── Players/
│   ├── Dockerfile
│   └── Dockerfile
│
├── typespec/                # TypeSpec スキーマ定義
│   ├── shared/
│   │   └── model.tsp        # 全サービス共通の基盤モデル (Error, Player, Page)
│   ├── admin/
│   │   ├── main.tsp         # admin エントリポイント
│   │   ├── model.tsp        # admin 固有モデル
│   │   ├── operations.tsp   # CRUD ルート定義
│   │   └── tspconfig.yaml
│   ├── game-server/
│   │   ├── main.tsp         # game-server エントリポイント
│   │   ├── model.tsp        # game-server 固有モデル
│   │   ├── operations.tsp   # Players CRUD, Health
│   │   └── tspconfig.yaml
│   ├── tsp-output/schema/   # 自動生成 OpenAPI JSON
│   ├── SCHEMA_GUIDE.md      # スキーマ設計ガイド
│   └── Dockerfile
│
├── scripts/
│   ├── init-db.sh                   # PostgreSQL DB 初期化 (gameserver, admin)
│   ├── clean-orphan-entities.mjs    # 孤立 *Entity.cs 自動削除
│   └── generate-resources.mjs       # OpenAPI → CRUD リソース名定数生成
│
├── docker-compose.yml
└── package.json             # npm scripts
```

各バックエンドサービスは **レイヤードアーキテクチャ（3プロジェクト構成）** で分割されています:

| 層 | 役割 | 依存先 |
|---|---|---|
| **Presentation** | ASP.NET Core Web, NSwag 生成コントローラー | → Application, Infrastructure |
| **Application** | ドメインモデル (Domain/) + アプリケーションサービス (Application/) | なし (純粋 C# classlib) |
| **Infrastructure** | EF Core, リポジトリ実装, DbContext | → Application |

`internal` 修飾子によりアセンブリ境界でレイヤー間のアクセスを制限しています。

## 技術スタック

| レイヤー | 技術 |
|---|---|
| スキーマ定義 | TypeSpec → OpenAPI (中間出力) |
| コード生成 (バックエンド) | NSwag (OpenAPI → C# コントローラー) |
| コード生成 (フロントエンド) | openapi-typescript (OpenAPI → TypeScript 型定義) + generate-resources (CRUD リソース名定数) |
| バックエンド | ASP.NET Core 10 + Entity Framework Core |
| フロントエンド | React + Vite + API Platform Admin (react-admin) + openapi-fetch |
| データベース | PostgreSQL 17 |
| インフラ | Docker Compose |

## 前提条件

- Docker / Docker Compose

以下はオプション（Docker だけでも `docker compose up -d` で起動・動作可能）:

- Node.js — npm scripts (`npm run up` 等) を使う場合
- .NET SDK 10.0 — NSwag コード生成 (`npm run nswag:generate`) をローカルで実行する場合

## クイックスタート

```bash
# リポジトリをクローンしてコンテナを起動するだけでいいはず
docker compose up -d
```

| URL | 用途 |
|---|---|
| http://localhost:3000 | フロントエンド (Vite dev server) |
| http://localhost:5000 | バックエンド API (ASP.NET Core) |
| http://localhost:5000/scalar | Scalar API リファレンス UI |
| http://localhost:5001 | Game Server API (ASP.NET Core) |
| http://localhost:5432 | PostgreSQL |

> **Note:** 本番環境を想定する場合はnginxなどを構成に追加しても良い

## ドキュメント

詳細なリファレンスは `docs/` を参照してください。

| ドキュメント | 内容 |
|---|---|
| [開発ワークフロー](docs/development-workflow.md) | npm scripts、スキーマ修正時の手順、NSwag 設定、DB 接続情報 |
| [設計方針・思想](docs/design-philosophy.md) | スキーマ駆動開発・コントラクトファースト開発の所感 |
| [URL 設計の注意点](docs/url-design-notes.md) | TypeSpec + API Platform Admin のルーティング規約と設計指針 |
| [フロントエンドの型安全性](docs/frontend-type-safety.md) | OpenAPI → TypeScript 型生成、リソース名・API 呼び出しの型安全性 |
| [技術調査: Blazor & NSwag](docs/technical-investigation-blazor-nswag.md) | Blazor UIフレームワーク比較、NSwag型マッピング・Liquidテンプレート |
| [フレームワーク比較](docs/framework-comparison.md) | API Platform Admin vs Refine vs Radzen Blazor の比較 |
| [AI エージェント設定](docs/ai-agent-configuration.md) | AGENTS.md の構成と AI エージェント向けルール |

## リンク
- TypeSpec  
https://github.com/microsoft/typespec
- API Platform Admin  
https://github.com/api-platform/admin
- React Admin  
https://github.com/marmelab/react-admin
- NSwag  
https://github.com/RicoSuter/NSwag
- openapi-typescript  
https://openapi-ts.dev/
- openapi-fetch  
https://openapi-ts.dev/openapi-fetch/
- Open API  
https://github.com/OAI/OpenAPI-Specification

## See also
- @typespec/openapi3  
https://www.npmjs.com/package/@typespec/openapi3
- TypeSpec client emitters  
https://typespec.io/docs/emitters/clients/introduction/
- TypeSpec typescript emitter  
https://github.com/crowbait/typespec-typescript-emitter
- TypeSpec C# server emitter  
https://typespec.io/docs/emitters/servers/http-server-csharp/reference/
- Scaffolding radzen Blazor UI from OpenAPI  
https://www.radzen.com/blazor-studio/documentation/openapi#connect-to-openapi-service
- Routing in React Admin  
https://marmelab.com/react-admin/Routing.html
- React Admin Resource  
https://marmelab.com/react-admin/Resource.html
- API Platform Admin Components (OpenApiAdmin)  
https://api-platform.com/docs/admin/components/#openapiadmin
- NSwag サンプルプロジェクト (nswag.json の設定例)  
https://github.com/RicoSuter/NSwag/tree/master/src

## ToDo 
思っているだけで実際にやるかは別ですがTypeSpecを軸にしたスキーマ駆動&コントラクトファースト開発のサンプルに拡張できたらいいなと思っています  
- [x] 別のASP.NETサーバーのプロジェクトを追加する (game.server)
- [x] 自動生成したAPIクライアント(TypeScript)をWebフロントエンドで使うサンプルの追加
- [ ] Unityのプロジェクトを追加する
- [ ] 自動生成したAPIクライアント(C#)をUnityや複数サービス間で使うサンプルの追加
- [ ] NSwagのコード生成でopenapi.jsonの型とC#の型のマッピングを指定するサンプルの追加（できるのか知らないけど）
- [ ] NSwagのコード生成でliquidを使用した出力テンプレートのカスタマイズをするサンプルの追加
- [ ] 管理ツールのフロントエンドでRazen Blazorを試してみる
- [ ] 管理ツールのフロントエンドでRazen Blazor Studioを使用してOpenAPIからUIを自動生成してみる
