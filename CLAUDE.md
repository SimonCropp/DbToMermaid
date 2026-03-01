# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

```bash
# Build
dotnet build src --configuration Release --nologo

# Run tests (dotnet test does NOT work on .NET 10 SDK — use dotnet run)
dotnet run --project src/EfToMermaid.Tests --configuration Release --no-build
dotnet run --project src/SqlServerToMermaid.Tests --configuration Release --no-build

# Run a single test project during development (build + run)
dotnet run --project src/EfToMermaid.Tests

# SqlServerToMermaidTool.Tests requires LocalDb and may not run in all environments
```

## Architecture

Three libraries convert database schemas to Mermaid ER diagrams, all sharing the same rendering pipeline:

```
EfToMermaid (EF Core IModel)  ──→  SchemaReader  ──→  Database record  ──→  DiagramRender
SqlServerToMermaid (SMO)      ──→  SchemaReader  ──→  Database record  ──→  DiagramRender
SqlServerToMermaid (Scripts)  ──→  ScriptParser  ──→  Database record  ──→  DiagramRender
SqlServerToMermaidTool        ──→  CLI wrapper (CliFx) around SqlServerToMermaid
```

**Shared code** (`src/Shared/`) is included via `<Compile Include>` in both library `.csproj` files (not a separate project reference). It contains `DiagramRender.cs` (Mermaid output), `SchemaModel.cs` (the `Database`/`Table`/`Column`/`ForeignKey` records), and `GlobalUsings.cs`.

## Testing

- **Framework:** TUnit with Verify (snapshot testing) and DiffPlex
- **Snapshots:** `*.verified.md` files are the expected outputs. When rendering changes, update verified files to match
- **EfToMermaid.Tests** use fake connection strings (no DB needed). Tests requiring resolved metadata (e.g. comments) use `IDesignTimeModel`
- **SqlServerToMermaid.Tests** use LocalDb for connection-based tests and `ScriptParser` for script-based tests
- **ModuleInitializer** pattern in each test project initializes Verify/DiffPlex

## Code Style

- C# 14, .NET 10, file-scoped namespaces (implicit), `TreatWarningsAsErrors`
- `.editorconfig` enforces strict style rules at build time
- `var` required everywhere, collection expressions, pattern matching preferred
- Use `_` for lambda parameters: `_.Name` not `fk.Name`, `.Select(_ => _.Build())` not `.Select(t => t.Build())`
