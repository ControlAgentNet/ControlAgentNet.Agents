# HelloWorld.CustomEngine

This sample is not meant to be a productized engine package.

Its purpose is to demonstrate that even though the base `ControlAgentNet.Agents` package now defaults to the Microsoft Agents stack, a host can still override `IAgentEngine` with a custom local implementation.

It also prints the generated agent manifest so you can see the sample channel and greeting capability registered through the minimal runtime registries.

## Run

```bash
dotnet run --project samples/HelloWorld.CustomEngine
```
