FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /src

# csproj のみ先にコピーして依存関係をリストア
# (ソースコードはボリュームマウントで提供)
COPY admin.backend/Backend.csproj ./admin.backend/
COPY shared.models/SharedModels.csproj ./shared.models/
RUN dotnet restore ./admin.backend/Backend.csproj

WORKDIR /src/admin.backend

EXPOSE 5000

# dotnet watch で変更検知 → 自動リビルド＆リスタート
ENTRYPOINT ["dotnet", "watch", "run", "--non-interactive", "--no-launch-profile"]
