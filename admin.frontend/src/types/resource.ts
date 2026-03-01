import type { paths } from "../generated/admin";

/**
 * OpenAPI の paths キーから React Admin の ResourceGuesser name に使えるリソース名を導出する。
 * - 先頭の "/" を除去
 * - パスパラメータを含むもの（例: "/admin-tool-users/{id}"）を除外
 *
 * 結果: "admin-tool-users" | "approval-requests" | "healthz"
 */
type StripLeadingSlash<T extends string> = T extends `/${infer Rest}`
  ? Rest
  : T;

type TopLevelPaths<T extends string> = T extends `${string}/{${string}}`
  ? never
  : T;

export type ResourceName = StripLeadingSlash<TopLevelPaths<keyof paths & string>>;
