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

namespace YamlDotNet.Helpers
{
    /// <summary>
    /// Adapts an <see cref="System.Collections.Generic.IDictionary{TKey, TValue}" /> to <see cref="IDictionary" />
    /// because not all generic dictionaries implement <see cref="IDictionary" />.
    /// </summary>
    internal sealed class GenericDictionaryToNonGenericAdapter<TKey, TValue> : IDictionary
        where TKey : notnull
    {
        private readonly IDictionary<TKey, TValue> genericDictionary;

        public GenericDictionaryToNonGenericAdapter(IDictionary<TKey, TValue> genericDictionary)
        {
            this.genericDictionary = genericDictionary ?? throw new ArgumentNullException(nameof(genericDictionary));
        }

        /// <inheritdoc />
        public void Add(object key, object? value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Contains(object key)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IDictionaryEnumerator GetEnumerator()
        {
            return new DictionaryEnumerator(genericDictionary.GetEnumerator());
        }

        /// <inheritdoc />
        public bool IsFixedSize
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public ICollection Keys
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public void Remove(object key)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ICollection Values
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public object? this[object key]
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                genericDictionary[(TKey)key] = (TValue)value!;
            }
        }

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public int Count
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsSynchronized
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DictionaryEnumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

            public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
            {
                this.enumerator = enumerator;
            }

            /// <inheritdoc />
            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry(Key, Value);
                }
            }

            /// <inheritdoc />
            public object Key
            {
                get { return enumerator.Current.Key!; }
            }

            /// <inheritdoc />
            public object? Value
            {
                get { return enumerator.Current.Value; }
            }

            /// <inheritdoc />
            public object Current
            {
                get { return Entry; }
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            /// <inheritdoc />
            public void Reset()
            {
                enumerator.Reset();
            }
        }
    }
}
