import { OpenApiAdmin, ResourceGuesser } from "@api-platform/admin";
import { CustomRoutes } from "react-admin";
import { Route } from "react-router-dom";
import { ApprovalRequestList } from "./approvalRequest/ApprovalRequestList";
import { ApprovalRequestShow } from "./approvalRequest/ApprovalRequestShow";
import { ApprovalRequestEdit } from "./approvalRequest/ApprovalRequestEdit";
import { ApprovalRequestCreate } from "./approvalRequest/ApprovalRequestCreate";
import { HelloWorld } from "./helloWorld/HelloWorld";
import { CustomLayout } from "./layout/CustomLayout";

export const API_URL = import.meta.env.VITE_API_URL || "http://localhost:8080";
const SCHEMA_URL = import.meta.env.VITE_SCHEMA_URL || `${API_URL}/openapi.json`;

const App = () => (
  <OpenApiAdmin
    entrypoint={API_URL}
    docEntrypoint={SCHEMA_URL}
    layout={CustomLayout}
  >
    {/* CRUDに対応したページ(Reactコンポーネント)を明示的に指定しない場合は自動生成される */}
    <ResourceGuesser name="admin-tool-users" />

    {/* CRUDに対応したページ(Reactコンポーネント)を明示的に指定 */}
    <ResourceGuesser
      name="approval-requests"
      list={ApprovalRequestList}
      show={ApprovalRequestShow}
      create={ApprovalRequestCreate}
      edit={ApprovalRequestEdit}
    />

    {/* OpenAPI仕様に対応しない独自のルートも追加することができる */}
    <CustomRoutes>
      {/* カスタムルートは自動的にはMenuに追加されないので以下の方法で追加する必要がある */}
      {/* https://marmelab.com/react-admin/CustomRoutes.html#adding-custom-routes-to-the-menu */}
      <Route path="/hello-world" element={<HelloWorld />} />
    </CustomRoutes>
  </OpenApiAdmin>
);

// NOTE 完全に自動生成されたUIだけを使用する場合は、以下のようにOpenApiAdminコンポーネントを定義するだけでOK
// const App = () => (
//   <OpenApiAdmin
//     entrypoint={API_URL}
//     docEntrypoint={SCHEMA_URL}
//   />
// );

export default App;
