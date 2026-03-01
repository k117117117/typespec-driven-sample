# AGENTS

## Output Efficiency

- **Do not narrate steps.** Do not explain what you are about to do or just did. Summarize results once at the end.
- Internal reasoning (thinking) is unrestricted — prioritize quality over token savings there.
- Per-project context is in each project's own `AGENTS.md`. Read only the ones relevant to the current task (may be multiple when work spans projects).
- `docs/` contains human-readable design docs. Read relevant ones when working on related topics (e.g., `url-design-notes.md` for routing, `design-philosophy.md` for architecture decisions).

## Architecture

This is a **TypeSpec-driven schema-first** monorepo. The data flow is:

```
typespec/shared/model.tsp            ← Base models (cross-service concept definitions)
typespec/<service>/model.tsp         ← Service-specific models (extends / Spread)
typespec/<service>/operations.tsp    ← Route definitions
         ↓ (tsp compile)
typespec/tsp-output/schema/openapi.<service>.json
         ↓ (NSwag)
<service>/Presentation/Generated/Controllers.g.cs ← Abstract controllers + DTOs
         ↓ (manual)
<service>/Presentation/<Resource>/    ← Implementation controllers
```

**Key principle:** TypeSpec `.tsp` files are the single source of truth for the API contract. All downstream artifacts (OpenAPI spec, C# controllers, frontend UI) derive from them.

## Design Principles

- `typespec/shared/model.tsp` defines foundational concepts. Services import from it.
- Service-specific extensions go in `typespec/<service>/model.tsp` using `extends` or `...` (Spread).
- **No shared C# class library needed.** NSwag generates independent DTOs per service. Concept sharing is handled entirely in TypeSpec.
- Each service's EF Core entities are service-local (`Infrastructure/<ResourceName>/`).
- Each service follows a 3-project split: Presentation / Application / Infrastructure (see per-service AGENTS.md).

## Build & Run Commands

```bash
docker compose up -d                         # Start everything (Docker required)
npm run tsp-and-nswag                        # TypeSpec compile + NSwag + orphan cleanup
npm run tsp:compile                          # TypeSpec → openapi.json (all services)
npm run tsp:compile:admin                    # admin only
npm run tsp:compile:game-server              # game-server only
npm run nswag:generate                       # openapi.json → C# controllers (all)
npm run clean:orphan-entities                # Detect/delete orphan *Entity.cs files
npm run build:backend                        # dotnet build
npm run build:frontend                       # npm install + vite build (admin.frontend/)
npm run openapi:generate:ts                  # openapi.json → TypeScript types (admin.frontend/)
npm run down:clean && npm run up:build       # Clean restart (rebuild images, reset DB)
```

No test suite exists in this project.

## Files You Must NOT Edit by Hand

- `*/Presentation/Generated/Controllers.g.cs` — NSwag auto-generated. Regenerate: `npm run nswag:generate`
- `typespec/tsp-output/` — TypeSpec compiler output. Regenerate: `npm run tsp:compile`
- `admin.frontend/src/generated/` — openapi-typescript output. Regenerate: `npm run openapi:generate:ts`

## Tech Stack

| Layer | Technology |
|---|---|
| Schema | TypeSpec → OpenAPI 3.1 JSON |
| Code gen (backend) | NSwag (`openapi2cscontroller`, abstract style) |
| Code gen (frontend) | openapi-typescript (OpenAPI → TypeScript types) |
| Backend | ASP.NET Core 10, EF Core, PostgreSQL 17 |
| Frontend | React 19, Vite, API Platform Admin (react-admin), openapi-fetch |
| Infra | Docker Compose (nginx reverse proxy per service) |

## Language

- **Internal reasoning / thinking:** English
- **User-facing output (responses, commit messages, code comments, PR descriptions):** User's input language (default: Japanese)
- **Interactive prompts (confirmation dialogs, choice labels):** User's input language (e.g., "はい" / "いいえ" instead of "Yes" / "No")
- **AGENTS.md files:** English (optimized for AI agent comprehension)

## File Formatting

- All created or edited files **must end with a newline** (final EOL before EOF).
- `*/Generated/` files are auto-generated — do not modify them, including EOF newline fixes.

## Git Workflow

- **Do not commit or push automatically.** Leave changes staged/unstaged unless explicitly requested.
- On commit request: create commit(s) **locally only** — do not push.
- **Only push when the user explicitly asks for both commit and push.**

