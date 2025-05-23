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
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
    /// <summary>
    /// Converter for System.Guid.
    /// </summary>
    public class GuidConverter : IYamlTypeConverter
    {
        private readonly bool jsonCompatible;

        public GuidConverter(bool jsonCompatible)
        {
            this.jsonCompatible = jsonCompatible;
        }

        /// <inheritdoc />
        public bool Accepts(Type type)
        {
            return type == typeof(Guid);
        }

        /// <inheritdoc />
        public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            var value = parser.Consume<Scalar>().Value;
            return new Guid(value);
        }

        /// <inheritdoc />
        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            var guid = (Guid)value!;
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, guid.ToString("D"), jsonCompatible ? ScalarStyle.DoubleQuoted : ScalarStyle.Any, true, false));
        }
    }
}
