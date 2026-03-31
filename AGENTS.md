# Agent Guidelines for scmod

This is a CLI tool for creating Survivalcraft mod projects from templates.

## Project Overview

- **Type**: Node.js CLI tool
- **Language**: TypeScript
- **Main entry**: `src/index.ts`
- **Package manager**: npm

## Commands

### Install dependencies
```bash
npm install
```

### Build project
```bash
npm run build
```

### Run the CLI
```bash
# Development mode (build + run)
npm run dev new <project-name>

# Build then run
npm run build && node dist/index.js new <project-name>

# Or after npm install -g
scmod new <project-name>
```

### Run tests
```bash
npm test
```
Currently there are no tests. To add tests, consider using Jest with ts-jest.

### Type check
```bash
npx tsc --noEmit
```

### Lint
```bash
# No linting configured - run manually with:
npm install --save-dev eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin
npx eslint src/ --ext .ts
```

### Format
```bash
# No formatter configured - run manually with:
npm install --save-dev prettier
npx prettier --write src/
```

## Code Style Guidelines

### General
- Use TypeScript (no plain JavaScript)
- Use ES modules `import/export` (with `"type": "module"` in package.json)
- 2-space indentation
- No semicolons at line endings
- Use single quotes for strings
- Trailing commas in arrays and objects

### File Structure
```
src/
  index.ts        # Main entry point
  # Add additional modules as needed
```

### Naming Conventions
- **Files**: kebab-case (e.g., `my-module.ts`)
- **Functions**: camelCase (e.g., `createProject`, `copyDir`)
- **Constants**: UPPER_SNAKE_CASE (e.g., `TEMPLATE_PATH`)
- **Classes**: PascalCase (e.g., `ProjectCreator`)
- **Interfaces**: PascalCase with `I` prefix (e.g., `IProjectOptions`)

### Imports
```typescript
// Use ES modules with type annotations
import inquirer from 'inquirer'
import fs from 'fs'
import path from 'path'
import { execSync } from 'child_process'
```

### TypeScript Specific
- Always define return types for functions
- Use interfaces for object shapes
- Enable strict mode in tsconfig.json
- Avoid using `any` type

### Functions
- Use arrow functions or function declarations consistently
- Keep functions small and focused
- Use descriptive names

### Error Handling
- Use `try/catch` for operations that may fail
- Use `process.exit(1)` for fatal errors
- Log errors with `console.error()`

### Example Pattern
```typescript
interface ProjectOptions {
  name: string
  gitInit: boolean
}

function createProject(options: ProjectOptions): void {
  if (!options.name) {
    console.error('Error: project name is required')
    process.exit(1)
  }

  try {
    const result = someOperation(options.name)
    console.log(`Success: ${result}`)
  } catch (e) {
    console.error(`Error: ${e instanceof Error ? e.message : 'Unknown error'}`)
    throw e
  }
}
```

### Output Formatting
- Use template literals for string interpolation
- Prefix success messages with `✓`
- Prefix warning messages with `⚠`
- Prefix error messages with `Error:`
- Use backticks for logging: `` console.log(`Value: ${value}`) ``

## Working with this Project

### Creating a new mod project
```bash
npm run dev new my-mod
```

This copies the template from `../SurvivalcraftMod` (relative to this project) and renames files/contents.

### Adding new features
1. Edit `src/index.ts` for CLI commands
2. Follow the existing code patterns
3. Run `npm run build` to compile
4. Test manually with `node dist/index.js <command>`

### Best Practices
- Always check if directories/files exist before operations
- Use `{ recursive: true }` for directory creation
- Handle errors gracefully
- Provide clear console output
- Run type checking before committing: `npx tsc --noEmit`
