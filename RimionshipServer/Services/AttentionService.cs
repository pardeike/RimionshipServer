using System.Buffers;
using System.Runtime.CompilerServices;
namespace RimionshipServer.Services;

public class AttentionService : IAsyncDisposable, IDisposable
{
    private long _cooldownMs = 1000;

    public int CooldownMs
    {
        get
        {
            return (int) Interlocked.Read(ref _cooldownMs);
        }
        set
        {
            Interlocked.Exchange(ref _cooldownMs, value);
        }
    }

    // ReSharper disable once RedundantDefaultMemberInitializer
    private          ulong  _currentIndex    = 0;
    private          long[] _attentionValues = Array.Empty<long>();
    private readonly object _syncLock        = new ();

    private readonly Dictionary<string, int> _users             = new ();
    private readonly ReaderWriterLockSlim    _userReferenceLock = new ();
    
    private readonly Task                    _decrementalTask;
    private          long                    _shouldStop;
    
    private          bool                    _disposed;
    
    public AttentionService()
    {
        Interlocked.Exchange(ref _shouldStop, 0L);
        _decrementalTask = Task.Run(async () =>
                                    {
                                        while (Interlocked.Read(ref _shouldStop) == 0L)
                                        {
                                            int count;
                                            count = (int) Interlocked.Read(ref _currentIndex);
                                            for (int i = 0; i < count; i++)
                                            {
                                                if (Interlocked.Read(ref _attentionValues[i]) > 0L)
                                                    Interlocked.Decrement(ref _attentionValues[i]);
                                            }
                                            await Task.Delay((int) Interlocked.Read(ref _cooldownMs));
                                        }
                                    }
                                   );
    }

    public long GetAttentionScore(string user)
    {
        var index = GetUserReference(user, out var needCreation);
        if (needCreation)
            return -1L;
      
        return Interlocked.Read(ref _attentionValues[index]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private int GetUserReference(string user, out bool needCreation)
    {
        int index;
        try
        {
            _userReferenceLock.EnterReadLock();
            needCreation = !_users.TryGetValue(user, out index);
        }
        finally
        {
            if (_userReferenceLock.IsReadLockHeld)
                _userReferenceLock.ExitReadLock();
        }
        return index;
    }

    public void IncreaseAttentionScore(string user, long delta)
    {
        var index = GetUserReference(user, out var needCreation);
        
        if (needCreation)
            lock (_syncLock)
            {
                ExpandUserAttentionArray();
                AddUserReference(user, out index);
            }
        
        Interlocked.Add(ref _attentionValues[index], delta);
    }
    
    private void ExpandUserAttentionArray()
    {
        var length     = (int) Interlocked.Read(ref _currentIndex);
        if (_attentionValues.Length > length)
            return;
        var needReturn = Environment.Is64BitProcess ? length > 512 : length > 128;
        //Can stackalloc up to              4kB on 64bit | 1kB on 32Bit
        //Maximum Values for stackalloc are 4MB on 64bit | 1MB on 32Bit
        if (needReturn)
        {
            var toReturn = ArrayPool<long>.Shared.Rent(length + 2);
            try
            {
                var tmp = toReturn.AsSpan(0, length); //Length is needed since ArrayPool can return larger Arrays.
                CopyTempArray(in tmp, length);
            }
            finally
            {
                ArrayPool<long>.Shared.Return(toReturn);
            }
        }
        else
        {
            Span<long> tmp = stackalloc long[length];
            CopyTempArray(in tmp, length);
        }
    }
    
    private void CopyTempArray(in Span<long> tmp, int length)
    {
        _attentionValues.AsSpan(0, length).CopyTo(tmp);
        ArrayPool<long>.Shared.Return(_attentionValues);
        _attentionValues = ArrayPool<long>.Shared.Rent(length + 1);
        tmp.CopyTo(_attentionValues);
    }

    private void AddUserReference(string user, out int index)
    {
        try
        {
            _userReferenceLock.EnterWriteLock();
            index        = (int) Interlocked.Read(ref _currentIndex);
            _users[user] = index;
            Interlocked.Increment(ref _currentIndex);
        }
        finally
        {
            if (_userReferenceLock.IsWriteLockHeld)
                _userReferenceLock.ExitWriteLock();
        }
    }

    ~AttentionService()
    {
        DisposeAsync(false).Wait();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async Task DisposeAsync(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Interlocked.Increment(ref _shouldStop);
            _userReferenceLock.Dispose();
            await _decrementalTask;
            _decrementalTask.Dispose();
            ArrayPool<long>.Shared.Return(_attentionValues);
        }
        
        _disposed = true;
    }

    public void Dispose()
    {
        DisposeAsync(true).Wait();
        GC.SuppressFinalize(this);
    }
}