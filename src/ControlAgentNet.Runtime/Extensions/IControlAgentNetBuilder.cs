using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ControlAgentNet.Core.Abstractions;

namespace ControlAgentNet.Runtime.Extensions;

public sealed class ControlAgentNetBuilder : IControlAgentNetBuilder
{
    public IServiceCollection Services { get; }

    public IConfiguration Configuration { get; }

    public IHostEnvironment Environment { get; }

    public ControlAgentNetBuilder(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        Services = services;
        Configuration = configuration;
        Environment = environment;
    }
}
