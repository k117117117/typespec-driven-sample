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

// NOTE 完全に自動生成されたUIだけを使用する場合は、以下のようにOpenApiAdminコンポーネントを定義するだけでOK
const App = () => (
  <OpenApiAdmin
    entrypoint={API_URL}
    docEntrypoint={SCHEMA_URL}
  />
);

export default App;
