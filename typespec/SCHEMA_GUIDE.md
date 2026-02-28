# TypeSpec スキーマ設計ガイド

## ディレクトリ構成

```
typespec/
├── shared/
│   └── model.tsp          # 全サービス共通の基盤モデル（Error, Player 等）
├── admin/
│   ├── main.tsp            # サービスエントリポイント
│   ├── model.tsp           # admin 固有モデル（AdminToolUser, ApprovalRequest 等）
│   ├── operations.tsp      # CRUD ルート定義
│   └── tspconfig.yaml      # output: openapi.admin.json
├── game-server/
│   ├── main.tsp
│   ├── model.tsp           # game-server 固有モデル
│   ├── operations.tsp      # Players CRUD, Health
│   └── tspconfig.yaml      # output: openapi.gameserver.json
└── tsp-output/schema/
    ├── openapi.admin.json       # 自動生成（編集禁止）
    └── openapi.gameserver.json  # 自動生成（編集禁止）
```

## モデル共有の考え方

### shared/model.tsp は「基盤となる概念」を定義する場所

複数サービスで共通して使うモデル（Player, Error 等）を定義する。
各サービスの `main.tsp` や `operations.tsp` から `import "../shared/model.tsp"` で参照できる。

### サービス固有の拡張は各サービスの model.tsp で行う

shared のモデルをそのまま使わず、サービスの文脈に合わせて拡張したい場合は
`extends`、`is`、`...`（Spread）を使って各サービスの `model.tsp` に拡張モデルを定義する。

```typespec
// shared/model.tsp — 基盤の概念
model Player {
  @visibility(Lifecycle.Read)
  id: int32;
  name: string;
  @visibility(Lifecycle.Read, Lifecycle.Update)
  level: int32;
}

// game-server/model.tsp — サービス固有の拡張（extends）
model GamePlayer extends Player {
  score: int64;
  lastLoginAt: offsetDateTime;
}

// admin/model.tsp — 別サービスの拡張（Spread）
model ManagedPlayer {
  ...Player;
  isBanned: boolean;
  banReason?: string;
}
```

**使い分け：**
- **`extends`** — 完全な is-a 関係。基盤モデルを継承した新しい型を定義
- **`...`（Spread）** — フィールドを取り込みつつ独立した型にしたいとき。継承関係を持たせない
- **`is`** — extends と似ているが TypeSpec では型エイリアスに近い使い方

## C# 側との関係

### NSwag がサービスごとに独立した DTO を自動生成する

```
shared/model.tsp の Player
    ↓ operations.tsp で使用しているサービスのみ
openapi.<service>.json に Player スキーマが含まれる
    ↓ NSwag
<service>/Generated/Controllers.g.cs に ReadPlayer, CreatePlayer 等の DTO が生成
```

**重要:** operations.tsp で使っていないモデルは OpenAPI に出力されない。
shared/model.tsp に定義しただけでは、そのモデルを操作で使っているサービスにだけ DTO が生成される。

### C# の共有クラスライブラリ（shared.models 等）は不要

1. **同じ概念でもサービスごとに API 契約は独立して進化する** — admin と game-server で公開フィールドが異なるのは自然
2. **二重管理の回避** — 共有 C# クラスを手書きすると、NSwag 生成 DTO との変換レイヤーが増え、メンテコストが上がる
3. **デプロイの独立性** — サービス A の変更で shared ライブラリが変わると、サービス B もリビルドが必要になる
4. **TypeSpec がスキーマレベルの single source of truth** — 概念の共有は TypeSpec で完結。C# は NSwag の自動生成に任せる

### EF Core エンティティはサービスに閉じる

各サービスの `Data/` ディレクトリに `*Entity.cs` を配置する。
サービスが自分の DB スキーマに対して責任を持つ。

## 孤立エンティティの自動クリーンアップ

`npm run tsp-and-nswag` の最後に `scripts/clean-orphan-entities.mjs` が実行される。

TypeSpec 側でモデルを削除 → OpenAPI からスキーマが消える → 対応する `*Entity.cs` が孤立として検出・自動削除される。

ただし **`AppDbContext.cs` の `DbSet<T>` に登録されているエンティティは保護される**。
API に露出しないが DB には必要なエンティティ（監査ログ、セッション等）が誤削除されるのを防ぐため。

手動で `npm run clean:orphan-entities` でも実行可能。

## 新しいリソースを追加する手順

1. モデルを定義（共通なら `shared/model.tsp`、固有なら `<service>/model.tsp`）
2. ルートを `<service>/operations.tsp` に定義（`interface` + `Read<T>`, `Create<T>`, `Update<T>` ラッパー）
3. `npm run tsp-and-nswag` を実行
4. `<service>/Controllers/` に実装コントローラーを作成（生成された抽象クラスを継承）
5. `<service>/Data/` に EF Core エンティティを作成し、`AppDbContext.cs` に `DbSet` を登録
