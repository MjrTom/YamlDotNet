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
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace YamlDotNet.Analyzers.StaticGenerator
{
    /// <summary>
    /// The class object.
    /// </summary>
    public class ClassObject
    {
        /// <summary>
        /// Gets the field symbols.
        /// </summary>
        public List<IFieldSymbol> FieldSymbols { get; }
        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName { get; }
        /// <summary>
        /// Gets the guid suffix.
        /// </summary>
        public string GuidSuffix { get; }
        /// <summary>
        /// Gets a value indicating whether is array.
        /// </summary>
        public bool IsArray { get; }
        /// <summary>
        /// Gets a value indicating whether is dictionary.
        /// </summary>
        public bool IsDictionary { get; }
        /// <summary>
        /// Gets a value indicating whether dictionary is override.
        /// </summary>
        public bool IsDictionaryOverride { get; }
        /// <summary>
        /// Gets a value indicating whether is list.
        /// </summary>
        public bool IsList { get; }
        /// <summary>
        /// Gets a value indicating whether list is override.
        /// </summary>
        public bool IsListOverride { get; }
        /// <summary>
        /// Gets the module symbol.
        /// </summary>
        public ITypeSymbol ModuleSymbol { get; }
        /// <summary>
        /// Gets the on deserialized methods.
        /// </summary>
        public List<IMethodSymbol> OnDeserializedMethods { get; }
        /// <summary>
        /// Gets the on deserializing methods.
        /// </summary>
        public List<IMethodSymbol> OnDeserializingMethods { get; }
        /// <summary>
        /// Gets the on serialized methods.
        /// </summary>
        public List<IMethodSymbol> OnSerializedMethods { get; }
        /// <summary>
        /// Gets the on serializing methods.
        /// </summary>
        public List<IMethodSymbol> OnSerializingMethods { get; }
        /// <summary>
        /// Gets the property symbols.
        /// </summary>
        public List<IPropertySymbol> PropertySymbols { get; }
        /// <summary>
        /// Gets the sanitized class name.
        /// </summary>
        public string SanitizedClassName { get; }

        public ClassObject(string sanitizedClassName,
            ITypeSymbol moduleSymbol,
            bool isDictionary = false,
            bool isList = false,
            bool isArray = false,
            bool isListOverride = false,
            bool isDictionaryOverride = false)
        {
            FieldSymbols = new List<IFieldSymbol>();
            PropertySymbols = new List<IPropertySymbol>();
            FullName = moduleSymbol.GetFullName() ?? string.Empty;
            GuidSuffix = Guid.NewGuid().ToString("N");
            IsDictionary = isDictionary;
            IsList = isList;
            IsArray = isArray;
            IsListOverride = isListOverride;
            ModuleSymbol = moduleSymbol;
            OnDeserializedMethods = new List<IMethodSymbol>();
            OnDeserializingMethods = new List<IMethodSymbol>();
            OnSerializedMethods = new List<IMethodSymbol>();
            OnSerializingMethods = new List<IMethodSymbol>();
            SanitizedClassName = sanitizedClassName;
            IsDictionaryOverride = isDictionaryOverride;
        }
    }
}
