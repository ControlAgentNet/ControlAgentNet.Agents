using Microsoft.Extensions.AI;
using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Runtime.Tools;

public interface IToolRegistration
{
    ToolDescriptor Descriptor { get; }

    AITool Tool { get; }
}
