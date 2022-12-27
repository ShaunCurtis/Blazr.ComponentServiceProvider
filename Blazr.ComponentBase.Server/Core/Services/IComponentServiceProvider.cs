/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Core;

public interface IComponentServiceProvider
{
    public object? GetOrCreateService(Guid componentId, Type? serviceType);

    public TService? GetOrCreateService<TService>(Guid componentId);

    public object? GetService(Guid componentId, Type serviceType);

    public TService? GetService<TService>(Guid componentId);

    public bool TryGetService<TService>(Guid componentId, [NotNullWhen(true)] out TService? value);

    public ValueTask<bool> RemoveServiceAsync<TService>(Guid componentId);

    public ValueTask<bool> RemoveServiceAsync(Guid componentId, Type serviceType);
}
