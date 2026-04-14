using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Runtime.Tools;

public sealed record ToolState(ToolDescriptor Descriptor, bool IsEnabled);
