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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using FluentAssertions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.Test.Serialization
{
    /// <summary>
    /// The serialization test helper.
    /// </summary>
    public class SerializationTestHelper
    {
        private SerializerBuilder serializerBuilder;
        private DeserializerBuilder deserializerBuilder;

        protected T DoRoundtripFromObjectTo<T>(object obj)
        {
            return DoRoundtripFromObjectTo<T>(obj, Serializer);
        }

        protected T DoRoundtripFromObjectTo<T>(object obj, ISerializer serializer)
        {
            return DoRoundtripFromObjectTo<T>(obj, serializer, Deserializer);
        }

        protected T DoRoundtripFromObjectTo<T>(object obj, ISerializer serializer, IDeserializer deserializer)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, obj);
            return deserializer.Deserialize<T>(UsingReaderFor(writer));
        }

        protected T DoRoundtripOn<T>(object obj)
        {
            return DoRoundtripOn<T>(obj, Serializer);
        }

        protected T DoRoundtripOn<T>(object obj, ISerializer serializer)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, obj, typeof(T));
            return new Deserializer().Deserialize<T>(UsingReaderFor(writer));
        }

        protected SerializerBuilder SerializerBuilder
        {
            get
            {
                return serializerBuilder ??= new SerializerBuilder();
            }
        }

        protected ISerializer Serializer
        {
            get
            {
                return SerializerBuilder.Build();
            }
        }

        protected DeserializerBuilder DeserializerBuilder
        {
            get
            {
                return deserializerBuilder ??= new DeserializerBuilder();
            }
        }

        protected IDeserializer Deserializer
        {
            get
            {
                return DeserializerBuilder.Build();
            }
        }

        protected void AssumingDeserializerWith(IObjectFactory factory)
        {
            deserializerBuilder = new DeserializerBuilder()
                .WithObjectFactory(factory);
        }

        protected TextReader UsingReaderFor(TextWriter buffer)
        {
            return UsingReaderFor(buffer.ToString());
        }

        protected TextReader UsingReaderFor(string text)
        {
            return new StringReader(text);
        }

        protected static IParser ParserFor(string yaml)
        {
            return new Parser(new StringReader(yaml));
        }

        protected string Lines(params string[] lines)
        {
            return string.Join("\r\n", lines);
        }

        protected object Entry(string key, string value)
        {
            return new DictionaryEntry(key, value);
        }
    }

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The enum example.
    /// </summary>
    [Flags]
    public enum EnumExample
    {
        None,
        One,
        Two
    }

    public struct StructExample
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// The s byte enum.
    /// </summary>
    public enum SByteEnum : sbyte { Default, Sbyte }
    /// <summary>
    /// The byte enum.
    /// </summary>
    public enum ByteEnum : byte { Default, Byte }
    /// <summary>
    /// The int16 enum.
    /// </summary>
    public enum Int16Enum : short { Default, Short }
    /// <summary>
    /// The u int16 enum.
    /// </summary>
    public enum UInt16Enum : ushort { Default, Ushort }
    /// <summary>
    /// The int32 enum.
    /// </summary>
    public enum Int32Enum : int { Default, Int }
    /// <summary>
    /// The u int32 enum.
    /// </summary>
    public enum UInt32Enum : uint { Default, Uint }
    /// <summary>
    /// The int64 enum.
    /// </summary>
    public enum Int64Enum : long { Default, Long }
    /// <summary>
    /// The u int64 enum.
    /// </summary>
    public enum UInt64Enum : ulong { Default, Ulong }

    /// <summary>
    /// The circular reference.
    /// </summary>
    public class CircularReference
    {
        /// <summary>
        /// Gets or sets the child1.
        /// </summary>
        public CircularReference Child1 { get; set; }
        /// <summary>
        /// Gets or sets the child2.
        /// </summary>
        public CircularReference Child2 { get; set; }
    }

    /// <summary>
    /// The convertible.
    /// </summary>
    [TypeConverter(typeof(ConvertibleConverter))]
    public class Convertible : IConvertible
    {
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        public string Left { get; set; }
        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        public string Right { get; set; }

        /// <summary>
        /// Tos the type.
        /// </summary>
        /// <param name="conversionType">The conversion type.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>An object.</returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            conversionType.Should().Be<string>();
            return ToString(provider);
        }

        /// <summary>
        /// Tos the string.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A string.</returns>
        public string ToString(IFormatProvider provider)
        {
            provider.Should().Be(CultureInfo.InvariantCulture);
            return string.Format(provider, "[{0}, {1}]", Left, Right);
        }

        #region Unsupported Members

        /// <summary>
        /// Gets the type code.
        /// </summary>
        /// <returns>A System.TypeCode.</returns>
        public System.TypeCode GetTypeCode()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the boolean.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A bool.</returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the byte.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A byte.</returns>
        public byte ToByte(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the char.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A char.</returns>
        public char ToChar(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the date time.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A DateTime.</returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the decimal.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A decimal.</returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the double.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A double.</returns>
        public double ToDouble(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the int16.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A short.</returns>
        public short ToInt16(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the int32.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>An int.</returns>
        public int ToInt32(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the int64.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A long.</returns>
        public long ToInt64(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the s byte.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A sbyte.</returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the single.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>A float.</returns>
        public float ToSingle(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the u int16.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>An ushort.</returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the u int32.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>An uint.</returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tos the u int64.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>An ulong.</returns>
        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    /// <summary>
    /// The convertible converter.
    /// </summary>
    public class ConvertibleConverter : TypeConverter
    {
        /// <summary>
        /// Cans the convert from.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>A bool.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Cans the convert to.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A bool.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// Converts the from.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value.</param>
        /// <returns>An object.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
            {
                throw new InvalidOperationException();
            }

            var parts = (value as string).Split(' ');
            return new Convertible
            {
                Left = parts[0],
                Right = parts[1]
            };
        }
    }

    /// <summary>
    /// The missing default ctor.
    /// </summary>
    public class MissingDefaultCtor
    {
        public string Value;

        public MissingDefaultCtor(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// The missing default ctor converter.
    /// </summary>
    public class MissingDefaultCtorConverter : IYamlTypeConverter
    {
        /// <summary>
        /// Accepts the.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A bool.</returns>
        public bool Accepts(Type type)
        {
            return type == typeof(MissingDefaultCtor);
        }

        /// <summary>
        /// Reads the yaml.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="type">The type.</param>
        /// <param name="deserializer">The deserializer.</param>
        /// <returns>An object.</returns>
        public object ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
        {
            var value = ((Scalar)parser.Current).Value;
            parser.MoveNext();
            return new MissingDefaultCtor(value);
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
            emitter.Emit(new Scalar(((MissingDefaultCtor)value).Value));
        }
    }

    /// <summary>
    /// The inheritance example.
    /// </summary>
    public class InheritanceExample
    {
        /// <summary>
        /// Gets or sets the some scalar.
        /// </summary>
        public object SomeScalar { get; set; }
        /// <summary>
        /// Gets or sets the regular base.
        /// </summary>
        public Base RegularBase { get; set; }

        /// <summary>
        /// Gets or sets the base with serialize as.
        /// </summary>
        [YamlMember(serializeAs: typeof(Base))]
        public Base BaseWithSerializeAs { get; set; }
    }

    /// <summary>
    /// The interface example.
    /// </summary>
    public class InterfaceExample
    {
        /// <summary>
        /// Gets or sets the derived.
        /// </summary>
        public IDerived Derived { get; set; }
    }

    public interface IBase
    {
        string BaseProperty { get; set; }
    }

    public interface IDerived : IBase
    {
        string DerivedProperty { get; set; }
    }

    /// <summary>
    /// The base.
    /// </summary>
    public class Base : IBase
    {
        /// <summary>
        /// Gets or sets the base property.
        /// </summary>
        public string BaseProperty { get; set; }
    }

    /// <summary>
    /// The derived.
    /// </summary>
    public class Derived : Base, IDerived
    {
        /// <summary>
        /// Gets or sets the derived property.
        /// </summary>
        public string DerivedProperty { get; set; }
    }

    /// <summary>
    /// The empty base.
    /// </summary>
    public class EmptyBase
    {
    }

    /// <summary>
    /// The empty derived.
    /// </summary>
    public class EmptyDerived : EmptyBase
    {
    }

    /// <summary>
    /// The simple.
    /// </summary>
    public class Simple
    {
        /// <summary>
        /// Gets or sets the aaa.
        /// </summary>
        public string aaa { get; set; }
    }

    /// <summary>
    /// The simple scratch.
    /// </summary>
    public class SimpleScratch
    {
        /// <summary>
        /// Gets or sets the scratch.
        /// </summary>
        public string Scratch { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether delete scratch.
        /// </summary>
        public bool DeleteScratch { get; set; }
        /// <summary>
        /// Gets or sets the mapped scratch.
        /// </summary>
        public IEnumerable<string> MappedScratch { get; set; }
    }

    /// <summary>
    /// The example.
    /// </summary>
    public class Example
    {
        /// <summary>
        /// Gets or sets a value indicating whether my flag.
        /// </summary>
        public bool MyFlag { get; set; }
        /// <summary>
        /// Gets or sets the nothing.
        /// </summary>
        public string Nothing { get; set; }
        /// <summary>
        /// Gets or sets the my int.
        /// </summary>
        public int MyInt { get; set; }
        /// <summary>
        /// Gets or sets the my double.
        /// </summary>
        public double MyDouble { get; set; }
        /// <summary>
        /// Gets or sets the my string.
        /// </summary>
        public string MyString { get; set; }
        /// <summary>
        /// Gets or sets the my date.
        /// </summary>
        public DateTime MyDate { get; set; }
        /// <summary>
        /// Gets or sets the my time span.
        /// </summary>
        public TimeSpan MyTimeSpan { get; set; }
        /// <summary>
        /// Gets or sets the my point.
        /// </summary>
        public Point MyPoint { get; set; }
        /// <summary>
        /// Gets or sets the my nullable with value.
        /// </summary>
        public int? MyNullableWithValue { get; set; }
        /// <summary>
        /// Gets or sets the my nullable without value.
        /// </summary>
        public int? MyNullableWithoutValue { get; set; }

        public Example()
        {
            MyInt = 1234;
            MyDouble = 6789.1011;
            MyString = "Hello world";
            MyDate = DateTime.Now;
            MyTimeSpan = TimeSpan.FromHours(1);
            MyPoint = new Point(100, 200);
            MyNullableWithValue = 8;
        }
    }

    /// <summary>
    /// The order example.
    /// </summary>
    public class OrderExample
    {
        public OrderExample()
        {
            this.Order1 = "Order1 value";
            this.Order2 = "Order2 value";
        }

        /// <summary>
        /// Gets or sets the order2.
        /// </summary>
        [YamlMember(Order = 2)]
        public string Order2 { get; set; }

        /// <summary>
        /// Gets or sets the order1.
        /// </summary>
        [YamlMember(Order = 1)]
        public string Order1 { get; set; }
    }

    /// <summary>
    /// The ignore example.
    /// </summary>
    public class IgnoreExample
    {
        /// <summary>
        /// Gets or sets the ignore me.
        /// </summary>
        [YamlIgnore]
        public string IgnoreMe
        {
            get { throw new InvalidOperationException("Accessing a [YamlIgnore] property"); }
            set { throw new InvalidOperationException("Accessing a [YamlIgnore] property"); }
        }
    }

    /// <summary>
    /// The ignore example base.
    /// </summary>
    public class IgnoreExampleBase
    {
        /// <summary>
        /// Gets or sets the ignore me.
        /// </summary>
        [YamlIgnore]
        public virtual string IgnoreMe
        {
            get { throw new InvalidOperationException("Accessing a [YamlIgnore] property"); }
            set { throw new InvalidOperationException("Accessing a [YamlIgnore] property"); }
        }
    }

    /// <summary>
    /// The ignore example derived.
    /// </summary>
    public class IgnoreExampleDerived : IgnoreExampleBase
    {
        /// <summary>
        /// Gets or sets the ignore me.
        /// </summary>
        public override string IgnoreMe
        {
            get { throw new InvalidOperationException("Accessing a [YamlIgnore] property"); }
            set { throw new InvalidOperationException("Accessing a [YamlIgnore] property"); }
        }
    }

    /// <summary>
    /// The scalar style example.
    /// </summary>
    public class ScalarStyleExample
    {
        public ScalarStyleExample()
        {
            var content = "Test";
            LiteralString = content;
            DoubleQuotedString = content;
        }

        /// <summary>
        /// Gets or sets the literal string.
        /// </summary>
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string LiteralString { get; set; }

        /// <summary>
        /// Gets or sets the double quoted string.
        /// </summary>
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string DoubleQuotedString { get; set; }
    }

    /// <summary>
    /// This class demonstrates an object with serializable data that will be
    /// written to YAML with strings that are formatted as valid numbers.
    /// </summary>
    public class MixedFormatScalarStyleExample
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        public string[] Data { get; }

        public MixedFormatScalarStyleExample(string[] data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// The defaults example.
    /// </summary>
    public class DefaultsExample
    {
        public const string DefaultValue = "myDefault";

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DefaultValue(DefaultValue)]
        public string Value { get; set; }
    }

    /// <summary>
    /// The custom generic dictionary.
    /// </summary>
    public class CustomGenericDictionary : IDictionary<string, string>
    {
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();

        /// <summary>
        /// Adds the.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, string value)
        {
            dictionary.Add(key, value);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>An IEnumerator.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Unsupported Members

        /// <summary>
        /// Contains the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A bool.</returns>
        public bool ContainsKey(string key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Removes the.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A bool.</returns>
        public bool Remove(string key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>A bool.</returns>
        public bool TryGetValue(string key, out string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public ICollection<string> Values
        {
            get { throw new NotSupportedException(); }
        }

        public string this[string key]
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Adds the.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Clears the.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Contains the.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A bool.</returns>
        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Copies the to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">The array index.</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether read is only.
        /// </summary>
        public bool IsReadOnly
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Removes the.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A bool.</returns>
        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    /// <summary>
    /// The name convention.
    /// </summary>
    public class NameConvention
    {
        /// <summary>
        /// Gets or sets the first test.
        /// </summary>
        public string FirstTest { get; set; }
        /// <summary>
        /// Gets or sets the second test.
        /// </summary>
        public string SecondTest { get; set; }
        /// <summary>
        /// Gets or sets the third test.
        /// </summary>
        public string ThirdTest { get; set; }

        /// <summary>
        /// Gets or sets the alias test.
        /// </summary>
        [YamlMember(Alias = "fourthTest")]
        public string AliasTest { get; set; }

        /// <summary>
        /// Gets or sets the fourth test.
        /// </summary>
        [YamlIgnore]
        public string fourthTest { get; set; }
    }

    /// <summary>
    /// The non public properties example.
    /// </summary>
    public class NonPublicPropertiesExample
    {
        /// <summary>
        /// Gets or sets the public.
        /// </summary>
        public string Public { get; set; } = "public";

        internal string Internal { get; set; } = "internal";

        protected string Protected { get; set; } = "protected";

        private string Private { get; set; } = "private";

        /// <inheritdoc />
        public override string ToString() => $"{Public},{Internal},{Protected},{Private}";
    }

#pragma warning disable IDE0044 // Add readonly modifier
    /// <summary>
    /// The non public fields example.
    /// </summary>
    public class NonPublicFieldsExample
    {
        public string Public = "public";

        internal string Internal = "internal";

        protected string Protected = "protected";

        private string Private = "private";

        /// <inheritdoc />
        public override string ToString() => $"{Public},{Internal},{Protected},{Private}";
    }
#pragma warning restore IDE0044 // Add readonly modifier
}
