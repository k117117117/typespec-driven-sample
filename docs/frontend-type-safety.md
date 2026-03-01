# フロントエンドの型安全性

## OpenAPI → TypeScript 型生成

`openapi-typescript` が OpenAPI 仕様から TypeScript 型定義 (`.d.ts`) を生成します。ランタイムコードは含まれません。

```bash
npm run openapi:generate:ts    # → admin.frontend/src/generated/admin.d.ts
```

## リソース名の型安全性

`src/types/resource.ts` で生成型の `paths` キーから `ResourceName` ユニオン型を導出しています。`ResourceGuesser` の `name` に存在しないリソース名を指定するとコンパイルエラーになります。

`scripts/generate-resources.mjs` が OpenAPI の paths から CRUD リソース（`/{path}` と `/{path}/{id}` の両方を持つパス）を検出し、`generated/resources.g.ts` にリソース名定数を自動生成します。`App.tsx` はこの定数を動的に参照するため、TypeSpec にリソースを追加すれば `tsp:compile` 後に自動的に UI に反映されます。

```typescript
// generated/resources.g.ts — scripts/generate-resources.mjs により自動生成
import type { ResourceName } from "../types/resource";

export const resources = {
  adminToolUsers: "admin-tool-users" as const satisfies ResourceName,
  approvalRequests: "approval-requests" as const satisfies ResourceName,
} as const;
```

## API 呼び出しの型安全性

`openapi-fetch` が生成型を使って**パス・メソッド・パラメータ**を型チェックします。IDE の補完で利用可能なエンドポイントが一覧表示されます。

```typescript
// api-client.ts
import createClient from "openapi-fetch";
import type { paths } from "./generated/admin";
export const apiClient = createClient<paths>({ baseUrl: API_URL });

// 使用例 — パスが OpenAPI 定義に存在しなければコンパイルエラー
const { error } = await apiClient.POST("/approval-requests/{id}/approve", {
  params: { path: { id: Number(record.id) } },
});
```
