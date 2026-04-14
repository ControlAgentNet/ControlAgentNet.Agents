namespace ControlAgentNet.Runtime.Middlewares;

/// <summary>
/// How the prompt-injection heuristic layer should behave for inbound user text.
/// </summary>
public enum PromptInjectionDefenseMode
{
    /// <summary>Heuristics are not evaluated.</summary>
    Off = 0,

    /// <summary>
    /// Suspicious patterns are logged; the agent still runs. Use to tune phrases before enabling <see cref="Block"/>.
    /// </summary>
    DetectOnly = 1,

    /// <summary>
    /// Stops the pipeline and returns <see cref="PromptInjectionDefenseOptions.BlockedResponseText"/> when heuristics match.
    /// </summary>
    Block = 2,
}

/// <summary>
/// Configuration for <see cref="PromptInjectionDefenseMiddleware"/>.
/// Binds from configuration section <see cref="SectionName"/>.
/// </summary>
public sealed class PromptInjectionDefenseOptions
{
    public const string SectionName = "ControlAgentNet:PromptInjectionDefense";

    public PromptInjectionDefenseMode Mode { get; set; } = PromptInjectionDefenseMode.Block;

    /// <summary>
    /// Response text when <see cref="Mode"/> is <see cref="PromptInjectionDefenseMode.Block"/> and a heuristic matches.
    /// </summary>
    public string BlockedResponseText { get; set; } =
        "Your message could not be processed because it appears to contain instructions aimed at changing how the assistant behaves. Please rephrase your request without system or role directives.";

    /// <summary>
    /// Optional extra case-insensitive substrings that count as suspicious when present in user text.
    /// </summary>
    public IList<string> ExtraSuspiciousPhrases { get; set; } = new List<string>();

    /// <summary>
    /// Optional regular expressions (default options: culture-invariant, case-insensitive). A match triggers the defense.
    /// Invalid patterns are logged once and skipped.
    /// </summary>
    public IList<string> SuspiciousRegexPatterns { get; set; } = new List<string>();
}
