// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    internal abstract partial class RefCache<TObject, TCacheEntryData, TKey>
    {
        internal readonly ref struct Scope
        {
            private readonly TObject _object;
            private CacheEntry Entry { get; }

            public Scope(TObject @object)
            {
                this = default;
                _object = @object;
            }

            public Scope(CacheEntry entry)
            {
                Debug.Assert(entry != null);
                this = default;
                Entry = entry;
                Entry.AddRef();
            }

            [MaybeNull]
            public TCacheEntryData Data => Entry is null ? default : Entry.Data;
            public TObject Object => this;
            public int RefCount => Entry?.RefCount ?? -1;

            public static implicit operator TObject(in Scope scope)
            {
                CacheEntry entry = scope.Entry;
                return entry is null ? scope._object : entry.Object;
            }

            public void Dispose()
            {
                Entry?.RemoveRef();
            }
        }
    }
}
