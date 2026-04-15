# ControlAgentNet.Agents

<p align="center">
  <img src="https://img.shields.io/github/license/ControlAgentNet/ControlAgentNet.Agents" alt="License">
  <img src="https://img.shields.io/github/actions/workflow/status/ControlAgentNet/ControlAgentNet.Agents/ci.yml?branch=main" alt="CI">
  <img src="https://img.shields.io/nuget/v/ControlAgentNet.Agents" alt="NuGet Version">
</p>

> Base packages for building modular .NET agents with ControlAgentNet.

## What This Repository Contains

This repository publishes the base packages of the ecosystem:

- `ControlAgentNet.Agents` - main facade and default entry point
- `ControlAgentNet.Core` - contracts, descriptors, models, and shared abstractions
- `ControlAgentNet.Runtime` - orchestration, middleware pipeline, tools, channels, and runtime composition

Everything else in the ecosystem plugs into these base packages.

## What You Get Out Of The Box

With only the packages from this repository you already get:

- agent registration via `AddControlAgentAgent(...)`
- a host-friendly runtime for workers and ASP.NET Core apps
- middleware support
- tool/channel registration infrastructure
- tool guard infrastructure
- policy contracts support
- integration points for channels, providers, tools, guards, policies, diagnostics, and features

What you do **not** get by default:

- a real LLM provider
- external channels like Telegram
- optional tools
- persistent policy stores
- runtime enforcement guards
- diagnostics/exporters

Those capabilities are intentionally split into separate packages.

## Installation

```bash
dotnet add package ControlAgentNet.Agents
```

## Quick Start

### 1. Minimal Agent With No Extra Packages

This is the smallest useful setup: a host plus a custom engine.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ControlAgentNet.Agents;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddControlAgentAgent(builder.Configuration, builder.Environment, configureAgent: options =>
{
    options.Id = "minimal-agent";
    options.Name = "Minimal Agent";
    options.Instructions = "Respond briefly and clearly.";
});

builder.Services.AddSingleton<IAgentEngine, MinimalAgentEngine>();

using var host = builder.Build();
await host.RunAsync();

internal sealed class MinimalAgentEngine : IAgentEngine
{
    public Task<AgentEngineResult> RunAsync(AgentContext context, CancellationToken cancellationToken)
        => Task.FromResult(AgentEngineResult.FromText($"You said: {context.Message.Text}"));

    public async IAsyncEnumerable<string> StreamAsync(AgentContext context, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return (await RunAsync(context, cancellationToken)).Text;
    }
}
```

### 2. Add A Provider

```bash
dotnet add package ControlAgentNet.Providers.OpenAI
```

```csharp
using ControlAgentNet.Providers.OpenAI;

builder.Services.AddControlAgentAgent(builder.Configuration, builder.Environment, configureAgent: options =>
{
    options.Id = "openai-agent";
    options.Name = "OpenAI Agent";
    options.Instructions = "You are a helpful assistant.";
})
    .AddOpenAI();
```

### 3. Add A Channel

```bash
dotnet add package ControlAgentNet.Channels.Console
```

```csharp
using ControlAgentNet.Channels.Console;

builder.Services.AddControlAgentAgent(builder.Configuration, builder.Environment, configureAgent: options =>
{
    options.Id = "console-agent";
    options.Name = "Console Agent";
    options.Instructions = "You are a helpful assistant.";
})
    .AddOpenAI()
    .AddConsoleChannel();
```

### 4. Add A Tool

```bash
dotnet add package ControlAgentNet.Tools.Greeting
```

```csharp
using ControlAgentNet.Tools.Greeting;

builder.Services.AddControlAgentAgent(builder.Configuration, builder.Environment, configureAgent: options =>
{
    options.Id = "greeting-agent";
    options.Name = "Greeting Agent";
    options.Instructions = "Use the greeting tool when a friendly greeting is needed.";
})
    .AddOpenAI()
    .AddConsoleChannel()
    .AddGreetingTools();
```

### 5. Add Policies And Guards

```bash
dotnet add package ControlAgentNet.Policies.InMemory
dotnet add package ControlAgentNet.Guards
dotnet add package ControlAgentNet.Guards.Policies
```

```csharp
using ControlAgentNet.Policies.InMemory;
using ControlAgentNet.Guards;
using ControlAgentNet.Guards.Policies;

builder.Services.AddInMemoryPolicyStore();
builder.Services.AddPolicyEnforcementGuard();
builder.Services.AddRiskDenyGuard(options =>
{
    options.MinimumDeniedRiskLevel = CapabilityRiskLevel.High;
});
```

## Agent Capabilities By Category

### Base Packages

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Agents` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Agents) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Agents) | Facade, host integration, default entry point |
| `ControlAgentNet.Core` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Agents) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Core) | Contracts, models, descriptors |
| `ControlAgentNet.Runtime` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Agents) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Runtime) | Runtime orchestration, middleware, registry infrastructure |

### Providers

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Providers.OpenAI` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Providers.OpenAI) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Providers.OpenAI) | OpenAI chat model integration |
| `ControlAgentNet.Providers.AzureOpenAI` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Providers.AzureOpenAI) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Providers.AzureOpenAI) | Azure OpenAI chat model integration |

### Channels

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Channels.Console` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Channels.Console) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Channels.Console) | Interactive console channel |
| `ControlAgentNet.Channels.Telegram` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Channels.Telegram) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Channels.Telegram) | Telegram bot channel |

### Tools

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Tools.Greeting` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Tools.Greeting) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Tools.Greeting) | Simple greeting tool package |

### Policies

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Policies` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Policies) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Policies) | Policy contracts and scoped policy model |
| `ControlAgentNet.Policies.InMemory` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Policies.InMemory) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Policies.InMemory) | In-memory policy store |
| `ControlAgentNet.Policies.Sqlite` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Policies.Sqlite) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Policies.Sqlite) | SQLite-backed policy store |
| `ControlAgentNet.Policies.Migrations` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Policies.Migrations) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Policies.Migrations) | Database migration support for policy storage |

### Guards

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Guards` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Guards) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Guards) | Core tool guards like allowlists and risk filters |
| `ControlAgentNet.Guards.Policies` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Guards.Policies) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Guards.Policies) | Runtime policy enforcement guard |

### Features

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Features.HumanInTheLoop` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Features.HumanInTheLoop) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Features.HumanInTheLoop) | Human approval and review flows |
| `ControlAgentNet.Features.PolicyScopes` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Features.PolicyScopes) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Features.PolicyScopes) | Policy scope support utilities and integration |

### Diagnostics

| Package | Repo | NuGet | What it adds |
|---|---|---|---|
| `ControlAgentNet.Diagnostics.OpenTelemetry` | [repo](https://github.com/ControlAgentNet/ControlAgentNet.Diagnostics.OpenTelemetry) | [nuget](https://www.nuget.org/packages/ControlAgentNet.Diagnostics.OpenTelemetry) | OpenTelemetry instrumentation |

## Common Compositions

### Local Development Agent

```bash
dotnet add package ControlAgentNet.Agents
dotnet add package ControlAgentNet.Providers.OpenAI
dotnet add package ControlAgentNet.Channels.Console
dotnet add package ControlAgentNet.Tools.Greeting
```

### Telegram Assistant With Policy Enforcement

```bash
dotnet add package ControlAgentNet.Agents
dotnet add package ControlAgentNet.Providers.OpenAI
dotnet add package ControlAgentNet.Channels.Telegram
dotnet add package ControlAgentNet.Policies.Sqlite
dotnet add package ControlAgentNet.Guards.Policies
```

### High-Risk Tool Controls

```bash
dotnet add package ControlAgentNet.Agents
dotnet add package ControlAgentNet.Guards
```

Use `ControlAgentNet.Guards` when you want:

- allowlists
- risk-based blocking
- custom runtime execution checks

Use `ControlAgentNet.Guards.Policies` when you want:

- policy-driven enable/disable
- `ApprovalRequired`
- runtime enforcement backed by a policy store

## Samples In This Repository

This repository includes:

- `samples/Samples.Minimal` - smallest base-host example
- `samples/Samples.CustomEngine` - custom engine integration example
- `samples/Samples.Common` - shared sample helpers used by the solution

## Build

```bash
dotnet restore ControlAgentNet.Agents.slnx
dotnet build ControlAgentNet.Agents.slnx -c Release
dotnet test ControlAgentNet.Agents.slnx -c Release --no-build
dotnet pack ControlAgentNet.Agents.slnx -c Release -o artifacts/nuget
```

## Versioning

`ControlAgentNet.Agents` uses the standard ControlAgentNet release flow:

- local builds: `0.1.1-dev`
- pull requests: `0.1.1-preview.<run_number>`
- pushes to `main`: `0.1.1-alpha.<run_number>`
- tags like `v0.1.1`: exact stable package version `0.1.1`

See `VERSIONING.md` for the release process.

## License

MIT License - See `LICENSE` for details.
