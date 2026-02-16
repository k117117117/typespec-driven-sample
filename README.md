# 概要

TypeSpec を軸にしたスキーマ駆動な管理ツール開発のサンプルプロジェクトです。

- TypeSpec で API スキーマを定義し、OpenAPI 仕様 (`openapi.json`) を出力
- NSwag で OpenAPI仕様 から ASP.NET Core コントローラーを自動生成（バックエンド）
- API Platform Admin (`OpenApiAdmin`) が OpenAPI 仕様をパースして CRUD UI を自動構築（フロントエンド）

ゆくゆくは TypeSpec を軸にしたコントラクトファースト開発のサンプルに拡張できたらいいなと思っています（思っているだけ）  
（自動生成した API クライアントを Unity や他のサーバーで使う例、管理ツールの対象となるサーバーとの接続、TypeSpec で定義した model から生成したクラスの共有方法のサンプル追加など）

## アーキテクチャ

### 概要

```
TypeSpec (.tsp)  ──tsp compile──▶  OpenAPI (openapi.json)
                                        │
                        ┌───────────────┼───────────────┐
                        ▼                               ▼
                  NSwag (コード生成)              API Platform Admin
                        │                        (OpenApiAdmin)
                        ▼                               │
              ASP.NET Core コントローラー                │
              (バックエンド API)◀── HTTP ──────(フロントエンド CRUD UI)
```

**OpenAPI 仕様 (`openapi.json`) が唯一の信頼できる情報源（Single Source of Truth）** として、バックエンド・フロントエンド両方のコード生成・動的構築を駆動します。

### 詳細

OpenAPI 仕様は以下の 2 つの役割を担っています。

**1. バックエンド: コード生成（ビルド時）**

```
typespec/*.tsp
  ↓ tsp compile
typespec/tsp-output/schema/openapi.json
  ↓ NSwag
admin.backend/Generated/Controllers.g.cs    (抽象コントローラー自動生成)
admin.backend/Controllers/*.cs              (開発者が実装を記述)
```

NSwag が OpenAPI 仕様から C# の抽象コントローラーを生成し、開発者はそれを継承して実装を書きます。

**2. フロントエンド: ランタイム解析（実行時）**

```
admin.frontend (App.tsx)
  └─ OpenApiAdmin
       ├── docEntrypoint から openapi.json を取得
       ├── OpenAPI の paths をパースしてリソースを検出
       ├── リソース名 ↔ API エンドポイント ↔ HTTP メソッドのマッピングを構築
       └── ResourceGuesser が各リソースの CRUD 画面を自動構築
```

`OpenApiAdmin` は OpenAPI 仕様の `paths` を読み取り、リソースの検出・API エンドポイントとの対応・入出力フィールドのスキーマを自動的に解析します。React Admin 単体は URL と API エンドポイントの対応を知りませんが、API Platform Admin の `openApiDataProvider` がこのマッピングを担います。

## 技術スタック

| レイヤー | 技術 |
|---|---|
| スキーマ定義 | TypeSpec → OpenAPI (中間出力) |
| コード生成 | NSwag (OpenAPI → C# コントローラー) |
| バックエンド | ASP.NET Core 9 + Entity Framework Core |
| フロントエンド | React + Vite + API Platform Admin (react-admin) |
| データベース | PostgreSQL 17 |
| インフラ | Docker Compose |

## 前提条件

- Docker / Docker Compose

以下はオプション（Docker だけでも `docker compose up -d` で起動・動作可能）:

- Node.js — npm scripts (`npm run up` 等) を使う場合
- .NET SDK 9.0 — NSwag コード生成 (`npm run nswag:generate`) をローカルで実行する場合

## クイックスタート

```bash
# リポジトリをクローンしてコンテナを起動するだけでいいはず
docker compose up -d
```

| URL | 用途 |
|---|---|
| http://localhost | フロントエンド (nginx → Vite dev server へのリバースプロキシ) |
| http://localhost:3000 | フロントエンド (Vite dev server 直接) |
| http://localhost:8080 | バックエンド API (nginx → ASP.NET Core へのリバースプロキシ) |
| http://localhost:5432 | PostgreSQL |

> **Note:** 開発環境では Vite dev server (`:3000`) や Kestrel (`:5000`) に直接アクセスしても動作します。nginx は本番構成を想定したリバースプロキシの例として配置しています。

## npm scripts

```bash
npm run up              # docker compose up -d
npm run up:build        # docker compose up -d --build
npm run down            # docker compose down
npm run down:clean      # docker compose down -v (ボリュームも削除)

npm run tsp-and-nswag   # TypeSpec コンパイル + NSwag コード生成 (セット)
npm run tsp:compile     # TypeSpec コンパイルのみ
npm run nswag:generate  # NSwag コード生成のみ

npm run build:backend   # バックエンドビルド
npm run build:frontend  # フロントエンドビルド
npm run build:all       # 全ビルド (tsp+nswag → backend → frontend)
```

## 開発ワークフロー

### フロントエンド (React) を修正した場合

→ **何もしなくてOK**。Vite HMR がファイル変更を検知して自動的にブラウザに反映されます。

### バックエンド (C#) を修正した場合

→ **何もしなくてOK**。`dotnet watch` がファイル変更を検知して自動的にリビルド＆再起動します。

### TypeSpec スキーマ (.tsp) を修正した場合

```bash
npm run tsp-and-nswag
```

TypeSpec コンパイル → NSwag コード生成が実行され、生成された C# コントローラーは `dotnet watch` が自動検知して反映します。

### 依存関係 (package.json / .csproj) を変更した場合

```bash
npm run down:clean
npm run up:build
```

Docker イメージの再ビルドとボリュームの再作成が必要です。

## プロジェクト構造

```
├── admin.frontend/          # React フロントエンド
│   ├── src/
│   │   ├── App.tsx          # メインアプリ (OpenApiAdmin)
│   │   ├── approvalRequest/ # 承認リクエスト画面 (カスタム)
│   │   ├── helloWorld/      # カスタムページ例
│   │   └── layout/          # カスタムレイアウト・メニュー
│   ├── Dockerfile
│   └── nginx/default.conf
│
├── admin.backend/           # ASP.NET Core バックエンド
│   ├── Controllers/         # API 実装 (生成された抽象クラスを継承)
│   ├── Data/                # EF Core DbContext・エンティティ
│   ├── Generated/           # NSwag 自動生成コード (編集不要)
│   ├── schema/              # ※ Docker マウント (typespec/tsp-output/schema)
│   ├── nswag.json           # NSwag コード生成の設定ファイル
│   ├── Dockerfile
│   └── nginx/default.conf
│
├── typespec/                # TypeSpec スキーマ定義
│   ├── main.tsp             # エントリポイント (model.tsp, routes.tsp を import)
│   ├── model.tsp            # モデル定義 (API の DTO, エラー型)
│   ├── routes.tsp           # ルート定義 (各リソースの CRUD エンドポイント)
│   ├── tspconfig.yaml
│   ├── Dockerfile
│   └── tsp-output/schema/openapi.json
│
├── docker-compose.yml
└── package.json             # npm scripts
```

## DB 接続情報 (開発環境)

| 項目 | 値 |
|---|---|
| ホスト | `localhost` |
| ポート | `5432` |
| データベース | `admin` |
| ユーザー | `admin` |
| パスワード | `admin` |

## TypeSpec + API Platform Admin における URL 設計の注意点

### React Admin のルーティング規約

React Admin は `<Resource>` コンポーネントで定義されたリソースに対して、以下の固定ルートを生成します（[Routing - Route Components](https://marmelab.com/react-admin/Routing.html#route-components)）。

| ルートパターン | 画面 | マウント時に呼ばれる dataProvider メソッド |
|---|---|---|
| `/:resource` | 一覧 (list) | `getList()` |
| `/:resource/create` | 新規作成 (create) | — (`create()` は submit 時) |
| `/:resource/:id/edit` | 編集 (edit) | `getOne()` (`update()` は submit 時) |
| `/:resource/:id/show` | 詳細 (show) | `getOne()` |

`create`, `edit`, `show` は予約語として扱われ、`:id` パラメータとは区別されます。

### API Platform Admin (`OpenApiAdmin`) の役割

React Admin 自体は URL と API エンドポイントの対応を知りません（[Resource - Usage](https://marmelab.com/react-admin/Resource.html#usage)）。

> "The `<Resource>` component doesn't know this mapping - it's the dataProvider's job to define it."

本プロジェクトでは **API Platform Admin の `openApiDataProvider`** がこの役割を担っています。`OpenApiAdmin` は OpenAPI 仕様の `paths` をパースして `ResourceGuesser` の `name`（例: `"approval-requests"`）を API エンドポイント（例: `/approval-requests`, `/approval-requests/{id}`）に自動マッピングします。

### ネストした URL を設計する際の注意

**React Admin はネストされたリソースをサポートしていません**（[Resource - Nested Resources](https://marmelab.com/react-admin/Resource.html#nested-resources)）。

> "React-admin doesn't support nested resources, but you can use the children prop to render a custom component for a given sub-route."

TypeSpec でルートを定義する際、以下のようなパスの衝突に注意が必要です。

#### 問題のある例

```typespec
@route("/resources")
interface Resources {
  @get list(): Resource[];
  @get read(@path id: int32): Resource;          // → /resources/{id}
}

@route("/resources/subresources")
interface SubResources {
  @get list(): SubResource[];                     // → /resources/subresources
  @get read(@path id: int32): SubResource;        // → /resources/subresources/{id}
}
```

この場合 `/resources/subresources` が `/resources/:id`（`:id = "subresources"`）と **ルートパターンが衝突** します。React Router は `subresources` という文字列をリソース `resources` の ID として解釈し、意図しない `getOne("resources", { id: "subresources" })` が発呼されることがあります。

これは React Admin の「親リソースを自動フェッチする仕様」ではなく、**ルートの衝突による副作用** です。

#### 推奨する設計

- リソース名にスラッシュを含むパスを使う場合は、既存リソースのパスと衝突しないようにする
- ネストが必要な場合は `<Resource>` の `children` prop で子ルートを定義し、`useParams` で手動パラメータ取得 + `resource` prop の明示指定を行う
- React Admin の CRUD 規約に乗らないルート（例: `approve`, `reject` などのアクション）は `<CustomRoutes>` を使うか、ボタン等から直接 API を呼ぶ

```tsx
// CustomRoutes の例 (本プロジェクトの App.tsx より)
<CustomRoutes>
  <Route path="/hello-world" element={<HelloWorld />} />
</CustomRoutes>
```

### 参考: 本プロジェクトの URL マッピング

TypeSpec で定義したルートが OpenAPI → API Platform Admin を経由して最終的にどう対応するかの一覧です。

| TypeSpec ルート | OpenAPI path | React Admin URL | 画面 |
|---|---|---|---|
| `@route("/admin-tool-users")` GET | `/admin-tool-users` | `/admin-tool-users` | 一覧 |
| `@route("/admin-tool-users")` POST | `/admin-tool-users` | `/admin-tool-users/create` | 新規作成 |
| `@route("/admin-tool-users")` GET `@path id` | `/admin-tool-users/{id}` | `/admin-tool-users/{id}/edit` | 編集 |
| `@route("/admin-tool-users")` GET `@path id` | `/admin-tool-users/{id}` | `/admin-tool-users/{id}/show` | 詳細 |
| `@route("/approval-requests")` GET | `/approval-requests` | `/approval-requests` | 一覧 |
| `@route("/approval-requests")` POST | `/approval-requests` | `/approval-requests/create` | 新規作成 |
| `@route("/approval-requests/{id}/approve")` POST | `/approval-requests/{id}/approve` | — (ボタンから直接呼出) | 承認アクション |
| `@route("/approval-requests/{id}/reject")` POST | `/approval-requests/{id}/reject` | — (ボタンから直接呼出) | 却下アクション |

`approve` / `reject` のようなリソース CRUD に該当しないアクションは React Admin のルーティング規約外のため、画面上のボタンから `dataProvider` や `fetch` で直接 API を呼び出しています。

## リンク
- TypeSpec  
https://github.com/microsoft/typespec
- API Platform Admin  
https://github.com/api-platform/admin
- React Admin  
https://github.com/marmelab/react-admin
- NSwag  
https://github.com/RicoSuter/NSwag
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
