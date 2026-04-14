using Microsoft.Extensions.Logging.Abstractions;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Agents;
using Xunit;

namespace ControlAgentNet.Agents.Tests;

public class AgentMiddlewarePipelineTests
{
    [Fact]
    public async Task ExecuteAsync_calls_middleware_in_order()
    {
        var callOrder = new List<string>();
        var middlewares = new IAgentMiddleware[]
        {
            new RecordingMiddleware("A", callOrder),
            new RecordingMiddleware("B", callOrder),
            new RecordingMiddleware("C", callOrder)
        };

        var pipeline = new AgentMiddlewarePipeline(middlewares, NullLogger<AgentMiddlewarePipeline>.Instance);

        await pipeline.ExecuteAsync(
            CreateMessage(),
            (ctx, ct) => 
            {
                callOrder.Add("Terminal");
                return Task.FromResult(new OutgoingMessage
                {
                    ConversationId = ctx.Message.ConversationId,
                    Text = "response",
                    ChannelId = ctx.Message.ChannelId
                });
            },
            CancellationToken.None);

        Assert.Equal(4, callOrder.Count);
        Assert.Equal("A", callOrder[0]);
        Assert.Equal("B", callOrder[1]);
        Assert.Equal("C", callOrder[2]);
        Assert.Equal("Terminal", callOrder[3]);
    }

    [Fact]
    public async Task ExecuteAsync_creates_agent_context()
    {
        var pipeline = new AgentMiddlewarePipeline([], NullLogger<AgentMiddlewarePipeline>.Instance);
        AgentContext? capturedContext = null;

        await pipeline.ExecuteAsync(
            CreateMessage(),
            (ctx, ct) =>
            {
                capturedContext = ctx;
                return Task.FromResult(new OutgoingMessage
                {
                    ConversationId = ctx.Message.ConversationId,
                    Text = "response",
                    ChannelId = ctx.Message.ChannelId
                });
            },
            CancellationToken.None);

        Assert.NotNull(capturedContext);
        Assert.Equal("conv-1", capturedContext.Message.ConversationId);
    }

    [Fact]
    public async Task ExecuteAsync_passes_through_exception()
    {
        var pipeline = new AgentMiddlewarePipeline([], NullLogger<AgentMiddlewarePipeline>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(
                CreateMessage(),
                (ctx, ct) => throw new InvalidOperationException("Test error"),
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_returns_response_from_terminal()
    {
        var pipeline = new AgentMiddlewarePipeline([], NullLogger<AgentMiddlewarePipeline>.Instance);

        var response = await pipeline.ExecuteAsync(
            CreateMessage(),
            (ctx, ct) => Task.FromResult(new OutgoingMessage
            {
                ConversationId = ctx.Message.ConversationId,
                Text = "Hello World",
                ChannelId = ctx.Message.ChannelId
            }),
            CancellationToken.None);

        Assert.Equal("Hello World", response.Text);
    }

    private static IncomingMessage CreateMessage() => new()
    {
        ConversationId = "conv-1",
        UserId = "user-1",
        Text = "test message",
        ChannelId = "test"
    };

    private sealed class RecordingMiddleware : IAgentMiddleware
    {
        private readonly string _name;
        private readonly List<string> _callOrder;

        public RecordingMiddleware(string name, List<string> callOrder)
        {
            _name = name;
            _callOrder = callOrder;
        }

        public async Task<OutgoingMessage> InvokeAsync(AgentContext context, AgentDelegate next, CancellationToken cancellationToken)
        {
            _callOrder.Add(_name);
            return await next(context, cancellationToken);
        }
    }
}
