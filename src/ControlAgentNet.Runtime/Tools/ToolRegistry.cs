using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Runtime.Agents;

namespace ControlAgentNet.Runtime.Tools;

public sealed class ToolRegistry
{
    private readonly IEnumerable<IToolRegistration> _registrations;
    private readonly IAgentContextProvider _contextProvider;
    private readonly IToolGuard[] _guards;
    private readonly ILogger _logger;

    public ToolRegistry(
        IEnumerable<IToolRegistration> registrations,
        IAgentContextProvider contextProvider,
        IEnumerable<IToolGuard> guards,
        ILoggerFactory loggerFactory)
    {
        _registrations = registrations;
        _contextProvider = contextProvider;
        _guards = guards.ToArray();
        _logger = loggerFactory.CreateLogger<ToolRegistry>();
    }

    public IReadOnlyList<AITool> GetEnabledTools()
        => _registrations.Select(WrapWithLoggingAndGuards).Select(x => x.Tool).ToList();

    public IReadOnlyList<ToolState> GetToolStates()
        => _registrations
            .Select(x => new ToolState(x.Descriptor, x.Descriptor.DefaultEnabled))
            .OrderBy(x => x.Descriptor.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

    private IToolRegistration WrapWithLoggingAndGuards(IToolRegistration registration)
    {
        if (registration.Tool is not AIFunction fn)
        {
            return registration;
        }

        AIFunction wrapped = new LoggedAIFunction(fn, registration.Descriptor, _logger);

        if (_guards.Length > 0)
        {
            wrapped = new GuardedAIFunction(wrapped, registration.Descriptor, _guards, _contextProvider);
        }

        return new ToolRegistration(registration.Descriptor, wrapped);
    }
}
