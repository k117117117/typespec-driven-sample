FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

# csproj のみ先にコピーして依存関係をリストア
# (ソースコードはボリュームマウントで提供)
COPY game.server/Presentation/GameServer.Presentation.csproj ./game.server/Presentation/
COPY game.server/Application/GameServer.Application.csproj ./game.server/Application/
COPY game.server/Infrastructure/GameServer.Infrastructure.csproj ./game.server/Infrastructure/
RUN dotnet restore ./game.server/Presentation/GameServer.Presentation.csproj

WORKDIR /src/game.server/Presentation

EXPOSE 5001

# dotnet watch で変更検知 → 自動リビルド＆リスタート
ENTRYPOINT ["dotnet", "watch", "run", "--non-interactive", "--no-launch-profile"]
