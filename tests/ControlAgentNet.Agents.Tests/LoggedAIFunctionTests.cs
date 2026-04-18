using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Runtime.Tools;
using Xunit;

namespace ControlAgentNet.Agents.Tests;

public class LoggedAIFunctionTests
{
    [Fact]
    public async Task InvokeAsync_logs_invocation_entry_and_result()
    {
        var capturingLogger = new CapturingLogger();
        var descriptor = CreateDescriptor("MyTool");
        var innerFunction = AIFunctionFactory.Create(() => "hello", "MyTool");
        var loggedFunction = new LoggedAIFunction(innerFunction, descriptor, capturingLogger);

        await loggedFunction.InvokeAsync([], cancellationToken: default);

        Assert.Equal(2, capturingLogger.Entries.Count);
        Assert.Contains("Tool invoked", capturingLogger.Entries[0].Message);
        Assert.Contains("MyTool", capturingLogger.Entries[0].Message);
        Assert.Contains("Tool result", capturingLogger.Entries[1].Message);
        Assert.Contains("MyTool", capturingLogger.Entries[1].Message);
    }

    [Fact]
    public async Task InvokeAsync_logs_debug_level()
    {
        var capturingLogger = new CapturingLogger();
        var descriptor = CreateDescriptor("MyTool");
        var innerFunction = AIFunctionFactory.Create(() => "hello", "MyTool");
        var loggedFunction = new LoggedAIFunction(innerFunction, descriptor, capturingLogger);

        await loggedFunction.InvokeAsync([], cancellationToken: default);

        Assert.All(capturingLogger.Entries, entry => Assert.Equal(LogLevel.Debug, entry.Level));
    }

    [Fact]
    public async Task InvokeAsync_includes_arguments_in_invocation_log()
    {
        var capturingLogger = new CapturingLogger();
        var descriptor = CreateDescriptor("EchoTool");
        var innerFunction = AIFunctionFactory.Create((string input) => input, "EchoTool");
        var loggedFunction = new LoggedAIFunction(innerFunction, descriptor, capturingLogger);

        var args = new AIFunctionArguments
        {
            ["input"] = "test-value"
        };

        await loggedFunction.InvokeAsync(args, cancellationToken: default);

        var invocationEntry = capturingLogger.Entries[0];
        Assert.Contains("test-value", invocationEntry.Message);
    }

    [Fact]
    public async Task InvokeAsync_includes_elapsed_time_in_result_log()
    {
        var capturingLogger = new CapturingLogger();
        var descriptor = CreateDescriptor("TimedTool");
        var innerFunction = AIFunctionFactory.Create(() => "done", "TimedTool");
        var loggedFunction = new LoggedAIFunction(innerFunction, descriptor, capturingLogger);

        await loggedFunction.InvokeAsync([], cancellationToken: default);

        var resultEntry = capturingLogger.Entries[1];
        Assert.Contains("ms", resultEntry.Message);
    }

    [Fact]
    public async Task InvokeAsync_returns_inner_function_result()
    {
        var capturingLogger = new CapturingLogger();
        var descriptor = CreateDescriptor("ValueTool");
        var innerFunction = AIFunctionFactory.Create(() => 42, "ValueTool");
        var loggedFunction = new LoggedAIFunction(innerFunction, descriptor, capturingLogger);

        var result = await loggedFunction.InvokeAsync([], cancellationToken: default);

        Assert.NotNull(result);
    }

    [Fact]
    public void LoggedAIFunction_forwards_name_description_and_schema()
    {
        var capturingLogger = new CapturingLogger();
        var descriptor = CreateDescriptor("SchemaTool");
        var innerFunction = AIFunctionFactory.Create(() => "x", "SchemaTool", "Schema tool description");
        var loggedFunction = new LoggedAIFunction(innerFunction, descriptor, capturingLogger);

        Assert.Equal(innerFunction.Name, loggedFunction.Name);
        Assert.Equal(innerFunction.Description, loggedFunction.Description);
        Assert.Equal(innerFunction.JsonSchema, loggedFunction.JsonSchema);
        Assert.Equal(innerFunction.ReturnJsonSchema, loggedFunction.ReturnJsonSchema);
    }

    private static ToolDescriptor CreateDescriptor(string name) => new(
        Id: name,
        Name: name,
        Description: $"Test tool {name}",
        DefaultEnabled: true,
        Kind: "function",
        Version: "1.0.0",
        RiskLevel: CapabilityRiskLevel.Low,
        SourceAssembly: nameof(LoggedAIFunctionTests));

    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }
}
