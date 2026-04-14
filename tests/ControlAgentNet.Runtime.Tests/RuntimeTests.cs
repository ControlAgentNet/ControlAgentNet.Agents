using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Channels;
using ControlAgentNet.Runtime.Extensions;
using ControlAgentNet.Runtime.Manifest;
using ControlAgentNet.Runtime.Tools;
using Xunit;

namespace ControlAgentNet.Runtime.Tests;

public class ChannelStateTests
{
    [Fact]
    public void ChannelState_DefaultValues_AreCorrect()
    {
        var descriptor = new ChannelDescriptor(
            Id: "channel-1",
            Name: "Channel1",
            Description: "A channel",
            DefaultEnabled: true,
            Transport: ChannelTransportKind.Chat,
            Version: "1.0.0",
            SourceAssembly: "Test",
            Category: "test");

        var state = new ChannelState(descriptor, true);

        Assert.Equal("channel-1", state.Descriptor.Id);
        Assert.True(state.IsEnabled);
    }

    [Fact]
    public void ChannelState_WithDisabled_SetsCorrectly()
    {
        var descriptor = new ChannelDescriptor(
            Id: "channel-1",
            Name: "Channel1",
            Description: "A channel",
            DefaultEnabled: false,
            Transport: ChannelTransportKind.Chat,
            Version: "1.0.0",
            SourceAssembly: "Test",
            Category: "test");

        var state = new ChannelState(descriptor, false);

        Assert.Equal("channel-1", state.Descriptor.Id);
        Assert.False(state.IsEnabled);
    }
}

public class ToolStateTests
{
    [Fact]
    public void ToolState_DefaultValues_AreCorrect()
    {
        var descriptor = new ToolDescriptor(
            Id: "tool-1",
            Name: "Tool1",
            Description: "A tool",
            DefaultEnabled: true,
            Kind: "utility",
            Version: "1.0.0",
            RiskLevel: CapabilityRiskLevel.Low,
            SourceAssembly: "Test",
            Category: "test");

        var state = new ToolState(descriptor, true);

        Assert.Equal("tool-1", state.Descriptor.Id);
        Assert.True(state.IsEnabled);
    }

    [Fact]
    public void ToolState_WithDisabled_SetsCorrectly()
    {
        var descriptor = new ToolDescriptor(
            Id: "tool-1",
            Name: "Tool1",
            Description: "A tool",
            DefaultEnabled: false,
            Kind: "utility",
            Version: "1.0.0",
            RiskLevel: CapabilityRiskLevel.Low,
            SourceAssembly: "Test",
            Category: "test");

        var state = new ToolState(descriptor, false);

        Assert.Equal("tool-1", state.Descriptor.Id);
        Assert.False(state.IsEnabled);
    }
}

public class ControlAgentRuntimeServiceCollectionExtensionsTests
{
    [Fact]
    public void AddControlAgentNet_RegistersRequiredServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Agent:Id"] = "test-agent"
            })
            .Build();

        services.AddSingleton<IAgentEngine, TestAgentEngine>();
        services.AddControlAgentNet(configuration, new TestHostEnvironment());

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ToolRegistry>());
        Assert.NotNull(provider.GetService<ChannelRegistry>());
        Assert.NotNull(provider.GetService<AgentManifestRegistry>());
        Assert.NotNull(provider.GetService<IAgentOrchestrator>());
    }

    [Fact]
    public void AddControlAgentNet_WithIncludeAgentOrchestratorFalse_DoesNotRegisterOrchestrator()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Agent:Id"] = "test-agent"
            })
            .Build();

        services.AddControlAgentNet(configuration, new TestHostEnvironment(), includeAgentOrchestrator: false);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ToolRegistry>());
        Assert.NotNull(provider.GetService<ChannelRegistry>());
        Assert.Null(provider.GetService<IAgentOrchestrator>());
    }
}

public class TestAgentEngine : IAgentEngine
{
    public Task<AgentEngineResult> RunAsync(AgentContext context, CancellationToken cancellationToken)
        => Task.FromResult(AgentEngineResult.FromText("test"));

    public async IAsyncEnumerable<string> StreamAsync(AgentContext context, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var result = await RunAsync(context, cancellationToken);
        yield return result.Text;
    }
}

public class TestHostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; } = "Test";
    public string ContentRootPath { get; set; } = "/";
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
