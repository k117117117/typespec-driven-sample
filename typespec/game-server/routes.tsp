import "@typespec/http";
import "../shared/model.tsp";

using Http;
using SampleGame;

namespace SampleGame.GameServer;

// TODO: ゲームサーバー固有のルートを追加

@route("/healthz")
@tag("Health")
interface Health {
  @get check(): { status: "healthy" };
}
