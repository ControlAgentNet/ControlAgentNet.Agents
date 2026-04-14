# ControlAgentNet Samples

This directory contains sample projects demonstrating how to use ControlAgentNet.

## Samples

### Samples.HelloWorld.Minimal

A minimal example showing how to use `ControlAgentNet.Agents` with Microsoft Agents AI engine.

**Key features:**
- Uses `AddControlAgentAgent()` - built-in Microsoft Agents integration
- Requires `IMicrosoftAgentsChatClientFactory` implementation
- No custom tools or channels

**Run:**
```bash
dotnet run --project Samples.HelloWorld.Minimal
```

### Samples.HelloWorld.CustomEngine

A more complete example showing how to use ControlAgentNet with a custom engine and tools.

**Key features:**
- Uses `AddControlAgentNet()` - core runtime without Microsoft Agents
- Custom `IAgentEngine` implementation
- Registered tools (GreetingTool, EchoTool)
- Registered channels (Console)
- Prints agent manifest at startup

**Run:**
```bash
dotnet run --project Samples.HelloWorld.CustomEngine
```

## Common Components

### Samples.Common

Shared components used by samples:

- `EchoChatClientFactory` - Demo implementation of `IMicrosoftAgentsChatClientFactory`
- `HelloWorldEngine` - Sample custom engine implementation
- `GreetingTool` - Sample tool that generates greetings
- `EchoTool` - Sample tool that echoes text

## Requirements

- .NET 10.0
- ControlAgentNet packages (included as project references)
