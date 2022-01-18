using System.Threading;

public class CancellationTokenContainer
{
    CancellationTokenSource _cancellationToken;

    public CancellationTokenContainer()
    {
        _cancellationToken = new CancellationTokenSource();
    }

    public CancellationToken CancellationToken => _cancellationToken.Token;

    ~CancellationTokenContainer()
    {
        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
        _cancellationToken = null;
    }

    public void Reset()
    {
        _cancellationToken.Dispose();
        _cancellationToken = new CancellationTokenSource();
    }

    public void Cancel()
    {
        _cancellationToken.Cancel();
        Reset();
    }
}