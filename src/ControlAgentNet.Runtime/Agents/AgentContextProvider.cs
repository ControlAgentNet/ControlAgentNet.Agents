using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Runtime.Agents;

public sealed class AgentContextProvider : IAgentContextProvider
{
    private readonly AsyncLocal<AgentContext?> _current = new();

    public AgentContext? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }
}
