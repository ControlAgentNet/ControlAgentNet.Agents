using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Runtime.Channels;

public sealed class ChannelRegistry
{
    private readonly IEnumerable<ChannelDescriptor> _descriptors;

    public ChannelRegistry(IEnumerable<ChannelDescriptor> descriptors)
    {
        _descriptors = descriptors;
    }

    public IReadOnlyList<ChannelState> GetChannelStates()
        => _descriptors
            .Select(x => new ChannelState(x, x.DefaultEnabled))
            .OrderBy(x => x.Descriptor.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
}
