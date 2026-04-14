using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Runtime.Channels;

public sealed record ChannelState(ChannelDescriptor Descriptor, bool IsEnabled);
