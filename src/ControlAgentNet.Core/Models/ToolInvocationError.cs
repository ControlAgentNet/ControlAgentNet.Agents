namespace ControlAgentNet.Core.Models;

/// <summary>
/// Structured error payload returned to the LLM when a tool invocation throws an unhandled exception.
/// </summary>
/// <remarks>
/// The LLM receives this as the tool result and can use the information to communicate
/// the failure to the user, attempt a retry with different parameters, or gracefully degrade.
/// </remarks>
/// <param name="Error">Always <c>true</c>; signals to the LLM that the tool invocation failed.</param>
/// <param name="ErrorCode">Machine-readable error code (e.g. <c>"TOOL_EXCEPTION"</c>).</param>
/// <param name="Message">Human-readable error message derived from the exception.</param>
/// <param name="Tool">The name of the tool that failed.</param>
public sealed record ToolInvocationError(
    bool Error,
    string ErrorCode,
    string Message,
    string Tool)
{
    public static ToolInvocationError FromException(Exception exception, string toolName) =>
        new(Error: true, ErrorCode: "TOOL_EXCEPTION", Message: exception.Message, Tool: toolName);
}
