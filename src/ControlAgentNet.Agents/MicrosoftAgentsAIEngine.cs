using System.Text;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Tools;

namespace ControlAgentNet.Agents;

public sealed class MicrosoftAgentsAIEngine : IAgentEngine, IDisposable
{
    private readonly IOptionsMonitor<AgentOptions> _agentOptionsMonitor;
    private readonly IMicrosoftAgentsChatClientFactory _chatClientFactory;
    private readonly AgentSessionStore _sessionStore;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<MicrosoftAgentsAIEngine> _logger;
    private readonly ToolRegistry? _toolRegistry;
    private readonly IDisposable? _optionsChangeListener;

    private readonly MemoryCache _agentCache = new(new MemoryCacheOptions
    {
        SizeLimit = 256,
    });

    public MicrosoftAgentsAIEngine(
        IOptionsMonitor<AgentOptions> agentOptionsMonitor,
        IMicrosoftAgentsChatClientFactory chatClientFactory,
        AgentSessionStore sessionStore,
        ILoggerFactory loggerFactory,
        ToolRegistry? toolRegistry = null)
    {
        _agentOptionsMonitor = agentOptionsMonitor;
        _chatClientFactory = chatClientFactory;
        _sessionStore = sessionStore;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<MicrosoftAgentsAIEngine>();
        _toolRegistry = toolRegistry;

        _optionsChangeListener = _agentOptionsMonitor.OnChange((options, _) =>
        {
            _logger.LogInformation("Agent options changed. Invalidating MicrosoftAgents AIHostAgent cache.");
            var cacheKey = $"microsoftagents:{options.Id}";
            _agentCache.Remove(cacheKey);
        });
    }

    public void Dispose()
    {
        _optionsChangeListener?.Dispose();
        _agentCache.Dispose();
    }

    public async Task<AgentEngineResult> RunAsync(AgentContext context, CancellationToken cancellationToken)
    {
        var hostAgent = GetOrCreateHostAgent();
        AgentSession? session = null;
        AgentResponse? response = null;

        try
        {
            session = await hostAgent.GetOrCreateSessionAsync(context.Message.ConversationId, cancellationToken);
            response = await hostAgent.RunAsync(context.Message.Text, session, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MAF Provider error during RunAsync for conversation {ConversationId}", context.Message.ConversationId);
            throw; // Rethrow to let the ExceptionHandlingMiddleware format it
        }

        if (session is not null)
        {
            await hostAgent.SaveSessionAsync(context.Message.ConversationId, session, cancellationToken);
        }

        TokenUsage? usage = null;
        if (response.Usage != null)
        {
            usage = new TokenUsage(
                ToIntTokenCount(response.Usage.InputTokenCount),
                ToIntTokenCount(response.Usage.OutputTokenCount));
            
            context.Usage = usage; // Set for legacy compatibility in context
        }

        return new AgentEngineResult
        {
            Text = response.Text ?? string.Empty,
            ModelId = null,
            FinishReason = null,
            Usage = usage
        };
    }

    public async IAsyncEnumerable<string> StreamAsync(AgentContext context, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var hostAgent = GetOrCreateHostAgent();
        var session = await hostAgent.GetOrCreateSessionAsync(context.Message.ConversationId, cancellationToken);
        var responseStream = hostAgent.RunStreamingAsync(context.Message.Text, session, cancellationToken: cancellationToken);

        await foreach (var chunk in responseStream.WithCancellation(cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(chunk.Text))
            {
                yield return chunk.Text;
            }
        }

        if (session is not null)
        {
            await hostAgent.SaveSessionAsync(context.Message.ConversationId, session, cancellationToken);
        }
    }

    private AIHostAgent GetOrCreateHostAgent()
    {
        var currentOptions = _agentOptionsMonitor.CurrentValue;
        var cacheKey = $"microsoftagents:{currentOptions.Id}";

        return _agentCache.GetOrCreate(cacheKey, entry =>
        {
            entry.Size = 1;
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

            _logger.LogInformation("Creating MicrosoftAgents AIHostAgent for {AgentId}", currentOptions.Id);

            var chatClient = _chatClientFactory.CreateChatClient();
            var tools = _toolRegistry?.GetEnabledTools().ToList() ?? [];
            var agent = chatClient.AsAIAgent(
                currentOptions.Instructions,
                currentOptions.Name,
                currentOptions.Description,
                tools,
                _loggerFactory);

            return new AIHostAgent(agent, _sessionStore);
        })!;
    }

    private static int ToIntTokenCount(long? value)
        => value is null ? 0 : (int)Math.Min(value.Value, int.MaxValue);
}
