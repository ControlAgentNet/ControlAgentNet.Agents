# ControlAgentNet.Agents

<p align="center">
  <img src="https://img.shields.io/github/license/ControlAgentNet/ControlAgentNet.Agents" alt="License">
  <img src="https://img.shields.io/github/stars/ControlAgentNet/ControlAgentNet.Agents" alt="Stars">
  <img src="https://img.shields.io/github/actions/workflow/status/ControlAgentNet/ControlAgentNet.Agents/ci.yml?branch=main" alt="CI">
  <img src="https://img.shields.io/nuget/v/ControlAgentNet.Agents" alt="NuGet Version">
  <img src="https://img.shields.io/nuget/dt/ControlAgentNet.Agents" alt="NuGet Downloads">
  <img src="https://img.shields.io/badge/.NET-10-512BD4" alt=".NET 10">
  <img src="https://img.shields.io/badge/built%20on-Microsoft%20Agent%20Framework-blue" alt="MAF">
</p>

> **Modular .NET Agent Framework** - Built on Microsoft Agent Framework for enterprise-grade AI agents.

---

## Philosophy

### Zero Mandatory Dependencies

Every capability is optional. You pay only for what you use. Nothing is forced.

```csharp
// Minimal - just the agent
builder.Services.AddControlAgentAgent(config)
    .AddAzureOpenAI();

// Add what you need
builder.Services.AddControlAgentAgent(config)
    .AddAzureOpenAI()
    .AddTelegramChannel()
    .AddGreetingTools()
    .AddAgentLogging()
    .AddHumanInTheLoop();
```

### Enterprise Ready

Built on **Microsoft Agent Framework (MAF)**, not against it. This means:

- ✅ Production-tested by Microsoft
- ✅ Full support for enterprise scenarios
- ✅ Seamless integration with Azure AI services
- ✅ Security, compliance, and scalability built-in

### Modular by Design

Each capability lives in its own package. Third parties can create new:

- **Channels** (Telegram, Slack, Discord, WebSocket)
- **Providers** (OpenAI, Anthropic, Ollama)
- **Tools** (custom tool packages)
- **Policies** (custom policy stores)

---

## Quick Start

### Installation

```bash
dotnet add package ControlAgentNet.Agents
dotnet add package ControlAgentNet.Providers.AzureOpenAI
dotnet add package ControlAgentNet.Channels.Console
```

### Basic Usage

```csharp
using ControlAgentNet.Agents;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddControlAgentAgent(builder.Configuration, builder.Environment, options =>
{
    options.Id = "my-agent";
    options.Name = "My Agent";
    options.Instructions = "You are a helpful assistant.";
})
    .AddAzureOpenAI()
    .AddConsoleChannel();

var host = builder.Build();
await host.RunAsync();
```

---

## Architecture

```
+---------------------------------------------------+
| Host Application                                  |
| (ASP.NET Core, Worker)                            |
+---------------------------------------------------+
| builder.Services.AddControlAgentAgent(...)        |
|   .AddAzureOpenAI()       <- Provider             |
|   .AddConsoleChannel()    <- Channel              |
|   .AddGreetingTools()     <- Tools                |
|   .AddAgentLogging()      <- Diagnostics          |
+---------------------------------------------------+
| ControlAgentNet Packages                          |
|   +----------+  +----------+  +----------+        |
|   | Core     |  | Runtime  |  | Agents   |        |
|   +----------+  +----------+  +----------+        |
+---------------------------------------------------+
                    |
                    v
          Microsoft Agent Framework (MAF)
```

---

## Packages

| Package | Description |
|---------|-------------|
| `ControlAgentNet.Agents` | Main facade - entry point |
| `ControlAgentNet.Core` | Interfaces, models, descriptors |
| `ControlAgentNet.Runtime` | Orchestration, middleware, DI |
| `ControlAgentNet.Providers.*` | AI provider integrations |
| `ControlAgentNet.Channels.*` | Channel implementations |
| `ControlAgentNet.Tools.*` | Tool packages |
| `ControlAgentNet.Policies.*` | Policy storage |
| `ControlAgentNet.Guards.*` | Tool execution guards |
| `ControlAgentNet.Features.*` | Optional features |

---

## Key Features

### Middleware Pipeline

Request/response processing through configurable middleware:

```csharp
builder.Services.AddControlAgentAgent(config)
    .AddAgentMiddleware<CustomMiddleware>();
```

Built-in middleware:
- Exception handling
- Prompt injection defense
- Agent logging (optional)
- OpenTelemetry (optional)

### Tool Guards

Control tool execution with guards:

```csharp
builder.Services.AddControlAgentAgent(config)
    .AddRiskDenyGuard(opts => opts.MinimumDeniedRiskLevel = CapabilityRiskLevel.High)
    .AddToolAllowlistGuard(opts => opts.AllowedToolIds = ["greeting"]);
```

### Policy-Based Control

Enable/disable tools and channels at runtime:

```csharp
// Disable a tool for specific users
await policyStore.SetUserToolPolicyAsync(
    toolId: "greeting",
    agentId: "my-agent",
    userId: "user-123",
    value: PolicyValue.Disabled);
```

---

## Why Microsoft Agent Framework?

1. **Production Ready** - Backed by Microsoft
2. **Azure Integration** - Native Azure AI services support
3. **Enterprise Features** - Security, compliance, monitoring
4. **Active Development** - Continuous improvements from Microsoft
5. **Ecosystem** - Growing community and tools

---

## About This Project

This is my **professional open-source project** demonstrating:

- **Advanced .NET Architecture** - Modular library design, DI patterns
- **Microsoft Agent Framework Expertise** - Deep integration with MAF
- **Production-Ready Patterns** - Policies, guards, observability
- **Open Source Development** - Community-focused, well-documented

### Goals

1. Provide a **modular foundation** for .NET AI agents
2. Enable **enterprise scenarios** with MAF
3. Build an **ecosystem** of channels, providers, and tools
4. Demonstrate **professional-grade** .NET development

---

## Contributing

Contributions are welcome! See `CONTRIBUTING.md` for guidelines.

## Versioning

`ControlAgentNet.Agents` uses a standard open-source NuGet release flow:

- local builds: `0.1.0-dev`
- pull requests: `0.1.0-preview.<run_number>`
- pushes to `main`: `0.1.0-alpha.<run_number>`
- release tags like `v0.1.0`: exact stable package version `0.1.0`

See `VERSIONING.md` for the release process.

---

## License

MIT License - See `LICENSE` for details.

---

## Links

- [Documentation](https://github.com/ControlAgentNet/ControlAgentNet.Agents)
- [Samples](./samples/)
- [NuGet Packages](https://nuget.org/packages/ControlAgentNet.Agents)
