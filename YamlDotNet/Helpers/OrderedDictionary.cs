﻿// This file is part of YamlDotNet - A .NET library for YAML.
// Copyright (c) Antoine Aubry and contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace YamlDotNet.Helpers
{
    [Serializable]
    internal sealed class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
        where TKey : notnull
    {
        [NonSerialized]
        private Dictionary<TKey, TValue> dictionary;
        private readonly List<KeyValuePair<TKey, TValue>> list;
        private readonly IEqualityComparer<TKey> comparer;

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                if (dictionary.ContainsKey(key))
                {
                    var index = list.FindIndex(kvp => comparer.Equals(kvp.Key, key));
                    dictionary[key] = value;
                    list[index] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys => new KeyCollection(this);

        /// <inheritdoc />
        public ICollection<TValue> Values => new ValueCollection(this);

        /// <inheritdoc />
        public int Count => dictionary.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public KeyValuePair<TKey, TValue> this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public OrderedDictionary() : this(EqualityComparer<TKey>.Default)
        {
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer)
        {
            list = [];
            dictionary = new Dictionary<TKey, TValue>(comparer);
            this.comparer = comparer;
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (!TryAdd(item))
            {
                ThrowDuplicateKeyException(item.Key);
            }
        }

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
            {
                ThrowDuplicateKeyException(key);
            }
        }

        private static void ThrowDuplicateKeyException(TKey key)
        {
            throw new ArgumentException($"An item with the same key {key} has already been added.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd(TKey key, TValue value)
        {
            if (dictionary.TryAdd(key, value))
            {
                list.Add(new KeyValuePair<TKey, TValue>(key, value));
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd(KeyValuePair<TKey, TValue> item)
        {
            if (dictionary.TryAdd(item.Key, item.Value))
            {
                list.Add(item);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Clear()
        {
            dictionary.Clear();
            list.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            list.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => list.GetEnumerator();

        /// <inheritdoc />
        public void Insert(int index, TKey key, TValue value)
        {
            dictionary.Add(key, value);
            list.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                var index = list.FindIndex(kvp => comparer.Equals(kvp.Key, key));
                list.RemoveAt(index);
                if (!dictionary.Remove(key))
                {
                    throw new InvalidOperationException();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            var key = list[index].Key;
            dictionary.Remove(key);
            list.RemoveAt(index);
        }

#if !NET
#pragma warning disable 8767 // Nullability of reference types in type of parameter ... doesn't match implicitly implemented member
#endif

        /// <inheritdoc />
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
            dictionary.TryGetValue(key, out value);

#if !NET
#pragma warning restore 8767
#endif

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            // Reconstruct the dictionary from the serialized list
            dictionary = [];
            foreach (var kvp in list)
            {
                dictionary[kvp.Key] = kvp.Value;
            }
        }

        private class KeyCollection : ICollection<TKey>
        {
            private readonly OrderedDictionary<TKey, TValue> orderedDictionary;

            /// <inheritdoc />
            public int Count => orderedDictionary.list.Count;

            /// <inheritdoc />
            public bool IsReadOnly => true;

            /// <inheritdoc />
            public void Add(TKey item) => throw new NotSupportedException();

            /// <inheritdoc />
            public void Clear() => throw new NotSupportedException();

            /// <inheritdoc />
            public bool Contains(TKey item) => orderedDictionary.dictionary.ContainsKey(item);

            public KeyCollection(OrderedDictionary<TKey, TValue> orderedDictionary)
            {
                this.orderedDictionary = orderedDictionary;
            }

            /// <inheritdoc />
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                for (var i = 0; i < orderedDictionary.list.Count; i++)
                {
                    array[i] = orderedDictionary.list[i + arrayIndex].Key;
                }
            }

            /// <inheritdoc />
            public IEnumerator<TKey> GetEnumerator() =>
                orderedDictionary.list.Select(kvp => kvp.Key).GetEnumerator();

            /// <inheritdoc />
            public bool Remove(TKey item) => throw new NotSupportedException();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class ValueCollection : ICollection<TValue>
        {
            private readonly OrderedDictionary<TKey, TValue> orderedDictionary;

            /// <inheritdoc />
            public int Count => orderedDictionary.list.Count;

            /// <inheritdoc />
            public bool IsReadOnly => true;

            /// <inheritdoc />
            public void Add(TValue item) => throw new NotSupportedException();

            /// <inheritdoc />
            public void Clear() => throw new NotSupportedException();

            /// <inheritdoc />
            public bool Contains(TValue item) => orderedDictionary.dictionary.ContainsValue(item);

            public ValueCollection(OrderedDictionary<TKey, TValue> orderedDictionary)
            {
                this.orderedDictionary = orderedDictionary;
            }

            /// <inheritdoc />
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                for (var i = 0; i < orderedDictionary.list.Count; i++)
                {
                    array[i] = orderedDictionary.list[i + arrayIndex].Value;
                }
            }

            /// <inheritdoc />
            public IEnumerator<TValue> GetEnumerator() =>
                orderedDictionary.list.Select(kvp => kvp.Value).GetEnumerator();

            /// <inheritdoc />
            public bool Remove(TValue item) => throw new NotSupportedException();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
