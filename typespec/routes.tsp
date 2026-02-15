import "@typespec/http";

using Http;

namespace BroGameAdmin;

@route("/admin-tool-users")
@tag("AdminToolUsers")
interface AdminToolUsers {
  @get list(): Read<AdminToolUser>[] | Error;
  @get read(@path id: int32): Read<AdminToolUser> | Error;
  @post create(@body body: Create<AdminToolUser>): CreatedResponse | Error;
  @put update(@path id: int32, @body body: Update<AdminToolUser>): OkResponse | Error;
  @delete delete(@path id: int32): OkResponse | Error;
}

@route("/approval-requests")
@tag("ApprovalRequests")
interface ApprovalRequests {
  @get list(): Read<ApprovalRequest>[] | Error;
  @get read(@path id: int32): Read<ApprovalRequest> | Error;
  @post create(@body body: Create<ApprovalRequest>): CreatedResponse | Error;
  @put update(@path id: int32, @body body: Update<ApprovalRequest>): OkResponse | Error;
  @delete delete(@path id: int32): OkResponse | Error;

  @route("{id}/approve") @post approve(@path id: int32): OkResponse | Error;
  @route("{id}/reject") @post reject(@path id: int32): OkResponse | Error;
}
