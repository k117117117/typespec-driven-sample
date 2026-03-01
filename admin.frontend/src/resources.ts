import type { ResourceName } from "./types/resource";

// リソース名を型安全に定義（OpenAPI スキーマと不一致ならコンパイルエラー）
export const resources = {
  adminToolUsers: "admin-tool-users" as const satisfies ResourceName,
  approvalRequests: "approval-requests" as const satisfies ResourceName,
};
