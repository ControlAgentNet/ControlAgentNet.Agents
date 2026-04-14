using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Core.Abstractions;

public interface IAgentOrchestrator
{
    Task<OutgoingMessage> ProcessAsync(IncomingMessage message, CancellationToken cancellationToken = default);

    Task<OutgoingMessage> ProcessStreamAsync(IncomingMessage message, CancellationToken cancellationToken = default);
}
