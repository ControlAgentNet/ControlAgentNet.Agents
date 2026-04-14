using Microsoft.Extensions.AI;

namespace ControlAgentNet.Agents;

/// <summary>
/// Creates the chat client used by the Microsoft Agents engine.
/// Concrete provider packages implement this abstraction.
/// </summary>
public interface IMicrosoftAgentsChatClientFactory
{
    IChatClient CreateChatClient();
}
