using Microsoft.Extensions.Options;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Channels;
using ControlAgentNet.Runtime.Tools;

namespace ControlAgentNet.Runtime.Manifest;

public sealed class AgentManifestRegistry
{
    private readonly AgentOptions _agentOptions;
    private readonly ChannelRegistry _channelRegistry;
    private readonly ToolRegistry _toolRegistry;

    public AgentManifestRegistry(
        IOptions<AgentOptions> agentOptions,
        ChannelRegistry channelRegistry,
        ToolRegistry toolRegistry)
    {
        _agentOptions = agentOptions.Value;
        _channelRegistry = channelRegistry;
        _toolRegistry = toolRegistry;
    }

    public AgentManifest GetManifest()
        => new(
            _agentOptions.TenantId,
            _agentOptions.Id,
            _agentOptions.Name,
            _agentOptions.Description,
            _agentOptions.Instructions,
            _channelRegistry.GetChannelStates(),
            _toolRegistry.GetToolStates());
}
