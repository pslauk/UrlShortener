# Agent Instructions

> **Source of truth:** This file is the canonical repo-wide guidance for coding agents and contains information about the repository.


## Project Overview

UrlShortener — a multi-layered ASP.NET Core MVC URL shortening service.

- Platform: C#, .NET 8.0.
- Database: SQL Server (LocalDB for local development).
- UI: Razor Views, CSS, JS.
- Architecture: Clean Architecture.

### Technologies

- ASP.NET Core MVC, Razor Views.
- EF Core + SQL Server LocalDB, connection string in `appsettings.json`.
- AutoMapper.
- xUnit + Moq for unit tests in `UrlServiceTests.cs`.

## Repository Structure

**Important:** The project follows Clean Architecture. The presentation layer handles user interaction and request processing. The application layer executes business logic and coordinates between the presentation and domain layers. The domain layer contains business logic and data-related components. The infrastructure layer supports application structures and communicates with external systems (e.g., databases, APIs, etc.). The domain layer must not depend on any other layer. This approach is mandatory and must be followed when adding new functionality.

| Layer | Project |
|-------|---------|
| Presentation layer | `UrlShortener` |
| Application layer | `UrlShortener.Application` |
| Domain layer | `UrlShortener.DomainModel` |
| Infrastructure layer | `UrlShortener.DataAccess` |

### Layer Boundaries

- `UrlShortener.DomainModel`:
	- Contains domain entities and abstractions.
	- Must not reference `Application`, `DataAccess`, or `UrlShortener`.
- `UrlShortener.Application`:
	- Contains use cases, services, application DTOs/models, and interfaces for infrastructure.
	- Must not contain EF Core details, SQL, or MVC controllers.
- `UrlShortener.DataAccess`:
	- Implements data access, repositories, DbContext, and migrations.
	- May depend on `DomainModel` and infrastructure packages.
- `UrlShortener`:
	- Controllers, views, web application configuration.
	- Must not contain business logic that belongs in `Application`.

## Task-Specific Agent Skills

The `.agents/skills/` folder contains skills for specific agents (e.g., the planning agent, the developer agent).

## Common rules for all Agents

### Instruction Priority

When instructions conflict, the agent must apply rules in the following order:

1. System and platform constraints of the runtime environment.
2. This `AGENTS.md` file.
3. The specific agent skill from `.agents/skills/` if it was used in the context of the task.
4. Local instructions in subfolders (if they exist and apply to the area being changed).
5. The user's request.

If the user's request conflicts with a higher-priority rule, the agent must:

1. Briefly point out the conflict.
2. Propose a safe and closest acceptable alternative.
3. Continue execution within the allowed boundaries.

### Minimal comments in code

**Important:** Write minimal comments in code. Code should be self-documenting. Only add comments for non-obvious business logic or workarounds.

### Secrets in Non-Committed Files
Never delete or overwrite secrets stored in files excluded from git, such as `.env`, local launch settings, or development-only config files. If a task touches such files, preserve user-provided secrets and non-committed local values.

### Core Execution Directives
> **CRITICAL: These rules override all other behavior and apply to EVERY prompt, agent, and skill. Violation is unacceptable.**

| Directive | Rule |
|-----------|------|
| **FULL IMPLEMENTATION** | Never use placeholders, stubs, `// TODO`, `// ...`, `/* implement */`, or skeleton code. Every function, type, module, and component MUST be fully implemented with production-ready code. If a plan step says "implement X" — implement X completely, not partially. |
| **DO NOT TRUNCATE CODE** | Never truncate, abbreviate, or skip parts of code. No "rest is similar", "continue the pattern", "same as above", or "remaining fields omitted". If a file needs 200 lines — write all 200 lines. |
| **CONTEXT BEFORE CHANGING** | Before modifying ANY file, read it first. Before suggesting architectural changes, understand the current architecture. Never guess about file contents, interfaces, or method signatures — always verify. |
| **AUTONOMOUS EXECUTION TO COMPLETION** | Once a task or prompt command is started, execute it fully from start to finish without stopping midway. Do not pause for confirmation unless explicitly required by the prompt workflow. Do not say "you can do the rest" or leave work for the user. |

### Security and Configuration

- Never delete or overwrite user secrets and local values in non-committed files (`.env`, local launch profiles, development configs).
- Never modify secrets or connection strings without an explicit request.
- Never perform destructive git or database operations without explicit user confirmation.

### Editing Constraints

- Keep changes local and minimal.
- Before editing, verify the correct project and layer are selected.
- Prefer existing patterns for DI, configuration, logging, repositories, and contracts.
- Do not edit generated or tooling folders:
	- `bin/`
	- `obj/`
	- `node_modules/`
	- `UrlShortener/wwwroot/lib`
	- publish artifacts
	- shadow-copy directories

### C# Code Conventions

These conventions apply only to C# code (`*.cs`) in this repository.

- Leave an empty line before a `return` statement, except when the method or expression is a one-liner, or when the `return` is the only statement inside a small control-flow block such as an `if` block.
- Leave an empty line before a `if` statement.
- Leave an empty line before a `_contextWorker.Commit();`.
- Keep method declarations and calls with up to 3 parameters on one line. For 4 or more parameters, place each parameter on its own line.

## Definition of Done

A task is considered complete only when ALL of the following are true:

1. All explicit user requirements are fulfilled.
2. Changes are made in the correct architectural layer.
3. The project passes the minimum relevant checks (build/tests).
4. No unrelated files or areas are touched.
5. The final response includes:
	 - a list of changed files,
	 - verification results,
	 - brief risks or limitations (if any).

## Verification of Changes

Minimum checks after making changes:

- For changes to .NET code:
	- `dotnet build UrlShortener.sln`
- For changes affecting business logic:
	- run the relevant tests from `UnitTests`.

If verification cannot be performed, the agent must explicitly state the reason in the final response.

## Final Response Format

The final response must be concise and include:

1. What was done.
2. Which files were changed.
3. How it was verified.
4. Any limitations, risks, or next steps.