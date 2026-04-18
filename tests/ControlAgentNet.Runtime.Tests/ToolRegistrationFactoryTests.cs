using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;
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

    private static readonly JsonSerializerOptions CaseInsensitiveOptions =
        new() { PropertyNameCaseInsensitive = true };

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

    // ── Exception handling ────────────────────────────────────────────────────

    [Fact]
    public async Task Create_ZeroArg_ReturnsToolInvocationError_OnException()
    {
        var services = new ServiceCollection();
        services.AddScoped<ThrowingTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<ThrowingTool, string>(
            provider, TestDescriptor, "TestTool",
            tool => tool.ThrowAsync());

        var result = await ((AIFunction)registration.Tool).InvokeAsync([], CancellationToken.None);

        var element = Assert.IsType<JsonElement>(result);
        var error = element.Deserialize<ToolInvocationError>(CaseInsensitiveOptions);
        Assert.NotNull(error);
        Assert.True(error.Error);
        Assert.Equal("TOOL_EXCEPTION", error.ErrorCode);
        Assert.Equal("boom", error.Message);
        Assert.Equal(TestDescriptor.Name, error.Tool);
    }

    [Fact]
    public async Task Create_ZeroArgWithCt_ReturnsToolInvocationError_OnException()
    {
        var services = new ServiceCollection();
        services.AddScoped<ThrowingTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<ThrowingTool, string>(
            provider, TestDescriptor, "TestTool",
            (tool, ct) => tool.ThrowAsync(ct));

        var result = await ((AIFunction)registration.Tool).InvokeAsync([], CancellationToken.None);

        var element = Assert.IsType<JsonElement>(result);
        var error = element.Deserialize<ToolInvocationError>(CaseInsensitiveOptions);
        Assert.NotNull(error);
        Assert.True(error.Error);
        Assert.Equal("TOOL_EXCEPTION", error.ErrorCode);
        Assert.Equal(TestDescriptor.Name, error.Tool);
    }

    [Fact]
    public async Task Create_OneArg_ReturnsToolInvocationError_OnException()
    {
        var services = new ServiceCollection();
        services.AddScoped<ThrowingTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<ThrowingTool, string, string>(
            provider, TestDescriptor, "TestTool",
            (tool, input) => tool.ThrowAsync(input));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["argument"] = "hello" });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);

        var element = Assert.IsType<JsonElement>(result);
        var error = element.Deserialize<ToolInvocationError>(CaseInsensitiveOptions);
        Assert.NotNull(error);
        Assert.True(error.Error);
        Assert.Equal("TOOL_EXCEPTION", error.ErrorCode);
        Assert.Equal(TestDescriptor.Name, error.Tool);
    }

    [Fact]
    public async Task Create_OneArgWithCt_ReturnsToolInvocationError_OnException()
    {
        var services = new ServiceCollection();
        services.AddScoped<ThrowingTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<ThrowingTool, string, string>(
            provider, TestDescriptor, "TestTool",
            (tool, input, ct) => tool.ThrowAsync(input, ct));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["argument"] = "hello" });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);

        var element = Assert.IsType<JsonElement>(result);
        var error = element.Deserialize<ToolInvocationError>(CaseInsensitiveOptions);
        Assert.NotNull(error);
        Assert.True(error.Error);
        Assert.Equal("TOOL_EXCEPTION", error.ErrorCode);
        Assert.Equal(TestDescriptor.Name, error.Tool);
    }

    [Fact]
    public async Task Create_TwoArg_ReturnsToolInvocationError_OnException()
    {
        var services = new ServiceCollection();
        services.AddScoped<ThrowingTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<ThrowingTool, string, int, string>(
            provider, TestDescriptor, "TestTool",
            (tool, a, b) => tool.ThrowAsync(a, b));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["arg1"] = "x", ["arg2"] = 1 });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);

        var element = Assert.IsType<JsonElement>(result);
        var error = element.Deserialize<ToolInvocationError>(CaseInsensitiveOptions);
        Assert.NotNull(error);
        Assert.True(error.Error);
        Assert.Equal("TOOL_EXCEPTION", error.ErrorCode);
        Assert.Equal(TestDescriptor.Name, error.Tool);
    }

    [Fact]
    public async Task Create_TwoArgWithCt_ReturnsToolInvocationError_OnException()
    {
        var services = new ServiceCollection();
        services.AddScoped<ThrowingTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<ThrowingTool, string, int, string>(
            provider, TestDescriptor, "TestTool",
            (tool, a, b, ct) => tool.ThrowAsync(a, b, ct));

        var args = new AIFunctionArguments(new Dictionary<string, object?> { ["arg1"] = "x", ["arg2"] = 1 });
        var result = await ((AIFunction)registration.Tool).InvokeAsync(args, CancellationToken.None);

        var element = Assert.IsType<JsonElement>(result);
        var error = element.Deserialize<ToolInvocationError>(CaseInsensitiveOptions);
        Assert.NotNull(error);
        Assert.True(error.Error);
        Assert.Equal("TOOL_EXCEPTION", error.ErrorCode);
        Assert.Equal(TestDescriptor.Name, error.Tool);
    }

    [Fact]
    public async Task Create_ZeroArg_PropagatesCancellation()
    {
        var services = new ServiceCollection();
        services.AddScoped<ThrowingTool>();
        var provider = services.BuildServiceProvider();

        var registration = ToolRegistrationFactory.Create<ThrowingTool, string>(
            provider, TestDescriptor, "TestTool",
            tool => tool.ThrowCancelledAsync());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ((AIFunction)registration.Tool).InvokeAsync([], CancellationToken.None).AsTask());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private sealed class ThrowingTool
    {
        public Task<string> ThrowAsync(CancellationToken ct = default)
            => throw new InvalidOperationException("boom");

        public Task<string> ThrowAsync(string input, CancellationToken ct = default)
            => throw new InvalidOperationException($"boom:{input}");

        public Task<string> ThrowAsync(string a, int b, CancellationToken ct = default)
            => throw new InvalidOperationException($"boom:{a}:{b}");

        public Task<string> ThrowCancelledAsync(CancellationToken ct = default)
            => throw new OperationCanceledException();
    }

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
