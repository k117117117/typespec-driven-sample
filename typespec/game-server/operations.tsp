import "@typespec/http";
import "../shared/model.tsp";
import "./model.tsp";

using Http;
using SampleGame;

namespace SampleGame.GameServer;

@route("/players")
@tag("Players")
interface Players {
  @list @get list(
    @offset @query offset?: int32,
    @pageSize @query limit?: int32,
  ): Page<Read<Player>> | Error;
  @get read(@path id: int32): Read<Player> | Error;
  @post create(@body body: Create<Player>): CreatedResponse | Error;
  @put update(@path id: int32, @body body: Update<Player>): OkResponse | Error;
  @delete delete(@path id: int32): OkResponse | Error;
}

@route("/healthz")
@tag("Health")
interface Health {
  @get check(): { status: "healthy" };
}
