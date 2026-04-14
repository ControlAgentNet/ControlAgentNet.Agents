using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Moq;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Agents;
using ControlAgentNet.Runtime.Channels;
using ControlAgentNet.Runtime.Extensions;
using ControlAgentNet.Runtime.Manifest;
using ControlAgentNet.Runtime.Tools;
using Xunit;

namespace ControlAgentNet.Agents.Tests;

public class RuntimeCompositionTests
{
    [Fact]
    public void AddControlAgentNet_registers_required_services_without_engine()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IAgentEngine, TestAgentEngine>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Agent:Id"] = "test-agent",
                ["Agent:Name"] = "Test Agent",
                ["Agent:Description"] = "Test description",
                ["Agent:Instructions"] = "Be helpful"
            })
            .Build();

        services.AddControlAgentNet(configuration, new TestHostEnvironment());

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IAgentOrchestrator>());
        Assert.NotNull(provider.GetRequiredService<AgentMiddlewarePipeline>());
        Assert.NotNull(provider.GetRequiredService<ToolRegistry>());
        Assert.NotNull(provider.GetRequiredService<ChannelRegistry>());
        Assert.NotNull(provider.GetRequiredService<AgentManifestRegistry>());
    }

    [Fact]
    public void AddControlAgentAgent_registers_microsoft_agents_engine()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Agent:Id"] = "test-agent",
                ["Agent:Name"] = "Test Agent",
                ["Agent:Description"] = "Test description",
                ["Agent:Instructions"] = "Be helpful"
            })
            .Build();

        services.AddSingleton<IMicrosoftAgentsChatClientFactory, TestChatClientFactory>();

        services.AddControlAgentAgent(configuration, new TestHostEnvironment());

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IAgentEngine>());
    }

    [Fact]
    public void AddAgentMiddleware_adds_middleware_to_pipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IAgentEngine, TestAgentEngine>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Agent:Id"] = "test-agent"
            })
            .Build();

        services.AddControlAgentNet(configuration, new TestHostEnvironment())
            .AddAgentMiddleware<TestMiddleware>();

        using var provider = services.BuildServiceProvider();
        var middlewares = provider.GetServices<IAgentMiddleware>().ToList();

        Assert.Contains(middlewares, m => m is TestMiddleware);
    }

    private sealed class TestMiddleware : IAgentMiddleware
    {
        public Task<OutgoingMessage> InvokeAsync(AgentContext context, AgentDelegate next, CancellationToken cancellationToken)
            => next(context, cancellationToken);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = "/";
        public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider("/");
    }

    private sealed class TestAgentEngine : IAgentEngine
    {
        public Task<AgentEngineResult> RunAsync(AgentContext context, CancellationToken cancellationToken)
            => Task.FromResult(AgentEngineResult.FromText("test response"));

        public IAsyncEnumerable<string> StreamAsync(AgentContext context, CancellationToken cancellationToken)
            => AsyncEnumerable.Empty<string>();
    }

    private sealed class TestChatClientFactory : IMicrosoftAgentsChatClientFactory
    {
        public IChatClient CreateChatClient()
        {
            var mock = new Mock<IChatClient>();
            return mock.Object;
        }
    }
}
