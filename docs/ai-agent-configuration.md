# AI エージェント設定

## 概要

`AGENTS.md` は AI エージェント（GitHub Copilot、Claude 等）がこのリポジトリで作業する際のルールとコンテキストを定義するファイルです。  
`CLAUDE.md` と `.github/copilot-instructions.md` はいずれも `AGENTS.md` へのシンボリックリンクであり、内容は同一です。

## ファイル構成

トークン消費を抑えるため、プロジェクトごとに分割しています。  
エージェントは作業対象のプロジェクトに関連する `AGENTS.md` のみを読み込みます。  
複数プロジェクトにまたがる作業では、関連するすべての `AGENTS.md` を参照します。

```
AGENTS.md                    ← 全体ルール（アーキテクチャ、ビルドコマンド、技術スタック、言語・書式・Git ルール）
CLAUDE.md                    ← AGENTS.md へのシンボリックリンク
.github/copilot-instructions.md ← AGENTS.md へのシンボリックリンク
typespec/AGENTS.md           ← TypeSpec 構造、モデル共有、孤立エンティティ自動クリーンアップ
admin.backend/AGENTS.md      ← レイヤードアーキテクチャ、名前空間規約、新リソース追加手順（C# 側）
game.server/AGENTS.md        ← admin.backend と同パターン（GameServer 固有の名前空間）
admin.frontend/AGENTS.md     ← ファイル構成、UI 追加手順、apiClient 利用、カスタムルート
```

## 各ファイルの役割

### ルート `AGENTS.md`（全体ルール）

- **出力効率**: 中間ステップの説明を抑制し、最後にまとめて報告
- **アーキテクチャ概要**: TypeSpec → OpenAPI → NSwag/frontend のデータフロー
- **設計原則**: TypeSpec によるスキーマ共有、共有 C# ライブラリ不要の方針
- **ビルド & 実行コマンド**: `npm run tsp-and-nswag` 等の一覧
- **手動編集禁止ファイル**: `Controllers.g.cs`、`tsp-output/`、`generated/`
- **言語ルール**: 思考は英語、ユーザー向け出力はユーザーの入力言語
- **ファイル書式**: 末尾改行必須
- **Git ワークフロー**: 自動コミット・プッシュ禁止

### `typespec/AGENTS.md`

- ディレクトリ構成
- モデル共有の考え方（`extends` vs `...` Spread の使い分け）
- 新リソース追加手順（TypeSpec 側: モデル定義 → ルート定義 → コンパイル）
- 孤立エンティティ自動クリーンアップの仕組み

### `admin.backend/AGENTS.md` / `game.server/AGENTS.md`

- 3 プロジェクト分割のレイヤードアーキテクチャ（Presentation / Application / Infrastructure）
- 名前空間規約（例: `AdminBackend.Domain.AdminToolUsers.Models`）
- `internal` 修飾子の使い方
- 新リソース追加手順（C# 側: Domain → Application → Infrastructure → Presentation）

### `admin.frontend/AGENTS.md`

- `src/` のファイル構成
- CRUD UI の追加方法（`ResourceGuesser` + `resources.ts`）
- カスタム UI の作成手順
- `apiClient`（openapi-fetch）による非 CRUD アクション
- カスタムルート（非 API ページ）の追加方法

## 既存ドキュメントとの関係

| ファイル | 対象読者 | 言語 |
|---|---|---|
| `AGENTS.md` (各所) | AI エージェント | 英語 |
| `typespec/SCHEMA_GUIDE.md` | 人間の開発者 | 日本語 |
| `docs/*.md` | 人間の開発者 | 日本語 |

`AGENTS.md` と `SCHEMA_GUIDE.md` は一部内容が重複しますが、それぞれ AI エージェント向け・人間向けに最適化されています。
