using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Core.Abstractions;

public interface IToolGuard
{
    int Order { get; }
    
    Task<ToolGuardDecision> EvaluateAsync(ToolExecutionRequest request, CancellationToken cancellationToken = default);
}
