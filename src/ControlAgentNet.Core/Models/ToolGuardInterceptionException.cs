using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Core.Models;

/// <summary>
/// Thrown when a tool guard denies execution or requires human approval before the tool may run.
/// </summary>
public sealed class ToolGuardInterceptionException : Exception
{
    public ToolDescriptor Descriptor { get; }
    public ToolGuardDecision Decision { get; }

    public ToolGuardInterceptionException(ToolDescriptor descriptor, ToolGuardDecision decision)
        : base(decision.Reason ?? decision.Kind.ToString())
    {
        Descriptor = descriptor;
        Decision = decision;
    }
}
