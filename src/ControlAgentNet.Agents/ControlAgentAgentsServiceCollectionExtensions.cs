using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Extensions;

namespace ControlAgentNet.Agents;

public static class ControlAgentAgentsServiceCollectionExtensions
{
    public static IControlAgentNetBuilder AddControlAgentAgent(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        bool includeAgentOrchestrator = true,
        Action<AgentOptions>? configureAgent = null)
        => services
            .AddControlAgentNet(configuration, environment, includeAgentOrchestrator, configureAgent)
            .AddMicrosoftAgentsEngine();
}
