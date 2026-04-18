using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Runtime.Tools;

/// <summary>
/// Wraps an <see cref="AIFunction"/> to emit structured log entries on every tool invocation.
/// Logs the tool name and arguments before invocation, then the result and elapsed time after.
/// </summary>
public sealed class LoggedAIFunction : AIFunction
{
    private readonly AIFunction _inner;
    private readonly ToolDescriptor _descriptor;
    private readonly ILogger _logger;

    public LoggedAIFunction(AIFunction inner, ToolDescriptor descriptor, ILogger logger)
    {
        _inner = inner;
        _descriptor = descriptor;
        _logger = logger;
    }

    public override string Name => _inner.Name;

    public override string Description => _inner.Description;

    public override JsonElement JsonSchema => _inner.JsonSchema;

    public override JsonElement? ReturnJsonSchema => _inner.ReturnJsonSchema;

    protected override async ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var argsJson = SerializeArguments(arguments);
        _logger.LogDebug("[ControlAgentNet] Tool invoked: {ToolName} — args: {Arguments}", _descriptor.Name, argsJson);

        var sw = Stopwatch.StartNew();
        var result = await _inner.InvokeAsync(arguments, cancellationToken).ConfigureAwait(false);
        sw.Stop();

        _logger.LogDebug("[ControlAgentNet] Tool result: {ToolName} — result: {Result} ({ElapsedMs} ms)",
            _descriptor.Name, SerializeResult(result), sw.ElapsedMilliseconds);

        return result;
    }

    private static string SerializeArguments(AIFunctionArguments arguments)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in arguments)
        {
            dict[kv.Key] = kv.Value;
        }

        return JsonSerializer.Serialize(dict);
    }

    private static string SerializeResult(object? result)
    {
        if (result is null)
        {
            return "null";
        }

        try
        {
            return JsonSerializer.Serialize(result);
        }
        catch
        {
            return result.ToString() ?? "null";
        }
    }
}
