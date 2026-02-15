# 概要

Typespecを軸にしたスキーマ駆動な管理ツール開発のサンプルプロジェクトです  
TypeSpecでAPIスキーマを定義してOpenAPI仕様(openapi.json)を出力 → NSwag でASP.NET CoreのWeb MVCコントローラーを自動生成  
フロントエンドはAPI Platform Admin(react-admin)で自動的にCRUD UIを構築するサンプルプロジェクト。  
ゆくゆくはTypespecを軸にしたコントラクトファースト開発のサンプルに拡張できたらいいなと思っています（思っているだけ）  
（自動生成したAPIクライアントをUnityや他のサーバーで使う例や、逆に管理ツールの対象となるサーバーとの接続、TypeSpecで定義したmodelから生成したクラスの共有方法の例など）  

## アーキテクチャ

```
typespec/*.tsp
  ↓ tsp compile
typespec/tsp-output/schema/openapi.json
  ↓ NSwag
admin.backend/Generated/Controllers.g.cs              (抽象コントローラー)
admin.backend/Controllers/*.cs                        (実装)
  ↓ OpenAPI仕様 (openapi.json) 配信
admin.frontend (react-admin + API Platform Admin)     (CRUD UI 自動生成)
```

## 技術スタック

| レイヤー | 技術 |
|---|---|
| スキーマ定義 | TypeSpec → OpenAPI(中間出力) |
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
# リポジトリをクローンして起動するだけ
docker compose up -d
```

| URL | 用途 |
|---|---|
| http://localhost | フロントエンド (nginx 経由) |
| http://localhost:3000 | フロントエンド (Vite dev server 直接) |
| http://localhost:8080 | バックエンド API (nginx 経由) |
| http://localhost:5432 | PostgreSQL |

## npm scripts

全ての操作は OS に依存しない npm scripts で実行できます。

```bash
npm run up              # docker compose up
npm run up:build        # docker compose up --build
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
│   ├── main.tsp
│   ├── model.tsp
│   ├── routes.tsp
│   ├── tspconfig.yaml
│   ├── Dockerfile
│   └── tsp-output/schema/openapi.json
│
├── docker-compose.yml
└── package.json             # npm scripts (OS非依存コマンド)
```

## DB 接続情報 (開発環境)

| 項目 | 値 |
|---|---|
| ホスト | `localhost` |
| ポート | `5432` |
| データベース | `admin` |
| ユーザー | `admin` |
| パスワード | `admin` |

VS Code の PostgreSQL 拡張機能で接続してテーブルの中身を確認できます。

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
