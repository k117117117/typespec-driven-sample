import "@typespec/http";
import "./model.tsp";
import "./routes.tsp";

using Http;
@service(#{ title: "Sample Game - 管理ツール" })
namespace BroGameAdmin;
