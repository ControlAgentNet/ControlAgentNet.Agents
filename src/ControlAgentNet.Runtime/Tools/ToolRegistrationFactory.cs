using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Runtime.Tools;

public static class ToolRegistrationFactory
{
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
                    await using var scope = rootProvider.CreateAsyncScope();
                    return await action(scope.ServiceProvider.GetRequiredService<TTool>());
                },
                functionName,
                descriptor.Description));
    }

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
                    await using var scope = rootProvider.CreateAsyncScope();
                    return await action(scope.ServiceProvider.GetRequiredService<TTool>(), cancellationToken);
                },
                functionName,
                descriptor.Description));
    }

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
                    await using var scope = rootProvider.CreateAsyncScope();
                    return await action(scope.ServiceProvider.GetRequiredService<TTool>(), argument);
                },
                functionName,
                descriptor.Description));
    }

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
                    await using var scope = rootProvider.CreateAsyncScope();
                    return await action(scope.ServiceProvider.GetRequiredService<TTool>(), argument, cancellationToken);
                },
                functionName,
                descriptor.Description));
    }

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
                    await using var scope = rootProvider.CreateAsyncScope();
                    return await action(scope.ServiceProvider.GetRequiredService<TTool>(), arg1, arg2);
                },
                functionName,
                descriptor.Description));
    }

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
                    await using var scope = rootProvider.CreateAsyncScope();
                    return await action(scope.ServiceProvider.GetRequiredService<TTool>(), arg1, arg2, cancellationToken);
                },
                functionName,
                descriptor.Description));
    }
}
