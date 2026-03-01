# TypeSpec

## Directory Structure

```
typespec/
├── shared/
│   └── model.tsp          # Common models shared across all services (Error, Player, etc.)
├── admin/
│   ├── main.tsp            # Service entry point
│   ├── model.tsp           # Admin-specific models (AdminToolUser, ApprovalRequest, etc.)
│   ├── operations.tsp      # CRUD route definitions
│   └── tspconfig.yaml      # output: openapi.admin.json
├── game-server/
│   ├── main.tsp
│   ├── model.tsp           # Game-server-specific models
│   ├── operations.tsp      # Players CRUD, Health
│   └── tspconfig.yaml      # output: openapi.gameserver.json
└── tsp-output/schema/
    ├── openapi.admin.json       # Auto-generated (do not edit)
    └── openapi.gameserver.json  # Auto-generated (do not edit)
```

## Model Sharing

- `shared/model.tsp` defines **foundational concepts** (Player, Error, etc.). All services can `import` from it.
- Service-specific extensions use `extends` or `...` (Spread) in `<service>/model.tsp`.

```typespec
// shared/model.tsp — Foundational concepts
model Player { id: int32; name: string; level: int32; }

// game-server/model.tsp — extends (is-a relationship)
model GamePlayer extends Player { score: int64; lastLoginAt: offsetDateTime; }

// admin/model.tsp — Spread (independent type, copies fields)
model ManagedPlayer { ...Player; isBanned: boolean; }
```

**Choosing between `extends` vs `...` (Spread):**
- **`extends`** — Full is-a inheritance. Creates a subtype of the base model.
- **`...` (Spread)** — Copies fields into an independent type. No inheritance relationship.
- **`is`** — Similar to extends but acts more like a type alias in TypeSpec.

**Important:** Models only appear in OpenAPI output if referenced by an operation. Defining a model in `shared/model.tsp` without using it in `operations.tsp` generates nothing.

## Adding a New Resource (TypeSpec Side)

1. Define the model in `<service>/model.tsp` (or `shared/model.tsp` if shared). Use `@visibility(Lifecycle.Read)` for read-only fields.
2. Define CRUD routes in `<service>/operations.tsp` as a TypeSpec `interface` using `Read<T>`, `Create<T>`, `Update<T>` lifecycle wrappers.
3. Run `npm run tsp-and-nswag` to regenerate OpenAPI JSON, TypeScript types, `Controllers.g.cs`, and clean up orphan entities.

Then proceed to the C# side — see the relevant service's AGENTS.md (`admin.backend/` or `game.server/`).

## Orphan Entity Auto-Cleanup

`scripts/clean-orphan-entities.mjs` runs at the end of `npm run tsp-and-nswag`.

When a model is deleted in TypeSpec → the corresponding `*Entity.cs` is detected as orphan and auto-deleted.

**Exception:** Entities registered in `AppDbContext.cs` via `DbSet<T>` are protected (DB-only entities like audit logs are not deleted).

Run manually: `npm run clean:orphan-entities`
