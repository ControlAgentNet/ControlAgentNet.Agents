# Changelog

All notable changes to ControlAgentNet packages are documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/).

---

## [Unreleased]

### Changed
- `PromptInjectionDefenseMiddleware` now implements `IDisposable` and properly invalidates its compiled regex cache when `PromptInjectionDefenseOptions` are updated via `IOptionsMonitor` hot-reload. Previously the `Lazy<>` field was built once at startup and never reflected option changes.
- **Engine Return Type**: Upgraded `IAgentEngine.RunAsync` to return the rich `AgentEngineResult` record instead of a plain `string`.
- **Engine Error Handling**: `MicrosoftAgentsAIEngine` now captures and logs underlying provider exceptions explicitly.
- **Agent Hot-Reload**: Replaced static options in `MicrosoftAgentsAIEngine` with `IOptionsMonitor<AgentOptions>`. The `AIHostAgent` cache is now automatically invalidated and repopulated when options change at runtime.

### Added
- `SECURITY.md` expanded with supported versions table, response SLA, in-scope/out-of-scope definitions, and security features overview.
- `CONTRIBUTING.md` expanded with prerequisites, build/test commands, local NuGet testing, branch conventions, and PR process.
- `CODE_OF_CONDUCT.md` replaced with a full Contributor Covenant 2.1-based policy.
- `RiskDenyGuardTests` — unit tests covering risk level thresholds and exemptions.
- **Startup Validation**: Introduced `ControlAgentNetStartupValidator` (`IHostedService`) to ensure the agent engine and channels are correctly registered in the DI container before the app boots.
- **Streaming Orchestration**: Added `ProcessStreamAsync` to `IAgentOrchestrator` to support real-time token streaming through the middleware pipeline.
- **Runtime Tool Overloads**: Expanded `ToolRegistrationFactory` with additional overloads to support AI tools requiring multiple parameters.

---

## [0.1.0-alpha] — TBD

### Added
- Initial release of:
  - `ControlAgentNet.Core` — interfaces, models, descriptors
  - `ControlAgentNet.Runtime` — orchestration, middleware pipeline, DI extensions
  - `ControlAgentNet.Agents` — facade package
  - `ControlAgentNet.Providers.AzureOpenAI` — Azure AI Inference SDK integration
  - `ControlAgentNet.Channels.Console` — interactive console channel
  - `ControlAgentNet.Channels.Telegram` — Telegram long-polling channel
  - `ControlAgentNet.Tools.Greeting` — sample tool demonstrating authoring pattern
  - `ControlAgentNet.Policies` — `IToolPolicyStore` / `IChannelPolicyStore` interfaces
  - `ControlAgentNet.Policies.InMemory` — in-memory policy store
  - `ControlAgentNet.Policies.Sqlite` — SQLite-backed policy store
  - `ControlAgentNet.Guards` — `RiskDenyGuard`, `ToolAllowlistGuard`
  - `ControlAgentNet.Guards.Policies` — `PolicyEnforcementGuard`
  - `ControlAgentNet.Features.HumanInTheLoop` — human approval workflow
  - `ControlAgentNet.Features.PolicyScopes` — scoped policy helpers
  - `ControlAgentNet.Diagnostics.OpenTelemetry` — agent logging + OpenTelemetry metrics
