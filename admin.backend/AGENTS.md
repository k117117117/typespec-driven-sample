# Admin Backend

## Layered Architecture (3-Project Split)

```
admin.backend/
├── Presentation/
│   ├── AdminBackend.Presentation.csproj    ← Web (ASP.NET, NSwag, Scalar)
│   ├── Program.cs                          ← DI registration
│   ├── Generated/Controllers.g.cs          ← NSwag auto-generated (do not edit)
│   ├── <Resource>/<Resource>Controller.cs  ← HTTP endpoint implementation
│   └── Health/HealthController.cs
├── Application/
│   ├── AdminBackend.Application.csproj     ← Domain + Application (pure C# classlib, RootNamespace: AdminBackend)
│   ├── Domain/
│   │   └── <Resource>/
│   │       ├── Models/<Resource>.cs        ← Domain model (internal Create/Update/Reconstruct)
│   │       └── Repositories/I<Resource>Repository.cs
│   └── Application/
│       └── <Resource>/
│           └── <Resource>ApplicationService.cs ← Use case orchestration
└── Infrastructure/
    ├── AdminBackend.Infrastructure.csproj   ← EF Core, Npgsql (→ Application reference)
    ├── DependencyInjection.cs              ← Public DI extension method
    ├── Data/AppDbContext.cs                ← internal DbContext
    └── <Resource>/
        ├── <Resource>Entity.cs             ← internal EF Core entity
        └── <Resource>Repository.cs         ← internal repository implementation
```

**Dependency direction:** Presentation → Application ← Infrastructure (dependency inversion via DI)

## Namespace Conventions

- Domain: `AdminBackend.Domain.<Resource>.*` (e.g., `AdminBackend.Domain.AdminToolUsers.Models`)
- Application: `AdminBackend.Application.<Resource>` (e.g., `AdminBackend.Application.AdminToolUsers`)
- Infrastructure: `AdminBackend.Infrastructure.*`
- Presentation: `AdminBackend.*`

## `internal` Modifier Usage

- Domain: `Create()`, `Update*()`, `Reconstruct()` methods → prevents bypassing ApplicationService or Repository
- Infrastructure: Entity, Repository, AppDbContext → prevents Web from bypassing Repository
- Application csproj has `InternalsVisibleTo` for Infrastructure (so Repository can call `Reconstruct`)

## Adding a New API Resource (C# Side)

After running `npm run tsp-and-nswag` (see `typespec/AGENTS.md` for TypeSpec steps):

1. **Domain layer** (`Application/Domain/<ResourceName>/`):
   - `Models/<ResourceName>.cs` — Domain model with `internal` factory/mutation/reconstruct methods, `public` properties only.
   - `Repositories/I<ResourceName>Repository.cs` — Repository interface.

2. **Application layer** (`Application/Application/<ResourceName>/`):
   - `<ResourceName>ApplicationService.cs` — Use case orchestration.

3. **Infrastructure layer** (`Infrastructure/<ResourceName>/`):
   - `<ResourceName>Entity.cs` — `internal` EF Core entity.
   - `<ResourceName>Repository.cs` — `internal` repository (Entity ↔ Domain model conversion).
   - Register `DbSet` in `Infrastructure/Data/AppDbContext.cs`. Enums use `HasConversion<string>()`.
   - Register DI in `Infrastructure/DependencyInjection.cs`.

4. **Presentation layer** (`Presentation/<ResourceName>/`):
   - `<ResourceName>Controller.cs` — Inherits NSwag abstract controller (e.g., `FooController : FooControllerBaseControllerBase`).
   - Uses **primary constructor DI**: `public class FooController(FooApplicationService appService)`.
   - Map between NSwag DTOs (e.g., `ReadFoo`, `CreateFoo`) and domain models via private static helpers (`ToReadDto`, `ToListItem`).

5. Register ApplicationService in `Presentation/Program.cs` via `AddScoped<>()`.
