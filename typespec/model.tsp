import "@typespec/http";

using Http;

namespace BroGameAdmin;

model AdminToolUser {
  id: int32;
  name: string;
  email: string;
  role: "admin" | "staff";
}

// NOTE API Platform Adminの仕様的に GET /approval-requests/:id のレスポンススキーマによってCreateとUpdateのフォームフィールドも決定されるらしい
// そのためRead<ApprovalRequest>、Create<ApprovalRequest>、Update<ApprovalRequest>のように通信定義用のmodelを分けてもCreateやUpdateのフォームフィールドに反映されない
// この場合はAPI Platform Admin側で明示的に必要なフォームフィールドだけ定義したReactコンポーネントを作成してそのコンポーネントを使うように指定する必要がある
model ApprovalRequest {
  @visibility(Lifecycle.Read)
  id: int32;
  
  reason: string;

  @visibility(Lifecycle.Read)
  status: "pending" | "approved" | "rejected";
  
  @visibility(Lifecycle.Read)
  createdAt: offsetDateTime;

  @visibility(Lifecycle.Read)
  updatedAt: offsetDateTime;
}

@error
model Error {
  code: int32;
  message: string;
}
