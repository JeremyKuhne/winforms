// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;

namespace System.Windows.Forms
{
    internal abstract partial class RefCache<TObject, TCacheEntryData, TKey>
    {
        [DebuggerDisplay("{DebuggerDisplay}")]
        internal abstract class CacheEntry : IDisposable
        {
            private readonly bool _cached;
            private int _refCount;

            public TCacheEntryData Data { get; private set; }

            public CacheEntry(TCacheEntryData data, bool cached)
            {
                Data = data;
                _cached = cached;
            }

            public void AddRef() => Interlocked.Increment(ref _refCount);

            public int RefCount => _refCount;

            public virtual void RemoveRef()
            {
                int refCount = Interlocked.Decrement(ref _refCount);

                if (!_cached && refCount == 0)
                {
                    // If this entry wasn't actually cached, we need to clean ourselves up when we're unreferenced.
                    // (This happens when there isn't enough room in the cache.)
                    Dispose(disposing: true);
                }
            }

            public abstract TObject Object { get; }

            private string DebuggerDisplay => $"Object: {Object} RefCount: {RefCount}";

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    var disposable = Object as IDisposable;
                    disposable?.Dispose();
                    disposable = Data as IDisposable;
                    disposable?.Dispose();
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
