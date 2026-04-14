using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ControlAgentNet.Core.Abstractions;

public interface IControlAgentNetBuilder
{
    IServiceCollection Services { get; }

    IConfiguration Configuration { get; }

    IHostEnvironment Environment { get; }
}
