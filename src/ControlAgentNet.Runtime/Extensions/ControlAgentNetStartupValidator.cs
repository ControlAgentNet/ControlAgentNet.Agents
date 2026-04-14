using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Runtime.Channels;

namespace ControlAgentNet.Runtime.Extensions;

/// <summary>
/// A hosted service that runs immediately upon application startup to validate 
/// that the ControlAgentNet DI container is correctly configured.
/// </summary>
public sealed class ControlAgentNetStartupValidator : IHostedService
{
    private readonly IAgentEngine _engine;
    private readonly ChannelRegistry _channelRegistry;
    private readonly ILogger<ControlAgentNetStartupValidator> _logger;

    public ControlAgentNetStartupValidator(
        IAgentEngine engine,
        ChannelRegistry channelRegistry,
        ILogger<ControlAgentNetStartupValidator> logger)
    {
        // If IAgentEngine is missing, the DI container will throw an exception during activation,
        // which serves our purpose of fast-failing during startup.
        _engine = engine;
        _channelRegistry = channelRegistry;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating ControlAgentNet configuration...");

        var channels = _channelRegistry.GetChannelStates();
        if (channels.Count == 0)
        {
            _logger.LogWarning("No ControlAgentNet channels have been registered. The agent will not be able to receive messages.");
        }
        else
        {
            var activeChannels = channels.Where(x => x.IsEnabled).Select(x => x.Descriptor.Name).ToList();
            if (activeChannels.Count == 0)
            {
                _logger.LogWarning("Channels are registered, but none are active by default. Ensure they can be enabled at runtime.");
            }
            else
            {
                _logger.LogInformation("ControlAgentNet is configured with {EngineType} and {ChannelCount} active channels: {Channels}", 
                    _engine.GetType().Name, activeChannels.Count, string.Join(", ", activeChannels));
            }
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
