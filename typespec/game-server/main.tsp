import "@typespec/http";
import "../shared/model.tsp";
import "./model.tsp";
import "./operations.tsp";

using Http;

@service(#{ title: "Sample Game - Game Server" })
namespace SampleGame.GameServer;
