using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Runtime.Tools;
using Xunit;

namespace ControlAgentNet.Runtime.Tests;

public class ToolRegistrationFactoryTests
{
    private static readonly ToolDescriptor TestDescriptor = new(
        Id: "test-tool",
        Name: "TestTool",
        Description: "A test tool",
        DefaultEnabled: true,
        Kind: "test",
        Version: "1.0.0",
        RiskLevel: CapabilityRiskLevel.Low,
        SourceAssembly: nameof(ToolRegistrationFactoryTests));

    // ── 0-arg overload ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_ZeroArg_ResolvesScopedTool()
    {
        var services = new ServiceCollection();
        services.AddScoped<TrackedTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<TrackedTool, string>(
            provider, TestDescriptor, "TestTool",
            tool => tool.ExecuteAsync());

        var result = await ((AIFunction)registration.Tool).InvokeAsync([], CancellationToken.None);
        Assert.Equal("ok", result?.ToString());
    }

    [Fact]
    public async Task Create_ZeroArg_DisposesScope()
    {
        var services = new ServiceCollection();
        services.AddScoped<AsyncDisposableTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<AsyncDisposableTool, string>(
            provider, TestDescriptor, "TestTool",
            tool => tool.ExecuteAsync());

        await ((AIFunction)registration.Tool).InvokeAsync([], CancellationToken.None);

        Assert.True(AsyncDisposableTool.WasDisposed);
    }

    // ── 0-arg + CancellationToken overload ──────────────────────────────────

    [Fact]
    public async Task Create_ZeroArgWithCt_ResolvesScopedTool()
    {
        var services = new ServiceCollection();
        services.AddScoped<TrackedTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<TrackedTool, string>(
            provider, TestDescriptor, "TestTool",
            (tool, ct) => tool.ExecuteAsync(ct));

        var result = await ((AIFunction)registration.Tool).InvokeAsync([], CancellationToken.None);
        Assert.Equal("ok", result?.ToString());
    }

    // ── 1-arg overload ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_OneArg_ResolvesScopedTool()
    {
        var services = new ServiceCollection();
        services.AddScoped<TrackedTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<TrackedTool, string, string>(
            provider, TestDescriptor, "TestTool",
            (tool, input) => tool.ExecuteAsync(input));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["argument"] = "hello" });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);
        Assert.Equal("echo:hello", result?.ToString());
    }

    // ── 1-arg + CancellationToken overload ──────────────────────────────────

    [Fact]
    public async Task Create_OneArgWithCt_ResolvesScopedTool()
    {
        var services = new ServiceCollection();
        services.AddScoped<TrackedTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<TrackedTool, string, string>(
            provider, TestDescriptor, "TestTool",
            (tool, input, ct) => tool.ExecuteAsync(input, ct));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["argument"] = "hello" });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);
        Assert.Equal("echo:hello", result?.ToString());
    }

    // ── 2-arg overload ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_TwoArg_ResolvesScopedTool()
    {
        var services = new ServiceCollection();
        services.AddScoped<TrackedTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<TrackedTool, string, int, string>(
            provider, TestDescriptor, "TestTool",
            (tool, a, b) => tool.ExecuteAsync(a, b));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["arg1"] = "x", ["arg2"] = 3 });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);
        Assert.Equal("x:3", result?.ToString());
    }

    // ── 2-arg + CancellationToken overload ──────────────────────────────────

    [Fact]
    public async Task Create_TwoArgWithCt_ResolvesScopedTool()
    {
        var services = new ServiceCollection();
        services.AddScoped<TrackedTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<TrackedTool, string, int, string>(
            provider, TestDescriptor, "TestTool",
            (tool, a, b, ct) => tool.ExecuteAsync(a, b, ct));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["arg1"] = "x", ["arg2"] = 3 });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);
        Assert.Equal("x:3", result?.ToString());
    }

    // ── Singleton still works ────────────────────────────────────────────────

    [Fact]
    public async Task Create_ZeroArg_SingletonToolStillWorks()
    {
        var services = new ServiceCollection();
        services.AddSingleton<TrackedTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<TrackedTool, string>(
            provider, TestDescriptor, "TestTool",
            tool => tool.ExecuteAsync());

        var result = await ((AIFunction)registration.Tool).InvokeAsync([], CancellationToken.None);
        Assert.Equal("ok", result?.ToString());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private sealed class TrackedTool
    {
        public Task<string> ExecuteAsync(CancellationToken ct = default) => Task.FromResult("ok");
        public Task<string> ExecuteAsync(string input, CancellationToken ct = default) => Task.FromResult($"echo:{input}");
        public Task<string> ExecuteAsync(string a, int b, CancellationToken ct = default) => Task.FromResult($"{a}:{b}");
    }

    private sealed class AsyncDisposableTool : IAsyncDisposable
    {
        public static bool WasDisposed { get; private set; }

        public AsyncDisposableTool() => WasDisposed = false;

        public Task<string> ExecuteAsync() => Task.FromResult("ok");

        public ValueTask DisposeAsync()
        {
            WasDisposed = true;
            return ValueTask.CompletedTask;
        }
    }
}
