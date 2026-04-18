using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;
using Xunit;
using ExecutionStatus = ControlAgentNet.Core.Models.ExecutionStatus;

namespace ControlAgentNet.Core.Tests;

public class IncomingMessageTests
{
    [Fact]
    public void IncomingMessage_SetProperties_WorksCorrectly()
    {
        var message = new IncomingMessage
        {
            ConversationId = "conv-123",
            UserId = "user-456",
            Text = "Hello",
            ChannelId = "console"
        };

        Assert.Equal("conv-123", message.ConversationId);
        Assert.Equal("user-456", message.UserId);
        Assert.Equal("Hello", message.Text);
        Assert.Equal("console", message.ChannelId);
    }

    [Fact]
    public void IncomingMessage_HasDefaultTimestamp()
    {
        var message = new IncomingMessage
        {
            ConversationId = "conv-123",
            UserId = "user-456",
            Text = "Hello",
            ChannelId = "console"
        };

        Assert.NotEqual(default, message.Timestamp);
    }
}

public class OutgoingMessageTests
{
    [Fact]
    public void OutgoingMessage_SetProperties_WorksCorrectly()
    {
        var message = new OutgoingMessage
        {
            ConversationId = "conv-123",
            Text = "Hello!",
            ChannelId = "console"
        };

        Assert.Equal("conv-123", message.ConversationId);
        Assert.Equal("Hello!", message.Text);
        Assert.Equal("console", message.ChannelId);
    }

    [Fact]
    public void OutgoingMessage_HasDefaultTimestamp()
    {
        var message = new OutgoingMessage
        {
            ConversationId = "conv-123",
            Text = "Hello!",
            ChannelId = "console"
        };

        Assert.NotEqual(default, message.Timestamp);
    }
}

public class AgentContextTests
{
    [Fact]
    public void AgentContext_DefaultValues_AreCorrect()
    {
        var message = new IncomingMessage
        {
            ConversationId = "conv-1",
            UserId = "user-1",
            Text = "test",
            ChannelId = "console"
        };
        var context = new AgentContext { Message = message };

        Assert.Equal(message, context.Message);
        Assert.Null(context.Usage);
        Assert.Equal(ExecutionStatus.Running, context.Status);
    }

    [Fact]
    public void AgentContext_SetUsage_WorksCorrectly()
    {
        var message = new IncomingMessage
        {
            ConversationId = "conv-1",
            UserId = "user-1",
            Text = "test",
            ChannelId = "console"
        };
        var context = new AgentContext { Message = message };
        context.Usage = new TokenUsage(100, 50);

        Assert.NotNull(context.Usage);
        Assert.Equal(100, context.Usage!.PromptTokens);
        Assert.Equal(50, context.Usage.CompletionTokens);
        Assert.Equal(150, context.Usage.TotalTokens);
    }
}

public class AgentOptionsTests
{
    [Fact]
    public void AgentOptions_DefaultValues_AreCorrect()
    {
        var options = new AgentOptions();

        Assert.Equal("controlagentnet-agent", options.Id);
        Assert.Equal("ControlAgentNet", options.Name);
        Assert.Equal("MAF-based modular agent", options.Description);
        Assert.Equal("You are ControlAgentNet, a helpful and direct agent.", options.Instructions);
    }

    [Fact]
    public void AgentOptions_CacheTtl_DefaultsToThirtyMinutes()
    {
        var options = new AgentOptions();

        Assert.Equal(TimeSpan.FromMinutes(30), options.CacheTtl);
    }

    [Fact]
    public void AgentOptions_SetProperties_WorksCorrectly()
    {
        var options = new AgentOptions
        {
            Id = "agent-1",
            Name = "Test Agent",
            Description = "A test agent",
            Instructions = "You are helpful."
        };

        Assert.Equal("agent-1", options.Id);
        Assert.Equal("Test Agent", options.Name);
        Assert.Equal("A test agent", options.Description);
        Assert.Equal("You are helpful.", options.Instructions);
    }

    [Fact]
    public void AgentOptions_CacheTtl_CanBeSetToCustomValue()
    {
        var expected = TimeSpan.FromMinutes(5);
        var options = new AgentOptions { CacheTtl = expected };

        Assert.Equal(expected, options.CacheTtl);
    }

    [Fact]
    public void AgentOptions_CacheTtl_ZeroDisablesTtl()
    {
        var options = new AgentOptions { CacheTtl = TimeSpan.Zero };

        Assert.Equal(TimeSpan.Zero, options.CacheTtl);
    }
}

public class ToolDescriptorTests
{
    [Fact]
    public void ToolDescriptor_DefaultValues_AreCorrect()
    {
        var descriptor = new ToolDescriptor(
            Id: "tool-1",
            Name: "Tool",
            Description: "A tool",
            DefaultEnabled: true,
            Kind: "utility",
            Version: "1.0.0",
            RiskLevel: CapabilityRiskLevel.Low,
            SourceAssembly: "Test",
            Category: "test");

        Assert.Equal("tool-1", descriptor.Id);
        Assert.Equal("Tool", descriptor.Name);
        Assert.Equal("A tool", descriptor.Description);
        Assert.True(descriptor.DefaultEnabled);
        Assert.Equal("utility", descriptor.Kind);
        Assert.Equal("1.0.0", descriptor.Version);
        Assert.Equal(CapabilityRiskLevel.Low, descriptor.RiskLevel);
        Assert.Equal("Test", descriptor.SourceAssembly);
        Assert.Equal("test", descriptor.Category);
    }
}

public class ChannelDescriptorTests
{
    [Fact]
    public void ChannelDescriptor_DefaultValues_AreCorrect()
    {
        var descriptor = new ChannelDescriptor(
            Id: "channel-1",
            Name: "Channel",
            Description: "A channel",
            DefaultEnabled: true,
            Transport: ChannelTransportKind.Chat,
            Version: "1.0.0",
            SourceAssembly: "Test",
            Category: "test");

        Assert.Equal("channel-1", descriptor.Id);
        Assert.Equal("Channel", descriptor.Name);
        Assert.Equal("A channel", descriptor.Description);
        Assert.True(descriptor.DefaultEnabled);
        Assert.Equal(ChannelTransportKind.Chat, descriptor.Transport);
        Assert.Equal("1.0.0", descriptor.Version);
        Assert.Equal("Test", descriptor.SourceAssembly);
        Assert.Equal("test", descriptor.Category);
    }
}

public class CapabilityRiskLevelTests
{
    [Fact]
    public void CapabilityRiskLevel_HasExpectedValues()
    {
        Assert.Equal(0, (int)CapabilityRiskLevel.Low);
        Assert.Equal(1, (int)CapabilityRiskLevel.Medium);
        Assert.Equal(2, (int)CapabilityRiskLevel.High);
        Assert.Equal(3, (int)CapabilityRiskLevel.Critical);
    }
}

public class ChannelTransportKindTests
{
    [Fact]
    public void ChannelTransportKind_HasExpectedValues()
    {
        Assert.Equal(0, (int)ChannelTransportKind.Console);
        Assert.Equal(1, (int)ChannelTransportKind.Chat);
        Assert.Equal(2, (int)ChannelTransportKind.Webhook);
        Assert.Equal(3, (int)ChannelTransportKind.Http);
    }
}

public class PolicyValueTests
{
    [Fact]
    public void PolicyValue_HasExpectedValues()
    {
        Assert.Equal(0, (int)PolicyValue.Inherit);
        Assert.Equal(1, (int)PolicyValue.Enabled);
        Assert.Equal(2, (int)PolicyValue.Disabled);
        Assert.Equal(3, (int)PolicyValue.ApprovalRequired);
    }
}

public class ToolGuardDecisionKindTests
{
    [Fact]
    public void ToolGuardDecisionKind_HasExpectedValues()
    {
        Assert.Equal(0, (int)ToolGuardDecisionKind.Allow);
        Assert.Equal(1, (int)ToolGuardDecisionKind.Deny);
        Assert.Equal(2, (int)ToolGuardDecisionKind.RequireApproval);
    }
}

public class ToolGuardDecisionTests
{
    [Fact]
    public void ToolGuardDecision_Allow_SetsCorrectValues()
    {
        var decision = ToolGuardDecision.Allow();

        Assert.Equal(ToolGuardDecisionKind.Allow, decision.Kind);
        Assert.Null(decision.Reason);
    }

    [Fact]
    public void ToolGuardDecision_Deny_SetsCorrectValues()
    {
        var decision = ToolGuardDecision.Deny("Too risky");

        Assert.Equal(ToolGuardDecisionKind.Deny, decision.Kind);
        Assert.Equal("Too risky", decision.Reason);
    }

    [Fact]
    public void ToolGuardDecision_RequireApproval_SetsCorrectValues()
    {
        var decision = ToolGuardDecision.RequireApproval("Needs approval", "req-123");

        Assert.Equal(ToolGuardDecisionKind.RequireApproval, decision.Kind);
        Assert.Equal("Needs approval", decision.Reason);
        Assert.Equal("req-123", decision.ApprovalRequestId);
    }
}

public class TokenUsageTests
{
    [Fact]
    public void TokenUsage_SetsValuesCorrectly()
    {
        var usage = new TokenUsage(100, 50);

        Assert.Equal(100, usage.PromptTokens);
        Assert.Equal(50, usage.CompletionTokens);
    }

    [Fact]
    public void TokenUsage_CalculatesTotalCorrectly()
    {
        var usage = new TokenUsage(100, 50);

        Assert.Equal(150, usage.TotalTokens);
    }
}
