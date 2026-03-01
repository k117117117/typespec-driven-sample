import { OpenApiAdmin, ResourceGuesser } from "@api-platform/admin";
import { CustomRoutes } from "react-admin";
import { Route } from "react-router-dom";
import {
  ApprovalRequestList,
  ApprovalRequestShow,
  ApprovalRequestEdit,
  ApprovalRequestCreate,
} from "./approval-request";
import { HelloWorld } from "./hello-world/HelloWorld";
import { CustomLayout } from "./layout";
import { i18nProvider } from "./i18n";
import { API_URL, SCHEMA_URL } from "./config";
import { resources } from "./resources";
import { routes } from "./routes";

const App = () => (
  <OpenApiAdmin
    entrypoint={API_URL}
    docEntrypoint={SCHEMA_URL}
    layout={CustomLayout}
    i18nProvider={i18nProvider}
  >
    {/* CRUDに対応したページ(Reactコンポーネント)を明示的に指定しない場合は自動生成される */}
    <ResourceGuesser name={resources.adminToolUsers} />

    {/* CRUDに対応したページ(Reactコンポーネント)を明示的に指定 */}
    <ResourceGuesser
      name={resources.approvalRequests}
      list={ApprovalRequestList}
      show={ApprovalRequestShow}
      create={ApprovalRequestCreate}
      edit={ApprovalRequestEdit}
    />

    {/* OpenAPI仕様に対応しない独自のルートも追加することができる */}
    <CustomRoutes>
      {/* カスタムルートは自動的にはMenuに追加されないので以下の方法で追加する必要がある */}
      {/* https://marmelab.com/react-admin/CustomRoutes.html#adding-custom-routes-to-the-menu */}
      <Route path={routes.helloWorld} element={<HelloWorld />} />
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
