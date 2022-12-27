/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Core;

public record ComponentService(Guid ComponentId, Type ServiceType, object ServiceInstance);

public class ComponentServiceProvider : IComponentServiceProvider, IDisposable, IAsyncDisposable
{
    private IServiceProvider _serviceProvider;
    private List<ComponentService> _componentServices = new List<ComponentService>();
    private bool asyncdisposedValue;
    private bool disposedValue;
    public readonly Guid InstanceId = Guid.NewGuid();

    public ComponentServiceProvider(IServiceProvider serviceProvider)
    {
        Debug.WriteLine($"ComponentServiceManager - instance {InstanceId} created");
        _serviceProvider = serviceProvider;
    }

    public object? GetOrCreateService(Guid componentId, Type? serviceType)
        => getOrCreateService(componentId, serviceType);

    public TService? GetOrCreateService<TService>(Guid componentId)
    {
        var service = this.getOrCreateService(componentId, typeof(TService));
        return (TService?)service;
    }

    public object? GetService(Guid componentId, Type? serviceType)
    {
        this.tryGetService(componentId, serviceType, out object? value);
        return value;
    }

    public TService? GetService<TService>(Guid componentId)
    {
        this.tryGetService(componentId, typeof(TService), out object? value);
        return (TService?)value;
    }

    public bool TryGetService<TService>(Guid componentId, [NotNullWhen(true)] out TService? value)
    {
        var result =this.tryGetService(componentId, typeof(TService), out object? service);
        value = (TService?)service;
        return result;
    }

    public ValueTask<bool> RemoveServiceAsync<TService>(Guid componentId)
        => removeServiceAsync(componentId, typeof(TService));

    public ValueTask<bool> RemoveServiceAsync(Guid componentId, Type serviceType)
        => removeServiceAsync(componentId, serviceType);

    private object? getOrCreateService(Guid componentId, Type? serviceType)
    {
        if (serviceType is null || componentId == Guid.Empty)
            return null;

        // Try getting the service from the collection
        if (this.tryFindComponentService(componentId, serviceType, out ComponentService? service))
            return service.ServiceInstance;

        // Try creating the service
        if (!this.tryCreateService(serviceType, out object? newService))
            this.tryCreateInterfaceService(serviceType, out newService);

        if (newService is null)
            return null;

        _componentServices.Add(new ComponentService(componentId, serviceType, newService));

        return newService;
    }

    private bool tryCreateService(Type serviceType, [NotNullWhen(true)] out object? service)
    {
        service = null;
        try
        {
            service = ActivatorUtilities.CreateInstance(_serviceProvider, serviceType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool tryCreateInterfaceService(Type serviceType, [NotNullWhen(true)] out object? service)
    {
        service = null;
        var concreteService = _serviceProvider.GetService(serviceType);
        if (concreteService is null)
            return false;

        var concreteInterfaceType = concreteService.GetType();

        try
        {
            service = ActivatorUtilities.CreateInstance(_serviceProvider, concreteInterfaceType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool tryGetService(Guid componentId, Type? serviceType, [NotNullWhen(true)] out object? service)
    {
        service= null;

        if (serviceType is null || componentId == Guid.Empty)
            return false;

        if (!this.tryFindComponentService(componentId, serviceType, out ComponentService? componentService))
            return false;

        service = componentService.ServiceInstance;
        return true;
    }

    private async ValueTask<bool> removeServiceAsync(Guid componentId, Type serviceType)
    {
        if (!this.tryFindComponentService(componentId, serviceType, out ComponentService? componentService))
            return false;

        if (componentService.ServiceInstance is IDisposable disposable)
            disposable.Dispose();

        if (componentService.ServiceInstance is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();

        _componentServices.Remove(componentService);

        return true;
    }

    private bool tryFindComponentService(Guid componentId, Type serviceType, [NotNullWhenAttribute(true)] out ComponentService? result)
    {
        result = _componentServices.SingleOrDefault(item => item.ComponentId == componentId && item.ServiceType == serviceType);
        if (result is default(ComponentService))
            return false;

        return true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue || !disposing)
        {
            disposedValue = true;
            return;
        }

        Debug.WriteLine($"ComponentServiceManager - instance {InstanceId} disposed");

        foreach (var componentService in _componentServices)
        {
            if (componentService.ServiceInstance is IDisposable disposable)
                disposable.Dispose();
        }

        disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (asyncdisposedValue)
            return;

        Debug.WriteLine($"ComponentServiceManager - instance {InstanceId} async disposed");

        foreach (var componentService in _componentServices)
        {
            if (componentService.ServiceInstance is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }

        asyncdisposedValue = true;
    }
}
