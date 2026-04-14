using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Agents;
using ControlAgentNet.Runtime.Channels;
using ControlAgentNet.Runtime.Manifest;
using ControlAgentNet.Runtime.Middlewares;
using ControlAgentNet.Runtime.Tools;

namespace ControlAgentNet.Runtime.Extensions;

public static class ControlAgentRuntimeServiceCollectionExtensions
{
    public static IControlAgentNetBuilder AddToolRegistration(
        this IControlAgentNetBuilder builder,
        ToolDescriptor descriptor,
        Func<IServiceProvider, IToolRegistration> registrationFactory)
    {
        builder.Services.AddSingleton(descriptor);
        builder.Services.AddSingleton(registrationFactory);
        return builder;
    }

    public static IControlAgentNetBuilder AddChannelDescriptor(
        this IControlAgentNetBuilder builder,
        ChannelDescriptor descriptor)
    {
        builder.Services.AddSingleton(descriptor);
        return builder;
    }

    public static IControlAgentNetBuilder AddControlAgentNet(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        bool includeAgentOrchestrator = true,
        Action<AgentOptions>? configureAgent = null)
    {
        services.AddSingleton(configuration);
        services.AddSingleton(environment);

        var agentOptionsBuilder = services.AddOptions<AgentOptions>()
            .Bind(configuration.GetSection(AgentOptions.SectionName));

        if (configureAgent != null)
        {
            agentOptionsBuilder.Configure(configureAgent);
        }

        var builderResult = new ControlAgentNetBuilder(services, configuration, environment);
        services.AddSingleton<IAgentContextProvider, AgentContextProvider>();
        services.AddSingleton<ToolRegistry>();
        services.AddSingleton<ChannelRegistry>();
        services.AddSingleton<AgentManifestRegistry>();

        if (includeAgentOrchestrator)
        {
            services.AddSingleton<AgentMiddlewarePipeline>();
            services.AddSingleton<IAgentOrchestrator, ControlAgentOrchestrator>();
            services.AddHostedService<ControlAgentNetStartupValidator>();
        }

        services.Configure<PromptInjectionDefenseOptions>(
            configuration.GetSection(PromptInjectionDefenseOptions.SectionName));

        // Order: first registered runs outermost. Exception handling wraps the rest.
        builderResult.AddAgentMiddleware<ExceptionHandlingMiddleware>();
        builderResult.AddAgentMiddleware<PromptInjectionDefenseMiddleware>();

        return builderResult;
    }

    /// <summary>
    /// Adds a middleware to the agent processing pipeline.
    /// </summary>
    public static IControlAgentNetBuilder AddAgentMiddleware<T>(this IControlAgentNetBuilder builder)
        where T : class, IAgentMiddleware
    {
        builder.Services.AddSingleton<IAgentMiddleware, T>();
        return builder;
    }
}
