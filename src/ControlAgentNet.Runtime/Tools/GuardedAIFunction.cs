using System.Text.Json;
using Microsoft.Extensions.AI;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Agents;

namespace ControlAgentNet.Runtime.Tools;

/// <summary>
/// Wraps an <see cref="AIFunction"/> so registered <see cref="IToolGuard"/> instances run before invocation.
/// </summary>
public sealed class GuardedAIFunction : AIFunction
{
    private readonly AIFunction _inner;
    private readonly ToolDescriptor _descriptor;
    private readonly IToolGuard[] _guards;
    private readonly IAgentContextProvider _contextProvider;

    public GuardedAIFunction(
        AIFunction inner,
        ToolDescriptor descriptor,
        IEnumerable<IToolGuard> guards,
        IAgentContextProvider contextProvider)
    {
        _inner = inner;
        _descriptor = descriptor;
        _guards = guards.ToArray();
        _contextProvider = contextProvider;
    }

    public override string Name => _inner.Name;

    public override string Description => _inner.Description;

    public override JsonElement JsonSchema => _inner.JsonSchema;

    public override JsonElement? ReturnJsonSchema => _inner.ReturnJsonSchema;

    protected override async ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        if (_guards.Length == 0)
        {
            return await _inner.InvokeAsync(arguments, cancellationToken).ConfigureAwait(false);
        }

        var ctx = _contextProvider.Current;
        var msg = ctx?.Message;
        var userId = msg?.UserId ?? string.Empty;
        var convId = msg?.ConversationId ?? string.Empty;
        var parameters = ToParameterDictionary(arguments);

        var request = new ToolExecutionRequest(
            _descriptor.Id,
            userId,
            convId,
            ToolInstance: _descriptor,
            Parameters: parameters,
            Descriptor: _descriptor);

        foreach (var guard in _guards)
        {
            var decision = await guard.EvaluateAsync(request, cancellationToken).ConfigureAwait(false);
            switch (decision.Kind)
            {
                case ToolGuardDecisionKind.Deny:
                case ToolGuardDecisionKind.RequireApproval:
                    throw new ToolGuardInterceptionException(_descriptor, decision);
                case ToolGuardDecisionKind.Allow:
                    break;
                default:
                    throw new ToolGuardInterceptionException(_descriptor, decision);
            }
        }

        return await _inner.InvokeAsync(arguments, cancellationToken).ConfigureAwait(false);
    }

    private static Dictionary<string, object?> ToParameterDictionary(AIFunctionArguments arguments)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in arguments)
        {
            dict[kv.Key] = kv.Value;
        }

        return dict;
    }
}
