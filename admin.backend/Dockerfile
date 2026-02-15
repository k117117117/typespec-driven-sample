FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /app

# csproj のみ先にコピーして依存関係をリストア
# (ソースコードはボリュームマウントで提供)
COPY admin.backend/*.csproj ./
RUN dotnet restore

EXPOSE 5000

# dotnet watch で変更検知 → 自動リビルド＆リスタート
ENTRYPOINT ["dotnet", "watch", "run", "--non-interactive", "--no-launch-profile"]
