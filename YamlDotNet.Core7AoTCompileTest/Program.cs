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
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8618 // Possible null reference argument.
#pragma warning disable CS8602 // Possible null reference argument.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Core7AoTCompileTest.Model;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Callbacks;

string yaml = string.Create(CultureInfo.InvariantCulture, $@"MyBool: true
hi: 1
MyChar: h
MyDateTime: {DateTime.Now}
MyDecimal: 123.935
MyDouble: 456.789
MyEnumY: Y
MyEnumZ: 1
MyInt16: {short.MaxValue}
MyInt32: {int.MaxValue}
MyInt64: {long.MaxValue}
MySByte: {sbyte.MaxValue}
MySingle: {float.MaxValue}
MyString: hello world
MyUInt16: {ushort.MaxValue}
MyUInt32: {uint.MaxValue}
MyUInt64: {ulong.MaxValue}
Inner:
  Text: yay
InnerArray:
  - Text: hello
  - Text: world
MyArray:
  myArray:
  - 1
  - 2
  - 3
MyDictionary:
  x: y
  a: b
MyDictionaryOfArrays:
  a:
  - a
  - b
  b:
  - c
  - d
MyList:
- a
- b
Inherited:
  Inherited: hello
  NotInherited: world
External:
  Text: hello
SomeCollectionStrings:
- test
- value
SomeEnumerableStrings:
- test
- value
SomeObject: a
SomeDictionary:
  a: 1
  b: 2
StructField:
  X: 1
  Y: 2
  Nested:
    X: 3
    Y: 4
StructProperty:
  X: 5
  Y: 6
  Nested:
    X: 7
    Y: 8
");

var input = new StringReader(yaml);

var aotContext = new YamlDotNet.Core7AoTCompileTest.StaticContext();
var deserializer = new StaticDeserializerBuilder(aotContext)
    .Build();

var x = deserializer.Deserialize<PrimitiveTypes>(input);
Console.WriteLine("Object read:");
Console.WriteLine("MyBool: <{0}>", x.MyBool);
Console.WriteLine("MyByte: <{0}>", x.MyByte);
Console.WriteLine("MyChar: <{0}>", x.MyChar);
Console.WriteLine("MyDateTime: <{0}>", x.MyDateTime);
Console.WriteLine("MyEnumY: <{0}>", x.MyEnumY);
Console.WriteLine("MyEnumZ: <{0}>", x.MyEnumZ);
Console.WriteLine("MyInt16: <{0}>", x.MyInt16);
Console.WriteLine("MyInt32: <{0}>", x.MyInt32);
Console.WriteLine("MyInt64: <{0}>", x.MyInt64);
Console.WriteLine("MySByte: <{0}>", x.MySByte);
Console.WriteLine("MyString: <{0}>", x.MyString);
Console.WriteLine("MyUInt16: <{0}>", x.MyUInt16);
Console.WriteLine("MyUInt32: <{0}>", x.MyUInt32);
Console.WriteLine("MyUInt64: <{0}>", x.MyUInt64);
Console.WriteLine("Inner == null: <{0}>", x.Inner == null);
Console.WriteLine("Inner.Text: <{0}>", x.Inner?.Text);
Console.WriteLine("External.Text: <{0}>", x.External?.Text);
foreach (var inner in x.InnerArray)
{
    Console.WriteLine("InnerArray.Text: <{0}>", inner.Text);
}
Console.WriteLine("MyArray == null: <{0}>", x.MyArray == null);
Console.WriteLine("MyArray.myArray == null: <{0}>", x.MyArray?.myArray == null);

if (x.MyArray?.myArray != null)
{
    Console.WriteLine("MyArray.myArray: <{0}>", string.Join(',', x.MyArray.myArray));
}

Console.WriteLine("MyDictionary == null: <{0}>", x.MyDictionary == null);
if (x.MyDictionary != null)
{
    foreach (var kvp in x.MyDictionary)
    {
        Console.WriteLine("MyDictionary[{0}] = <{1}>", kvp.Key, kvp.Value);
    }
}

Console.WriteLine("MyDictionaryOfArrays == null: <{0}>", x.MyDictionaryOfArrays == null);
if (x.MyDictionaryOfArrays != null)
{
    foreach (var kvp in x.MyDictionaryOfArrays)
    {
        Console.WriteLine("MyDictionaryOfArrays[{0}] = <{1}>", kvp.Key, string.Join(',', kvp.Value));
    }
}

Console.WriteLine("MyList == null: <{0}>", x.MyList == null);
if (x.MyList != null)
{
    foreach (var value in x.MyList)
    {
        Console.WriteLine("MyList = <{0}>", value);
    }
}
Console.WriteLine("Inherited == null: <{0}>", x.Inherited == null);
Console.WriteLine("Inherited.Inherited: <{0}>", x.Inherited?.Inherited);
Console.WriteLine("Inherited.NotInherited: <{0}>", x.Inherited?.NotInherited);
Console.WriteLine("SomeEnumerableStrings:");
foreach (var s in x.SomeEnumerableStrings)
{
    Console.WriteLine("  {0}", s);
}
Console.ReadLine();
Console.WriteLine("SomeCollectionStrings:");
foreach (var s in x.SomeCollectionStrings)
{
    Console.WriteLine("  {0}", s);
}
Console.WriteLine("Structs:");
Console.WriteLine("  StructField: <{0},{1}>", x.StructField.X, x.StructField.Y);
Console.WriteLine("    Nested: <{0},{1}>", x.StructField.Nested.X, x.StructField.Nested.Y);
Console.WriteLine("  StructProperty: <{0},{1}>", x.StructProperty.X, x.StructProperty.Y);
Console.WriteLine("    Nested: <{0},{1}>", x.StructProperty.Nested.X, x.StructProperty.Nested.Y);

Console.WriteLine("==============");
Console.WriteLine("Serialized:");

var serializer = new StaticSerializerBuilder(aotContext)
    .Build();

var output = serializer.Serialize(x);
Console.WriteLine(output);
Console.WriteLine("============== Done with the primary object");

yaml = @"- myArray:
  - 1
  - 2
- myArray:
  - 3
  - 4
";

var o = deserializer.Deserialize<MyArray[]>(yaml);
Console.WriteLine("Length: <{0}>", o.Length);
Console.WriteLine("Items[0]: <{0}>", string.Join(',', o[0].myArray));
Console.WriteLine("Items[1]: <{0}>", string.Join(',', o[1].myArray));

deserializer = new StaticDeserializerBuilder(aotContext).WithEnforceNullability().Build();
yaml = "Nullable: null";
var nullable = deserializer.Deserialize<NullableTestClass>(yaml);
Console.WriteLine("Nullable Value (should be empty): <{0}>", nullable.Nullable);
yaml = "NotNullable: test";
nullable = deserializer.Deserialize<NullableTestClass>(yaml);
Console.WriteLine("NotNullable Value (should be test): <{0}>", nullable.NotNullable);
try
{
    yaml = "NotNullable: null";
    nullable = deserializer.Deserialize<NullableTestClass>(yaml);
    throw new Exception("NotNullable should not be allowed to be set to null.");
}
catch (YamlException exception)
{
    if (exception.InnerException is NullReferenceException)
    {
        Console.WriteLine("Exception thrown while setting non nullable value to null, as it should.");
    }
    else
    {
        throw new Exception("NotNullable should not be allowed to be set to null.");
    }
}

Console.WriteLine("The next line should say goodbye");
Console.WriteLine(serializer.Serialize(EnumMemberedEnum.Hello));
Console.WriteLine("The next line should say hello");
Console.WriteLine(deserializer.Deserialize<EnumMemberedEnum>("goodbye"));
/// <summary>
/// The my array.
/// </summary>

[YamlSerializable]
public class MyArray
{
    /// <summary>
    /// Gets or sets the my array.
    /// </summary>
    public int[]? myArray { get; set; }
}
/// <summary>
/// The inner.
/// </summary>

[YamlSerializable]
public class Inner
{
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string? Text { get; set; }
}
/// <summary>
/// The nullable test class.
/// </summary>

[YamlSerializable]
public class NullableTestClass
{
    /// <summary>
    /// Gets or sets the nullable.
    /// </summary>
    public string? Nullable { get; set; }
    /// <summary>
    /// Gets or sets the not nullable.
    /// </summary>
    public string NotNullable { get; set; }
}
/// <summary>
/// The primitive types.
/// </summary>

[YamlSerializable]
public class PrimitiveTypes
{
    /// <summary>
    /// Gets or sets a value indicating whether my bool.
    /// </summary>
    [YamlMember(Description = "hi world!")]
    public bool MyBool { get; set; }
    /// <summary>
    /// Gets or sets the my byte.
    /// </summary>
    [YamlMember(Alias = "hi")]
    public byte MyByte { get; set; }
    /// <summary>
    /// Gets or sets the my char.
    /// </summary>
    public char MyChar { get; set; }
    /// <summary>
    /// Gets or sets the my decimal.
    /// </summary>
    public decimal MyDecimal { get; set; }
    /// <summary>
    /// Gets or sets the my double.
    /// </summary>
    public double MyDouble { get; set; }
    /// <summary>
    /// Gets or sets the my date time.
    /// </summary>
    public DateTime MyDateTime { get; set; }
    /// <summary>
    /// Gets or sets the my enum y.
    /// </summary>
    public MyTestEnum MyEnumY { get; set; }
    /// <summary>
    /// Gets or sets the my enum z.
    /// </summary>
    public MyTestEnum MyEnumZ { get; set; }
    /// <summary>
    /// Gets or sets the my int16.
    /// </summary>
    public short MyInt16 { get; set; }
    /// <summary>
    /// Gets or sets the my int32.
    /// </summary>
    public int MyInt32 { get; set; }
    /// <summary>
    /// Gets or sets the my int64.
    /// </summary>
    public long MyInt64 { get; set; }
    /// <summary>
    /// Gets or sets the my s byte.
    /// </summary>
    public sbyte MySByte { get; set; }
    /// <summary>
    /// Gets or sets the my single.
    /// </summary>
    public float MySingle { get; set; }
    /// <summary>
    /// Gets or sets the my string.
    /// </summary>
    [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
    public string MyString { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the my nullable string.
    /// </summary>
    public string? MyNullableString { get; set; }
    /// <summary>
    /// Gets or sets the my u int16.
    /// </summary>
    public ushort MyUInt16 { get; set; }
    /// <summary>
    /// Gets or sets the my u int32.
    /// </summary>
    public uint MyUInt32 { get; set; }
    /// <summary>
    /// Gets or sets the my u int64.
    /// </summary>
    public ulong MyUInt64 { get; set; }
    /// <summary>
    /// Gets or sets the inner.
    /// </summary>
    public Inner? Inner { get; set; }
    /// <summary>
    /// Gets or sets the inner array.
    /// </summary>
    public Inner[]? InnerArray { get; set; }
    /// <summary>
    /// Gets or sets the my array.
    /// </summary>
    public MyArray? MyArray { get; set; }
    /// <summary>
    /// Gets or sets the my dictionary.
    /// </summary>
    public Dictionary<string, string>? MyDictionary { get; set; }
    /// <summary>
    /// Gets or sets the my dictionary of arrays.
    /// </summary>
    public Dictionary<string, string[]>? MyDictionaryOfArrays { get; set; }
    /// <summary>
    /// Gets or sets the my list.
    /// </summary>
    public List<string>? MyList { get; set; }
    /// <summary>
    /// Gets or sets the inherited.
    /// </summary>
    public Inherited Inherited { get; set; }
    /// <summary>
    /// Gets or sets the external.
    /// </summary>
    public ExternalModel External { get; set; }
    /// <summary>
    /// Gets or sets the some enumerable strings.
    /// </summary>
    public IEnumerable<string> SomeEnumerableStrings { get; set; }
    /// <summary>
    /// Gets or sets the some collection strings.
    /// </summary>
    public ICollection<string> SomeCollectionStrings { get; set; }
    /// <summary>
    /// Gets or sets the some object.
    /// </summary>
    public object SomeObject { get; set; }
    /// <summary>
    /// Gets or sets the some dictionary.
    /// </summary>
    public object SomeDictionary { get; set; }
    public MyTestStruct StructField;
    /// <summary>
    /// Gets or sets the struct property.
    /// </summary>
    public MyTestStruct StructProperty { get; set; }
}
/// <summary>
/// The inherited base.
/// </summary>

public class InheritedBase
{
    /// <summary>
    /// Gets or sets the inherited.
    /// </summary>
    public string Inherited { get; set; }
}
/// <summary>
/// The inherited.
/// </summary>

[YamlSerializable]
public class Inherited : InheritedBase
{
    /// <summary>
    /// Gets or sets the not inherited.
    /// </summary>
    public string NotInherited { get; set; }


    /// <summary>
    /// Serializings the.
    /// </summary>
    [OnSerializing]
    public void Serializing()
    {
        Console.WriteLine("Serializing");
    }

    /// <summary>
    /// Serializeds the.
    /// </summary>
    [OnSerialized]
    public void Serialized()
    {
        Console.WriteLine("Serialized");
    }

    /// <summary>
    /// Deserializeds the.
    /// </summary>
    [OnDeserialized]
    public void Deserialized()
    {
        Console.WriteLine("Deserialized");
    }

    /// <summary>
    /// Deserializings the.
    /// </summary>
    [OnDeserializing]
    public void Deserializing()
    {
        Console.WriteLine("Deserializing");
    }

}
/// <summary>
/// The my test enum.
/// </summary>

public enum MyTestEnum
{
    Y = 0,
    Z = 1,
}
/// <summary>
/// The enum membered enum.
/// </summary>

[YamlSerializable]
public enum EnumMemberedEnum
{
    No = 0,

    [System.Runtime.Serialization.EnumMember(Value = "goodbye")]
    Hello = 1
}

[YamlSerializable]
public struct MyTestStruct
{
    public float X;
    public float Y;
    public MyTestNestedStruct Nested;

    [OnSerializing]
    public void Serializing()
    {
        Console.WriteLine("MyTestStruct: Serializing");
    }

    [OnSerialized]
    public void Serialized()
    {
        Console.WriteLine("MyTestStruct: Serialized");
    }

    [OnDeserialized]
    public void Deserialized()
    {
        Console.WriteLine("MyTestStruct: Deserialized");
    }

    [OnDeserializing]
    public void Deserializing()
    {
        Console.WriteLine("MyTestStruct: Deserializing");
    }
}

[YamlSerializable]
public struct MyTestNestedStruct
{
    public float X;
    public float Y;
}

#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8618 // Possible null reference argument.
#pragma warning restore CS8602 // Possible null reference argument.
