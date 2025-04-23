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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace YamlDotNet.Analyzers.StaticGenerator
{
    /// <summary>
    /// The static object factory file.
    /// </summary>
    public class StaticObjectFactoryFile : File
    {
        public StaticObjectFactoryFile(Action<string, bool> write, Action indent, Action unindent, GeneratorExecutionContext context) : base(write, indent, unindent, context)
        {
        }

        /// <summary>
        /// Writes the.
        /// </summary>
        /// <param name="syntaxReceiver">The syntax receiver.</param>
        public override void Write(SerializableSyntaxReceiver syntaxReceiver)
        {
            Write($"class StaticObjectFactory : YamlDotNet.Serialization.ObjectFactories.StaticObjectFactory");
            Write("{"); Indent();

            Write("public override object Create(Type type)");
            Write("{"); Indent();
            foreach (var o in syntaxReceiver.Classes.Where(c => !c.Value.IsArray))
            {
                var classObject = o.Value;
                if (o.Value.IsListOverride)
                {
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetNamespace()}.{classObject.ModuleSymbol.Name}<{((INamedTypeSymbol)classObject.ModuleSymbol).TypeArguments[0].GetFullName().Replace("?", string.Empty)}>)) return new System.Collections.Generic.List<{((INamedTypeSymbol)classObject.ModuleSymbol).TypeArguments[0].GetFullName().Replace("?", string.Empty)}>();");
                }
                else if (o.Value.IsDictionaryOverride)
                {
                    var keyType = ((INamedTypeSymbol)classObject.ModuleSymbol).TypeArguments[0].GetFullName().Replace("?", string.Empty);
                    var valueType = ((INamedTypeSymbol)classObject.ModuleSymbol).TypeArguments[1].GetFullName().Replace("?", string.Empty);
                    //Write("/* this is a dictionary override: ");
                    //Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return new System.Collections.Dictionary<{keyType}, {valueType}>();");
                    //Write("*/");
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return new System.Collections.Generic.Dictionary<{keyType}, {valueType}>();");
                }
                else
                {
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return new {classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}();");
                }
                //always support a list and dictionary of the type
                Write($"if (type == typeof(System.Collections.Generic.List<{classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return new System.Collections.Generic.List<{classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>();");
                Write($"if (type == typeof(System.Collections.Generic.Dictionary<string, {classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return new System.Collections.Generic.Dictionary<string, {classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>();");
            }
            // always support dictionary when deserializing object
            Write("if (type == typeof(System.Collections.Generic.Dictionary<object, object>)) return new System.Collections.Generic.Dictionary<object, object>();");
            Write($"throw new ArgumentOutOfRangeException(\"Unknown type: \" + type.ToString());");
            UnIndent(); Write("}");

            Write("public override Array CreateArray(Type type, int count)");
            Write("{"); Indent();
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                if (classObject.IsArray)
                {
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return new {classObject.ModuleSymbol.GetFullName(false).Replace("?", string.Empty)}[count];");
                }
                else
                {
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}[])) return new {classObject.ModuleSymbol.GetFullName(false).Replace("?", string.Empty)}[count];");
                }
            }
            Write($"throw new ArgumentOutOfRangeException(\"Unknown type: \" + type.ToString());");
            UnIndent(); Write("}");

            Write("public override bool IsDictionary(Type type)");
            Write("{"); Indent();
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                if (classObject.IsDictionary || classObject.IsDictionaryOverride)
                {
                    Write($"if (type == typeof({o.Value.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return true;");
                }
                else
                {
                    //always support a dictionary of the type
                    Write($"if (type == typeof(System.Collections.Generic.Dictionary<string, {classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return true;");
                }
            }
            // always support dictionary object
            Write("if (type == typeof(System.Collections.Generic.Dictionary<object, object>)) return true;");
            Write("return false;");
            UnIndent(); Write("}");

            Write("public override bool IsArray(Type type)");
            Write("{"); Indent();
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                if (classObject.IsArray)
                {
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return true;");
                }
                else
                {
                    //always support an array of the type
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}[])) return true;");
                }
            }
            Write("return false;");
            UnIndent(); Write("}");

            Write("public override bool IsList(Type type)");
            Write("{"); Indent();
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                if (classObject.IsList || classObject.IsListOverride)
                {
                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return true;");
                }
                else
                {
                    //always support a list of the type
                    Write($"if (type == typeof(System.Collections.Generic.List<{classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return true;");

                    //we'll make ienumerables lists.
                    Write($"if (type == typeof(System.Collections.Generic.IEnumerable<{classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return true;");
                }
            }
            Write("return false;");
            UnIndent(); Write("}");

            Write("public override Type GetKeyType(Type type)");
            Write("{"); Indent();
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                if (classObject.IsDictionary || classObject.IsDictionaryOverride)
                {
                    var keyType = "object";
                    var type = (INamedTypeSymbol)classObject.ModuleSymbol;

                    if (type.IsGenericType)
                    {
                        keyType = type.TypeArguments[0].GetFullName().Replace("?", string.Empty);
                    }

                    Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return typeof({keyType});");
                }
                else if (!classObject.IsArray && !classObject.IsList && !classObject.IsListOverride)
                {
                    //always support a dictionary of the type
                    Write($"if (type == typeof(System.Collections.Generic.Dictionary<string, {classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)});");
                }
            }

            // always support dictionary object
            Write("if (type == typeof(System.Collections.Generic.Dictionary<object, object>)) return typeof(object);");
            Write("throw new ArgumentOutOfRangeException(\"Unknown type: \" + type.ToString());");
            UnIndent(); Write("}");

            Write("public override Type GetValueType(Type type)");
            Write("{"); Indent();
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                if (!(classObject.IsList || classObject.IsDictionary || classObject.IsDictionaryOverride || classObject.IsArray || classObject.IsListOverride))
                {
                    continue;
                }

                string valueType;
                if (classObject.IsDictionary || classObject.IsDictionaryOverride)
                {
                    valueType = ((INamedTypeSymbol)classObject.ModuleSymbol).TypeArguments[1].GetFullName().Replace("?", string.Empty);
                }
                else if (classObject.IsList || classObject.IsListOverride)
                {
                    valueType = ((INamedTypeSymbol)classObject.ModuleSymbol).TypeArguments[0].GetFullName().Replace("?", string.Empty);
                }
                else
                {
                    valueType = ((IArrayTypeSymbol)(classObject.ModuleSymbol)).ElementType.GetFullName().Replace("?", string.Empty);
                }

                Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)})) return typeof({valueType});");
            }

            //always support array, list, dictionary and Ienumerables of all types
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                Write($"if (type == typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}[])) return typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)});");
                Write($"if (type == typeof(System.Collections.Generic.IEnumerable<{classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)});");
                Write($"if (type == typeof(System.Collections.Generic.List<{classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)});");
                Write($"if (type == typeof(System.Collections.Generic.Dictionary<string, {classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)}>)) return typeof({classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty)});");
            }

            Write("if (type == typeof(System.Collections.Generic.Dictionary<object, object>)) return typeof(object);");
            Write("throw new ArgumentOutOfRangeException(\"Unknown type: \" + type.ToString());");
            UnIndent(); Write("}");
            WriteExecuteMethod(syntaxReceiver, "ExecuteOnDeserializing", (c) => c.OnDeserializingMethods);
            WriteExecuteMethod(syntaxReceiver, "ExecuteOnDeserialized", (c) => c.OnDeserializedMethods);
            WriteExecuteMethod(syntaxReceiver, "ExecuteOnSerializing", (c) => c.OnSerializingMethods);
            WriteExecuteMethod(syntaxReceiver, "ExecuteOnSerialized", (c) => c.OnSerializedMethods);
            UnIndent(); Write("}");
        }

        private void WriteExecuteMethod(SerializableSyntaxReceiver syntaxReceiver, string methodName, Func<ClassObject, IEnumerable<IMethodSymbol>> selector)
        {
            Write($"public override void {methodName}(object value)");
            Write("{"); Indent();
            Write("if (value == null) return;");
            Write("var type = value.GetType();");
            foreach (var o in syntaxReceiver.Classes)
            {
                var classObject = o.Value;
                var methods = selector(classObject);
                if (methods.Any())
                {
                    var className = classObject.ModuleSymbol.GetFullName().Replace("?", string.Empty);
                    Write($"if (type == typeof({className}))");
                    Write("{"); Indent();
                    foreach (var m in methods)
                    {
                        Write($"(({className})value).{m.Name}();");
                    }
                    Write("return;");
                    UnIndent(); Write("}");
                }
            }
            UnIndent(); Write("}");
        }
    }
}
