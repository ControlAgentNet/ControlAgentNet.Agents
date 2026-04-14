# ControlAgentNet.Agents

MAF-coupled base facade package for the ControlAgentNet agent foundation.

This is the recommended package entrypoint when you want the base ControlAgentNet runtime plus the default Microsoft Agent Framework execution path.

## What it does

- references `ControlAgentNet.Core`
- references `ControlAgentNet.Runtime`
- includes the default MAF-based engine implementation
- exposes `AddControlAgentAgent(...)`
- still allows hosts to override `IAgentEngine` if needed

## Typical usage

Pair it with `ControlAgentNet.Providers.AzureOpenAI` plus optional channel and tool packages such as `ControlAgentNet.Channels.Console` and `ControlAgentNet.Tools.Greeting`.
