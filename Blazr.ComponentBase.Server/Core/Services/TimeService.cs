/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics;

namespace Blazr.Core;

public class TimeService : ITimeService, IDisposable, IAsyncDisposable
{
    public readonly Guid InstanceId = Guid.NewGuid();
    private bool asyncdisposedValue;
    private bool disposedValue;

    public string Message { get; private set; } = DateTime.Now.ToLongTimeString();
    public event EventHandler? TimeChanged;

    public TimeService()
        => Debug.WriteLine($"TimeService - instance {InstanceId} created");

    public void UpdateTime()
    {
        Message = DateTime.Now.ToLongTimeString();
        TimeChanged?.Invoke(this, EventArgs.Empty);
    }

    public ValueTask DisposeAsync()
    {
        if (!asyncdisposedValue)
            Debug.WriteLine($"TimeService - instance {InstanceId} async disposed");

        asyncdisposedValue = true;
        return ValueTask.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                Debug.WriteLine($"TimeService - instance {InstanceId} disposed");

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
