using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ControlAgentNet.Core.Descriptors;
using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Runtime.Tools;

public static class ToolRegistrationFactory
{
    // ── 0-arg ────────────────────────────────────────────────────────────────
    public static IToolRegistration Create<TTool, TResult>(
        IServiceProvider rootProvider,
        ToolDescriptor descriptor,
        string functionName,
        Func<TTool, Task<TResult>> action)
        where TTool : class
    {
        return new ToolRegistration(
            descriptor,
            AIFunctionFactory.Create(
                async () =>
                {
                    try
                    {
                        await using var scope = rootProvider.CreateAsyncScope();
                        return (object?)(await action(scope.ServiceProvider.GetRequiredService<TTool>()));
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        return (object?)ToolInvocationError.FromException(ex, descriptor.Name);
                    }
                },
                functionName,
                descriptor.Description));
    }

    // ── 0-arg + CancellationToken ────────────────────────────────────────────
    public static IToolRegistration Create<TTool, TResult>(
        IServiceProvider rootProvider,
        ToolDescriptor descriptor,
        string functionName,
        Func<TTool, CancellationToken, Task<TResult>> action)
        where TTool : class
    {
        return new ToolRegistration(
            descriptor,
            AIFunctionFactory.Create(
                async (CancellationToken cancellationToken) =>
                {
                    try
                    {
                        await using var scope = rootProvider.CreateAsyncScope();
                        return (object?)(await action(scope.ServiceProvider.GetRequiredService<TTool>(), cancellationToken));
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        return (object?)ToolInvocationError.FromException(ex, descriptor.Name);
                    }
                },
                functionName,
                descriptor.Description));
    }

    // ── 1-arg ────────────────────────────────────────────────────────────────
    public static IToolRegistration Create<TTool, TArg, TResult>(
        IServiceProvider rootProvider,
        ToolDescriptor descriptor,
        string functionName,
        Func<TTool, TArg, Task<TResult>> action)
        where TTool : class
    {
        return new ToolRegistration(
            descriptor,
            AIFunctionFactory.Create(
                async (TArg argument) =>
                {
                    try
                    {
                        await using var scope = rootProvider.CreateAsyncScope();
                        return (object?)(await action(scope.ServiceProvider.GetRequiredService<TTool>(), argument));
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        return (object?)ToolInvocationError.FromException(ex, descriptor.Name);
                    }
                },
                functionName,
                descriptor.Description));
    }

    // ── 1-arg + CancellationToken ────────────────────────────────────────────
    public static IToolRegistration Create<TTool, TArg, TResult>(
        IServiceProvider rootProvider,
        ToolDescriptor descriptor,
        string functionName,
        Func<TTool, TArg, CancellationToken, Task<TResult>> action)
        where TTool : class
    {
        return new ToolRegistration(
            descriptor,
            AIFunctionFactory.Create(
                async (TArg argument, CancellationToken cancellationToken) =>
                {
                    try
                    {
                        await using var scope = rootProvider.CreateAsyncScope();
                        return (object?)(await action(scope.ServiceProvider.GetRequiredService<TTool>(), argument, cancellationToken));
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        return (object?)ToolInvocationError.FromException(ex, descriptor.Name);
                    }
                },
                functionName,
                descriptor.Description));
    }

    // ── 2-arg ────────────────────────────────────────────────────────────────
    public static IToolRegistration Create<TTool, TArg1, TArg2, TResult>(
        IServiceProvider rootProvider,
        ToolDescriptor descriptor,
        string functionName,
        Func<TTool, TArg1, TArg2, Task<TResult>> action)
        where TTool : class
    {
        return new ToolRegistration(
            descriptor,
            AIFunctionFactory.Create(
                async (TArg1 arg1, TArg2 arg2) =>
                {
                    try
                    {
                        await using var scope = rootProvider.CreateAsyncScope();
                        return (object?)(await action(scope.ServiceProvider.GetRequiredService<TTool>(), arg1, arg2));
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        return (object?)ToolInvocationError.FromException(ex, descriptor.Name);
                    }
                },
                functionName,
                descriptor.Description));
    }

    // ── 2-arg + CancellationToken ────────────────────────────────────────────
    public static IToolRegistration Create<TTool, TArg1, TArg2, TResult>(
        IServiceProvider rootProvider,
        ToolDescriptor descriptor,
        string functionName,
        Func<TTool, TArg1, TArg2, CancellationToken, Task<TResult>> action)
        where TTool : class
    {
        return new ToolRegistration(
            descriptor,
            AIFunctionFactory.Create(
                async (TArg1 arg1, TArg2 arg2, CancellationToken cancellationToken) =>
                {
                    try
                    {
                        await using var scope = rootProvider.CreateAsyncScope();
                        return (object?)(await action(scope.ServiceProvider.GetRequiredService<TTool>(), arg1, arg2, cancellationToken));
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        return (object?)ToolInvocationError.FromException(ex, descriptor.Name);
                    }
                },
                functionName,
                descriptor.Description));
    }
}
