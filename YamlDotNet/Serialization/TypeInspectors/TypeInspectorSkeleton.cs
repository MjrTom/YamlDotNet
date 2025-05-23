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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace YamlDotNet.Serialization.TypeInspectors
{
    /// <summary>
    /// The type inspector skeleton.
    /// </summary>
    public abstract class TypeInspectorSkeleton : ITypeInspector
    {
        /// <inheritdoc />
        public abstract string GetEnumName(Type enumType, string name);

        /// <inheritdoc />
        public abstract string GetEnumValue(object enumValue);

        /// <inheritdoc />
        public abstract IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container);

        /// <inheritdoc />
        public IPropertyDescriptor GetProperty(Type type, object? container, string name, [MaybeNullWhen(true)] bool ignoreUnmatched, bool caseInsensitivePropertyMatching)
        {
            IEnumerable<IPropertyDescriptor> candidates;

            if (caseInsensitivePropertyMatching)
            {
                candidates = GetProperties(type, container)
                    .Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                candidates = GetProperties(type, container)
                    .Where(p => p.Name == name);
            }

            using var enumerator = candidates.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                if (ignoreUnmatched)
                {
                    return null!;
                }

                throw new SerializationException($"Property '{name}' not found on type '{type.FullName}'.");
            }

            var property = enumerator.Current;

            if (enumerator.MoveNext())
            {
                throw new SerializationException(
                    $"Multiple properties with the name/alias '{name}' already exists on type '{type.FullName}', maybe you're misusing YamlAlias or maybe you are using the wrong naming convention? The matching properties are: {string.Join(", ", candidates.Select(p => p.Name).ToArray())}"
                );
            }

            return property;
        }
    }
}
