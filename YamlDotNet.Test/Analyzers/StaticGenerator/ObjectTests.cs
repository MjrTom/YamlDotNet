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
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Callbacks;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Test.Analyzers.StaticGenerator
{
    /// <summary>
    /// The object tests.
    /// </summary>
    public class ObjectTests
    {
        /// <summary>
        /// Inheriteds the members works.
        /// </summary>
        [Fact]
        public void InheritedMembersWorks()
        {
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
            var yaml = @"NotInherited: hello
Inherited: world
";
            var actual = deserializer.Deserialize<SerializedInheritedClass>(yaml);
            Assert.Equal("hello", actual.NotInherited);
            Assert.Equal("world", actual.Inherited);
            var serializer = new StaticSerializerBuilder(new StaticContext()).Build();
            var actualYaml = serializer.Serialize(actual);
            Assert.Equal(yaml.NormalizeNewLines().TrimNewLines(), actualYaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Regulars the object works.
        /// </summary>
        [Fact]
        public void RegularObjectWorks()
        {
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
            var yaml = @"Prop1: hello
Prop2: 1
Hello: world
Inner:
  Prop1: a
  Prop2: 2
Nested:
  NestedProp: abc
DictionaryOfArrays:
  a:
  - 1
  b:
  - 2
SomeValue: ""abc""
SomeDictionary:
  a: 1
  b: 2
";
            var actual = deserializer.Deserialize<RegularObjectOuter>(yaml);
            Assert.Equal("hello", actual.Prop1);
            Assert.Equal(1, actual.Prop2);
            Assert.Equal("world", actual.Member);
            Assert.Equal("I am ignored", actual.Ignored);
            Assert.NotNull(actual.Inner);
            Assert.Equal("a", actual.Inner.Prop1);
            Assert.Equal(2, actual.Inner.Prop2);
            Assert.NotNull(actual.Nested);
            Assert.Equal("abc", actual.Nested.NestedProp);
            Assert.Equal("1", actual.DictionaryOfArrays["a"][0]);
            Assert.Equal("2", actual.DictionaryOfArrays["b"][0]);
            Assert.Equal("abc", actual.SomeValue);
            Assert.Equal("1", ((IDictionary<object, object>)actual.SomeDictionary)["a"]);
            Assert.Equal("2", ((IDictionary<object, object>)actual.SomeDictionary)["b"]);

            var serializer = new StaticSerializerBuilder(new StaticContext()).Build();
            var actualYaml = serializer.Serialize(actual);
            yaml = @"Prop1: hello
Prop2: 1
# A Description
Hello: ""world""
Inner:
  Prop1: a
  Prop2: 2
Nested:
  NestedProp: abc
DictionaryOfArrays:
  a:
  - 1
  b:
  - 2
SomeValue: abc
SomeDictionary:
  a: 1
  b: 2";
            Assert.Equal(yaml.NormalizeNewLines().TrimNewLines(), actualYaml.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Enumerables the are treated as lists.
        /// </summary>
        [Fact]
        public void EnumerablesAreTreatedAsLists() => ExecuteListOverrideTest<EnumerableClass>();

        /// <summary>
        /// Collections the are treated as lists.
        /// </summary>
        [Fact]
        public void CollectionsAreTreatedAsLists() => ExecuteListOverrideTest<CollectionClass>();

        /// <summary>
        /// IS the lists are treated as lists.
        /// </summary>
        [Fact]
        public void IListsAreTreatedAsLists() => ExecuteListOverrideTest<ListClass>();

        /// <summary>
        /// Reads the only collections are treated as lists.
        /// </summary>
        [Fact]
        public void ReadOnlyCollectionsAreTreatedAsLists() => ExecuteListOverrideTest<ReadOnlyCollectionClass>();

        /// <summary>
        /// Reads the only lists are treated as lists.
        /// </summary>
        [Fact]
        public void ReadOnlyListsAreTreatedAsLists() => ExecuteListOverrideTest<ReadOnlyListClass>();

        /// <summary>
        /// IS the list are treated as lists.
        /// </summary>
        [Fact]
        public void IListAreTreatedAsLists()
        {
            var yaml = @"Test:
- value1
- value2
";
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
            var actual = deserializer.Deserialize<CollectionClass>(yaml);
            Assert.NotNull(actual);
            Assert.IsType<List<string>>(actual.Test);
            Assert.Equal("value1", ((List<string>)actual.Test)[0]);
            Assert.Equal("value2", ((List<string>)actual.Test)[1]);
        }

        /// <summary>
        /// Callbacks the are executed.
        /// </summary>
        [Fact]
        public void CallbacksAreExecuted()
        {
            var yaml = "Test: Hi";
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
            var test = deserializer.Deserialize<TestState>(yaml);

            Assert.Equal(1, test.OnDeserializedCallCount);
            Assert.Equal(1, test.OnDeserializingCallCount);

            var serializer = new StaticSerializerBuilder(new StaticContext()).Build();
            yaml = serializer.Serialize(test);
            Assert.Equal(1, test.OnSerializedCallCount);
            Assert.Equal(1, test.OnSerializingCallCount);
        }

        /// <summary>
        /// Namings the convention applied to enum.
        /// </summary>
        [Fact]
        public void NamingConventionAppliedToEnum()
        {
            var serializer = new StaticSerializerBuilder(new StaticContext()).WithEnumNamingConvention(CamelCaseNamingConvention.Instance).Build();
            ScalarStyle style = ScalarStyle.Plain;
            var serialized = serializer.Serialize(style);
            Assert.Equal("plain", serialized.TrimNewLines());
        }

        /// <summary>
        /// Namings the convention applied to enum when deserializing.
        /// </summary>
        [Fact]
        public void NamingConventionAppliedToEnumWhenDeserializing()
        {
            var serializer = new StaticDeserializerBuilder(new StaticContext()).WithEnumNamingConvention(UnderscoredNamingConvention.Instance).Build();
            var yaml = "Double_Quoted";
            ScalarStyle expected = ScalarStyle.DoubleQuoted;
            var actual = serializer.Deserialize<ScalarStyle>(yaml);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Reads the only dictionaries are treated as dictionaries.
        /// </summary>
        [Fact]
        public void ReadOnlyDictionariesAreTreatedAsDictionaries()
        {
            var yaml = @"Test:
  a: b
  c: d
";
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
            var actual = deserializer.Deserialize<ReadOnlyDictionaryClass>(yaml);
            Assert.NotNull(actual);
            Assert.IsType<Dictionary<string, string>>(actual.Test);
            var dictionary = (Dictionary<string, string>)actual.Test;
            Assert.Equal(2, dictionary.Count);
            Assert.Equal("b", dictionary["a"]);
            Assert.Equal("d", dictionary["c"]);
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Enums the deserialization uses enum member attribute.
        /// </summary>
        [Fact]
        public void EnumDeserializationUsesEnumMemberAttribute()
        {
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
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
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
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
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
            var yaml = "NullValue";
            var actual = deserializer.Deserialize<EnumMemberedEnum>(yaml);
            Assert.Equal(EnumMemberedEnum.NullValue, actual);
        }

        /// <summary>
        /// Enums the serialization uses enum member attribute.
        /// </summary>
        [Fact]
        public void EnumSerializationUsesEnumMemberAttribute()
        {
            var serializer = new StaticSerializerBuilder(new StaticContext()).Build();
            var actual = serializer.Serialize(EnumMemberedEnum.Hello);
            Assert.Equal("goodbye", actual.TrimNewLines());
        }

        /// <summary>
        /// Enums the serialization uses enum member attribute with empty value.
        /// </summary>
        [Fact]
        public void EnumSerializationUsesEnumMemberAttributeWithEmptyValue()
        {
            var serializer = new StaticSerializerBuilder(new StaticContext()).Build();
            var actual = serializer.Serialize(new EnumMemberedEnumHarness { Test = EnumMemberedEnum.EmptyValue });
            Assert.Equal("Test: ''", actual.TrimNewLines());
        }

        /// <summary>
        /// Enums the serialization uses enum member attribute with null value.
        /// </summary>
        [Fact]
        public void EnumSerializationUsesEnumMemberAttributeWithNullValue()
        {
            var serializer = new StaticSerializerBuilder(new StaticContext()).Build();
            var actual = serializer.Serialize(EnumMemberedEnum.NullValue);
            Assert.Equal("NullValue", actual.TrimNewLines());
        }

        /// <summary>
        /// The enum membered enum harness.
        /// </summary>
        [YamlSerializable]
        public class EnumMemberedEnumHarness
        {
            /// <summary>
            /// Gets or sets the test.
            /// </summary>
            public EnumMemberedEnum Test { get; set; }
        }

        /// <summary>
        /// The enum membered enum.
        /// </summary>
        [YamlSerializable]
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
        /// <summary>
        /// Complexes the type converter_ uses serializer to serialize complex types.
        /// </summary>
        [Fact]
        public void ComplexTypeConverter_UsesSerializerToSerializeComplexTypes()
        {
            var serializer = new StaticSerializerBuilder(new StaticContext()).WithTypeConverter(new ComplexTypeConverter()).Build();
            var o = new ComplexType
            {
                InnerType1 = new InnerType
                {
                    Prop1 = "prop1",
                    Prop2 = "prop2"
                },
                InnerType2 = new InnerType
                {
                    Prop1 = "2.1",
                    Prop2 = "2.2"
                }
            };
            var actual = serializer.Serialize(o);
            var expected = @"inner.prop1: prop1
inner.prop2: prop2
prop2:
  Prop1: 2.1
  Prop2: 2.2".NormalizeNewLines();
            Assert.Equal(expected, actual.NormalizeNewLines().TrimNewLines());
        }

        /// <summary>
        /// Complexes the type converter_ uses deserializer to deserialize complex types.
        /// </summary>
        [Fact]
        public void ComplexTypeConverter_UsesDeserializerToDeserializeComplexTypes()
        {
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).WithTypeConverter(new ComplexTypeConverter()).Build();
            var yaml = @"inner.prop1: prop1
inner.prop2: prop2
prop2:
  Prop1: 2.1
  Prop2: 2.2";
            var actual = deserializer.Deserialize<ComplexType>(yaml);
            Assert.Equal("prop1", actual.InnerType1.Prop1);
            Assert.Equal("prop2", actual.InnerType1.Prop2);
            Assert.Equal("2.1", actual.InnerType2.Prop1);
            Assert.Equal("2.2", actual.InnerType2.Prop2);
        }

        /// <summary>
        /// The complex type.
        /// </summary>
        [YamlSerializable]
        public class ComplexType
        {
            /// <summary>
            /// Gets or sets the inner type1.
            /// </summary>
            public InnerType InnerType1 { get; set; }
            /// <summary>
            /// Gets or sets the inner type2.
            /// </summary>
            public InnerType InnerType2 { get; set; }
        }

        /// <summary>
        /// The inner type.
        /// </summary>
        [YamlSerializable]
        public class InnerType
        {
            /// <summary>
            /// Gets or sets the prop1.
            /// </summary>
            public string Prop1 { get; set; }
            /// <summary>
            /// Gets or sets the prop2.
            /// </summary>
            public string Prop2 { get; set; }
        }

        private class ComplexTypeConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return type == typeof(ComplexType);
            }

            public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
            {
                parser.Consume<MappingStart>();

                var result = new ComplexType();
                result.InnerType1 = new InnerType();

                Consume(parser, result, rootDeserializer);
                Consume(parser, result, rootDeserializer);
                Consume(parser, result, rootDeserializer);

                parser.Consume<MappingEnd>();

                return result;
            }

            public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
            {
                var c = (ComplexType)value;
                emitter.Emit(new MappingStart());
                emitter.Emit(new Scalar("inner.prop1"));
                emitter.Emit(new Scalar(c.InnerType1.Prop1));
                emitter.Emit(new Scalar("inner.prop2"));
                emitter.Emit(new Scalar(c.InnerType1.Prop2));
                emitter.Emit(new Scalar("prop2"));
                serializer(c.InnerType2);
                emitter.Emit(new MappingEnd());
            }

            private void Consume(IParser parser, ComplexType type, ObjectDeserializer deserializer)
            {
                var name = parser.Consume<Scalar>();
                if (name.Value == "inner.prop1")
                {
                    var value = parser.Consume<Scalar>();
                    type.InnerType1.Prop1 = value.Value;
                }
                else if (name.Value == "inner.prop2")
                {
                    var value = parser.Consume<Scalar>();
                    type.InnerType1.Prop2 = value.Value;
                }
                else if (name.Value == "prop2")
                {
                    var value = deserializer(typeof(InnerType));
                    type.InnerType2 = (InnerType)value;
                }
                else
                {
                    throw new Exception("Invalid property name");
                }
            }
        }

        private void ExecuteListOverrideTest<TClass>() where TClass : InterfaceLists
        {
            var yaml = @"Test:
- value1
- value2
";
            var deserializer = new StaticDeserializerBuilder(new StaticContext()).Build();
            var actual = deserializer.Deserialize<TClass>(yaml);
            Assert.NotNull(actual);
            Assert.IsType<List<string>>(actual.TestValue);
            Assert.Equal("value1", ((List<string>)actual.TestValue)[0]);
            Assert.Equal("value2", ((List<string>)actual.TestValue)[1]);
        }

        /// <summary>
        /// The test state.
        /// </summary>
        [YamlSerializable]
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
            /// Deserializeds the.
            /// </summary>
            [OnDeserialized]
            public void Deserialized() => OnDeserializedCallCount++;

            /// <summary>
            /// Deserializings the.
            /// </summary>
            [OnDeserializing]
            public void Deserializing() => OnDeserializingCallCount++;

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

    }
    /// <summary>
    /// The inherited class.
    /// </summary>
    public class InheritedClass
    {
        /// <summary>
        /// Gets or sets the inherited.
        /// </summary>
        public string Inherited { get; set; }
    }

    /// <summary>
    /// The serialized inherited class.
    /// </summary>
    [YamlSerializable]
    public class SerializedInheritedClass : InheritedClass
    {
        /// <summary>
        /// Gets or sets the not inherited.
        /// </summary>
        public string NotInherited { get; set; }
    }

    /// <summary>
    /// The regular object outer.
    /// </summary>
    [YamlSerializable]
    public class RegularObjectOuter
    {
        /// <summary>
        /// Gets or sets the prop1.
        /// </summary>
        public string Prop1 { get; set; }
        /// <summary>
        /// Gets or sets the prop2.
        /// </summary>
        public int Prop2 { get; set; }
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        [YamlMember(Alias = "Hello", Description = "A Description", ScalarStyle = YamlDotNet.Core.ScalarStyle.DoubleQuoted)]
        public string Member { get; set; }
        /// <summary>
        /// Gets or sets the ignored.
        /// </summary>
        [YamlIgnore]
        public string Ignored { get; set; } = "I am ignored";
        /// <summary>
        /// Gets or sets the inner.
        /// </summary>
        public RegularObjectInner Inner { get; set; }
        /// <summary>
        /// Gets or sets the nested.
        /// </summary>
        public NestedClass Nested { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of arrays.
        /// </summary>
        public Dictionary<string, string[]> DictionaryOfArrays { get; set; }

        /// <summary>
        /// Gets or sets the some value.
        /// </summary>
        public object SomeValue { get; set; }

        /// <summary>
        /// Gets or sets the some dictionary.
        /// </summary>
        public object SomeDictionary { get; set; }

        /// <summary>
        /// The nested class.
        /// </summary>
        [YamlSerializable]
        public class NestedClass
        {
            /// <summary>
            /// Gets or sets the nested prop.
            /// </summary>
            public string NestedProp { get; set; }
        }
    }

    /// <summary>
    /// The regular object inner.
    /// </summary>
    [YamlSerializable]
    public class RegularObjectInner
    {
        /// <summary>
        /// Gets or sets the prop1.
        /// </summary>
        public string Prop1 { get; set; }
        /// <summary>
        /// Gets or sets the prop2.
        /// </summary>
        public int Prop2 { get; set; }
    }

    /// <summary>
    /// The enumerable class.
    /// </summary>
    [YamlSerializable]
    public class EnumerableClass : InterfaceLists<IEnumerable<string>>
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public IEnumerable<string> Test { get; set; }
        /// <summary>
        /// Gets the test value.
        /// </summary>
        public object TestValue => Test;
    }

    /// <summary>
    /// The collection class.
    /// </summary>
    [YamlSerializable]
    public class CollectionClass : InterfaceLists<ICollection<string>>
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public ICollection<string> Test { get; set; }
        /// <summary>
        /// Gets the test value.
        /// </summary>
        public object TestValue => Test;
    }

    /// <summary>
    /// The list class.
    /// </summary>
    [YamlSerializable]
    public class ListClass : InterfaceLists<IList<string>>
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public IList<string> Test { get; set; }
        /// <summary>
        /// Gets the test value.
        /// </summary>
        public object TestValue => Test;
    }

    /// <summary>
    /// The read only collection class.
    /// </summary>
    [YamlSerializable]
    public class ReadOnlyCollectionClass : InterfaceLists<IReadOnlyCollection<string>>
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public IReadOnlyCollection<string> Test { get; set; }
        /// <summary>
        /// Gets the test value.
        /// </summary>
        public object TestValue => Test;
    }

    /// <summary>
    /// The read only list class.
    /// </summary>
    [YamlSerializable]
    public class ReadOnlyListClass : InterfaceLists<IReadOnlyList<string>>
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public IReadOnlyList<string> Test { get; set; }
        /// <summary>
        /// Gets the test value.
        /// </summary>
        public object TestValue => Test;
    }

    /// <summary>
    /// The read only dictionary class.
    /// </summary>
    [YamlSerializable]
    public class ReadOnlyDictionaryClass
    {
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        public IReadOnlyDictionary<string, string> Test { get; set; }
    }

    public interface InterfaceLists<TType> : InterfaceLists where TType : IEnumerable
    {
        TType Test { get; set; }
    }

    public interface InterfaceLists
    {
        object TestValue { get; }
    }
}
