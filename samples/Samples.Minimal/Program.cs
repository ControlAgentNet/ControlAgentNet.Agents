using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;
using ControlAgentNet.Runtime.Extensions;

using Samples.Common.Engine;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddControlAgentNet(builder.Configuration, builder.Environment);
builder.Services.AddSingleton<IAgentEngine, HelloWorldEngine>();

using var host = builder.Build();

var orchestrator = host.Services.GetRequiredService<IAgentOrchestrator>();

var message = new IncomingMessage
{
    ConversationId = "hello-world-minimal-conversation",
    UserId = "developer",
    Text = "Who are you?",
    ChannelId = "console"
};

var response = await orchestrator.ProcessAsync(message);

Console.WriteLine(response.Text);
