// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace System.Tools;

public static partial class ResGen
{
    private partial class ResGenRunner
    {
        internal sealed class ResourceSet
        {
            private readonly Hashtable _hashTable = new(10, StringComparer.InvariantCultureIgnoreCase);
            private readonly List<ResourceEntry> _resources = new();

            public string OutputFileName { get; set; }
            public string CultureName { get; set; }

            public IReadOnlyList<ResourceEntry> Resources => _resources;
            public IDictionary AsDictionary => _hashTable;

            public bool TryAddResource(string key, object value)
            {
                if (_hashTable.ContainsKey(key))
                {
                    return false;
                }

                _hashTable.Add(key, value);
                _resources.Add(new(key, value));
                return true;
            }
        }
    }
}
