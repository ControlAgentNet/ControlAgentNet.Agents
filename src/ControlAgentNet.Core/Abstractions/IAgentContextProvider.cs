using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Core.Abstractions;

/// <summary>
/// Allows components to access the current AgentContext during execution.
/// </summary>
public interface IAgentContextProvider
{
    AgentContext? Current { get; set; }
}
