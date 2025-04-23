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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Callbacks;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Test.Serialization
{
    /// <summary>
    /// The deserializer test.
    /// </summary>
    public class DeserializerTest
    {
        /// <summary>
        /// Deserialize_S the yaml with interface type and mapping_ returns model.
        /// </summary>
        [Fact]
        public void Deserialize_YamlWithInterfaceTypeAndMapping_ReturnsModel()
        {
            var yaml = @"
name: Jack
momentOfBirth: 1983-04-21T20:21:03.0041599Z
cars:
- name: Mercedes
  year: 2018
- name: Honda
  year: 2021
";

            var sut = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeMapping<ICar, Car>()
                .Build();

            var person = sut.Deserialize<Person>(yaml);
            person.Name.Should().Be("Jack");
            person.MomentOfBirth.Kind.Should().Be(DateTimeKind.Utc);
            person.MomentOfBirth.ToUniversalTime().Year.Should().Be(1983);
            person.MomentOfBirth.ToUniversalTime().Month.Should().Be(4);
            person.MomentOfBirth.ToUniversalTime().Day.Should().Be(21);
            person.MomentOfBirth.ToUniversalTime().Hour.Should().Be(20);
            person.MomentOfBirth.ToUniversalTime().Minute.Should().Be(21);
            person.MomentOfBirth.ToUniversalTime().Second.Should().Be(3);
            person.Cars.Should().HaveCount(2);
            person.Cars[0].Name.Should().Be("Mercedes");
            person.Cars[0].Spec.Should().BeNull();
            person.Cars[1].Name.Should().Be("Honda");
            person.Cars[1].Spec.Should().BeNull();
        }

        /// <summary>
        /// Deserialize_S the yaml with two interface types and mappings_ returns model.
        /// </summary>
        [Fact]
        public void Deserialize_YamlWithTwoInterfaceTypesAndMappings_ReturnsModel()
        {
            var yaml = @"
name: Jack
momentOfBirth: 1983-04-21T20:21:03.0041599Z
cars:
- name: Mercedes
  year: 2018
  spec:
    engineType: V6
    driveType: AWD
- name: Honda
  year: 2021
  spec:
    engineType: V4
    driveType: FWD
";

            var sut = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeMapping<ICar, Car>()
                .WithTypeMapping<IModelSpec, ModelSpec>()
                .Build();

            var person = sut.Deserialize<Person>(yaml);
            person.Name.Should().Be("Jack");
            person.MomentOfBirth.Kind.Should().Be(DateTimeKind.Utc);
            person.MomentOfBirth.ToUniversalTime().Year.Should().Be(1983);
            person.MomentOfBirth.ToUniversalTime().Month.Should().Be(4);
            person.MomentOfBirth.ToUniversalTime().Day.Should().Be(21);
            person.MomentOfBirth.ToUniversalTime().Hour.Should().Be(20);
            person.MomentOfBirth.ToUniversalTime().Minute.Should().Be(21);
            person.MomentOfBirth.ToUniversalTime().Second.Should().Be(3);
            person.Cars.Should().HaveCount(2);
            person.Cars[0].Name.Should().Be("Mercedes");
            person.Cars[0].Spec.EngineType.Should().Be("V6");
            person.Cars[0].Spec.DriveType.Should().Be("AWD");
            person.Cars[1].Name.Should().Be("Honda");
            person.Cars[1].Spec.EngineType.Should().Be("V4");
            person.Cars[1].Spec.DriveType.Should().Be("FWD");
        }

        /// <summary>
        /// Setters the only sets without exception.
        /// </summary>
        [Fact]
        public void SetterOnlySetsWithoutException()
        {
            var yaml = @"
Value: bar
";
            var deserializer = new DeserializerBuilder().Build();
            var result = deserializer.Deserialize<SetterOnly>(yaml);
            result.Actual.Should().Be("bar");
        }

        /// <summary>
        /// Keys the on dynamic class dont get quoted.
        /// </summary>
        [Fact]
        public void KeysOnDynamicClassDontGetQuoted()
        {
            var serializer = new SerializerBuilder().WithQuotingNecessaryStrings().Build();
            var deserializer = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
            var yaml = @"
True: null
False: hello
Null: true
X:
";
            var obj = deserializer.Deserialize(yaml, typeof(object));
            var result = serializer.Serialize(obj);
            var dictionary = (Dictionary<object, object>)obj;
            var keys = dictionary.Keys.ToArray();
            Assert.Equal(keys, new[] { "True", "False", "Null", "X" });
            Assert.Equal(dictionary.Values, new object[] { null, "hello", true, null });
        }

        /// <summary>
        /// Empties the quoted strings arent null.
        /// </summary>
        [Fact]
        public void EmptyQuotedStringsArentNull()
        {
            var deserializer = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
            var yaml = "Value: \"\"";
            var result = deserializer.Deserialize<Test>(yaml);
            Assert.Equal(string.Empty, result.Value);
        }

        /// <summary>
        /// Keys the anchor is handled with type deserialization.
        /// </summary>
        [Fact]
        public void KeyAnchorIsHandledWithTypeDeserialization()
        {
            var yaml = @"a: &some_scalar this is also a key
b: &number 1
*some_scalar: ""will this key be handled correctly?""
*number: 1";
            var deserializer = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
            var result = deserializer.Deserialize(yaml, typeof(object));
            Assert.IsType<Dictionary<object, object>>(result);
            var dictionary = (Dictionary<object, object>)result;
            Assert.Equal(new object[] { "a", "b", "this is also a key", (byte)1 }, dictionary.Keys);
            Assert.Equal(new object[] { "this is also a key", (byte)1, "will this key be handled correctly?", (byte)1 }, dictionary.Values);
        }

        /// <summary>
        /// Nons the scalar key is handled with type deserialization.
        /// </summary>
        [Fact]
        public void NonScalarKeyIsHandledWithTypeDeserialization()
        {
            var yaml = @"scalar: foo
{ a: mapping }: bar
[ a, sequence, 1 ]: baz";
            var deserializer = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
            var result = deserializer.Deserialize(yaml, typeof(object));
            Assert.IsType<Dictionary<object, object>>(result);

            var dictionary = (Dictionary<object, object>)result;
            var item = dictionary.ElementAt(0);
            Assert.Equal("scalar", item.Key);
            Assert.Equal("foo", item.Value);

            item = dictionary.ElementAt(1);
            Assert.IsType<Dictionary<object, object>>(item.Key);
            Assert.Equal("bar", item.Value);
            dictionary = (Dictionary<object, object>)item.Key;
            item = dictionary.ElementAt(0);
            Assert.Equal("a", item.Key);
            Assert.Equal("mapping", item.Value);

            dictionary = (Dictionary<object, object>)result;
            item = dictionary.ElementAt(2);
            Assert.IsType<List<object>>(item.Key);
            Assert.Equal(new List<object> { "a", "sequence", (byte)1 }, (List<object>)item.Key);
            Assert.Equal("baz", item.Value);
        }

        /// <summary>
        /// News the lines in keys.
        /// </summary>
        [Fact]
        public void NewLinesInKeys()
        {
            var yaml = @"? >-
  key

  a

  b
: >-
  value

  a

  b
";
            var deserializer = new DeserializerBuilder().Build();
            var o = deserializer.Deserialize(yaml, typeof(object));
            Assert.IsType<Dictionary<object, object>>(o);
            var dictionary = (Dictionary<object, object>)o;
            Assert.Equal($"key\na\nb", dictionary.First().Key);
            Assert.Equal($"value\na\nb", dictionary.First().Value);
        }

        /// <summary>
        /// Unquoteds the string type deserialization_ regular numbers.
        /// </summary>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData(System.Byte.MinValue)]
        [InlineData(System.Byte.MaxValue)]
        [InlineData(System.Int16.MinValue)]
        [InlineData(System.Int16.MaxValue)]
        [InlineData(System.Int32.MinValue)]
        [InlineData(System.Int32.MaxValue)]
        [InlineData(System.Int64.MinValue)]
        [InlineData(System.Int64.MaxValue)]
        [InlineData(System.UInt64.MaxValue)]
        [InlineData(System.Single.MinValue)]
        [InlineData(System.Single.MaxValue)]
        [InlineData(System.Double.MinValue)]
        [InlineData(System.Double.MaxValue)]
        public void UnquotedStringTypeDeserialization_RegularNumbers(object expected)
        {
            var deserializer = new DeserializerBuilder()
                .WithAttemptingUnquotedStringTypeDeserialization().Build();

            var yaml = $"Value: {expected}";

#if NETFRAMEWORK
            // It needs explicitly specifying maximum precision for value roundtrip. 
            if (expected is float floatValue)
            {
                yaml = $"Value: {floatValue:G9}";
            }
            if (expected is double doubleValue)
            {
                yaml = $"Value: {doubleValue:G17}";
            }
#endif

            var resultDict = deserializer.Deserialize<IDictionary<string, object>>(yaml);
            Assert.True(resultDict.ContainsKey("Value"));
            Assert.Equal(expected, resultDict["Value"]);
        }

        /// <summary>
        /// Unquoteds the string type deserialization_ hex numbers.
        /// </summary>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData(System.Byte.MinValue)]
        [InlineData(System.Byte.MaxValue)]
        [InlineData(System.Int16.MinValue)]
        [InlineData(System.Int16.MaxValue)]
        [InlineData(System.Int32.MinValue)]
        [InlineData(System.Int32.MaxValue)]
        [InlineData(System.Int64.MinValue)]
        [InlineData(System.Int64.MaxValue)]
        public void UnquotedStringTypeDeserialization_HexNumbers(object expected)
        {
            var deserializer = new DeserializerBuilder()
                .WithAttemptingUnquotedStringTypeDeserialization().Build();

            var yaml = $"Value: 0x{expected:X2}";

            var resultDict = deserializer.Deserialize<IDictionary<string, object>>(yaml);
            Assert.True(resultDict.ContainsKey("Value"));
            Assert.Equal(expected, resultDict["Value"]);
        }

        /// <summary>
        /// Unquoteds the string type deserialization handles inf and na n.
        /// </summary>
        /// <param name="yamlValue">The yaml value.</param>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData(".nan", System.Single.NaN)]
        [InlineData(".NaN", System.Single.NaN)]
        [InlineData(".NAN", System.Single.NaN)]
        [InlineData("-.inf", System.Single.NegativeInfinity)]
        [InlineData("+.inf", System.Single.PositiveInfinity)]
        [InlineData(".inf", System.Single.PositiveInfinity)]
        [InlineData("start.nan", "start.nan")]
        [InlineData(".nano", ".nano")]
        [InlineData(".infinity", ".infinity")]
        [InlineData("www.infinitetechnology.com", "www.infinitetechnology.com")]
        [InlineData("https://api.inference.azure.com", "https://api.inference.azure.com")]
        public void UnquotedStringTypeDeserializationHandlesInfAndNaN(string yamlValue, object expected)
        {
            var deserializer = new DeserializerBuilder()
                .WithAttemptingUnquotedStringTypeDeserialization().Build();
            var yaml = $"Value: {yamlValue}";

            var resultDict = deserializer.Deserialize<IDictionary<string, object>>(yaml);
            Assert.True(resultDict.ContainsKey("Value"));
            Assert.Equal(expected, resultDict["Value"]);
        }

        /// <summary>
        /// Gets the deserialize scalar edge cases_ test cases.
        /// </summary>
        public static IEnumerable<object[]> DeserializeScalarEdgeCases_TestCases
        {
            get
            {
                yield return new object[] { byte.MinValue, typeof(byte) };
                yield return new object[] { byte.MaxValue, typeof(byte) };
                yield return new object[] { short.MinValue, typeof(short) };
                yield return new object[] { short.MaxValue, typeof(short) };
                yield return new object[] { int.MinValue, typeof(int) };
                yield return new object[] { int.MaxValue, typeof(int) };
                yield return new object[] { long.MinValue, typeof(long) };
                yield return new object[] { long.MaxValue, typeof(long) };
                yield return new object[] { sbyte.MinValue, typeof(sbyte) };
                yield return new object[] { sbyte.MaxValue, typeof(sbyte) };
                yield return new object[] { ushort.MinValue, typeof(ushort) };
                yield return new object[] { ushort.MaxValue, typeof(ushort) };
                yield return new object[] { uint.MinValue, typeof(uint) };
                yield return new object[] { uint.MaxValue, typeof(uint) };
                yield return new object[] { ulong.MinValue, typeof(ulong) };
                yield return new object[] { ulong.MaxValue, typeof(ulong) };
                yield return new object[] { decimal.MinValue, typeof(decimal) };
                yield return new object[] { decimal.MaxValue, typeof(decimal) };
                yield return new object[] { char.MaxValue, typeof(char) };

#if NET
                yield return new object[] { float.MinValue, typeof(float) };
                yield return new object[] { float.MaxValue, typeof(float) };
                yield return new object[] { double.MinValue, typeof(double) };
                yield return new object[] { double.MaxValue, typeof(double) };
#endif
            }
        }

        /// <summary>
        /// Deserializes the scalar edge cases.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        [Theory]
        [MemberData(nameof(DeserializeScalarEdgeCases_TestCases))]
        public void DeserializeScalarEdgeCases(IConvertible value, Type type)
        {
            var deserializer = new DeserializerBuilder().Build();
            var result = deserializer.Deserialize(value.ToString(YamlFormatter.Default.NumberFormat), type);

            result.Should().Be(value);
        }

        /// <summary>
        /// Deserializes the with duplicate key checking_ yaml with duplicate keys_ throws yaml exception.
        /// </summary>
        [Fact]
        public void DeserializeWithDuplicateKeyChecking_YamlWithDuplicateKeys_ThrowsYamlException()
        {
            var yaml = @"
name: Jack
momentOfBirth: 1983-04-21T20:21:03.0041599Z
name: Jake
";

            var sut = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithDuplicateKeyChecking()
                .Build();

            Action act = () => sut.Deserialize<Person>(yaml);
            act.Should().Throw<YamlException>("Because there are duplicate name keys with concrete class");
            act = () => sut.Deserialize<IDictionary<object, object>>(yaml);
            act.Should().Throw<YamlException>("Because there are duplicate name keys with dictionary");

            var stream = Yaml.ReaderFrom("backreference.yaml");
            var parser = new MergingParser(new Parser(stream));
            act = () => sut.Deserialize<Dictionary<string, Dictionary<string, string>>>(parser);
            act.Should().Throw<YamlException>("Because there are duplicate name keys with merging parser");
        }

        /// <summary>
        /// Deserializes the without duplicate key checking_ yaml with duplicate keys_ does not throw yaml exception.
        /// </summary>
        [Fact]
        public void DeserializeWithoutDuplicateKeyChecking_YamlWithDuplicateKeys_DoesNotThrowYamlException()
        {
            var yaml = @"
name: Jack
momentOfBirth: 1983-04-21T20:21:03.0041599Z
name: Jake
";

            var sut = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            Action act = () => sut.Deserialize<Person>(yaml);
            act.Should().NotThrow<YamlException>("Because duplicate key checking is not enabled");
            act = () => sut.Deserialize<IDictionary<object, object>>(yaml);
            act.Should().NotThrow<YamlException>("Because duplicate key checking is not enabled");

            var stream = Yaml.ReaderFrom("backreference.yaml");
            var parser = new MergingParser(new Parser(stream));
            act = () => sut.Deserialize<Dictionary<string, Dictionary<string, string>>>(parser);
            act.Should().NotThrow<YamlException>("Because duplicate key checking is not enabled");
        }

        /// <summary>
        /// Enforces the nullable when class is default nullable throws.
        /// </summary>
        [Fact]
        public void EnforceNullableWhenClassIsDefaultNullableThrows()
        {
            var deserializer = new DeserializerBuilder().WithEnforceNullability().Build();
            var yaml = @"
TestString: null
TestBool: null
TestBool1: null
";
            try
            {
                var test = deserializer.Deserialize<NullableDefaultClass>(yaml);
            }
            catch (YamlException e)
            {
                if (e.InnerException is NullReferenceException)
                {
                    return;
                }
            }

            throw new Exception("Non nullable property was set to null.");
        }

        /// <summary>
        /// Enforces the nullable when class is not default nullable throws.
        /// </summary>
        [Fact]
        public void EnforceNullableWhenClassIsNotDefaultNullableThrows()
        {
            var deserializer = new DeserializerBuilder().WithEnforceNullability().Build();
            var yaml = @"
TestString: null
TestBool: null
TestBool1: null
";
            try
            {
                var test = deserializer.Deserialize<NullableNotDefaultClass>(yaml);
            }
            catch (YamlException e)
            {
                if (e.InnerException is NullReferenceException)
                {
                    return;
                }
            }

            throw new Exception("Non nullable property was set to null.");
        }

        /// <summary>
        /// Enforces the nullable types when null throws exception.
        /// </summary>
        [Fact]
        public void EnforceNullableTypesWhenNullThrowsException()
        {
            var deserializer = new DeserializerBuilder().WithEnforceNullability().Build();
            var yaml = @"
Test: null
";
            try
            {
                var o = deserializer.Deserialize<NonNullableClass>(yaml);
            }
            catch (YamlException e)
            {
                if (e.InnerException is NullReferenceException)
                {
                    return;
                }
            }

            throw new Exception("Non nullable property was set to null.");
        }

        /// <summary>
        /// Enforces the nullable types when not null does not throw exception.
        /// </summary>
        [Fact]
        public void EnforceNullableTypesWhenNotNullDoesNotThrowException()
        {
            var deserializer = new DeserializerBuilder().WithEnforceNullability().Build();
            var yaml = @"
Test: test 123
";
            var o = deserializer.Deserialize<NonNullableClass>(yaml);
        }

        /// <summary>
        /// Serializes the state methods get called once.
        /// </summary>
        [Fact]
        public void SerializeStateMethodsGetCalledOnce()
        {
            var yaml = "Test: Hi";
            var deserializer = new DeserializerBuilder().Build();
            var test = deserializer.Deserialize<TestState>(yaml);

            Assert.Equal(1, test.OnDeserializedCallCount);
            Assert.Equal(1, test.OnDeserializingCallCount);
        }

        /// <summary>
        /// Deserializes the concurrently.
        /// </summary>
        [Fact]
        public void DeserializeConcurrently()
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

                var yaml = "Test: Hi";
                var deserializer = new DeserializerBuilder().Build();

                for (var i = 0; i < threadCount; i++)
                {
                    threads.Add(new Thread(Deserialize));
                }

                threads.ForEach(t => t.Start());
                // Each thread will wait for the semaphore before proceeding.
                // Release them all simultaneously to maximise concurrency
                control.Release(threadCount);
                threads.ForEach(t => t.Join());

                Assert.Empty(exceptions);
                return;

                void Deserialize()
                {
                    control.Wait();

                    try
                    {
                        var result = deserializer.Deserialize<TestState>(yaml);
                        result.Test.Should().Be("Hi");
                    }
                    catch (Exception e)
                    {
                        exceptions.Push(e.InnerException ?? e);
                    }
                }
            }
        }

        /// <summary>
        /// Withs the case insensitive property matching_ ignore case.
        /// </summary>
        [Fact]
        public void WithCaseInsensitivePropertyMatching_IgnoreCase()
        {
            var yaml = @"PrOpErTy: Value
fIeLd: Value
";
            var deserializer = new DeserializerBuilder().WithCaseInsensitivePropertyMatching().Build();
            var test = deserializer.Deserialize<CaseInsensitiveTest>(yaml);
            Assert.Equal("Value", test.Property);
            Assert.Equal("Value", test.Field);
        }

        /// <summary>
        /// Privates the members expose yaml member attribute.
        /// </summary>
        [Fact]
        public void PrivateMembersExposeYamlMemberAttribute()
        {
            var yaml = "key: value";

            var result = new DeserializerBuilder()
                .IncludeNonPublicProperties()
                .Build()
                .Deserialize<PrivateYamlMemberTest>(yaml);

            Assert.Equal("value", result.PublicValue);
        }

        class PrivateYamlMemberTest
        {
            [YamlMember(Alias = "key")]
            private string YamlValue { get; set; } = null!;

            public string PublicValue => YamlValue;
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Withs the required member set_ throws when field not set.
        /// </summary>
        [Fact]
        public void WithRequiredMemberSet_ThrowsWhenFieldNotSet()
        {
            var deserializer = new DeserializerBuilder().WithEnforceRequiredMembers().Build();
            var yaml = "Property: test";
            Assert.Throws<YamlException>(() =>
            {
                deserializer.Deserialize<RequiredMemberClass>(yaml);
            });
        }

        /// <summary>
        /// Withs the required member set_ throws when property not set.
        /// </summary>
        [Fact]
        public void WithRequiredMemberSet_ThrowsWhenPropertyNotSet()
        {
            var deserializer = new DeserializerBuilder().WithEnforceRequiredMembers().Build();
            var yaml = "Field: test";
            Assert.Throws<YamlException>(() =>
            {
                deserializer.Deserialize<RequiredMemberClass>(yaml);
            });
        }

        /// <summary>
        /// Withs the required member set_ does not throw.
        /// </summary>
        [Fact]
        public void WithRequiredMemberSet_DoesNotThrow()
        {
            var deserializer = new DeserializerBuilder().WithEnforceRequiredMembers().Build();
            var yaml = @"Field: test-field
Property: test-property";
            var actual = deserializer.Deserialize<RequiredMemberClass>(yaml);
            Assert.Equal("test-field", actual.Field);
            Assert.Equal("test-property", actual.Property);
        }

        /// <summary>
        /// The required member class.
        /// </summary>
        public class RequiredMemberClass
        {
            public required string Field = string.Empty;
            /// <summary>
            /// Gets or sets the property.
            /// </summary>
            public required string Property { get; set; } = string.Empty;
        }
#endif

#if NET6_0_OR_GREATER
        /// <summary>
        /// Enums the deserialization uses enum member attribute.
        /// </summary>
        [Fact]
        public void EnumDeserializationUsesEnumMemberAttribute()
        {
            var deserializer = new DeserializerBuilder().Build();
            var yaml = "goodbye";
            var actual = deserializer.Deserialize<EnumMemberedEnum>(yaml);
            Assert.Equal(EnumMemberedEnum.Hello, actual);
        }

        /// <summary>
        /// Enums the deserialized uses enum name when member is empty.
        /// </summary>
        [Fact]
        public void EnumDeserializedUsesEnumNameWhenMemberIsEmpty()
        {
            var deserializer = new DeserializerBuilder().Build();
            var yaml = "EmptyValue";
            var actual = deserializer.Deserialize<EnumMemberedEnum>(yaml);
            Assert.Equal(EnumMemberedEnum.EmptyValue, actual);
        }

        /// <summary>
        /// Enums the deserialized uses enum name when member is null.
        /// </summary>
        [Fact]
        public void EnumDeserializedUsesEnumNameWhenMemberIsNull()
        {
            var deserializer = new DeserializerBuilder().Build();
            var yaml = "NullValue";
            var actual = deserializer.Deserialize<EnumMemberedEnum>(yaml);
            Assert.Equal(EnumMemberedEnum.NullValue, actual);
        }

        /// <summary>
        /// The enum membered enum.
        /// </summary>
        public enum EnumMemberedEnum
        {
            No = 0,

            [System.Runtime.Serialization.EnumMember(Value = "goodbye")]
            Hello = 1,

            [System.Runtime.Serialization.EnumMember(Value = "")]
            EmptyValue = 2,

            [System.Runtime.Serialization.EnumMember()]
            NullValue = 3

        }
#endif

#nullable enable
        /// <summary>
        /// The nullable default class.
        /// </summary>
        public class NullableDefaultClass
        {
            /// <summary>
            /// Gets or sets the test string.
            /// </summary>
            public string? TestString { get; set; }
            /// <summary>
            /// Gets or sets the test bool.
            /// </summary>
            public string? TestBool { get; set; }
            /// <summary>
            /// Gets or sets the test bool1.
            /// </summary>
            public string TestBool1 { get; set; } = "";
        }

        /// <summary>
        /// The nullable not default class.
        /// </summary>
        public class NullableNotDefaultClass
        {
            /// <summary>
            /// Gets or sets the test string.
            /// </summary>
            public string? TestString { get; set; }
            /// <summary>
            /// Gets or sets the test bool.
            /// </summary>
            public string TestBool { get; set; } = "";
            /// <summary>
            /// Gets or sets the test bool1.
            /// </summary>
            public string TestBool1 { get; set; } = "";
        }

        /// <summary>
        /// The non nullable class.
        /// </summary>
        public class NonNullableClass
        {
            /// <summary>
            /// Gets or sets the test.
            /// </summary>
            public string Test { get; set; } = "Some default value";
        }
#nullable disable

        /// <summary>
        /// The case insensitive test.
        /// </summary>
        public class CaseInsensitiveTest
        {
            /// <summary>
            /// Gets or sets the property.
            /// </summary>
            public string Property { get; set; }
            public string Field;
        }

        /// <summary>
        /// The test state.
        /// </summary>
        public class TestState
        {
            /// <summary>
            /// Gets or sets the on deserialized call count.
            /// </summary>
            public int OnDeserializedCallCount { get; set; }
            /// <summary>
            /// Gets or sets the on deserializing call count.
            /// </summary>
            public int OnDeserializingCallCount { get; set; }

            /// <summary>
            /// Gets or sets the test.
            /// </summary>
            public string Test { get; set; } = string.Empty;

            /// <summary>
            /// Deserializeds the.
            /// </summary>
            [OnDeserialized]
            public void Deserialized() => OnDeserializedCallCount++;

            /// <summary>
            /// Deserializings the.
            /// </summary>
            [OnDeserializing]
            public void Deserializing() => OnDeserializingCallCount++;
        }

        /// <summary>
        /// The test.
        /// </summary>
        public class Test
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// The setter only.
        /// </summary>
        public class SetterOnly
        {
            private string _value;
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { set => _value = value; }
            /// <summary>
            /// Gets the actual.
            /// </summary>
            public string Actual { get => _value; }
        }

        /// <summary>
        /// The person.
        /// </summary>
        public class Person
        {
            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the moment of birth.
            /// </summary>
            public DateTime MomentOfBirth { get; private set; }

            /// <summary>
            /// Gets the cars.
            /// </summary>
            public IList<ICar> Cars { get; private set; }
        }

        /// <summary>
        /// The car.
        /// </summary>
        public class Car : ICar
        {
            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the year.
            /// </summary>
            public int Year { get; private set; }

            /// <summary>
            /// Gets the spec.
            /// </summary>
            public IModelSpec Spec { get; private set; }
        }

        public interface ICar
        {
            string Name { get; }

            int Year { get; }
            IModelSpec Spec { get; }
        }

        /// <summary>
        /// The model spec.
        /// </summary>
        public class ModelSpec : IModelSpec
        {
            /// <summary>
            /// Gets the engine type.
            /// </summary>
            public string EngineType { get; private set; }

            /// <summary>
            /// Gets the drive type.
            /// </summary>
            public string DriveType { get; private set; }
        }

        public interface IModelSpec
        {
            string EngineType { get; }

            string DriveType { get; }
        }

        /// <summary>
        /// The conference.
        /// </summary>
        public class Conference
        {
            /// <summary>
            /// Gets the sessions.
            /// </summary>
            public Session[] Sessions { get; private set; }

            /// <summary>
            /// Gets the attendees.
            /// </summary>
            public Attendee[] Attendees { get; private set; }
        }
        /// <summary>
        /// The attendee.
        /// </summary>
        public class Attendee
        {
            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the sessions.
            /// </summary>
            public Session[] Sessions { get; private set; }
        }

        /// <summary>
        /// The session.
        /// </summary>
        public class Session
        {
            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the greeter.
            /// </summary>
            public Attendee Greeter { get; private set; }
        }
    }
}
