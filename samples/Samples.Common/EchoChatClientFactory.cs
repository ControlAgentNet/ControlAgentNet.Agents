using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using ControlAgentNet.Agents;

namespace Samples.Common;

public sealed class EchoChatClientFactory : IMicrosoftAgentsChatClientFactory
{
    public IChatClient CreateChatClient()
    {
        return new EchoChatClient();
    }
}

public sealed class EchoChatClient : IChatClient
{
    public ChatClientMetadata Metadata { get; } = new("EchoChatClient");

    public void Dispose() { }

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var userMessage = messages.LastOrDefault()?.Contents
            .OfType<TextContent>()
            .LastOrDefault()?.Text ?? "Hello";

        await Task.Yield();

        return new ChatResponse([
            new ChatMessage(ChatRole.Assistant, $"Echo: {userMessage}")
        ]);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var userMessage = messages.LastOrDefault()?.Contents
            .OfType<TextContent>()
            .LastOrDefault()?.Text ?? "Hello";

        var response = $"Echo: {userMessage}";
        foreach (var word in response.Split(' '))
        {
            await Task.Delay(50, cancellationToken);
            yield return new ChatResponseUpdate(ChatRole.Assistant, word + " ");
        }
    }

    public TService? GetService<TService>(object? key = null) where TService : class
        => this as TService;

    public object? GetService(Type serviceType, object? serviceKey = null)
        => serviceType.IsAssignableFrom(typeof(IChatClient)) ? this : null;
}
