/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface ITimeService
{
    public string Message { get; }
    public event EventHandler? TimeChanged;

    public void UpdateTime();
}
