# Admin Frontend

## File Organization

```
admin.frontend/src/
├── App.tsx              ← Main app component (OpenApiAdmin)
├── config.ts            ← Environment variables (API_URL, SCHEMA_URL)
├── resources.ts         ← Type-safe resource name constants (validated against OpenAPI paths)
├── routes.ts            ← Frontend internal routing paths (as const)
├── api-client.ts        ← openapi-fetch typed API client
├── main.tsx             ← React entry point
├── generated/           ← openapi-typescript auto-generated types (do not edit, gitignored)
│   └── admin.d.ts
├── types/               ← Hand-written types derived from generated types
│   └── resource.ts      ← ResourceName union type (derived from paths keys)
├── i18n/                ← Internationalization
│   ├── index.ts         ← i18nProvider export
│   └── ja.ts            ← Japanese translations
├── <resource-name>/     ← Feature folders (kebab-case, with index.ts barrel export)
├── layout/              ← Custom layout and menu
└── hello-world/         ← Custom page example
```

## Key Conventions

- `generated/` is gitignored. Regenerate: `npm run openapi:generate:ts`
- `types/` contains hand-written types extending generated types
- Feature folders use barrel exports (`index.ts`) for clean imports
- API paths are type-checked via `openapi-fetch` (no constants — use IDE autocomplete)
- Frontend routing paths are centralized in `routes.ts`

## Adding UI for a Resource

### Auto-Generated CRUD UI

Add a `ResourceGuesser` in `App.tsx`:

```tsx
import { resources } from "./resources";
<ResourceGuesser name={resources.newResource} />
```

Add the resource name to `resources.ts` with `satisfies ResourceName` for compile-time validation.

### Custom UI

1. Create components in `src/<resource-name>/` (kebab-case).
2. Pass as props to `ResourceGuesser` (`list`, `show`, `create`, `edit`).
3. Add a barrel export (`index.ts`) in the feature folder.
4. Use `FieldGuesser`/`InputGuesser` from `@api-platform/admin` — they auto-detect types from OpenAPI schema.

### Non-CRUD Actions

Use the type-safe `apiClient` from `api-client.ts` (powered by `openapi-fetch`). Paths, methods, and parameters are validated against generated OpenAPI types at compile time.

### Custom Routes (Non-API Pages)

Use react-admin's `<CustomRoutes>` with paths from `routes.ts`. Manually add to the menu in `layout/CustomMenu`.
