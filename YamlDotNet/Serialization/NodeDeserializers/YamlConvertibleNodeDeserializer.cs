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

namespace YamlDotNet.Serialization.NodeDeserializers
{
    /// <summary>
    /// The yaml convertible node deserializer.
    /// </summary>
    public sealed class YamlConvertibleNodeDeserializer : INodeDeserializer
    {
        private readonly IObjectFactory objectFactory;

        public YamlConvertibleNodeDeserializer(IObjectFactory objectFactory)
        {
            this.objectFactory = objectFactory;
        }

        /// <summary>
        /// Deserializes the.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="nestedObjectDeserializer">The nested object deserializer.</param>
        /// <param name="value">The value.</param>
        /// <param name="rootDeserializer">The root deserializer.</param>
        /// <returns>A bool.</returns>
        public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value, ObjectDeserializer rootDeserializer)
        {
            if (typeof(IYamlConvertible).IsAssignableFrom(expectedType))
            {
                var convertible = (IYamlConvertible)objectFactory.Create(expectedType);
                convertible.Read(parser, expectedType, type => nestedObjectDeserializer(parser, type));
                value = convertible;
                return true;
            }

            value = null;
            return false;
        }
    }
}
