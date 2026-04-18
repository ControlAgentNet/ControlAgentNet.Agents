using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Runtime.Agents;
using ControlAgentNet.Runtime.Channels;
using ControlAgentNet.Runtime.Extensions;
using ControlAgentNet.Runtime.Tools;
using Xunit;

namespace ControlAgentNet.Agents.Tests;

public class ControlAgentNetStartupValidatorTests
{
    [Fact]
    public async Task StartAsync_logs_warning_when_no_tools_registered()
    {
        var (validator, logger) = CreateValidator(toolRegistrations: []);

        await validator.StartAsync(CancellationToken.None);

        Assert.Contains(logger.Entries, e =>
            e.Level == LogLevel.Warning &&
            e.Message.Contains("No ControlAgentNet tools have been registered"));
    }

    [Fact]
    public async Task StartAsync_logs_tool_count_and_names_when_tools_registered()
    {
        var registrations = new[]
        {
            CreateToolRegistration("ListCalendars"),
            CreateToolRegistration("QueryEvents"),
            CreateToolRegistration("CreateEvent")
        };

        var (validator, logger) = CreateValidator(toolRegistrations: registrations);

        await validator.StartAsync(CancellationToken.None);

        var infoEntries = logger.Entries
            .Where(e => e.Level == LogLevel.Information)
            .ToList();

        Assert.Contains(infoEntries, e =>
            e.Message.Contains("Tools registered") &&
            e.Message.Contains("3") &&
            e.Message.Contains("ListCalendars") &&
            e.Message.Contains("QueryEvents") &&
            e.Message.Contains("CreateEvent"));
    }

    [Fact]
    public async Task StartAsync_logs_single_tool_correctly()
    {
        var registrations = new[] { CreateToolRegistration("MyTool") };

        var (validator, logger) = CreateValidator(toolRegistrations: registrations);

        await validator.StartAsync(CancellationToken.None);

        Assert.Contains(logger.Entries, e =>
            e.Level == LogLevel.Information &&
            e.Message.Contains("Tools registered") &&
            e.Message.Contains("1") &&
            e.Message.Contains("MyTool"));
    }

    private static (ControlAgentNetStartupValidator Validator, CapturingLogger<ControlAgentNetStartupValidator> Logger) CreateValidator(
        IToolRegistration[] toolRegistrations)
    {
        var engine = new Mock<IAgentEngine>().Object;
        var channelRegistry = new ChannelRegistry([]);
        var contextProvider = new AgentContextProvider();
        var toolRegistry = new ToolRegistry(toolRegistrations, contextProvider, [], Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var logger = new CapturingLogger<ControlAgentNetStartupValidator>();

        var validator = new ControlAgentNetStartupValidator(engine, channelRegistry, toolRegistry, logger);
        return (validator, logger);
    }

    private static IToolRegistration CreateToolRegistration(string name)
    {
        var descriptor = new ToolDescriptor(
            Id: name,
            Name: name,
            Description: $"Test tool {name}",
            DefaultEnabled: true,
            Kind: "function",
            Version: "1.0.0",
            RiskLevel: CapabilityRiskLevel.Low,
            SourceAssembly: nameof(ControlAgentNetStartupValidatorTests));

        var aiFunction = AIFunctionFactory.Create(() => "test", name);
        return new ToolRegistration(descriptor, aiFunction);
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }
}
