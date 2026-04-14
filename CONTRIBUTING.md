# Contributing to ControlAgentNet.Agents

This repository contains the engine-agnostic base for ControlAgentNet agents — the Core abstractions, the Runtime pipeline, and the `ControlAgentNet.Agents` facade package.

## Principles

- Keep the base runtime **independent of any concrete AI engine** — no LLM-specific code here.
- Keep `ControlAgentNet.Core` focused on **contracts and models only** — no business logic, no dependencies.
- Keep `ControlAgentNet.Runtime` focused on **orchestration and middleware** — no channel or provider knowledge.
- Keep product-facing convenience inside `ControlAgentNet.Agents`.
- Follow the **Zero Mandatory Dependencies** philosophy — every optional capability lives in its own package.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (preview)
- Any IDE: Visual Studio 2022+, Rider, VS Code with the C# Dev Kit

---

## Build

```bash
dotnet restore ControlAgentNet.Agents.slnx
dotnet build ControlAgentNet.Agents.slnx -c Release
```

## Run Tests

```bash
# Run all tests
dotnet test ControlAgentNet.Agents.slnx

# Run tests with coverage
dotnet test ControlAgentNet.Agents.slnx --collect:"XPlat Code Coverage"

# Run a specific test project
dotnet test tests/ControlAgentNet.Agents.Tests/ControlAgentNet.Agents.Tests.csproj
```

## Local Package Testing

To test a local build against another project before publishing to NuGet:

```bash
# Pack all projects
dotnet pack ControlAgentNet.Agents.slnx -c Release -o ./artifacts/packages

# In your consuming project, add a local NuGet source
dotnet nuget add source /path/to/ControlAgentNet/release/ControlAgentNet.Agents/artifacts/packages --name ControlAgentNetLocal
dotnet add package ControlAgentNet.Agents --prerelease
```

---

## Branch Conventions

| Branch pattern    | Purpose                                      |
|-------------------|----------------------------------------------|
| `main`            | Stable, always builds and tests pass         |
| `feature/<name>`  | New features or capabilities                 |
| `fix/<name>`      | Bug fixes                                    |
| `docs/<name>`     | Documentation-only changes                   |
| `chore/<name>`    | Tooling, CI, dependency bumps                |

---

## Pull Request Process

1. **Fork** the repo and create a branch from `main`.
2. **Write tests** — new behavior must have at least one test.
3. **Run the full test suite** locally before opening a PR.
4. **Update documentation** — if you change a public API, update the relevant `README.md` and XML docs.
5. **Open a PR** against `main`. Describe what changed and why.
6. **One approve required** to merge.

> Small, focused PRs are much easier to review than large ones. Prefer multiple small PRs.

---

## Adding a New Package

If you want to contribute a new channel, provider, or feature package:

1. Create the new project under the appropriate folder (e.g., `ControlAgentNet.Channels.Discord`).
2. Reference only `ControlAgentNet.Core` and/or `ControlAgentNet.Runtime` — not external packages unless strictly necessary.
3. Add the project to the solution: `dotnet sln ControlAgentNet.Agents.slnx add <path>`.
4. Follow the **Composition Pattern** documented in the main `README.md`.
5. Include `README.md`, `SECURITY.md`, `VERSIONING.md`, `CODE_OF_CONDUCT.md`, and `CONTRIBUTING.md` in the new package root.

---

## Reporting Issues

- **Bugs**: Open a GitHub issue with reproduction steps and expected vs. actual behavior.
- **Security vulnerabilities**: Do NOT open a public issue. Follow the process in `SECURITY.md`.
- **Feature requests**: Open a GitHub issue describing the use case and the proposed API surface.
