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

using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
    /// <summary>
    /// The object descriptor.
    /// </summary>
    public sealed class ObjectDescriptor : IObjectDescriptor
    {
        /// <inheritdoc />
        public object? Value { get; }
        /// <inheritdoc />
        public Type Type { get; }
        /// <inheritdoc />
        public Type StaticType { get; }
        /// <inheritdoc />
        public ScalarStyle ScalarStyle { get; }

        public ObjectDescriptor(object? value, Type type, Type staticType)
            : this(value, type, staticType, ScalarStyle.Any)
        {
        }

        public ObjectDescriptor(object? value, Type type, Type staticType, ScalarStyle scalarStyle)
        {
            Value = value;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            StaticType = staticType ?? throw new ArgumentNullException(nameof(staticType));

            ScalarStyle = scalarStyle;
        }
    }
}
