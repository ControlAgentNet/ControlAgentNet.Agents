using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Agents;
using Xunit;

namespace ControlAgentNet.Agents.Tests;

public class ControlAgentOrchestratorTests
{
    [Fact]
    public async Task ProcessAsync_returns_response_from_engine()
    {
        var orchestrator = CreateOrchestrator(engineResponse: "Hello from agent");

        var response = await orchestrator.ProcessAsync(
            new IncomingMessage
            {
                ConversationId = "conv-1",
                UserId = "user-1",
                Text = "Hello",
                ChannelId = "test"
            },
            CancellationToken.None);

        Assert.Equal("Hello from agent", response.Text);
    }

    [Fact]
    public async Task ProcessAsync_returns_fallback_when_engine_returns_empty()
    {
        var orchestrator = CreateOrchestrator(engineResponse: "");

        var response = await orchestrator.ProcessAsync(
            new IncomingMessage
            {
                ConversationId = "conv-1",
                UserId = "user-1",
                Text = "Hello",
                ChannelId = "test"
            },
            CancellationToken.None);

        Assert.Equal("I was unable to generate a response.", response.Text);
    }

    [Fact]
    public async Task ProcessAsync_propagates_exception()
    {
        var engine = new Mock<IAgentEngine>();
        engine.Setup(e => e.RunAsync(It.IsAny<AgentContext>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Engine error"));

        var orchestrator = CreateOrchestrator(engine: engine.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ProcessAsync(
                new IncomingMessage
                {
                    ConversationId = "conv-1",
                    UserId = "user-1",
                    Text = "Hello",
                    ChannelId = "test"
                },
                CancellationToken.None));
    }

    [Fact]
    public async Task ProcessAsync_captures_token_usage()
    {
        var engine = new Mock<IAgentEngine>();
        engine.Setup(e => e.RunAsync(It.IsAny<AgentContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AgentEngineResult.FromText("response"));

        var orchestrator = CreateOrchestrator(engine: engine.Object);

        await orchestrator.ProcessAsync(
            new IncomingMessage
            {
                ConversationId = "conv-1",
                UserId = "user-1",
                Text = "Hello",
                ChannelId = "test"
            },
            CancellationToken.None);

        engine.Verify(e => e.RunAsync(It.IsAny<AgentContext>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ControlAgentOrchestrator CreateOrchestrator(
        IAgentEngine? engine = null,
        string engineResponse = "Agent response")
    {
        var actualEngine = engine ?? CreateMockEngine(engineResponse);

        return new ControlAgentOrchestrator(
            actualEngine,
            new AgentMiddlewarePipeline([], NullLogger<AgentMiddlewarePipeline>.Instance),
            new AgentContextProvider(),
            NullLogger<ControlAgentOrchestrator>.Instance);
    }

    private static IAgentEngine CreateMockEngine(string response)
    {
        var mock = new Mock<IAgentEngine>(MockBehavior.Strict);
        mock.Setup(e => e.RunAsync(It.IsAny<AgentContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AgentEngineResult.FromText(response));
        return mock.Object;
    }
}
