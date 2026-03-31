# AGENTS.md - scmod-cli

## Project Overview
.NET 10.0 CLI tool (`scmod`) for scaffolding Survivalcraft game mods.

## Build/Test Commands

```bash
# Build
dotnet build

# Run (from repo root)
dotnet run -- new MyMod

# Run with specific entry point
dotnet run --project src/ScmodCli -- new MyMod

# Build and run as tool
dotnet pack
dotnet tool install -g --add-source ./nupkg ScmodCli
scmod new MyMod

# Clean
dotnet clean
```

**No test framework configured.** No automated tests exist. Manual testing is done by running the CLI and inspecting output in `test-output/`.

## Code Style

### Structure
- Single project: `src/ScmodCli/`
- File-scoped namespaces preferred (`namespace ScmodCli.Commands;`)
- Static classes for command logic (`ProjectCreator`, `TemplateExtractor`)
- `Program.cs` uses top-level statements with explicit `class Program` and `Main` method

### Imports
- Implicit usings enabled in `.csproj`
- Explicit `using` directives at file top, before namespace
- Fully qualify when ambiguous: `System.Diagnostics.ProcessStartInfo`

### Naming
- Classes: PascalCase (`ProjectCreator`)
- Public methods: PascalCase with `Async` suffix (`CreateAsync`)
- Private methods: PascalCase (`ValidateProjectName`, `CopyDirectory`, `ReplaceInFile`)
- Variables/parameters: camelCase (`projectName`, `outputDir`, `modinfoPath`)
- Namespaces match folder structure (`ScmodCli`, `ScmodCli.Commands`)

### Types
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- Use `string?` for nullable strings, `string.Empty` or `??` for defaults
- Prefer `var` when type is obvious; explicit types for clarity
- Use `HashSet<string>` with `StringComparer.OrdinalIgnoreCase` for case-insensitive sets
- Tuple deconstruction for paired data: `(string oldStr, string newStr)[]`

### Async Patterns
- All I/O methods are `async Task` or `async Task<T>`
- Accept `CancellationToken` where applicable (via `SetAction` callbacks)
- Use `await` consistently: `WaitForExitAsync()`, `ReadToEndAsync()`, `ReadAllTextAsync()`

### Error Handling
- Write errors to `Console.Error` with `"Error: "` prefix
- Fatal errors: call `Environment.Exit(1)` then `return`
- Wrap risky operations (JSON parsing, git init) in `try/catch`
- Use null-coalescing: `?? string.Empty`, `?? "UnknownMod"`
- Throw `InvalidOperationException` for unrecoverable internal errors

### Output
- Success messages prefixed with `✓`
- Warnings prefixed with `⚠`
- Use interpolated strings: `$"Created: {path}"`

### Formatting
- 4-space indentation, CRLF line endings
- Braces on new lines for class/method bodies
- Expression-bodied members acceptable for simple methods
- Max line length: ~150 chars
