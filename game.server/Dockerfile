FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /src

# csproj のみ先にコピーして依存関係をリストア
COPY game.server/GameServer.csproj ./game.server/
COPY shared.models/SharedModels.csproj ./shared.models/
RUN dotnet restore ./game.server/GameServer.csproj

WORKDIR /src/game.server

EXPOSE 5001

# dotnet watch で変更検知 → 自動リビルド＆リスタート
ENTRYPOINT ["dotnet", "watch", "run", "--non-interactive", "--no-launch-profile"]
