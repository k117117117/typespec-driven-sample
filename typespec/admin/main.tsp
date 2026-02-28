import "@typespec/http";
import "../shared/model.tsp";
import "./routes.tsp";

using Http;

@service(#{ title: "Sample Game - Admin" })
namespace SampleGame.Admin;
