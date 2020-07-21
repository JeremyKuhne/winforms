// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    internal abstract partial class RefCache<TObject, TCacheEntryData, TKey> : IDisposable
    {
        private readonly SinglyLinkedList<CacheEntry> _list = new SinglyLinkedList<CacheEntry>();

        private readonly object _lock = new object();

        protected bool _clean;
        private readonly int _softLimit;
        private readonly int _hardLimit;

        // Retrieving any node is at least 500ns. It takes around 10ns for every step through the linked list with a
        // very simple key match. It costs around 300ns to move an object to the front of the list, so we'll move a
        // node to the front when one additional retrieval will win back the cost of shifting.
        private const int MoveToFront = 30;

        public RefCache(int softLimit = 20, int hardLimit = 40)
        {
            Debug.Assert(softLimit > 0 && hardLimit > 0);
            Debug.Assert(softLimit <= hardLimit);

            _softLimit = softLimit;
            _hardLimit = hardLimit;
        }

        protected abstract CacheEntry CreateEntry(TKey key, bool cached);

        protected abstract bool IsMatch(TKey key, CacheEntry entry);

        public Scope GetEntry(TKey key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            Scope scope;

            lock (_lock)
            {
                if (Find(key, out Scope foundScope))
                {
                    scope = foundScope;
                }
                else
                {
                    scope = Add(key);
                }
            }

            return scope;

            bool Find(TKey key, out Scope scope)
            {
                bool success = false;
                scope = default;
                int position = MoveToFront;

                var enumerator = _list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var node = enumerator.Current;
                    var data = node.Value;

                    if (IsMatch(key, data))
                    {
                        scope = new Scope(data);
                        if (position < 0)
                        {
                            // Moving to the front as the cost of walking this far in outweighs the cost of moving
                            // the node to the front of the list.
                            enumerator.MoveCurrentToFront();
                        }
                        success = true;
                        break;
                    }

                    position--;
                }

                return success;
            }

            Scope Add(TKey key)
            {
                if (_list.Count >= _hardLimit)
                {
                    // Try to free up space
                    Clean();
                }

                if (_list.Count < _hardLimit)
                {
                    // We've got space, add to the cache
                    var data = CreateEntry(key, cached: true);
                    _list.AddFirst(data);
                    if (_list.Count > _softLimit)
                    {
                        // Clean on our next add
                        _clean = true;
                    }
                    return new Scope(data);
                }

                return new Scope(CreateEntry(key, cached: false));
            }

            void Clean()
            {
                // If the count is over the soft limit, try to get back under

                int overage = _list.Count - _softLimit;

                if (overage <= 0)
                {
                    return;
                }

                // Skip to the last part of the list and try to remove what we can

                int skip = _list.Count - overage;
                var enumerator = _list.GetEnumerator();
                int removed = 0;

                while (enumerator.MoveNext())
                {
                    skip--;
                    if (skip < 0)
                    {
                        var node = enumerator.Current;
                        if (node.Value.RefCount == 0)
                        {
                            enumerator.RemoveCurrent();
                            node.Value.Dispose();
                            removed++;
                        }
                    }
                }

                // All of the end of the list is in use? Are we leaking ref counts?
                Debug.Assert(removed != 0 || _softLimit < 20);

                _clean = _list.Count - _softLimit > 0;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var enumerator = _list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Dispose();
                    enumerator.RemoveCurrent();
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
