using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Agents;
using ControlAgentNet.Runtime.Tools;
using Xunit;

namespace ControlAgentNet.Agents.Tests;

public class ToolRegistryTests
{
    [Fact]
    public void GetEnabledTools_returns_tools_when_no_guards()
    {
        var registry = CreateRegistry(guards: []);

        var tools = registry.GetEnabledTools();

        Assert.Single(tools);
        Assert.Equal("TestTool", tools[0].Name);
    }

    [Fact]
    public void GetEnabledTools_wraps_tools_with_guards_when_guards_present()
    {
        var guards = new[] { new TestGuard() };
        var registry = CreateRegistry(guards: guards);

        var tools = registry.GetEnabledTools();

        Assert.Single(tools);
    }

    [Fact]
    public void GetToolStates_returns_all_registered_tools()
    {
        var registry = CreateRegistry(guards: []);

        var states = registry.GetToolStates();

        Assert.Single(states);
        Assert.Equal("TestTool", states[0].Descriptor.Name);
        Assert.True(states[0].IsEnabled);
    }

    [Fact]
    public void GetToolStates_returns_tools_sorted_by_name()
    {
        var registrations = new List<IToolRegistration>
        {
            CreateRegistration("Zebra"),
            CreateRegistration("Apple"),
            CreateRegistration("Mango")
        };

        var provider = new AgentContextProvider();
        var registry = new ToolRegistry(registrations, provider, []);

        var states = registry.GetToolStates();

        Assert.Equal(3, states.Count);
        Assert.Equal("Apple", states[0].Descriptor.Name);
        Assert.Equal("Mango", states[1].Descriptor.Name);
        Assert.Equal("Zebra", states[2].Descriptor.Name);
    }

    private static ToolRegistry CreateRegistry(IToolGuard[] guards)
    {
        var registration = CreateRegistration("TestTool");
        var provider = new AgentContextProvider();
        return new ToolRegistry(new[] { registration }, provider, guards);
    }

    private static IToolRegistration CreateRegistration(string name)
    {
        var descriptor = new ToolDescriptor(
            Id: name,
            Name: name,
            Description: $"Test tool {name}",
            DefaultEnabled: true,
            Kind: "function",
            Version: "1.0.0",
            RiskLevel: CapabilityRiskLevel.Low,
            SourceAssembly: nameof(ToolRegistryTests));

        var aiFunction = AIFunctionFactory.Create(() => "test", name);
        return new ToolRegistration(descriptor, aiFunction);
    }

    private sealed class TestGuard : IToolGuard
    {
        public int Order => 0;

        public Task<ToolGuardDecision> EvaluateAsync(ToolExecutionRequest request, CancellationToken cancellationToken)
            => Task.FromResult(ToolGuardDecision.Allow());
    }
}
