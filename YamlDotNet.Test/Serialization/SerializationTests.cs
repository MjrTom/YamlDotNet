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
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Callbacks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectFactories;

namespace YamlDotNet.Test.Serialization
{
    /// <summary>
    /// The serialization tests.
    /// </summary>
    public class SerializationTests : SerializationTestHelper
    {
        #region Test Cases

        private static readonly string[] TrueStrings = { "true", "y", "yes", "on" };
        private static readonly string[] FalseStrings = { "false", "n", "no", "off" };

        /// <summary>
        /// Gets the deserialize scalar boolean_ test cases.
        /// </summary>
        public static IEnumerable<object[]> DeserializeScalarBoolean_TestCases
        {
            get
            {
                foreach (var trueString in TrueStrings)
                {
                    yield return new object[] { trueString, true };
                    yield return new object[] { trueString.ToUpper(), true };
                }

                foreach (var falseString in FalseStrings)
                {
                    yield return new object[] { falseString, false };
                    yield return new object[] { falseString.ToUpper(), false };
                }
            }
        }

        #endregion

        /// <summary>
        /// Deserializes the empty document.
        /// </summary>
        [Fact]
        public void DeserializeEmptyDocument()
        {
            var emptyText = string.Empty;

            var array = Deserializer.Deserialize<int[]>(UsingReaderFor(emptyText));

            array.Should().BeNull();
        }

        /// <summary>
        /// Deserializes the scalar.
        /// </summary>
        [Fact]
        public void DeserializeScalar()
        {
            var stream = Yaml.ReaderFrom("02-scalar-in-imp-doc.yaml");

            var result = Deserializer.Deserialize(stream);

            result.Should().Be("a scalar");
        }

        /// <summary>
        /// Deserializes the scalar boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expected">If true, expected.</param>
        [Theory]
        [MemberData(nameof(DeserializeScalarBoolean_TestCases))]
        public void DeserializeScalarBoolean(string value, bool expected)
        {
            var result = Deserializer.Deserialize<bool>(UsingReaderFor(value));

            result.Should().Be(expected);
        }

        /// <summary>
        /// Deserializes the scalar boolean throws when invalid.
        /// </summary>
        [Fact]
        public void DeserializeScalarBooleanThrowsWhenInvalid()
        {
            Action action = () => Deserializer.Deserialize<bool>(UsingReaderFor("not-a-boolean"));

            action.Should().Throw<YamlException>().WithInnerException<FormatException>();
        }

        /// <summary>
        /// Deserializes the scalar zero.
        /// </summary>
        [Fact]
        public void DeserializeScalarZero()
        {
            var result = Deserializer.Deserialize<int>(UsingReaderFor("0"));

            result.Should().Be(0);
        }

        /// <summary>
        /// Deserializes the scalar decimal.
        /// </summary>
        [Fact]
        public void DeserializeScalarDecimal()
        {
            var result = Deserializer.Deserialize<int>(UsingReaderFor("+1_234_567"));

            result.Should().Be(1234567);
        }

        /// <summary>
        /// Deserializes the scalar binary number.
        /// </summary>
        [Fact]
        public void DeserializeScalarBinaryNumber()
        {
            var result = Deserializer.Deserialize<int>(UsingReaderFor("-0b1_0010_1001_0010"));

            result.Should().Be(-4754);
        }

        /// <summary>
        /// Deserializes the scalar octal number.
        /// </summary>
        [Fact]
        public void DeserializeScalarOctalNumber()
        {
            var result = Deserializer.Deserialize<int>(UsingReaderFor("+071_352"));

            result.Should().Be(29418);
        }

        /// <summary>
        /// Deserializes the nullable scalar octal number.
        /// </summary>
        [Fact]
        public void DeserializeNullableScalarOctalNumber()
        {
            var result = Deserializer.Deserialize<int?>(UsingReaderFor("+071_352"));

            result.Should().Be(29418);
        }

        /// <summary>
        /// Deserializes the scalar hex number.
        /// </summary>
        [Fact]
        public void DeserializeScalarHexNumber()
        {
            var result = Deserializer.Deserialize<int>(UsingReaderFor("-0x_0F_B9"));

            result.Should().Be(-0xFB9);
        }

        /// <summary>
        /// Deserializes the scalar long base60 number.
        /// </summary>
        [Fact]
        public void DeserializeScalarLongBase60Number()
        {
            var result = Deserializer.Deserialize<long>(UsingReaderFor("99_:_58:47:3:6_2:10"));

            result.Should().Be(77744246530L);
        }

        /// <summary>
        /// Roundtrips the enums.
        /// </summary>
        /// <param name="value">The value.</param>
        [Theory]
        [InlineData(EnumExample.One)]
        [InlineData(EnumExample.One | EnumExample.Two)]
        public void RoundtripEnums(EnumExample value)
        {
            var result = DoRoundtripFromObjectTo<EnumExample>(value);

            result.Should().Be(value);
        }

        /// <summary>
        /// Roundtrips the nullable enums.
        /// </summary>
        /// <param name="value">The value.</param>
        [Theory]
        [InlineData(EnumExample.One)]
        [InlineData(EnumExample.One | EnumExample.Two)]
        [InlineData(null)]
        public void RoundtripNullableEnums(EnumExample? value)
        {
            var result = DoRoundtripFromObjectTo<EnumExample?>(value);

            result.Should().Be(value);
        }

        /// <summary>
        /// Roundtrips the nullable struct with value.
        /// </summary>
        [Fact]
        public void RoundtripNullableStructWithValue()
        {
            var value = new StructExample { Value = 2 };

            var result = DoRoundtripFromObjectTo<StructExample?>(value);

            result.Should().Be(value);
        }

        /// <summary>
        /// Roundtrips the nullable struct without value.
        /// </summary>
        [Fact]
        public void RoundtripNullableStructWithoutValue()
        {
            var result = DoRoundtripFromObjectTo<StructExample?>(null);

            result.Should().Be(null);
        }

        /// <summary>
        /// Serializes the circular reference.
        /// </summary>
        [Fact]
        public void SerializeCircularReference()
        {
            var obj = new CircularReference();
            obj.Child1 = new CircularReference
            {
                Child1 = obj,
                Child2 = obj
            };

            Action action = () => SerializerBuilder.EnsureRoundtrip().Build().Serialize(new StringWriter(), obj, typeof(CircularReference));

            action.Should().NotThrow();
        }

        /// <summary>
        /// Deserializes the incomplete directive.
        /// </summary>
        [Fact]
        public void DeserializeIncompleteDirective()
        {
            Action action = () => Deserializer.Deserialize<object>(UsingReaderFor("%Y"));

            action.Should().Throw<SyntaxErrorException>()
                .WithMessage("While scanning a directive, found unexpected end of stream.");
        }

        /// <summary>
        /// Deserializes the skipped reserved directive.
        /// </summary>
        [Fact]
        public void DeserializeSkippedReservedDirective()
        {
            Action action = () => Deserializer.Deserialize<object>(UsingReaderFor("%Y "));

            action.Should().NotThrow();
        }

        /// <summary>
        /// Deserializes the custom tags.
        /// </summary>
        [Fact]
        public void DeserializeCustomTags()
        {
            var stream = Yaml.ReaderFrom("tags.yaml");

            DeserializerBuilder.WithTagMapping("tag:yaml.org,2002:point", typeof(Point));
            var result = Deserializer.Deserialize(stream);

            result.Should().BeOfType<Point>().And
                .Subject.As<Point>()
                .Should().BeEquivalentTo(new { X = 10, Y = 20 }, o => o.ExcludingMissingMembers());
        }

        /// <summary>
        /// Deserializes the with gaps between keys.
        /// </summary>
        [Fact]
        public void DeserializeWithGapsBetweenKeys()
        {
            var yamlReader = new StringReader(@"Text: >
  Some Text.

Value: foo");
            var result = Deserializer.Deserialize(yamlReader);

            result.Should().NotBeNull();
        }

        /// <summary>
        /// Serializes the custom tags.
        /// </summary>
        [Fact]
        public void SerializeCustomTags()
        {
            var expectedResult = Yaml.ReaderFrom("tags.yaml").ReadToEnd().NormalizeNewLines();
            SerializerBuilder
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .WithTagMapping(new TagName("tag:yaml.org,2002:point"), typeof(Point));

            var point = new Point(10, 20);
            var result = Serializer.Serialize(point);

            result.Should().Be(expectedResult);
        }

        /// <summary>
        /// Serializes the with c r l f new line.
        /// </summary>
        [Fact]
        public void SerializeWithCRLFNewLine()
        {
            var expectedResult = Yaml
                .ReaderFrom("list.yaml")
                .ReadToEnd()
                .NormalizeNewLines()
                .Replace(Environment.NewLine, "\r\n");

            var list = new string[] { "one", "two", "three" };
            var result = SerializerBuilder
                .WithNewLine("\r\n")
                .Build()
                .Serialize(list);

            result.Should().Be(expectedResult);
        }

        /// <summary>
        /// Serializes the with l f new line.
        /// </summary>
        [Fact]
        public void SerializeWithLFNewLine()
        {
            var expectedResult = Yaml
                .ReaderFrom("list.yaml")
                .ReadToEnd()
                .NormalizeNewLines()
                .Replace(Environment.NewLine, "\n");

            var list = new string[] { "one", "two", "three" };
            var result = SerializerBuilder
                .WithNewLine("\n")
                .Build()
                .Serialize(list);

            result.Should().Be(expectedResult);
        }

        /// <summary>
        /// Serializes the with c r new line.
        /// </summary>
        [Fact]
        public void SerializeWithCRNewLine()
        {
            var expectedResult = Yaml
                .ReaderFrom("list.yaml")
                .ReadToEnd()
                .NormalizeNewLines()
                .Replace(Environment.NewLine, "\r");

            var list = new string[] { "one", "two", "three" };
            var result = SerializerBuilder
                .WithNewLine("\r")
                .Build()
                .Serialize(list);

            result.Should().Be(expectedResult);
        }


        /// <summary>
        /// Tests the serialization of a dictionary containing a list of strings using IndentedTextWriter.
        /// </summary>
        [Fact]
        public void SerializeWithTabs()
        {
            var tabString = " ";
            using var writer = new StringWriter();
            using var indentedTextWriter = new IndentedTextWriter(writer, tabString) { Indent = 2 };

            //create a dictionary with a list of strings to test the tabbed serialization
            var items = new List<string> { "item 1", "item 2" };
            var list = new Dictionary<string, List<string>> { { "key", items } };

            SerializerBuilder
                .Build()
                .Serialize(indentedTextWriter, list);

            //split serialized output into lines
            var lines = indentedTextWriter.InnerWriter.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            //expected indentation
            var indent = string.Join(string.Empty, Enumerable.Repeat(tabString, indentedTextWriter.Indent).ToList());

            //check that the serialized lines (excluding the first and last) start with the expected indentation
            lines.Skip(1).Take(lines.Length - 2).Where(element => element.StartsWith(indent)).Should().HaveCount(items.Count);
        }

        /// <summary>
        /// Deserializes the explicit type.
        /// </summary>
        [Fact]
        public void DeserializeExplicitType()
        {
            var text = Yaml.ReaderFrom("explicit-type.template").TemplatedOn<Simple>();

            var result = new DeserializerBuilder()
                .WithTagMapping("!Simple", typeof(Simple))
                .Build()
                .Deserialize<Simple>(UsingReaderFor(text));

            result.aaa.Should().Be("bbb");
        }

        /// <summary>
        /// Deserializes the convertible.
        /// </summary>
        [Fact]
        public void DeserializeConvertible()
        {
            var text = Yaml.ReaderFrom("convertible.template").TemplatedOn<Convertible>();

            var result = new DeserializerBuilder()
                .WithTagMapping("!Convertible", typeof(Convertible))
                .Build()
                .Deserialize<Simple>(UsingReaderFor(text));

            result.aaa.Should().Be("[hello, world]");
        }

        /// <summary>
        /// Deserializations the fails for undefined forward references.
        /// </summary>
        [Fact]
        public void DeserializationFailsForUndefinedForwardReferences()
        {
            var text = Lines(
                "Nothing: *forward",
                "MyString: ForwardReference");

            Action action = () => Deserializer.Deserialize<Example>(UsingReaderFor(text));

            action.Should().Throw<AnchorNotFoundException>();
        }

        /// <summary>
        /// Roundtrips the object.
        /// </summary>
        [Fact]
        public void RoundtripObject()
        {
            var obj = new Example();

            var result = DoRoundtripFromObjectTo<Example>(
                obj,
                new SerializerBuilder()
                    .WithTagMapping("!Example", typeof(Example))
                    .EnsureRoundtrip()
                    .Build(),
                new DeserializerBuilder()
                    .WithTagMapping("!Example", typeof(Example))
                    .Build()
            );

            result.Should().BeEquivalentTo(obj);
        }

        /// <summary>
        /// Roundtrips the object with defaults.
        /// </summary>
        [Fact]
        public void RoundtripObjectWithDefaults()
        {
            var obj = new Example();

            var result = DoRoundtripFromObjectTo<Example>(
                obj,
                new SerializerBuilder()
                    .WithTagMapping("!Example", typeof(Example))
                    .EnsureRoundtrip()
                    .Build(),
                new DeserializerBuilder()
                    .WithTagMapping("!Example", typeof(Example))
                    .Build()
            );

            result.Should().BeEquivalentTo(obj);
        }

        /// <summary>
        /// Roundtrips the anonymous type.
        /// </summary>
        [Fact]
        public void RoundtripAnonymousType()
        {
            var data = new { Key = 3 };

            var result = DoRoundtripFromObjectTo<Dictionary<string, string>>(data);

            result.Should().Equal(new Dictionary<string, string> {
                { "Key", "3" }
            });
        }

        /// <summary>
        /// Roundtrips the with yaml type converter.
        /// </summary>
        [Fact]
        public void RoundtripWithYamlTypeConverter()
        {
            var obj = new MissingDefaultCtor("Yo");

            SerializerBuilder
                .EnsureRoundtrip()
                .WithTypeConverter(new MissingDefaultCtorConverter());

            DeserializerBuilder
                .WithTypeConverter(new MissingDefaultCtorConverter());

            var result = DoRoundtripFromObjectTo<MissingDefaultCtor>(obj, Serializer, Deserializer);

            result.Value.Should().Be("Yo");
        }

        /// <summary>
        /// Roundtrips the alias.
        /// </summary>
        [Fact]
        public void RoundtripAlias()
        {
            var writer = new StringWriter();
            var input = new NameConvention { AliasTest = "Fourth" };

            SerializerBuilder
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults);

            Serializer.Serialize(writer, input, input.GetType());
            var text = writer.ToString();

            // Todo: use RegEx once FluentAssertions 2.2 is released
            text.TrimEnd('\r', '\n').Should().Be("fourthTest: Fourth");

            var output = Deserializer.Deserialize<NameConvention>(UsingReaderFor(text));

            output.AliasTest.Should().Be(input.AliasTest);
        }

        /// <summary>
        /// Roundtrips the alias override.
        /// </summary>
        [Fact]
        public void RoundtripAliasOverride()
        {
            var writer = new StringWriter();
            var input = new NameConvention { AliasTest = "Fourth" };

            var attribute = new YamlMemberAttribute
            {
                Alias = "fourthOverride"
            };

            var serializer = new SerializerBuilder()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .WithAttributeOverride<NameConvention>(nc => nc.AliasTest, attribute)
                .Build();

            serializer.Serialize(writer, input, input.GetType());
            var text = writer.ToString();

            // Todo: use RegEx once FluentAssertions 2.2 is released
            text.TrimEnd('\r', '\n').Should().Be("fourthOverride: Fourth");

            DeserializerBuilder.WithAttributeOverride<NameConvention>(n => n.AliasTest, attribute);
            var output = Deserializer.Deserialize<NameConvention>(UsingReaderFor(text));

            output.AliasTest.Should().Be(input.AliasTest);
        }

        /// <summary>
        /// Roundtrips the derived class.
        /// </summary>
        [Fact]
        // Todo: is the assert on the string necessary?
        public void RoundtripDerivedClass()
        {
            var obj = new InheritanceExample
            {
                SomeScalar = "Hello",
                RegularBase = new Derived { BaseProperty = "foo", DerivedProperty = "bar" }
            };

            var result = DoRoundtripFromObjectTo<InheritanceExample>(
                obj,
                new SerializerBuilder()
                    .WithTagMapping("!InheritanceExample", typeof(InheritanceExample))
                    .WithTagMapping("!Derived", typeof(Derived))
                    .EnsureRoundtrip()
                    .Build(),
                new DeserializerBuilder()
                    .WithTagMapping("!InheritanceExample", typeof(InheritanceExample))
                    .WithTagMapping("!Derived", typeof(Derived))
                    .Build()
            );

            result.SomeScalar.Should().Be("Hello");
            result.RegularBase.Should().BeOfType<Derived>().And
                .Subject.As<Derived>().Should().BeEquivalentTo(new { ChildProp = "bar" }, o => o.ExcludingMissingMembers());
        }

        /// <summary>
        /// Roundtrips the derived class with serialize as.
        /// </summary>
        [Fact]
        public void RoundtripDerivedClassWithSerializeAs()
        {
            var obj = new InheritanceExample
            {
                SomeScalar = "Hello",
                BaseWithSerializeAs = new Derived { BaseProperty = "foo", DerivedProperty = "bar" }
            };

            var result = DoRoundtripFromObjectTo<InheritanceExample>(
                obj,
                new SerializerBuilder()
                    .WithTagMapping("!InheritanceExample", typeof(InheritanceExample))
                    .EnsureRoundtrip()
                    .Build(),
                new DeserializerBuilder()
                    .WithTagMapping("!InheritanceExample", typeof(InheritanceExample))
                    .Build()
            );

            result.BaseWithSerializeAs.Should().BeOfType<Base>().And
                .Subject.As<Base>().Should().BeEquivalentTo(new { ParentProp = "foo" }, o => o.ExcludingMissingMembers());
        }

        /// <summary>
        /// Roundtrips the interface properties.
        /// </summary>
        [Fact]
        public void RoundtripInterfaceProperties()
        {
            AssumingDeserializerWith(new LambdaObjectFactory(t =>
            {
                if (t == typeof(InterfaceExample)) { return new InterfaceExample(); }
                else if (t == typeof(IDerived)) { return new Derived(); }
                return null;
            }));

            var obj = new InterfaceExample
            {
                Derived = new Derived { BaseProperty = "foo", DerivedProperty = "bar" }
            };

            var result = DoRoundtripFromObjectTo<InterfaceExample>(obj);

            result.Derived.Should().BeOfType<Derived>().And
                .Subject.As<IDerived>().Should().BeEquivalentTo(new { BaseProperty = "foo", DerivedProperty = "bar" }, o => o.ExcludingMissingMembers());
        }

        /// <summary>
        /// Deserializes the guid.
        /// </summary>
        [Fact]
        public void DeserializeGuid()
        {
            var stream = Yaml.ReaderFrom("guid.yaml");
            var result = Deserializer.Deserialize<Guid>(stream);

            result.Should().Be(new Guid("9462790d5c44468985425e2dd38ebd98"));
        }

        /// <summary>
        /// Deserializations the of ordered properties.
        /// </summary>
        [Fact]
        public void DeserializationOfOrderedProperties()
        {
            var stream = Yaml.ReaderFrom("ordered-properties.yaml");

            var orderExample = Deserializer.Deserialize<OrderExample>(stream);

            orderExample.Order1.Should().Be("Order1 value");
            orderExample.Order2.Should().Be("Order2 value");
        }

        /// <summary>
        /// Deserializes the enumerable.
        /// </summary>
        [Fact]
        public void DeserializeEnumerable()
        {
            var obj = new[] { new Simple { aaa = "bbb" } };

            var result = DoRoundtripFromObjectTo<IEnumerable<Simple>>(obj);

            result.Should().ContainSingle(item => "bbb".Equals(item.aaa));
        }

        /// <summary>
        /// Deserializes the array.
        /// </summary>
        [Fact]
        public void DeserializeArray()
        {
            var stream = Yaml.ReaderFrom("list.yaml");

            var result = Deserializer.Deserialize<string[]>(stream);

            result.Should().Equal(new[] { "one", "two", "three" });
        }

        /// <summary>
        /// Deserializes the list.
        /// </summary>
        [Fact]
        public void DeserializeList()
        {
            var stream = Yaml.ReaderFrom("list.yaml");

            var result = Deserializer.Deserialize(stream);

            result.Should().BeEquivalentTo(new[] { "one", "two", "three" });
        }

        /// <summary>
        /// Deserializes the explicit list.
        /// </summary>
        [Fact]
        public void DeserializeExplicitList()
        {
            var stream = Yaml.ReaderFrom("list-explicit.yaml");

            var result = new DeserializerBuilder()
                .WithTagMapping("!List", typeof(List<int>))
                .Build()
                .Deserialize(stream);

            result.Should().BeAssignableTo<IList<int>>().And
                .Subject.As<IList<int>>().Should().Equal(3, 4, 5);
        }

        /// <summary>
        /// Roundtrips the list.
        /// </summary>
        [Fact]
        public void RoundtripList()
        {
            var obj = new List<int> { 2, 4, 6 };

            var result = DoRoundtripOn<List<int>>(obj, SerializerBuilder.EnsureRoundtrip().Build());

            result.Should().Equal(obj);
        }

        /// <summary>
        /// Roundtrips the array with type conversion.
        /// </summary>
        [Fact]
        public void RoundtripArrayWithTypeConversion()
        {
            var obj = new object[] { 1, 2, "3" };

            var result = DoRoundtripFromObjectTo<int[]>(obj);

            result.Should().Equal(1, 2, 3);
        }

        /// <summary>
        /// Roundtrips the array of identical objects.
        /// </summary>
        [Fact]
        public void RoundtripArrayOfIdenticalObjects()
        {
            var z = new Simple { aaa = "bbb" };
            var obj = new[] { z, z, z };

            var result = DoRoundtripOn<Simple[]>(obj);

            result.Should().HaveCount(3).And.OnlyContain(x => z.aaa.Equals(x.aaa));
            result[0].Should().BeSameAs(result[1]).And.BeSameAs(result[2]);
        }

        /// <summary>
        /// Deserializes the dictionary.
        /// </summary>
        [Fact]
        public void DeserializeDictionary()
        {
            var stream = Yaml.ReaderFrom("dictionary.yaml");

            var result = Deserializer.Deserialize(stream);

            result.Should().BeAssignableTo<IDictionary<object, object>>().And.Subject
                .As<IDictionary<object, object>>().Should().Equal(new Dictionary<object, object> {
                    { "key1", "value1" },
                    { "key2", "value2" }
                });
        }

        /// <summary>
        /// Deserializes the explicit dictionary.
        /// </summary>
        [Fact]
        public void DeserializeExplicitDictionary()
        {
            var stream = Yaml.ReaderFrom("dictionary-explicit.yaml");

            var result = new DeserializerBuilder()
                .WithTagMapping("!Dictionary", typeof(Dictionary<string, int>))
                .Build()
                .Deserialize(stream);

            result.Should().BeAssignableTo<IDictionary<string, int>>().And.Subject
                .As<IDictionary<string, int>>().Should().Equal(new Dictionary<string, int> {
                    { "key1", 1 },
                    { "key2", 2 }
                });
        }

        /// <summary>
        /// Roundtrips the dictionary.
        /// </summary>
        [Fact]
        public void RoundtripDictionary()
        {
            var obj = new Dictionary<string, string> {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" }
            };

            var result = DoRoundtripFromObjectTo<Dictionary<string, string>>(obj);

            result.Should().Equal(obj);
        }

        /// <summary>
        /// Deserializes the list of dictionaries.
        /// </summary>
        [Fact]
        public void DeserializeListOfDictionaries()
        {
            var stream = Yaml.ReaderFrom("list-of-dictionaries.yaml");

            var result = Deserializer.Deserialize<List<Dictionary<string, string>>>(stream);

            result.Should().BeEquivalentTo(new[] {
                new Dictionary<string, string> {
                    { "connection", "conn1" },
                    { "path", "path1" }
                },
                new Dictionary<string, string> {
                    { "connection", "conn2" },
                    { "path", "path2" }
                }}, opt => opt.WithStrictOrderingFor(root => root));
        }

        /// <summary>
        /// Deserializes the two documents.
        /// </summary>
        [Fact]
        public void DeserializeTwoDocuments()
        {
            var reader = ParserFor(Lines(
                "---",
                "aaa: 111",
                "---",
                "aaa: 222",
                "..."));

            reader.Consume<StreamStart>();
            var one = Deserializer.Deserialize<Simple>(reader);
            var two = Deserializer.Deserialize<Simple>(reader);

            one.Should().BeEquivalentTo(new { aaa = "111" });
            two.Should().BeEquivalentTo(new { aaa = "222" });
        }

        /// <summary>
        /// Deserializes the three documents.
        /// </summary>
        [Fact]
        public void DeserializeThreeDocuments()
        {
            var reader = ParserFor(Lines(
                "---",
                "aaa: 111",
                "---",
                "aaa: 222",
                "---",
                "aaa: 333",
                "..."));

            reader.Consume<StreamStart>();
            var one = Deserializer.Deserialize<Simple>(reader);
            var two = Deserializer.Deserialize<Simple>(reader);
            var three = Deserializer.Deserialize<Simple>(reader);

            reader.Accept<StreamEnd>(out var _).Should().BeTrue("reader should have reached StreamEnd");
            one.Should().BeEquivalentTo(new { aaa = "111" });
            two.Should().BeEquivalentTo(new { aaa = "222" });
            three.Should().BeEquivalentTo(new { aaa = "333" });
        }

        /// <summary>
        /// Serializes the guid.
        /// </summary>
        [Fact]
        public void SerializeGuid()
        {
            var guid = new Guid("{9462790D-5C44-4689-8542-5E2DD38EBD98}");

            var writer = new StringWriter();

            Serializer.Serialize(writer, guid);
            var serialized = writer.ToString();
            Regex.IsMatch(serialized, "^" + guid.ToString("D")).Should().BeTrue("serialized content should contain the guid, but instead contained: " + serialized);
        }

        /// <summary>
        /// Serializes the null object.
        /// </summary>
        [Fact]
        public void SerializeNullObject()
        {
#nullable enable
            object? obj = null;

            var writer = new StringWriter();

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();
            serialized.Should().Be("--- " + writer.NewLine);
#nullable restore
        }

        /// <summary>
        /// Serializations the of null in lists are always emitted without using emit defaults.
        /// </summary>
        [Fact]
        public void SerializationOfNullInListsAreAlwaysEmittedWithoutUsingEmitDefaults()
        {
            var writer = new StringWriter();
            var obj = new[] { "foo", null, "bar" };

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            Regex.Matches(serialized, "-").Count.Should().Be(3, "there should have been 3 elements");
        }

        /// <summary>
        /// Serializations the of null in lists are always emitted when using emit defaults.
        /// </summary>
        [Fact]
        public void SerializationOfNullInListsAreAlwaysEmittedWhenUsingEmitDefaults()
        {
            var writer = new StringWriter();
            var obj = new[] { "foo", null, "bar" };

            SerializerBuilder.Build().Serialize(writer, obj);
            var serialized = writer.ToString();

            Regex.Matches(serialized, "-").Count.Should().Be(3, "there should have been 3 elements");
        }

        /// <summary>
        /// Serializations the includes key when emitting defaults.
        /// </summary>
        [Fact]
        public void SerializationIncludesKeyWhenEmittingDefaults()
        {
            var writer = new StringWriter();
            var obj = new Example { MyString = null };

            SerializerBuilder.Build().Serialize(writer, obj, typeof(Example));

            writer.ToString().Should().Contain("MyString");
        }

        /// <summary>
        /// Serializations the includes key from anonymous type when emitting defaults.
        /// </summary>
        [Fact]
        [Trait("Motive", "Bug fix")]
        public void SerializationIncludesKeyFromAnonymousTypeWhenEmittingDefaults()
        {
            var writer = new StringWriter();
            var obj = new { MyString = (string)null };

            SerializerBuilder.Build().Serialize(writer, obj, obj.GetType());

            writer.ToString().Should().Contain("MyString");
        }

        /// <summary>
        /// Serializations the does not include key when disregarding defaults.
        /// </summary>
        [Fact]
        public void SerializationDoesNotIncludeKeyWhenDisregardingDefaults()
        {
            var writer = new StringWriter();
            var obj = new Example { MyString = null };

            SerializerBuilder
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults);

            Serializer.Serialize(writer, obj, typeof(Example));

            writer.ToString().Should().NotContain("MyString");
        }

        /// <summary>
        /// Serializations the of defaults work in json.
        /// </summary>
        [Fact]
        public void SerializationOfDefaultsWorkInJson()
        {
            var writer = new StringWriter();
            var obj = new Example { MyString = null };

            SerializerBuilder.JsonCompatible().Build().Serialize(writer, obj, typeof(Example));

            writer.ToString().Should().Contain("MyString");
        }

        /// <summary>
        /// Serializations the of long keys works in json.
        /// </summary>
        [Fact]
        public void SerializationOfLongKeysWorksInJson()
        {
            var writer = new StringWriter();
            var obj = new Dictionary<string, string>
            {
                { new string('x', 3000), "extremely long key" }
            };

            SerializerBuilder.JsonCompatible().Build().Serialize(writer, obj, typeof(Dictionary<string, string>));

            writer.ToString().Should().NotContain("?");
        }

        /// <summary>
        /// Serializations the of anchor works in json.
        /// </summary>
        [Fact]
        public void SerializationOfAnchorWorksInJson()
        {
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(Yaml.ReaderForText(@"
x: &anchor1
  z:
    v: 1
y:
  k: *anchor1"));

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            serializer.Serialize(yamlObject).Trim().Should()
                .BeEquivalentTo(@"{""x"": {""z"": {""v"": ""1""}}, ""y"": {""k"": {""z"": {""v"": ""1""}}}}");
        }

        /// <summary>
        /// Serializations the of utf32 works in json.
        /// </summary>
        [Fact]
        public void SerializationOfUtf32WorksInJson()
        {
            var obj = new { TestProperty = "Sea life \U0001F99E" };

            SerializerBuilder.JsonCompatible().Build().Serialize(obj).Trim().Should()
                .Be(@"{""TestProperty"": ""Sea life \uD83E\uDD9E""}");
        }

        /// <summary>
        /// Deserializations the of defaults work in json.
        /// </summary>
        [Fact]
        // Todo: this is actually roundtrip
        public void DeserializationOfDefaultsWorkInJson()
        {
            var writer = new StringWriter();
            var obj = new Example { MyString = null };

            SerializerBuilder.EnsureRoundtrip().JsonCompatible().Build().Serialize(writer, obj, typeof(Example));
            var result = Deserializer.Deserialize<Example>(UsingReaderFor(writer));

            result.MyString.Should().BeNull();
        }

        /// <summary>
        /// Nulls the round trip.
        /// </summary>
        [Fact]
        public void NullsRoundTrip()
        {
            var writer = new StringWriter();
            var obj = new Example { MyString = null };

            SerializerBuilder.EnsureRoundtrip().Build().Serialize(writer, obj, typeof(Example));
            var result = Deserializer.Deserialize<Example>(UsingReaderFor(writer));

            result.MyString.Should().BeNull();
        }

        /// <summary>
        /// Serializations the of numerics as json rount trip.
        /// </summary>
        [Fact]
        public void SerializationOfNumericsAsJsonRountTrip()
        {
            var serializer = new SerializerBuilder().JsonCompatible().Build();
            var deserializer = new DeserializerBuilder().Build();

            var data = new
            {
                FloatValue1 = float.MinValue,
                FloatValue2 = float.MaxValue,
                FloatValue3 = float.NaN,
                FloatValue4 = float.PositiveInfinity,
                FloatValue5 = float.NegativeInfinity,
                FloatValue6 = 0.0f,
                DoubleValue1 = double.MinValue,
                DoubleValue2 = double.MaxValue,
                DoubleValue3 = double.NaN,
                DoubleValue4 = double.PositiveInfinity,
                DoubleValue5 = double.NegativeInfinity,
                DoubleValue6 = 3.0d,
                DecimalValue1 = decimal.MinValue,
                DecimalValue2 = decimal.MaxValue,
                DecimalValue3 = 1.234567890d,
            };

            var json = serializer.Serialize(data);

#if NETFRAMEWORK
            json.Should().Contain("\"FloatValue3\": \"NaN\"");
            json.Should().Contain("\"FloatValue4\": \"Infinity\"");
            json.Should().Contain("\"FloatValue5\": \"-Infinity\"");

            json.Should().Contain("\"DoubleValue3\": \"NaN\"");
            json.Should().Contain("\"DoubleValue4\": \"Infinity\"");
            json.Should().Contain("\"DoubleValue5\": \"-Infinity\"");
#else
            // Run JSON roundtrip with System.Text.Json and Newtonsoft.Json
            var systemTextJson = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json, new System.Text.Json.JsonSerializerOptions { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals }).ToString();
            var newtonsoftJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(json).ToString(Newtonsoft.Json.Formatting.None);

            // Deserialize JSON  with YamlDotNet
            var systemTextJsonResult = deserializer.Deserialize<Dictionary<string, object>>(systemTextJson);
            var newtonsoftJsonResult = deserializer.Deserialize<Dictionary<string, object>>(newtonsoftJson);

            // Assert
            systemTextJsonResult.Should().BeEquivalentTo(newtonsoftJsonResult);
#endif
        }

        /// <summary>
        /// Deserializations the of enum works in json.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        [Theory]
        [InlineData(typeof(SByteEnum))]
        [InlineData(typeof(ByteEnum))]
        [InlineData(typeof(Int16Enum))]
        [InlineData(typeof(UInt16Enum))]
        [InlineData(typeof(Int32Enum))]
        [InlineData(typeof(UInt32Enum))]
        [InlineData(typeof(Int64Enum))]
        [InlineData(typeof(UInt64Enum))]
        public void DeserializationOfEnumWorksInJson(Type enumType)
        {
            var defaultEnumValue = 0;
            var nonDefaultEnumValue = Enum.GetValues(enumType).GetValue(1);

            var jsonSerializer = SerializerBuilder.EnsureRoundtrip().JsonCompatible().Build();
            var jsonSerializedEnum = jsonSerializer.Serialize(nonDefaultEnumValue);

            nonDefaultEnumValue.Should().NotBe(defaultEnumValue);
            jsonSerializedEnum.Should().Contain($"\"{nonDefaultEnumValue}\"");
        }

        /// <summary>
        /// Serializations the of ordered properties.
        /// </summary>
        [Fact]
        public void SerializationOfOrderedProperties()
        {
            var obj = new OrderExample();
            var writer = new StringWriter();

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should()
                .Be("Order1: Order1 value\r\nOrder2: Order2 value\r\n".NormalizeNewLines(), "the properties should be in the right order");
        }

        /// <summary>
        /// Serializations the respects yaml ignore attribute.
        /// </summary>
        [Fact]
        public void SerializationRespectsYamlIgnoreAttribute()
        {

            var writer = new StringWriter();
            var obj = new IgnoreExample();

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should().NotContain("IgnoreMe");
        }

        /// <summary>
        /// Serializations the respects yaml ignore attribute of derived classes.
        /// </summary>
        [Fact]
        public void SerializationRespectsYamlIgnoreAttributeOfDerivedClasses()
        {

            var writer = new StringWriter();
            var obj = new IgnoreExampleDerived();

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should().NotContain("IgnoreMe");
        }

        /// <summary>
        /// Serializations the respects yaml ignore override.
        /// </summary>
        [Fact]
        public void SerializationRespectsYamlIgnoreOverride()
        {

            var writer = new StringWriter();
            var obj = new Simple();

            var ignore = new YamlIgnoreAttribute();
            var serializer = new SerializerBuilder()
                .WithAttributeOverride<Simple>(s => s.aaa, ignore)
                .Build();

            serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should().NotContain("aaa");
        }

        /// <summary>
        /// Serializations the respects scalar style.
        /// </summary>
        [Fact]
        public void SerializationRespectsScalarStyle()
        {
            var writer = new StringWriter();
            var obj = new ScalarStyleExample();

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should()
                .Be("LiteralString: |-\r\n  Test\r\nDoubleQuotedString: \"Test\"\r\n".NormalizeNewLines(), "the properties should be specifically styled");
        }

        /// <summary>
        /// Serializations the respects scalar style override.
        /// </summary>
        [Fact]
        public void SerializationRespectsScalarStyleOverride()
        {
            var writer = new StringWriter();
            var obj = new ScalarStyleExample();

            var serializer = new SerializerBuilder()
                .WithAttributeOverride<ScalarStyleExample>(e => e.LiteralString, new YamlMemberAttribute { ScalarStyle = ScalarStyle.DoubleQuoted })
                .WithAttributeOverride<ScalarStyleExample>(e => e.DoubleQuotedString, new YamlMemberAttribute { ScalarStyle = ScalarStyle.Literal })
                .Build();

            serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should()
                .Be("LiteralString: \"Test\"\r\nDoubleQuotedString: |-\r\n  Test\r\n".NormalizeNewLines(), "the properties should be specifically styled");
        }

        /// <summary>
        /// Serializations the respects default scalar style.
        /// </summary>
        [Fact]
        public void SerializationRespectsDefaultScalarStyle()
        {
            var writer = new StringWriter();
            var obj = new MixedFormatScalarStyleExample(new string[] { "01", "0.1", "myString" });

            var serializer = new SerializerBuilder().WithDefaultScalarStyle(ScalarStyle.SingleQuoted).Build();

            serializer.Serialize(writer, obj);

            var yaml = writer.ToString();

            var expected = Yaml.Text(@"
                Data:
                - '01'
                - '0.1'
                - 'myString'
            ");

            Assert.Equal(expected.NormalizeNewLines(), yaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Serializations the derived attribute override.
        /// </summary>
        [Fact]
        public void SerializationDerivedAttributeOverride()
        {
            var writer = new StringWriter();
            var obj = new Derived { DerivedProperty = "Derived", BaseProperty = "Base" };

            var ignore = new YamlIgnoreAttribute();
            var serializer = new SerializerBuilder()
                .WithAttributeOverride<Derived>(d => d.DerivedProperty, ignore)
                .Build();

            serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should()
                .Be("BaseProperty: Base\r\n".NormalizeNewLines(), "the derived property should be specifically ignored");
        }

        /// <summary>
        /// Serializations the base attribute override.
        /// </summary>
        [Fact]
        public void SerializationBaseAttributeOverride()
        {
            var writer = new StringWriter();
            var obj = new Derived { DerivedProperty = "Derived", BaseProperty = "Base" };

            var ignore = new YamlIgnoreAttribute();
            var serializer = new SerializerBuilder()
                .WithAttributeOverride<Base>(b => b.BaseProperty, ignore)
                .Build();

            serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should()
                .Be("DerivedProperty: Derived\r\n".NormalizeNewLines(), "the base property should be specifically ignored");
        }

        /// <summary>
        /// Serializations the skips property when using default value attribute.
        /// </summary>
        [Fact]
        public void SerializationSkipsPropertyWhenUsingDefaultValueAttribute()
        {
            var writer = new StringWriter();
            var obj = new DefaultsExample { Value = DefaultsExample.DefaultValue };

            SerializerBuilder
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults);

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should().NotContain("Value");
        }

        /// <summary>
        /// Serializations the emits property when using emit defaults and default value attribute.
        /// </summary>
        [Fact]
        public void SerializationEmitsPropertyWhenUsingEmitDefaultsAndDefaultValueAttribute()
        {
            var writer = new StringWriter();
            var obj = new DefaultsExample { Value = DefaultsExample.DefaultValue };

            SerializerBuilder.Build().Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should().Contain("Value");
        }

        /// <summary>
        /// Serializations the emits property when value differ from default value attribute.
        /// </summary>
        [Fact]
        public void SerializationEmitsPropertyWhenValueDifferFromDefaultValueAttribute()
        {
            var writer = new StringWriter();
            var obj = new DefaultsExample { Value = "non-default" };

            Serializer.Serialize(writer, obj);
            var serialized = writer.ToString();

            serialized.Should().Contain("Value");
        }

        /// <summary>
        /// Serializings the a generic dictionary should not throw target exception.
        /// </summary>
        [Fact]
        public void SerializingAGenericDictionaryShouldNotThrowTargetException()
        {
            var obj = new CustomGenericDictionary {
                { "hello", "world" }
            };

            Action action = () => Serializer.Serialize(new StringWriter(), obj);

            action.Should().NotThrow<TargetException>();
        }

        /// <summary>
        /// Serializations the utilize naming conventions.
        /// </summary>
        [Fact]
        public void SerializationUtilizeNamingConventions()
        {
            var convention = A.Fake<INamingConvention>();
            A.CallTo(() => convention.Apply(A<string>._)).ReturnsLazily((string x) => x);
            var obj = new NameConvention { FirstTest = "1", SecondTest = "2" };

            var serializer = new SerializerBuilder()
                .WithNamingConvention(convention)
                .Build();

            serializer.Serialize(new StringWriter(), obj);

            A.CallTo(() => convention.Apply("FirstTest")).MustHaveHappened();
            A.CallTo(() => convention.Apply("SecondTest")).MustHaveHappened();
        }

        /// <summary>
        /// Deserializations the utilize naming conventions.
        /// </summary>
        [Fact]
        public void DeserializationUtilizeNamingConventions()
        {
            var convention = A.Fake<INamingConvention>();
            A.CallTo(() => convention.Apply(A<string>._)).ReturnsLazily((string x) => x);
            var text = Lines(
                "FirstTest: 1",
                "SecondTest: 2");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(convention)
                .Build();

            deserializer.Deserialize<NameConvention>(UsingReaderFor(text));

            A.CallTo(() => convention.Apply("FirstTest")).MustHaveHappened();
            A.CallTo(() => convention.Apply("SecondTest")).MustHaveHappened();
        }

        /// <summary>
        /// Types the converter is used on list items.
        /// </summary>
        [Fact]
        public void TypeConverterIsUsedOnListItems()
        {
            var text = Lines(
                "- !{type}",
                "  Left: hello",
                "  Right: world")
                .TemplatedOn<Convertible>();

            var list = new DeserializerBuilder()
                .WithTagMapping("!Convertible", typeof(Convertible))
                .Build()
                .Deserialize<List<string>>(UsingReaderFor(text));

            list
                .Should().NotBeNull()
                .And.ContainSingle(c => c.Equals("[hello, world]"));
        }

        /// <summary>
        /// Backreferences the are merged with mappings.
        /// </summary>
        [Fact]
        public void BackreferencesAreMergedWithMappings()
        {
            var stream = Yaml.ReaderFrom("backreference.yaml");

            var parser = new MergingParser(new Parser(stream));
            var result = Deserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(parser);

            var alias = result["alias"];
            alias.Should()
                .Contain("key1", "value1", "key1 should be inherited from the backreferenced mapping")
                .And.Contain("key2", "Overriding key2", "key2 should be overriden by the actual mapping")
                .And.Contain("key3", "value3", "key3 is defined in the actual mapping");
        }

        /// <summary>
        /// Mergings the does not produce duplicate anchors.
        /// </summary>
        [Fact]
        public void MergingDoesNotProduceDuplicateAnchors()
        {
            var parser = new MergingParser(Yaml.ParserForText(@"
                anchor: &default
                  key1: &myValue value1
                  key2: value2
                alias:
                  <<: *default
                  key2: Overriding key2
                  key3: value3
                useMyValue:
                  key: *myValue
            "));
            var result = Deserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(parser);

            var alias = result["alias"];
            alias.Should()
                .Contain("key1", "value1", "key1 should be inherited from the backreferenced mapping")
                .And.Contain("key2", "Overriding key2", "key2 should be overriden by the actual mapping")
                .And.Contain("key3", "value3", "key3 is defined in the actual mapping");

            result["useMyValue"].Should()
                .Contain("key", "value1", "key should be copied");
        }

        /// <summary>
        /// Examples the from specification is handled correctly.
        /// </summary>
        [Fact]
        public void ExampleFromSpecificationIsHandledCorrectly()
        {
            var parser = new MergingParser(Yaml.ParserForText(@"
                obj:
                  - &CENTER { x: 1, y: 2 }
                  - &LEFT { x: 0, y: 2 }
                  - &BIG { r: 10 }
                  - &SMALL { r: 1 }

                # All the following maps are equal:
                results:
                  - # Explicit keys
                    x: 1
                    y: 2
                    r: 10
                    label: center/big

                  - # Merge one map
                    << : *CENTER
                    r: 10
                    label: center/big

                  - # Merge multiple maps
                    << : [ *CENTER, *BIG ]
                    label: center/big

                  - # Override
                    << : [ *BIG, *LEFT, *SMALL ]
                    x: 1
                    label: center/big
            "));

            var result = Deserializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(parser);

            var index = 0;
            foreach (var mapping in result["results"])
            {
                mapping.Should()
                    .Contain("x", "1", "'x' should be '1' in result #{0}", index)
                    .And.Contain("y", "2", "'y' should be '2' in result #{0}", index)
                    .And.Contain("r", "10", "'r' should be '10' in result #{0}", index)
                    .And.Contain("label", "center/big", "'label' should be 'center/big' in result #{0}", index);

                ++index;
            }
        }

        /// <summary>
        /// Merges the nested reference correctly.
        /// </summary>
        [Fact]
        public void MergeNestedReferenceCorrectly()
        {
            var parser = new MergingParser(Yaml.ParserForText(@"
                base1: &level1
                  key: X
                  level: 1
                base2: &level2
                  <<: *level1
                  key: Y
                  level: 2
                derived1:
                  <<: *level1
                  key: D1
                derived2:
                  <<: *level2
                  key: D2
                derived3:
                  <<: [ *level1, *level2 ]
                  key: D3
            "));

            var result = Deserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(parser);

            result["derived1"].Should()
                .Contain("key", "D1", "key should be overriden by the actual mapping")
                .And.Contain("level", "1", "level should be inherited from the backreferenced mapping");

            result["derived2"].Should()
                .Contain("key", "D2", "key should be overriden by the actual mapping")
                .And.Contain("level", "2", "level should be inherited from the backreferenced mapping");

            result["derived3"].Should()
                .Contain("key", "D3", "key should be overriden by the actual mapping")
                .And.Contain("level", "1", "level should be inherited from the backreferenced mapping");
        }

        /// <summary>
        /// Ignores the extra properties if wanted.
        /// </summary>
        [Fact]
        public void IgnoreExtraPropertiesIfWanted()
        {
            var text = Lines("aaa: hello", "bbb: world");
            DeserializerBuilder.IgnoreUnmatchedProperties();
            var actual = Deserializer.Deserialize<Simple>(UsingReaderFor(text));
            actual.aaa.Should().Be("hello");
        }

        /// <summary>
        /// Donts the ignore extra properties if wanted.
        /// </summary>
        [Fact]
        public void DontIgnoreExtraPropertiesIfWanted()
        {
            var text = Lines("aaa: hello", "bbb: world");
            var actual = Record.Exception(() => Deserializer.Deserialize<Simple>(UsingReaderFor(text)));
            Assert.IsType<YamlException>(actual);
            ((YamlException)actual).Start.Column.Should().Be(1);
            ((YamlException)actual).Start.Line.Should().Be(2);
            ((YamlException)actual).Start.Index.Should().Be(12);
            ((YamlException)actual).End.Column.Should().Be(4);
            ((YamlException)actual).End.Line.Should().Be(2);
            ((YamlException)actual).End.Index.Should().Be(15);
            ((YamlException)actual).Message.Should().Be("Property 'bbb' not found on type 'YamlDotNet.Test.Serialization.Simple'.");
        }

        /// <summary>
        /// Ignores the extra properties if wanted before.
        /// </summary>
        [Fact]
        public void IgnoreExtraPropertiesIfWantedBefore()
        {
            var text = Lines("bbb: [200,100]", "aaa: hello");
            DeserializerBuilder.IgnoreUnmatchedProperties();
            var actual = Deserializer.Deserialize<Simple>(UsingReaderFor(text));
            actual.aaa.Should().Be("hello");
        }

        /// <summary>
        /// Ignores the extra properties if wanted naming scheme.
        /// </summary>
        [Fact]
        public void IgnoreExtraPropertiesIfWantedNamingScheme()
        {
            var text = Lines(
                    "scratch: 'scratcher'",
                    "deleteScratch: false",
                    "notScratch: 9443",
                    "notScratch: 192.168.1.30",
                    "mappedScratch:",
                    "- '/work/'"
                );

            DeserializerBuilder
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties();

            var actual = Deserializer.Deserialize<SimpleScratch>(UsingReaderFor(text));
            actual.Scratch.Should().Be("scratcher");
            actual.DeleteScratch.Should().Be(false);
            actual.MappedScratch.Should().ContainInOrder(new[] { "/work/" });
        }

        /// <summary>
        /// Invalids the type conversions produce proper exceptions.
        /// </summary>
        [Fact]
        public void InvalidTypeConversionsProduceProperExceptions()
        {
            var text = Lines("- 1", "- two", "- 3");

            var sut = new Deserializer();
            var exception = Assert.Throws<YamlException>(() => sut.Deserialize<List<int>>(UsingReaderFor(text)));

            Assert.Equal(2, exception.Start.Line);
            Assert.Equal(3, exception.Start.Column);
        }

        /// <summary>
        /// Values the allowed after document start token.
        /// </summary>
        /// <param name="text">The text.</param>
        [Theory]
        [InlineData("blah")]
        [InlineData("hello=world")]
        [InlineData("+190:20:30")]
        [InlineData("x:y")]
        public void ValueAllowedAfterDocumentStartToken(string text)
        {
            var value = Lines("--- " + text);

            var sut = new Deserializer();
            var actual = sut.Deserialize<string>(UsingReaderFor(value));

            Assert.Equal(text, actual);
        }

        /// <summary>
        /// Mappings the disallowed after document start token.
        /// </summary>
        [Fact]
        public void MappingDisallowedAfterDocumentStartToken()
        {
            var value = Lines("--- x: y");

            var sut = new Deserializer();
            var exception = Assert.Throws<SemanticErrorException>(() => sut.Deserialize<string>(UsingReaderFor(value)));

            Assert.Equal(1, exception.Start.Line);
            Assert.Equal(6, exception.Start.Column);
        }

        /// <summary>
        /// Serializes the dynamic property and apply naming convention.
        /// </summary>
        [Fact]
        public void SerializeDynamicPropertyAndApplyNamingConvention()
        {
            dynamic obj = new ExpandoObject();
            obj.property_one = new ExpandoObject();
            ((IDictionary<string, object>)obj.property_one).Add("new_key_here", "new_value");

            var mockNamingConvention = A.Fake<INamingConvention>();
            A.CallTo(() => mockNamingConvention.Apply(A<string>.Ignored)).Returns("xxx");

            var serializer = new SerializerBuilder()
                .WithNamingConvention(mockNamingConvention)
                .Build();

            var writer = new StringWriter();
            serializer.Serialize(writer, obj);

            writer.ToString().Should().Contain("xxx: new_value");
        }

        /// <summary>
        /// Serializes the generic dictionary property and do not apply naming convention.
        /// </summary>
        [Fact]
        public void SerializeGenericDictionaryPropertyAndDoNotApplyNamingConvention()
        {
            var obj = new Dictionary<string, object>
            {
                ["property_one"] = new GenericTestDictionary<string, object>()
            };

            ((IDictionary<string, object>)obj["property_one"]).Add("new_key_here", "new_value");

            var mockNamingConvention = A.Fake<INamingConvention>();
            A.CallTo(() => mockNamingConvention.Apply(A<string>.Ignored)).Returns("xxx");

            var serializer = new SerializerBuilder()
                .WithNamingConvention(mockNamingConvention)
                .Build();

            var writer = new StringWriter();
            serializer.Serialize(writer, obj);

            writer.ToString().Should().Contain("new_key_here: new_value");
        }

        /// <summary>
        /// Specials the floats are handled correctly.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        [Theory, MemberData(nameof(SpecialFloats))]
        public void SpecialFloatsAreHandledCorrectly(FloatTestCase testCase)
        {
            var buffer = new StringWriter();
            Serializer.Serialize(buffer, testCase.Value);

            var firstLine = buffer.ToString().Split('\r', '\n')[0];
            Assert.Equal(testCase.ExpectedTextRepresentation, firstLine);

            var deserializer = new Deserializer();
            var deserializedValue = deserializer.Deserialize(new StringReader(buffer.ToString()), testCase.Value.GetType());

            Assert.Equal(testCase.Value, deserializedValue);
        }

        /// <summary>
        /// Rounds the trip special enum.
        /// </summary>
        /// <param name="testValue">The test value.</param>
        [Theory]
        [InlineData(TestEnum.True)]
        [InlineData(TestEnum.False)]
        [InlineData(TestEnum.ABC)]
        [InlineData(TestEnum.Null)]
        public void RoundTripSpecialEnum(object testValue)
        {
            var test = new TestEnumTestCase { TestEnum = (TestEnum)testValue };
            var serializer = new SerializerBuilder().WithQuotingNecessaryStrings().Build();
            var deserializer = new DeserializerBuilder().Build();
            var serialized = serializer.Serialize(test);
            var actual = deserializer.Deserialize<TestEnumTestCase>(serialized);
            Assert.Equal(testValue, actual.TestEnum);
        }

        /// <summary>
        /// Empties the strings are quoted.
        /// </summary>
        [Fact]
        public void EmptyStringsAreQuoted()
        {
            var serializer = new SerializerBuilder().WithQuotingNecessaryStrings().Build();
            var o = new { test = string.Empty };
            var result = serializer.Serialize(o);
            var expected = $"test: \"\"{Environment.NewLine}";
            Assert.Equal(expected, result);
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Enums the serialization uses enum member attribute.
        /// </summary>
        [Fact]
        public void EnumSerializationUsesEnumMemberAttribute()
        {
            var serializer = new SerializerBuilder().Build();
            var actual = serializer.Serialize(EnumMemberedEnum.Hello);
            Assert.Equal("goodbye", actual.TrimNewLines());
        }

        /// <summary>
        /// Enums the serialization uses enum member attribute with empty value.
        /// </summary>
        [Fact]
        public void EnumSerializationUsesEnumMemberAttributeWithEmptyValue()
        {
            var serializer = new SerializerBuilder().Build();
            var actual = serializer.Serialize(new { Test = EnumMemberedEnum.EmptyValue });
            Assert.Equal("Test: ''", actual.TrimNewLines());
        }

        /// <summary>
        /// Enums the serialization uses enum member attribute with null value.
        /// </summary>
        [Fact]
        public void EnumSerializationUsesEnumMemberAttributeWithNullValue()
        {
            var serializer = new SerializerBuilder().Build();
            var actual = serializer.Serialize(EnumMemberedEnum.NullValue);
            Assert.Equal("NullValue", actual.TrimNewLines());
        }

        /// <summary>
        /// The enum membered enum.
        /// </summary>
        public enum EnumMemberedEnum
        {
            [System.Runtime.Serialization.EnumMember(Value = "goodbye")]
            Hello = 1,

            [System.Runtime.Serialization.EnumMember(Value = "")]
            EmptyValue = 2,

            [System.Runtime.Serialization.EnumMember()]
            NullValue = 3
        }
#endif

        /// <summary>
        /// The test enum.
        /// </summary>
        public enum TestEnum
        {
            True,
            False,
            ABC,
            Null
        }

        /// <summary>
        /// The test enum test case.
        /// </summary>
        public class TestEnumTestCase
        {
            /// <summary>
            /// Gets or sets the test enum.
            /// </summary>
            public TestEnum TestEnum { get; set; }
        }

        /// <summary>
        /// The float test case.
        /// </summary>
        public class FloatTestCase
        {
            private readonly string description;
            /// <summary>
            /// Gets the value.
            /// </summary>
            public object Value { get; private set; }
            /// <summary>
            /// Gets the expected text representation.
            /// </summary>
            public string ExpectedTextRepresentation { get; private set; }

            public FloatTestCase(string description, object value, string expectedTextRepresentation)
            {
                this.description = description;
                Value = value;
                ExpectedTextRepresentation = expectedTextRepresentation;
            }

            /// <summary>
            /// Tos the string.
            /// </summary>
            /// <returns>A string.</returns>
            public override string ToString()
            {
                return description;
            }
        }

        /// <summary>
        /// Gets the special floats.
        /// </summary>
        public static IEnumerable<object[]> SpecialFloats
        {
            get
            {
                return
                    new[]
                    {
                        new FloatTestCase("double.NaN", double.NaN, ".nan"),
                        new FloatTestCase("double.PositiveInfinity", double.PositiveInfinity, ".inf"),
                        new FloatTestCase("double.NegativeInfinity", double.NegativeInfinity, "-.inf"),
                        new FloatTestCase("double.Epsilon", double.Epsilon,  double.Epsilon.ToString("G", CultureInfo.InvariantCulture)),
                        new FloatTestCase("double.26.67", 26.67D, "26.67"),

                        new FloatTestCase("float.NaN", float.NaN, ".nan"),
                        new FloatTestCase("float.PositiveInfinity", float.PositiveInfinity, ".inf"),
                        new FloatTestCase("float.NegativeInfinity", float.NegativeInfinity, "-.inf"),
                        new FloatTestCase("float.Epsilon", float.Epsilon, float.Epsilon.ToString("G", CultureInfo.InvariantCulture)),
                        new FloatTestCase("float.26.67", 26.67F, "26.67"),

#if NET
                        new FloatTestCase("double.MinValue", double.MinValue, double.MinValue.ToString("G", CultureInfo.InvariantCulture)),
                        new FloatTestCase("double.MaxValue", double.MaxValue, double.MaxValue.ToString("G", CultureInfo.InvariantCulture)),
                        new FloatTestCase("float.MinValue", float.MinValue, float.MinValue.ToString("G", CultureInfo.InvariantCulture)),
                        new FloatTestCase("float.MaxValue", float.MaxValue, float.MaxValue.ToString("G", CultureInfo.InvariantCulture)),
#endif
                    }
                    .Select(tc => new object[] { tc });
            }
        }

        /// <summary>
        /// Negatives the integers can be deserialized.
        /// </summary>
        [Fact]
        public void NegativeIntegersCanBeDeserialized()
        {
            var deserializer = new Deserializer();

            var value = deserializer.Deserialize<int>(Yaml.ReaderForText(@"
                '-123'
            "));
            Assert.Equal(-123, value);
        }

        /// <summary>
        /// Generics the dictionary that does not implement i dictionary can be deserialized.
        /// </summary>
        [Fact]
        public void GenericDictionaryThatDoesNotImplementIDictionaryCanBeDeserialized()
        {
            var sut = new Deserializer();
            var deserialized = sut.Deserialize<GenericTestDictionary<string, string>>(Yaml.ReaderForText(@"
                a: 1
                b: 2
            "));

            Assert.Equal("1", deserialized["a"]);
            Assert.Equal("2", deserialized["b"]);
        }

        /// <summary>
        /// Generics the list that does not implement i list can be deserialized.
        /// </summary>
        [Fact]
        public void GenericListThatDoesNotImplementIListCanBeDeserialized()
        {
            var sut = new Deserializer();
            var deserialized = sut.Deserialize<GenericTestList<string>>(Yaml.ReaderForText(@"
                - a
                - b
            "));

            Assert.Contains("a", deserialized);
            Assert.Contains("b", deserialized);
        }

        /// <summary>
        /// Guids the should be quoted when serialized as json.
        /// </summary>
        [Fact]
        public void GuidsShouldBeQuotedWhenSerializedAsJson()
        {
            var sut = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var yamlAsJson = new StringWriter();
            sut.Serialize(yamlAsJson, new
            {
                id = Guid.Empty
            });

            Assert.Contains("\"00000000-0000-0000-0000-000000000000\"", yamlAsJson.ToString());
        }

        /// <summary>
        /// The foo.
        /// </summary>
        public class Foo
        {
            /// <summary>
            /// Gets or sets a value indicating whether is required.
            /// </summary>
            public bool IsRequired { get; set; }
        }

        /// <summary>
        /// Attributes the overrides and naming convention do not conflict.
        /// </summary>
        [Fact]
        public void AttributeOverridesAndNamingConventionDoNotConflict()
        {
            var namingConvention = CamelCaseNamingConvention.Instance;

            var yamlMember = new YamlMemberAttribute
            {
                Alias = "Required"
            };

            var serializer = new SerializerBuilder()
                .WithNamingConvention(namingConvention)
                .WithAttributeOverride<Foo>(f => f.IsRequired, yamlMember)
                .Build();

            var yaml = serializer.Serialize(new Foo { IsRequired = true });
            Assert.Contains("required: true", yaml);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(namingConvention)
                .WithAttributeOverride<Foo>(f => f.IsRequired, yamlMember)
                .Build();

            var deserializedFoo = deserializer.Deserialize<Foo>(yaml);
            Assert.True(deserializedFoo.IsRequired);
        }

        /// <summary>
        /// Yamls the convertibles are able to emit and parse comments.
        /// </summary>
        [Fact]
        public void YamlConvertiblesAreAbleToEmitAndParseComments()
        {
            var serializer = new Serializer();
            var yaml = serializer.Serialize(new CommentWrapper<string> { Comment = "A comment", Value = "The value" });

            var deserializer = new Deserializer();
            var parser = new Parser(new Scanner(new StringReader(yaml), skipComments: false));
            var parsed = deserializer.Deserialize<CommentWrapper<string>>(parser);

            Assert.Equal("A comment", parsed.Comment);
            Assert.Equal("The value", parsed.Value);
        }

        /// <summary>
        /// The comment wrapper.
        /// </summary>
        public class CommentWrapper<T> : IYamlConvertible
        {
            /// <summary>
            /// Gets or sets the comment.
            /// </summary>
            public string Comment { get; set; }
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public T Value { get; set; }

            /// <summary>
            /// Reads the.
            /// </summary>
            /// <param name="parser">The parser.</param>
            /// <param name="expectedType">The expected type.</param>
            /// <param name="nestedObjectDeserializer">The nested object deserializer.</param>
            public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
            {
                if (parser.TryConsume<Comment>(out var comment))
                {
                    Comment = comment.Value;
                }

                Value = (T)nestedObjectDeserializer(typeof(T));
            }

            /// <summary>
            /// Writes the.
            /// </summary>
            /// <param name="emitter">The emitter.</param>
            /// <param name="nestedObjectSerializer">The nested object serializer.</param>
            public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
            {
                if (!string.IsNullOrEmpty(Comment))
                {
                    emitter.Emit(new Comment(Comment, false));
                }

                nestedObjectSerializer(Value, typeof(T));
            }
        }

        /// <summary>
        /// Deserializations the of u int64 succeeds.
        /// </summary>
        /// <param name="value">The value.</param>
        [Theory]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(0x8000000000000000UL)]
        public void DeserializationOfUInt64Succeeds(ulong value)
        {
            var yaml = new Serializer().Serialize(value);
            Assert.Contains(value.ToString(), yaml);

            var parsed = new Deserializer().Deserialize<ulong>(yaml);
            Assert.Equal(value, parsed);
        }

        /// <summary>
        /// Deserializations the of int64 succeeds.
        /// </summary>
        /// <param name="value">The value.</param>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0L)]
        public void DeserializationOfInt64Succeeds(long value)
        {
            var yaml = new Serializer().Serialize(value);
            Assert.Contains(value.ToString(), yaml);

            var parsed = new Deserializer().Deserialize<long>(yaml);
            Assert.Equal(value, parsed);
        }

        /// <summary>
        /// The anchors overwriting test case.
        /// </summary>
        public class AnchorsOverwritingTestCase
        {
            /// <summary>
            /// Gets or sets the a.
            /// </summary>
            public List<string> a { get; set; }
            /// <summary>
            /// Gets or sets the b.
            /// </summary>
            public List<string> b { get; set; }
            /// <summary>
            /// Gets or sets the c.
            /// </summary>
            public List<string> c { get; set; }
            /// <summary>
            /// Gets or sets the d.
            /// </summary>
            public List<string> d { get; set; }
        }

        /// <summary>
        /// Deserializations the of stream with duplicate anchors succeeds.
        /// </summary>
        [Fact]
        public void DeserializationOfStreamWithDuplicateAnchorsSucceeds()
        {
            var yaml = Yaml.ParserForResource("anchors-overwriting.yaml");
            var serializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            var deserialized = serializer.Deserialize<AnchorsOverwritingTestCase>(yaml);
            Assert.NotNull(deserialized);
        }

        private sealed class AnchorPrecedence
        {
            internal sealed class AnchorPrecedenceNested
            {
                public string b1 { get; set; }
                public Dictionary<string, string> b2 { get; set; }
            }

            public string a { get; set; }
            public AnchorPrecedenceNested b { get; set; }
            public string c { get; set; }
        }

        /// <summary>
        /// Deserializations the with duplicate anchors succeeds.
        /// </summary>
        [Fact]
        public void DeserializationWithDuplicateAnchorsSucceeds()
        {
            var sut = new Deserializer();
            var deserialized = sut.Deserialize<AnchorPrecedence>(@"
a: &anchor1 test0
b:
  b1: &anchor1 test1
  b2:
    b21:  &anchor1 test2
c:  *anchor1");

            Assert.Equal("test0", deserialized.a);
            Assert.Equal("test1", deserialized.b.b1);
            Assert.Contains("b21", deserialized.b.b2.Keys);
            Assert.Equal("test2", deserialized.b.b2["b21"]);
            Assert.Equal("test2", deserialized.c);
        }

        /// <summary>
        /// Serializes the exception with stack trace.
        /// </summary>
        [Fact]
        public void SerializeExceptionWithStackTrace()
        {
            var ex = GetExceptionWithStackTrace();
            var serializer = new SerializerBuilder()
                .WithTypeConverter(new MethodInfoConverter())
                .Build();
            var yaml = serializer.Serialize(ex);
            Assert.Contains("GetExceptionWithStackTrace", yaml);
        }

        private class MethodInfoConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return typeof(MethodInfo).IsAssignableFrom(type);
            }

            public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
            {
                var method = (MethodInfo)value;
                emitter.Emit(new Scalar(string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name)));
            }
        }

        static Exception GetExceptionWithStackTrace()
        {
            try
            {
                throw new ArgumentNullException("foo");
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Registerings the a type converter prevents the type from being visited.
        /// </summary>
        [Fact]
        public void RegisteringATypeConverterPreventsTheTypeFromBeingVisited()
        {
            var serializer = new SerializerBuilder()
                .WithTypeConverter(new NonSerializableTypeConverter())
                .Build();

            var yaml = serializer.Serialize(new NonSerializableContainer
            {
                Value = new NonSerializable { Text = "hello" },
            });

            var deserializer = new DeserializerBuilder()
                .WithTypeConverter(new NonSerializableTypeConverter())
                .Build();

            var result = deserializer.Deserialize<NonSerializableContainer>(yaml);

            Assert.Equal("hello", result.Value.Text);
        }

        /// <summary>
        /// Namings the convention is not applied by serializer when apply naming conventions is false.
        /// </summary>
        [Fact]
        public void NamingConventionIsNotAppliedBySerializerWhenApplyNamingConventionsIsFalse()
        {
            var sut = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = sut.Serialize(new NamingConventionDisabled { NoConvention = "value" });

            Assert.Contains("NoConvention", yaml);
        }

        /// <summary>
        /// Namings the convention is not applied by deserializer when apply naming conventions is false.
        /// </summary>
        [Fact]
        public void NamingConventionIsNotAppliedByDeserializerWhenApplyNamingConventionsIsFalse()
        {
            var sut = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = "NoConvention: value";

            var parsed = sut.Deserialize<NamingConventionDisabled>(yaml);

            Assert.Equal("value", parsed.NoConvention);
        }

        /// <summary>
        /// Types the are serializable.
        /// </summary>
        [Fact]
        public void TypesAreSerializable()
        {
            var sut = new SerializerBuilder()
                .Build();

            var yaml = sut.Serialize(typeof(string));

            Assert.Contains(typeof(string).AssemblyQualifiedName, yaml);
        }

        /// <summary>
        /// Types the are deserializable.
        /// </summary>
        [Fact]
        public void TypesAreDeserializable()
        {
            var sut = new DeserializerBuilder()
                .Build();

            var type = sut.Deserialize<Type>(typeof(string).AssemblyQualifiedName);

            Assert.Equal(typeof(string), type);
        }

        /// <summary>
        /// Types the are converted when needed from scalars.
        /// </summary>
        [Fact]
        public void TypesAreConvertedWhenNeededFromScalars()
        {
            var sut = new DeserializerBuilder()
                .WithTagMapping("!dbl", typeof(DoublyConverted))
                .Build();

            var result = sut.Deserialize<int>("!dbl hello");

            Assert.Equal(5, result);
        }

        /// <summary>
        /// Types the are converted when needed inside lists.
        /// </summary>
        [Fact]
        public void TypesAreConvertedWhenNeededInsideLists()
        {
            var sut = new DeserializerBuilder()
                .WithTagMapping("!dbl", typeof(DoublyConverted))
                .Build();

            var result = sut.Deserialize<List<int>>("- !dbl hello");

            Assert.Equal(5, result[0]);
        }

        /// <summary>
        /// Types the are converted when needed inside dictionary.
        /// </summary>
        [Fact]
        public void TypesAreConvertedWhenNeededInsideDictionary()
        {
            var sut = new DeserializerBuilder()
                .WithTagMapping("!dbl", typeof(DoublyConverted))
                .Build();

            var result = sut.Deserialize<Dictionary<int, int>>("!dbl hello: !dbl you");

            Assert.True(result.ContainsKey(5));
            Assert.Equal(3, result[5]);
        }

        /// <summary>
        /// Infinites the recursion is detected.
        /// </summary>
        [Fact]
        public void InfiniteRecursionIsDetected()
        {
            var sut = new SerializerBuilder()
                .DisableAliases()
                .Build();

            var recursionRoot = new
            {
                Nested = new[]
                {
                    new Dictionary<string, object>()
                }
            };

            recursionRoot.Nested[0].Add("loop", recursionRoot);

            var exception = Assert.Throws<MaximumRecursionLevelReachedException>(() => sut.Serialize(recursionRoot));
        }

        /// <summary>
        /// Tuples the are serializable.
        /// </summary>
        [Fact]
        public void TuplesAreSerializable()
        {
            var sut = new SerializerBuilder()
                .Build();

            var yaml = sut.Serialize(new[]
            {
                Tuple.Create(1, "one"),
                Tuple.Create(2, "two"),
            });

            var expected = Yaml.Text(@"
                - Item1: 1
                  Item2: one
                - Item1: 2
                  Item2: two
            ");

            Assert.Equal(expected.NormalizeNewLines(), yaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Values the tuples are serializable without metadata.
        /// </summary>
        [Fact]
        public void ValueTuplesAreSerializableWithoutMetadata()
        {
            var sut = new SerializerBuilder()
                .Build();

            var yaml = sut.Serialize(new[]
            {
                (num: 1, txt: "one"),
                (num: 2, txt: "two"),
            });

            var expected = Yaml.Text(@"
                - Item1: 1
                  Item2: one
                - Item1: 2
                  Item2: two
            ");

            Assert.Equal(expected.NormalizeNewLines(), yaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Anchors the name with trailing colon referenced in key can be deserialized.
        /// </summary>
        [Fact]
        public void AnchorNameWithTrailingColonReferencedInKeyCanBeDeserialized()
        {
            var sut = new Deserializer();
            var deserialized = sut.Deserialize<GenericTestDictionary<string, string>>(Yaml.ReaderForText(@"
                a: &::::scaryanchor:::: anchor "" value ""
                *::::scaryanchor::::: 2
                myvalue: *::::scaryanchor::::
            "));

            Assert.Equal(@"anchor "" value """, deserialized["a"]);
            Assert.Equal("2", deserialized[@"anchor "" value """]);
            Assert.Equal(@"anchor "" value """, deserialized["myvalue"]);
        }

        /// <summary>
        /// Aliases the before anchor cannot be deserialized.
        /// </summary>
        [Fact]
        public void AliasBeforeAnchorCannotBeDeserialized()
        {
            var sut = new Deserializer();
            Action action = () => sut.Deserialize<GenericTestDictionary<string, string>>(@"
a: *anchor1
b: &anchor1 test0
c: *anchor1");

            action.Should().Throw<AnchorNotFoundException>();
        }

        /// <summary>
        /// Anchors the with allowed characters can be deserialized.
        /// </summary>
        [Fact]
        public void AnchorWithAllowedCharactersCanBeDeserialized()
        {
            var sut = new Deserializer();
            var deserialized = sut.Deserialize<GenericTestDictionary<string, string>>(Yaml.ReaderForText(@"
                a: &@nchor<>""@-_123$>>>😁🎉🐻🍔end some value
                myvalue: my *@nchor<>""@-_123$>>>😁🎉🐻🍔end test
                interpolated value: *@nchor<>""@-_123$>>>😁🎉🐻🍔end
            "));

            Assert.Equal("some value", deserialized["a"]);
            Assert.Equal(@"my *@nchor<>""@-_123$>>>😁🎉🐻🍔end test", deserialized["myvalue"]);
            Assert.Equal("some value", deserialized["interpolated value"]);
        }

        /// <summary>
        /// Serializations the non public properties are ignored.
        /// </summary>
        [Fact]
        public void SerializationNonPublicPropertiesAreIgnored()
        {
            var sut = new SerializerBuilder().Build();
            var yaml = sut.Serialize(new NonPublicPropertiesExample());
            Assert.Equal("Public: public", yaml.TrimNewLines());
        }

        /// <summary>
        /// Serializations the non public properties are included.
        /// </summary>
        [Fact]
        public void SerializationNonPublicPropertiesAreIncluded()
        {
            var sut = new SerializerBuilder().IncludeNonPublicProperties().Build();
            var yaml = sut.Serialize(new NonPublicPropertiesExample());

            var expected = Yaml.Text(@"
                Public: public
                Internal: internal
                Protected: protected
                Private: private
            ");

            Assert.Equal(expected.NormalizeNewLines(), yaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Deserializations the non public properties are ignored.
        /// </summary>
        [Fact]
        public void DeserializationNonPublicPropertiesAreIgnored()
        {
            var sut = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            var deserialized = sut.Deserialize<NonPublicPropertiesExample>(Yaml.ReaderForText(@"
                Public: public2
                Internal: internal2
                Protected: protected2
                Private: private2
            "));

            Assert.Equal("public2,internal,protected,private", deserialized.ToString());
        }

        /// <summary>
        /// Deserializations the non public properties are included.
        /// </summary>
        [Fact]
        public void DeserializationNonPublicPropertiesAreIncluded()
        {
            var sut = new DeserializerBuilder().IncludeNonPublicProperties().Build();
            var deserialized = sut.Deserialize<NonPublicPropertiesExample>(Yaml.ReaderForText(@"
                Public: public2
                Internal: internal2
                Protected: protected2
                Private: private2
            "));

            Assert.Equal("public2,internal2,protected2,private2", deserialized.ToString());
        }

        /// <summary>
        /// Serializations the non public fields are ignored.
        /// </summary>
        [Fact]
        public void SerializationNonPublicFieldsAreIgnored()
        {
            var sut = new SerializerBuilder().Build();
            var yaml = sut.Serialize(new NonPublicFieldsExample());
            Assert.Equal("Public: public", yaml.TrimNewLines());
        }

        /// <summary>
        /// Deserializations the non public fields are ignored.
        /// </summary>
        [Fact]
        public void DeserializationNonPublicFieldsAreIgnored()
        {
            var sut = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            var deserialized = sut.Deserialize<NonPublicFieldsExample>(Yaml.ReaderForText(@"
                Public: public2
                Internal: internal2
                Protected: protected2
                Private: private2
            "));

            Assert.Equal("public2,internal,protected,private", deserialized.ToString());
        }

        /// <summary>
        /// Shoulds the not indent sequences.
        /// </summary>
        [Fact]
        public void ShouldNotIndentSequences()
        {
            var sut = new SerializerBuilder()
                .Build();

            var yaml = sut.Serialize(new
            {
                first = "first",
                items = new[]
                {
                    "item1",
                    "item2"
                },
                nested = new[]
                {
                    new
                    {
                        name = "name1",
                        more = new[]
                        {
                            "nested1",
                            "nested2"
                        }
                    }
                }
            });

            var expected = Yaml.Text(@"
                first: first
                items:
                - item1
                - item2
                nested:
                - name: name1
                  more:
                  - nested1
                  - nested2
            ");

            Assert.Equal(expected.NormalizeNewLines(), yaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Shoulds the indent sequences.
        /// </summary>
        [Fact]
        public void ShouldIndentSequences()
        {
            var sut = new SerializerBuilder()
                .WithIndentedSequences()
                .Build();

            var yaml = sut.Serialize(new
            {
                first = "first",
                items = new[]
                {
                    "item1",
                    "item2"
                },
                nested = new[]
                {
                    new
                    {
                        name = "name1",
                        more = new[]
                        {
                            "nested1",
                            "nested2"
                        }
                    }
                }
            });

            var expected = Yaml.Text(@"
                first: first
                items:
                  - item1
                  - item2
                nested:
                  - name: name1
                    more:
                      - nested1
                      - nested2
            ");

            Assert.Equal(expected.NormalizeNewLines(), yaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Examples the from specification is handled correctly with late define.
        /// </summary>
        [Fact]
        public void ExampleFromSpecificationIsHandledCorrectlyWithLateDefine()
        {
            var parser = new MergingParser(Yaml.ParserForText(@"
                # All the following maps are equal:
                results:
                  - # Explicit keys
                    x: 1
                    y: 2
                    r: 10
                    label: center/big

                  - # Merge one map
                    << : *CENTER
                    r: 10
                    label: center/big

                  - # Merge multiple maps
                    << : [ *CENTER, *BIG ]
                    label: center/big

                  - # Override
                    << : [ *BIG, *LEFT, *SMALL ]
                    x: 1
                    label: center/big

                obj:
                  - &CENTER { x: 1, y: 2 }
                  - &LEFT { x: 0, y: 2 }
                  - &SMALL { r: 1 }
                  - &BIG { r: 10 }
            "));

            var result = Deserializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(parser);

            int index = 0;
            foreach (var mapping in result["results"])
            {
                mapping.Should()
                    .Contain("x", "1", "'x' should be '1' in result #{0}", index)
                    .And.Contain("y", "2", "'y' should be '2' in result #{0}", index)
                    .And.Contain("r", "10", "'r' should be '10' in result #{0}", index)
                    .And.Contain("label", "center/big", "'label' should be 'center/big' in result #{0}", index);

                ++index;
            }
        }

        /// <summary>
        /// The cycle test entity.
        /// </summary>
        public class CycleTestEntity
        {
            /// <summary>
            /// Gets or sets the cycle.
            /// </summary>
            public CycleTestEntity Cycle { get; set; }
        }

        /// <summary>
        /// Serializes the cycle with alias.
        /// </summary>
        [Fact]
        public void SerializeCycleWithAlias()
        {
            var sut = new SerializerBuilder()
                .WithTagMapping("!CycleTag", typeof(CycleTestEntity))
                .Build();

            var entity = new CycleTestEntity();
            entity.Cycle = entity;
            var yaml = sut.Serialize(entity);
            var expected = Yaml.Text(@"&o0 !CycleTag
Cycle: *o0");

            Assert.Equal(expected.NormalizeNewLines(), yaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Deserializes the cycle with alias.
        /// </summary>
        [Fact]
        public void DeserializeCycleWithAlias()
        {
            var sut = new DeserializerBuilder()
                .WithTagMapping("!CycleTag", typeof(CycleTestEntity))
                .Build();

            var yaml = Yaml.Text(@"&o0 !CycleTag
Cycle: *o0");
            var obj = sut.Deserialize<CycleTestEntity>(yaml);

            Assert.Same(obj, obj.Cycle);
        }

        /// <summary>
        /// Deserializes the cycle without alias.
        /// </summary>
        [Fact]
        public void DeserializeCycleWithoutAlias()
        {
            var sut = new DeserializerBuilder()
                .Build();

            var yaml = Yaml.Text(@"&o0
Cycle: *o0");
            var obj = sut.Deserialize<CycleTestEntity>(yaml);

            Assert.Same(obj, obj.Cycle);
        }

        /// <summary>
        /// Gets the depths.
        /// </summary>
        public static IEnumerable<object[]> Depths => Enumerable.Range(1, 10).Select(i => new[] { (object)i });

        /// <summary>
        /// Deserializes the cycle with anchors with depth.
        /// </summary>
        /// <param name="depth">The depth.</param>
        [Theory]
        [MemberData(nameof(Depths))]
        public void DeserializeCycleWithAnchorsWithDepth(int? depth)
        {
            var sut = new DeserializerBuilder()
                .WithTagMapping("!CycleTag", typeof(CycleTestEntity))
                .Build();

            StringBuilder builder = new StringBuilder(@"&o0 !CycleTag");
            builder.AppendLine();
            string indentation;
            for (int i = 0; i < depth - 1; ++i)
            {
                indentation = string.Concat(Enumerable.Repeat("  ", i));
                builder.AppendLine($"{indentation}Cycle: !CycleTag");
            }
            indentation = string.Concat(Enumerable.Repeat("  ", depth.Value - 1));
            builder.AppendLine($"{indentation}Cycle: *o0");
            var yaml = Yaml.Text(builder.ToString());
            var obj = sut.Deserialize<CycleTestEntity>(yaml);
            CycleTestEntity iterator = obj;
            for (int i = 0; i < depth; ++i)
            {
                iterator = iterator.Cycle;
            }
            Assert.Same(obj, iterator);
        }

        /// <summary>
        /// Roundtrips the windows newlines.
        /// </summary>
        [Fact]
        public void RoundtripWindowsNewlines()
        {
            var text = $"Line1{Environment.NewLine}Line2{Environment.NewLine}Line3{Environment.NewLine}{Environment.NewLine}Line4";

            var sut = new SerializerBuilder().Build();
            var dut = new DeserializerBuilder().Build();

            using var writer = new StringWriter { NewLine = Environment.NewLine };
            sut.Serialize(writer, new StringContainer { Text = text });
            var serialized = writer.ToString();

            using var reader = new StringReader(serialized);
            var roundtrippedText = dut.Deserialize<StringContainer>(reader).Text.NormalizeNewLines();
            Assert.Equal(text, roundtrippedText);
        }

        /// <summary>
        /// Strings the that match keywords are quoted.
        /// </summary>
        /// <param name="input">The input.</param>
        [Theory]
        [InlineData("NULL")]
        [InlineData("Null")]
        [InlineData("null")]
        [InlineData("~")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("True")]
        [InlineData("False")]
        [InlineData("TRUE")]
        [InlineData("FALSE")]
        [InlineData("0o77")]
        [InlineData("0x7A")]
        [InlineData("+1e10")]
        [InlineData("1E10")]
        [InlineData("+.inf")]
        [InlineData("-.inf")]
        [InlineData(".inf")]
        [InlineData(".nan")]
        [InlineData(".NaN")]
        [InlineData(".NAN")]
        public void StringsThatMatchKeywordsAreQuoted(string input)
        {
            var serializer = new SerializerBuilder().WithQuotingNecessaryStrings().Build();
            var o = new { text = input };
            var yaml = serializer.Serialize(o);
            Assert.Equal($"text: \"{input}\"{Environment.NewLine}", yaml);
        }

        public static IEnumerable<object[]> Yaml1_1SpecialStringsData = new[]
        {
            "-.inf", "-.Inf", "-.INF", "-0", "-0100_200", "-0b101", "-0x30", "-190:20:30", "-23", "-3.14",
            "._", "._14", ".", ".0", ".1_4", ".14", ".3E-1", ".3e+3", ".inf", ".Inf",
            ".INF", ".nan", ".NaN", ".NAN", "+.inf", "+.Inf", "+.INF", "+0.3e+3", "+0",
            "+0100_200", "+0b100", "+190:20:30", "+23", "+3.14", "~", "0.0", "0", "00", "001.23",
            "0011", "010", "02_0", "07", "0b0", "0b100_101", "0o0", "0o10", "0o7", "0x0",
            "0x10", "0x2_0", "0x42", "0xa", "100_000", "190:20:30.15", "190:20:30", "23", "3.", "3.14", "3.3e+3",
            "85_230.15", "85.230_15e+03", "false", "False", "FALSE", "n", "N", "no", "No", "NO",
            "null", "Null", "NULL", "off", "Off", "OFF", "on", "On", "ON", "true", "True", "TRUE",
            "y", "Y", "yes", "Yes", "YES"
        }.Select(v => new object[] { v }).ToList();

        /// <summary>
        /// Strings the that match yaml1_1 keywords are quoted.
        /// </summary>
        /// <param name="input">The input.</param>
        [Theory]
        [MemberData(nameof(Yaml1_1SpecialStringsData))]
        public void StringsThatMatchYaml1_1KeywordsAreQuoted(string input)
        {
            var serializer = new SerializerBuilder().WithQuotingNecessaryStrings(true).Build();
            var o = new { text = input };
            var yaml = serializer.Serialize(o);
            Assert.Equal($"text: \"{input}\"{Environment.NewLine}", yaml);
        }

        /// <summary>
        /// Keys the on concrete class dont get quoted_ type string gets quoted.
        /// </summary>
        [Fact]
        public void KeysOnConcreteClassDontGetQuoted_TypeStringGetsQuoted()
        {
            var serializer = new SerializerBuilder().WithQuotingNecessaryStrings().Build();
            var deserializer = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
            var yaml = @"
True: null
False: hello
Null: true
";
            var obj = deserializer.Deserialize<ReservedWordsTestClass<string>>(yaml);
            var result = serializer.Serialize(obj);
            obj.True.Should().BeNull();
            obj.False.Should().Be("hello");
            obj.Null.Should().Be("true");
            result.Should().Be($"True: {Environment.NewLine}False: hello{Environment.NewLine}Null: \"true\"{Environment.NewLine}");
        }

        /// <summary>
        /// Keys the on concrete class dont get quoted_ type bool does not get quoted.
        /// </summary>
        [Fact]
        public void KeysOnConcreteClassDontGetQuoted_TypeBoolDoesNotGetQuoted()
        {
            var serializer = new SerializerBuilder().WithQuotingNecessaryStrings().Build();
            var deserializer = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
            var yaml = @"
True: null
False: hello
Null: true
";
            var obj = deserializer.Deserialize<ReservedWordsTestClass<bool>>(yaml);
            var result = serializer.Serialize(obj);
            obj.True.Should().BeNull();
            obj.False.Should().Be("hello");
            obj.Null.Should().BeTrue();
            result.Should().Be($"True: {Environment.NewLine}False: hello{Environment.NewLine}Null: true{Environment.NewLine}");
        }

        /// <summary>
        /// Serializes the state methods get called once.
        /// </summary>
        [Fact]
        public void SerializeStateMethodsGetCalledOnce()
        {
            var serializer = new SerializerBuilder().Build();
            var test = new TestState();
            serializer.Serialize(test);

            Assert.Equal(1, test.OnSerializedCallCount);
            Assert.Equal(1, test.OnSerializingCallCount);
        }

        /// <summary>
        /// Serializes the concurrently.
        /// </summary>
        [Fact]
        public void SerializeConcurrently()
        {
            var exceptions = new ConcurrentStack<Exception>();
            var runCount = 10;

            for (var i = 0; i < runCount; i++)
            {
                // Failures don't occur consistently - running repeatedly increases the chances
                RunTest();
            }

            Assert.Empty(exceptions);

            void RunTest()
            {
                var threadCount = 100;
                var threads = new List<Thread>();
                var control = new SemaphoreSlim(0, threadCount);

                var serializer = new SerializerBuilder().Build();

                for (var i = 0; i < threadCount; i++)
                {
                    threads.Add(new Thread(Serialize));
                }

                threads.ForEach(t => t.Start());
                // Each thread will wait for the semaphore before proceeding.
                // Release them all simultaneously to try to maximise concurrency
                control.Release(threadCount);
                threads.ForEach(t => t.Join());

                void Serialize()
                {
                    control.Wait();

                    try
                    {
                        var test = new TestState();
                        serializer.Serialize(test);
                    }
                    catch (Exception e)
                    {
                        exceptions.Push(e.InnerException ?? e);
                    }
                }
            }
        }

        /// <summary>
        /// Serializes the enum as number.
        /// </summary>
        [Fact]
        public void SerializeEnumAsNumber()
        {
            var serializer = new SerializerBuilder().WithYamlFormatter(new YamlFormatter
            {
                FormatEnum = (o, typeInspector, namingConvention) => ((int)o).ToString(),
                PotentiallyQuoteEnums = (_) => false
            }).Build();
            var deserializer = DeserializerBuilder.Build();

            var value = serializer.Serialize(TestEnumAsNumber.Test1);
            Assert.Equal("1", value.TrimNewLines());
            var v = deserializer.Deserialize<TestEnumAsNumber>(value);
            Assert.Equal(TestEnumAsNumber.Test1, v);

            value = serializer.Serialize(TestEnumAsNumber.Test1 | TestEnumAsNumber.Test2);
            Assert.Equal("3", value.TrimNewLines());
            v = deserializer.Deserialize<TestEnumAsNumber>(value);
            Assert.Equal(TestEnumAsNumber.Test1 | TestEnumAsNumber.Test2, v);
        }

        /// <summary>
        /// Tabs the get quoted when quote necessary strings is on.
        /// </summary>
        [Fact]
        public void TabsGetQuotedWhenQuoteNecessaryStringsIsOn()
        {
            var serializer = new SerializerBuilder()
                .WithQuotingNecessaryStrings()
                .Build();

            var s = "\t, something";
            var yaml = serializer.Serialize(s);
            var deserializer = new DeserializerBuilder().Build();
            var value = deserializer.Deserialize(yaml);
            Assert.Equal(s, value);
        }

        /// <summary>
        /// Spaces the get quoted when quote necessary strings is on.
        /// </summary>
        [Fact]
        public void SpacesGetQuotedWhenQuoteNecessaryStringsIsOn()
        {
            var serializer = new SerializerBuilder()
                .WithQuotingNecessaryStrings()
                .Build();

            var s = " , something";
            var yaml = serializer.Serialize(s);
            var deserializer = new DeserializerBuilder().Build();
            var value = deserializer.Deserialize(yaml);
            Assert.Equal(s, value);
        }

        /// <summary>
        /// The test enum as number.
        /// </summary>
        [Flags]
        private enum TestEnumAsNumber
        {
            Test1 = 1,
            Test2 = 2
        }

        /// <summary>
        /// Namings the convention applied to enum.
        /// </summary>
        [Fact]
        public void NamingConventionAppliedToEnum()
        {
            var serializer = new SerializerBuilder().WithEnumNamingConvention(CamelCaseNamingConvention.Instance).Build();
            ScalarStyle style = ScalarStyle.Plain;
            var serialized = serializer.Serialize(style);
            Assert.Equal("plain", serialized.Replace("\r\n", "").Replace("\n", ""));
        }

        /// <summary>
        /// Namings the convention applied to enum when deserializing.
        /// </summary>
        [Fact]
        public void NamingConventionAppliedToEnumWhenDeserializing()
        {
            var serializer = new DeserializerBuilder().WithEnumNamingConvention(UnderscoredNamingConvention.Instance).Build();
            var yaml = "Double_Quoted";
            ScalarStyle expected = ScalarStyle.DoubleQuoted;
            var actual = serializer.Deserialize<ScalarStyle>(yaml);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Nesteds the dictionary types_ should roundtrip.
        /// </summary>
        [Fact]
        [Trait("motive", "issue #656")]
        public void NestedDictionaryTypes_ShouldRoundtrip()
        {
            var serializer = new SerializerBuilder().EnsureRoundtrip().Build();
            var yaml = serializer.Serialize(new HasNestedDictionary { Lookups = { [1] = new HasNestedDictionary.Payload { I = 1 } } }, typeof(HasNestedDictionary));
            var dct = new DeserializerBuilder().Build().Deserialize<HasNestedDictionary>(yaml);
            Assert.Contains(new KeyValuePair<int, HasNestedDictionary.Payload>(1, new HasNestedDictionary.Payload { I = 1 }), dct.Lookups);
        }

        /// <summary>
        /// The test state.
        /// </summary>
        public class TestState
        {
            /// <summary>
            /// Gets or sets the on serialized call count.
            /// </summary>
            public int OnSerializedCallCount { get; set; }
            /// <summary>
            /// Gets or sets the on serializing call count.
            /// </summary>
            public int OnSerializingCallCount { get; set; }

            /// <summary>
            /// Gets or sets the test.
            /// </summary>
            public string Test { get; set; } = string.Empty;

            /// <summary>
            /// Serializeds the.
            /// </summary>
            [OnSerialized]
            public void Serialized() => OnSerializedCallCount++;

            /// <summary>
            /// Serializings the.
            /// </summary>
            [OnSerializing]
            public void Serializing() => OnSerializingCallCount++;
        }

        /// <summary>
        /// The reserved words test class.
        /// </summary>
        public class ReservedWordsTestClass<TNullType>
        {
            /// <summary>
            /// Gets or sets the true.
            /// </summary>
            public string True { get; set; }
            /// <summary>
            /// Gets or sets the false.
            /// </summary>
            public string False { get; set; }
            /// <summary>
            /// Gets or sets the null.
            /// </summary>
            public TNullType Null { get; set; }
        }

        /// <summary>
        /// The doubly converted.
        /// </summary>
        [TypeConverter(typeof(DoublyConvertedTypeConverter))]
        public class DoublyConverted
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// The doubly converted type converter.
        /// </summary>
        public class DoublyConvertedTypeConverter : TypeConverter
        {
            /// <summary>
            /// Cans the convert to.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="destinationType">The destination type.</param>
            /// <returns>A bool.</returns>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(int);
            }

            /// <summary>
            /// Converts the to.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="culture">The culture.</param>
            /// <param name="value">The value.</param>
            /// <param name="destinationType">The destination type.</param>
            /// <returns>An object.</returns>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return ((DoublyConverted)value).Value.Length;
            }

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
            /// Converts the from.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="culture">The culture.</param>
            /// <param name="value">The value.</param>
            /// <returns>An object.</returns>
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return new DoublyConverted { Value = (string)value };
            }
        }

        /// <summary>
        /// The naming convention disabled.
        /// </summary>
        public class NamingConventionDisabled
        {
            /// <summary>
            /// Gets or sets the no convention.
            /// </summary>
            [YamlMember(ApplyNamingConventions = false)]
            public string NoConvention { get; set; }
        }

        /// <summary>
        /// The non serializable container.
        /// </summary>
        public class NonSerializableContainer
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public NonSerializable Value { get; set; }
        }

        /// <summary>
        /// The non serializable.
        /// </summary>
        public class NonSerializable
        {
            /// <summary>
            /// Gets the will throw.
            /// </summary>
            public string WillThrow { get { throw new Exception(); } }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }
        }

        /// <summary>
        /// The string container.
        /// </summary>
        public class StringContainer
        {
            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }
        }

        /// <summary>
        /// The non serializable type converter.
        /// </summary>
        public class NonSerializableTypeConverter : IYamlTypeConverter
        {
            /// <summary>
            /// Accepts the.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns>A bool.</returns>
            public bool Accepts(Type type)
            {
                return typeof(NonSerializable).IsAssignableFrom(type);
            }

            /// <summary>
            /// Reads the yaml.
            /// </summary>
            /// <param name="parser">The parser.</param>
            /// <param name="type">The type.</param>
            /// <param name="rootDeserializer">The root deserializer.</param>
            /// <returns>An object.</returns>
            public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
            {
                var scalar = parser.Consume<Scalar>();
                return new NonSerializable { Text = scalar.Value };
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
                emitter.Emit(new Scalar(((NonSerializable)value).Text));
            }
        }

        /// <summary>
        /// The has nested dictionary.
        /// </summary>
        public sealed class HasNestedDictionary
        {
            /// <summary>
            /// Gets or sets the lookups.
            /// </summary>
            public Dictionary<int, Payload> Lookups { get; set; } = new Dictionary<int, Payload>();

            public struct Payload
            {
                public int I { get; set; }
            }
        }
    }
}
