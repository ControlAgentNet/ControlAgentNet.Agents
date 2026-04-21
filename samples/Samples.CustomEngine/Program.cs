using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Runtime.Extensions;
using ControlAgentNet.Runtime.Manifest;
using ControlAgentNet.Runtime.Tools;

using Samples.Common.Engine;
using Samples.Common.Tools;

var builder = Host.CreateApplicationBuilder(args);

var channelDescriptor = new ChannelDescriptor(
    Id: "console",
    Name: "Console",
    Description: "Console channel for testing.",
    DefaultEnabled: true,
    Transport: ChannelTransportKind.Console,
    Version: "1.0.0",
    SourceAssembly: "HelloWorld.CustomEngine",
    Category: "sample");

var greetingToolDescriptor = new ToolDescriptor(
    Id: "greeting",
    Name: "Greeting",
    Description: "Generates a personalized greeting.",
    DefaultEnabled: true,
    Kind: "utility",
    Version: "1.0.0",
    RiskLevel: CapabilityRiskLevel.Low,
    SourceAssembly: "HelloWorld.CustomEngine",
    Category: "sample");

var echoToolDescriptor = new ToolDescriptor(
    Id: "echo",
    Name: "Echo",
    Description: "Echoes back the input text.",
    DefaultEnabled: true,
    Kind: "utility",
    Version: "1.0.0",
    RiskLevel: CapabilityRiskLevel.Low,
    SourceAssembly: "HelloWorld.CustomEngine",
    Category: "sample");

builder.Services.AddSingleton<GreetingTool>();
builder.Services.AddSingleton<EchoTool>();

builder.Services.AddControlAgentNet(builder.Configuration, builder.Environment, configureAgent: options =>
{
    options.Id = "hello-world-custom-engine";
    options.Name = "Hello World Custom Engine Agent";
    options.Description = "A sample demonstrating ControlAgentNet with custom engine and tools.";
    options.Instructions = "You are ControlAgentNet. Introduce yourself and offer to help.";
})
.AddChannelDescriptor(channelDescriptor)
.AddToolRegistration(
    greetingToolDescriptor,
    sp => ToolRegistrationFactory.Create<GreetingTool, string, string>(
        sp,
        greetingToolDescriptor,
        "Greeting",
        (tool, name, ct) => tool.SayGreetingAsync(name, ct)))
.AddToolRegistration(
    echoToolDescriptor,
    sp => ToolRegistrationFactory.Create<EchoTool, string, string>(
        sp,
        echoToolDescriptor,
        "Echo",
        (tool, text, ct) => tool.EchoAsync(text, ct)));

builder.Services.AddSingleton<IAgentEngine, HelloWorldEngine>();

using var host = builder.Build();

var manifestRegistry = host.Services.GetRequiredService<AgentManifestRegistry>();
var manifest = manifestRegistry.GetManifest();

Console.WriteLine($"=== Agent Manifest ===");
Console.WriteLine($"Agent: {manifest.AgentName}");
Console.WriteLine($"Channels: {string.Join(", ", manifest.Channels.Select(c => c.Descriptor.Name))}");
Console.WriteLine($"Tools: {string.Join(", ", manifest.Tools.Select(t => t.Descriptor.Name))}");
Console.WriteLine();
