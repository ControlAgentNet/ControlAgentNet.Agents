# ControlAgentNet.Core

Core contracts and neutral models for ControlAgentNet - the foundation every package builds upon.

---

## Purpose

`ControlAgentNet.Core` defines the **shared abstractions** that all other packages depend on. This enables:

- **Modularity** - Any package can reference Core without pulling in extra dependencies
- **Extensibility** - Third parties can build new implementations
- **Consistency** - All packages share the same types and interfaces

---

## What's in Core

### Interfaces

| Interface | Description |
|-----------|-------------|
| `IAgentOrchestrator` | Main entry point for agent processing |
| `IAgentEngine` | Cognitive execution of AI interactions |
| `IAgentMiddleware` | Pipeline component for request/response processing |
| `IToolGuard` | Tool execution policy enforcement |
| `IApprovalStore` | Human-in-the-loop approval persistence |

### Models

| Model | Description |
|-------|-------------|
| `IncomingMessage` | User input to the agent |
| `IncomingAttachment` | Channel-neutral inbound file |
| `OutgoingMessage` | Agent response |
| `OutgoingAttachment` | Channel-neutral outbound file |
| `OutgoingAction` | Channel-neutral action such as a button, link, or command |
| `AgentContext` | Runtime context passed through middleware |
| `ToolExecutionRequest` | Tool invocation context |
| `ToolGuardDecision` | Guard evaluation result |
| `PolicyValue` | Tool/channel enable/disable/approval states |
| `PolicyContext` | Scoping for policy resolution |

### Descriptors

| Descriptor | Description |
|-----------|-------------|
| `ToolDescriptor` | Tool metadata (id, name, risk level, etc.) |
| `ChannelDescriptor` | Channel metadata (id, transport, etc.) |
| `CapabilityRiskLevel` | Risk classification (Low, Medium, High, Critical) |

---

## Why These Types?

### Design Principle: Core Stays Lean

Core contains only **types that need to be shared** across packages:

```
┌─────────────────────────────────────────┐
│            ControlAgentNet.Core            │
│  (interfaces, models, descriptors)     │
└─────────────────────────────────────────┘
         ▲          ▲          ▲
         │          │          │
    Policies    Runtime     Guards
    (stores)    (orch)    (enforce)
```

**Core knows nothing about:**
- Database connections
- HTTP clients
- Specific AI providers
- Channel implementations

This keeps Core lightweight and **focused on contracts**.

---

## Extending Core

Third parties can create new implementations:

```csharp
// Create a custom engine
public sealed class MyCustomEngine : IAgentEngine
{
    public Task<AgentEngineResult> RunAsync(AgentContext context, CancellationToken ct)
    {
        // Custom AI logic
        return Task.FromResult(AgentEngineResult.FromText("Hello from my custom engine"));
    }

    public IAsyncEnumerable<string> StreamAsync(AgentContext context, CancellationToken ct)
    {
        // Streaming support
    }
}

// Register it
builder.Services.AddSingleton<IAgentEngine, MyCustomEngine>();
```

`RunAsync` returns `AgentEngineResult` so engines can surface response text plus optional metadata such as model id, finish reason, and token usage.

Channels exchange data with the runtime through `IncomingMessage` and `OutgoingMessage`. Platform-specific concepts should be normalized into `Text`, `Attachments`, `Actions`, and `Metadata` instead of exposing SDK types to agents.

---

## Versioning

Core follows [semantic versioning](https://semver.org/). Once v1.0.0 is released:

- Breaking changes = major version bump
- New features = minor version bump
- Bug fixes = patch version bump

---

## Dependencies

Core keeps dependencies limited to .NET extension abstractions used by the shared contracts:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="10.0.5" />
  <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.5" />
  <PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="10.0.5" />
</ItemGroup>
```

Core should stay limited to abstractions and neutral contracts. Provider SDKs, HTTP clients, database drivers, and channel implementations belong in other packages.

---

## Related Packages

- [ControlAgentNet.Agents](../ControlAgentNet.Agents/README.md) - Main facade
- [ControlAgentNet.Runtime](../ControlAgentNet.Runtime/README.md) - Implementation
