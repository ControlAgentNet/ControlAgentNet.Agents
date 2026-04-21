# ControlAgentNet.Runtime

Shared runtime composition, orchestration, and middleware pipeline for ControlAgentNet.

---

## Purpose

`ControlAgentNet.Runtime` implements the **execution engine** that powers every ControlAgentNet agent. It contains:

- **Orchestration** - Request/response flow management
- **Middleware Pipeline** - Pluggable request processing
- **Registries** - Tool and channel management
- **Manifest Generation** - Agent introspection

---

## Architecture

### Request Flow

```
IncomingMessage
       │
       ▼
┌──────────────────┐
│ Middleware       │  ← Exception Handling
│ Pipeline         │  ← Prompt Injection Defense
│                  │  ← Logging
│                  │  ← Custom Middleware
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│ IAgentEngine    │  ← Microsoft Agents (default)
│ (MAF)            │
└────────┬─────────┘
         │
         ▼
    OutgoingMessage
```

### Middleware Pipeline

Middleware runs in **registration order** (first registered = outermost):

```csharp
builder.Services.AddControlAgentAgent(config)
    .AddAgentMiddleware<LoggingMiddleware>()      // Runs first
    .AddAgentMiddleware<CustomMiddleware>()        // Runs second
    .AddAgentMiddleware<MetricsMiddleware>();     // Runs third
```

Each middleware can:
- **Inspect** the request before passing to next
- **Modify** the context
- **Short-circuit** by returning early
- **Handle** exceptions

---

## Key Components

### ControlAgentOrchestrator

The main entry point - orchestrates the entire flow:

```csharp
public sealed class ControlAgentOrchestrator : IAgentOrchestrator
{
    public async Task<OutgoingMessage> ProcessAsync(
        IncomingMessage message,
        CancellationToken cancellationToken)
    {
        // 1. Create context
        // 2. Run middleware pipeline
        // 3. Execute engine
        // 4. Return response
    }
}
```

### AgentMiddlewarePipeline

Builds and executes the middleware chain:

```csharp
public sealed class AgentMiddlewarePipeline
{
    public async Task<OutgoingMessage> ExecuteAsync(
        IncomingMessage message,
        Func<AgentContext, CancellationToken, Task<OutgoingMessage>> terminal,
        CancellationToken cancellationToken)
    {
        // Chain middleware: A → B → C → terminal
    }
}
```

### Registries

**ToolRegistry** - Manages tool registration and guard wrapping:

```csharp
// Tools are automatically wrapped with guards
var tools = toolRegistry.GetEnabledTools();
```

**ChannelRegistry** - Manages channel descriptors:

```csharp
var channels = channelRegistry.GetChannelStates();
```

### AgentManifest

Introspection endpoint for tooling:

```csharp
var manifest = manifestRegistry.GetManifest();
// {
//   AgentId: "my-agent",
//   Tools: [...],
//   Channels: [...]
// }
```

### ToolRegistrationFactory

Creates tool invocations that are automatically wrapped with **structured error handling**. When a tool throws an unhandled exception the invocation does **not** throw — instead it returns a `ToolInvocationError` payload that the LLM receives as the tool result:

```json
{
  "error": true,
  "errorCode": "TOOL_EXCEPTION",
  "message": "CalDAV request timed out after 30 s",
  "tool": "QueryCalendarEvents"
}
```

The `error` and `errorCode` fields can be referenced in system prompts so the LLM knows how to communicate failures to the user:

> If a tool result contains `"error": true`, tell the user that the action could not be completed and include the `message` in your reply.

`OperationCanceledException` is **not** converted — it propagates normally so that cancellation is always respected.

---

## Built-in Middleware

### ExceptionHandlingMiddleware

Catches all unhandled exceptions and returns a **safe error message**:

```csharp
// Default: "An internal error occurred..."
// Configurable via AgentOptions.ErrorMessage
```

### PromptInjectionDefenseMiddleware

Lightweight heuristic defense against prompt injection attacks:

```csharp
// Blocks: "ignore previous instructions", "you are now a...", etc.
// Configurable via PromptInjectionDefenseOptions
```

---

## Extending Runtime

### Custom Middleware

```csharp
public sealed class MyMiddleware : IAgentMiddleware
{
    public async Task<OutgoingMessage> InvokeAsync(
        AgentContext context,
        AgentDelegate next,
        CancellationToken cancellationToken)
    {
        // Before
        Console.WriteLine($"Processing: {context.Message.Text}");

        var response = await next(context, cancellationToken);

        // After
        Console.WriteLine($"Response: {response.Text}");

        return response;
    }
}

// Register
builder.Services.AddControlAgentAgent(config)
    .AddAgentMiddleware<MyMiddleware>();
```

---

## Dependencies

`Runtime` depends on:

- `ControlAgentNet.Core` - Interfaces and models

This keeps Runtime **focused on orchestration** without coupling to specific implementations.

---

## Related Packages

- [ControlAgentNet.Agents](../ControlAgentNet.Agents/README.md) - Main facade
- [ControlAgentNet.Core](../ControlAgentNet.Core/README.md) - Core interfaces
