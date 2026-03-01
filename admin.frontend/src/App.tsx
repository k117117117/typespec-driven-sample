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
import { resources } from "./generated/resources.g";
import { routes } from "./routes";

// カスタムコンポーネントを使用するリソースのオーバーライド定義
const customResources: Partial<
  Record<string, { list?: React.ComponentType; show?: React.ComponentType; create?: React.ComponentType; edit?: React.ComponentType }>
> = {
  "approval-requests": {
    list: ApprovalRequestList,
    show: ApprovalRequestShow,
    create: ApprovalRequestCreate,
    edit: ApprovalRequestEdit,
  },
};

const App = () => (
  <OpenApiAdmin
    entrypoint={API_URL}
    docEntrypoint={SCHEMA_URL}
    layout={CustomLayout}
    i18nProvider={i18nProvider}
  >
    {Object.values(resources).map((name) => (
      <ResourceGuesser key={name} name={name} {...customResources[name]} />
    ))}

    {/* OpenAPI仕様に対応しない独自のルートを追加する例 */}
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
