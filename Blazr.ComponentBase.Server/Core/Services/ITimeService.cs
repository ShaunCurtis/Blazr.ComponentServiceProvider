using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Blazr.Core;

public interface ITimeService
{
    public string Message { get;}
    public event EventHandler? TimeChanged;

    public void UpdateTime();
}
