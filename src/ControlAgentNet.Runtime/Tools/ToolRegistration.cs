using Microsoft.Extensions.AI;
using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Runtime.Tools;

public sealed class ToolRegistration : IToolRegistration
{
    public ToolRegistration(ToolDescriptor descriptor, AITool tool)
    {
        Descriptor = descriptor;
        Tool = tool;
    }

    public ToolDescriptor Descriptor { get; }

    public AITool Tool { get; }
}
