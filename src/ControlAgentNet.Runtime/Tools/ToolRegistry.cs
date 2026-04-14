using Microsoft.Extensions.AI;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Runtime.Agents;

namespace ControlAgentNet.Runtime.Tools;

public sealed class ToolRegistry
{
    private readonly IEnumerable<IToolRegistration> _registrations;
    private readonly IAgentContextProvider _contextProvider;
    private readonly IToolGuard[] _guards;

    public ToolRegistry(
        IEnumerable<IToolRegistration> registrations,
        IAgentContextProvider contextProvider,
        IEnumerable<IToolGuard> guards)
    {
        _registrations = registrations;
        _contextProvider = contextProvider;
        _guards = guards.ToArray();
    }

    public IReadOnlyList<AITool> GetEnabledTools()
    {
        if (_guards.Length == 0)
        {
            return _registrations.Select(x => x.Tool).ToList();
        }

        return _registrations.Select(WrapIfNeeded).Select(x => x.Tool).ToList();
    }

    public IReadOnlyList<ToolState> GetToolStates()
        => _registrations
            .Select(x => new ToolState(x.Descriptor, x.Descriptor.DefaultEnabled))
            .OrderBy(x => x.Descriptor.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

    private IToolRegistration WrapIfNeeded(IToolRegistration registration)
    {
        if (registration.Tool is not AIFunction fn)
        {
            return registration;
        }

        return new ToolRegistration(
            registration.Descriptor,
            new GuardedAIFunction(fn, registration.Descriptor, _guards, _contextProvider));
    }
}
