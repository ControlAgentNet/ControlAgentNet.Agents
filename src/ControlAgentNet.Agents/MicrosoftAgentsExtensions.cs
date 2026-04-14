using Microsoft.Extensions.DependencyInjection;
using Microsoft.Agents.AI.Hosting;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Runtime.Extensions;

namespace ControlAgentNet.Agents;

public static class MicrosoftAgentsExtensions
{
    /// <summary>
    /// Registers the Microsoft Agents AI engine as the primary IAgentEngine.
    /// This also registers necessary Microsoft SDK services.
    /// </summary>
    public static IControlAgentNetBuilder AddMicrosoftAgentsEngine(this IControlAgentNetBuilder builder)
    {
        builder.Services.AddSingleton<IAgentEngine, MicrosoftAgentsAIEngine>();
        builder.Services.AddSingleton<AgentSessionStore, InMemoryMafAgentSessionStore>();

        return builder;
    }
}
