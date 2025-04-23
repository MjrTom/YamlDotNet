// This file is part of YamlDotNet - A .NET library for YAML.
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

using System;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.Test.Serialization
{
    /// <summary>
    /// The type converter attribute tests.
    /// </summary>
    public class TypeConverterAttributeTests
    {
        /// <summary>
        /// Tests the converter in attribute override_ deserializes.
        /// </summary>
        [Fact]
        public void TestConverterInAttributeOverride_Deserializes()
        {
            var deserializer = new DeserializerBuilder()
                .WithAttributeOverride<OuterClassWithoutAttribute>(
                    c => c.Value,
                    new YamlConverterAttribute(typeof(AttributedTypeConverter)))
                .WithTypeConverter(new AttributedTypeConverter())
                .Build();
            var yaml = @"Value:
  abc: def";
            var actual = deserializer.Deserialize<OuterClassWithoutAttribute>(yaml);
            Assert.Equal("abc", actual.Value.Key);
            Assert.Equal("def", actual.Value.Value);
        }

        /// <summary>
        /// Tests the converter in attribute override_ serializes.
        /// </summary>
        [Fact]
        public void TestConverterInAttributeOverride_Serializes()
        {
            var serializer = new SerializerBuilder()
                .WithAttributeOverride<OuterClassWithoutAttribute>(
                    c => c.Value,
                    new YamlConverterAttribute(typeof(AttributedTypeConverter)))
                .WithTypeConverter(new AttributedTypeConverter())
                .Build();
            var o = new OuterClassWithoutAttribute
            {
                Value = new ValueClass
                {
                    Key = "abc",
                    Value = "def"
                }
            };
            var actual = serializer.Serialize(o).NormalizeNewLines().TrimNewLines();
            var expected = @"Value:
  abc: def".NormalizeNewLines().TrimNewLines();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Tests the converter on attribute_ deserializes.
        /// </summary>
        [Fact]
        public void TestConverterOnAttribute_Deserializes()
        {
            var deserializer = new DeserializerBuilder().WithTypeConverter(new AttributedTypeConverter()).Build();
            var yaml = @"Value:
  abc: def";
            var actual = deserializer.Deserialize<OuterClass>(yaml);
            Assert.Equal("abc", actual.Value.Key);
            Assert.Equal("def", actual.Value.Value);
        }

        /// <summary>
        /// Tests the converter on attribute_ serializes.
        /// </summary>
        [Fact]
        public void TestConverterOnAttribute_Serializes()
        {
            var serializer = new SerializerBuilder().WithTypeConverter(new AttributedTypeConverter()).Build();
            var o = new OuterClass
            {
                Value = new ValueClass
                {
                    Key = "abc",
                    Value = "def"
                }
            };
            var actual = serializer.Serialize(o).NormalizeNewLines().TrimNewLines();
            var expected = @"Value:
  abc: def".NormalizeNewLines().TrimNewLines();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// The attributed type converter.
        /// </summary>
        public class AttributedTypeConverter : IYamlTypeConverter
        {
            /// <summary>
            /// Accepts the.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns>A bool.</returns>
            public bool Accepts(Type type) => false;

            /// <summary>
            /// Reads the yaml.
            /// </summary>
            /// <param name="parser">The parser.</param>
            /// <param name="type">The type.</param>
            /// <param name="rootDeserializer">The root deserializer.</param>
            /// <returns>An object.</returns>
            public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
            {
                parser.Consume<MappingStart>();
                var key = parser.Consume<Scalar>();
                var value = parser.Consume<Scalar>();
                parser.Consume<MappingEnd>();

                var result = new ValueClass
                {
                    Key = key.Value,
                    Value = value.Value
                };
                return result;
            }

            /// <summary>
            /// Writes the yaml.
            /// </summary>
            /// <param name="emitter">The emitter.</param>
            /// <param name="value">The value.</param>
            /// <param name="type">The type.</param>
            /// <param name="serializer">The serializer.</param>
            public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
            {
                var v = (ValueClass)value;

                emitter.Emit(new MappingStart());
                emitter.Emit(new Scalar(v.Key));
                emitter.Emit(new Scalar(v.Value));
                emitter.Emit(new MappingEnd());
            }
        }

        /// <summary>
        /// The outer class.
        /// </summary>
        public class OuterClass
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            [YamlConverter(typeof(AttributedTypeConverter))]
            public ValueClass Value { get; set; }
        }

        /// <summary>
        /// The outer class without attribute.
        /// </summary>
        public class OuterClassWithoutAttribute
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public ValueClass Value { get; set; }
        }

        /// <summary>
        /// The value class.
        /// </summary>
        public class ValueClass
        {
            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { get; set; }
        }
    }
}
