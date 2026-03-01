FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

# csproj のみ先にコピーして依存関係をリストア
# (ソースコードはボリュームマウントで提供)
COPY admin.backend/Presentation/AdminBackend.Presentation.csproj ./admin.backend/Presentation/
COPY admin.backend/Application/AdminBackend.Application.csproj ./admin.backend/Application/
COPY admin.backend/Infrastructure/AdminBackend.Infrastructure.csproj ./admin.backend/Infrastructure/
RUN dotnet restore ./admin.backend/Presentation/AdminBackend.Presentation.csproj

WORKDIR /src/admin.backend/Presentation

EXPOSE 5000

# dotnet watch で変更検知 → 自動リビルド＆リスタート
ENTRYPOINT ["dotnet", "watch", "run", "--non-interactive", "--no-launch-profile"]
