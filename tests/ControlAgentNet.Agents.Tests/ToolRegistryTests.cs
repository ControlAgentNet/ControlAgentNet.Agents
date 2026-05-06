using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
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
    public async Task GetEnabledTools_invokes_guards_in_order()
    {
        var executionOrder = new List<int>();
        var registry = CreateRegistry(
            guards:
            [
                new OrderedGuard(20, executionOrder),
                new OrderedGuard(5, executionOrder)
            ]);

        var tools = registry.GetEnabledTools();
        var contextProvider = new AgentContextProvider
        {
            Current = new AgentContext
            {
                Message = new IncomingMessage
                {
                    ConversationId = "conv-1",
                    UserId = "user-1",
                    Text = "run",
                    ChannelId = "console",
                    ChannelType = ChannelTransportKind.Console
                }
            }
        };

        var orderedRegistry = new ToolRegistry(
            [CreateRegistration("TestTool")],
            contextProvider,
            [new OrderedGuard(20, executionOrder), new OrderedGuard(5, executionOrder)],
            Options.Create(new AgentOptions { Id = "agent-1" }),
            NullLoggerFactory.Instance);

        var orderedTools = orderedRegistry.GetEnabledTools();
        await ((AIFunction)orderedTools[0]).InvokeAsync([], CancellationToken.None);

        Assert.Equal([5, 20], executionOrder);
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
        var registry = new ToolRegistry(registrations, provider, [], Options.Create(new AgentOptions { Id = "agent-1" }), NullLoggerFactory.Instance);

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
        return new ToolRegistry(new[] { registration }, provider, guards, Options.Create(new AgentOptions { Id = "agent-1" }), NullLoggerFactory.Instance);
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

    private sealed class OrderedGuard : IToolGuard
    {
        private readonly int _order;
        private readonly List<int> _executionOrder;

        public OrderedGuard(int order, List<int> executionOrder)
        {
            _order = order;
            _executionOrder = executionOrder;
        }

        public int Order => _order;

        public Task<ToolGuardDecision> EvaluateAsync(ToolExecutionRequest request, CancellationToken cancellationToken)
        {
            _executionOrder.Add(_order);
            return Task.FromResult(ToolGuardDecision.Allow());
        }
    }
}
