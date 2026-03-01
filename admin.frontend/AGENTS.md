# Admin Frontend

## File Organization

```
admin.frontend/src/
├── App.tsx              ← Main app component (OpenApiAdmin, dynamic ResourceGuesser rendering)
├── config.ts            ← Environment variables (API_URL, SCHEMA_URL)
├── routes.ts            ← Frontend internal routing paths (as const)
├── api-client.ts        ← openapi-fetch typed API client
├── main.tsx             ← React entry point
├── generated/           ← Auto-generated types and resources (do not edit, gitignored)
│   ├── admin.d.ts       ← openapi-typescript types
│   └── resources.g.ts   ← CRUD resource name constants (scripts/generate-resources.mjs)
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

- `generated/` is gitignored. Regenerate: `npm run tsp:compile`
- New CRUD resources appear automatically in the UI after `tsp:compile` (via `generated/resources.g.ts`)
- `types/` contains hand-written types extending generated types
- Feature folders use barrel exports (`index.ts`) for clean imports
- API paths are type-checked via `openapi-fetch` (no constants — use IDE autocomplete)
- Frontend routing paths are centralized in `routes.ts`

## Adding UI for a Resource

### Auto-Generated CRUD UI

New CRUD resources are automatically rendered after `npm run tsp:compile` — no manual `App.tsx` changes needed.

To customize a specific resource's UI, add an entry to `customResources` in `App.tsx`:

1. Create components in `src/<resource-name>/` (kebab-case).
2. Pass as props to `ResourceGuesser` (`list`, `show`, `create`, `edit`).
3. Add a barrel export (`index.ts`) in the feature folder.
4. Use `FieldGuesser`/`InputGuesser` from `@api-platform/admin` — they auto-detect types from OpenAPI schema.

### Non-CRUD Actions

Use the type-safe `apiClient` from `api-client.ts` (powered by `openapi-fetch`). Paths, methods, and parameters are validated against generated OpenAPI types at compile time.

### Custom Routes (Non-API Pages)

Use react-admin's `<CustomRoutes>` with paths from `routes.ts`. Manually add to the menu in `layout/CustomMenu`.
