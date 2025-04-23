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

using System.Reflection;
using YamlDotNet.Serialization;

namespace YamlDotNet.Helpers
{
    /// <summary>
    /// The default fsharp helper.
    /// </summary>
    public class DefaultFsharpHelper : IFsharpHelper
    {
        private static bool IsFsharpCore(Type t)
        {
            return t.Namespace == "Microsoft.FSharp.Core";
        }

        /// <summary>
        /// Are the option type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>A bool.</returns>
        public bool IsOptionType(Type t)
        {
            return IsFsharpCore(t) && t.Name == "FSharpOption`1";
        }

        /// <summary>
        /// Gets the option underlying type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>A Type? .</returns>
        public Type? GetOptionUnderlyingType(Type t)
        {
            return t.IsGenericType && IsOptionType(t) ? t.GenericTypeArguments[0] : null;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="objectDescriptor">The object descriptor.</param>
        /// <returns>An object? .</returns>
        public object? GetValue(IObjectDescriptor objectDescriptor)
        {
            if (!IsOptionType(objectDescriptor.Type))
            {
                throw new InvalidOperationException("Should not be called on non-Option<> type");
            }

            if (objectDescriptor.Value is null)
            {
                return null;
            }

            return objectDescriptor.Type.GetProperty("Value").GetValue(objectDescriptor.Value);
        }

        /// <summary>
        /// Are the fsharp list type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>A bool.</returns>
        public bool IsFsharpListType(Type t)
        {
            return t.Namespace == "Microsoft.FSharp.Collections" && t.Name == "FSharpList`1";
        }

        /// <summary>
        /// Creates the fsharp list from array.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="itemsType">The items type.</param>
        /// <param name="arr">The arr.</param>
        /// <returns>An object? .</returns>
        public object? CreateFsharpListFromArray(Type t, Type itemsType, Array arr)
        {
            if (!IsFsharpListType(t))
            {
                return null;
            }

            var listModuleType = Type.GetType("Microsoft.FSharp.Collections.ListModule, FSharp.Core");
            if (listModuleType == null)
            {
                throw new InvalidOperationException("Cannot find FSharp.Core ListModule");
            }

            var ofArrayMethod = listModuleType.GetMethod("OfArray", BindingFlags.Public | BindingFlags.Static);
            if (ofArrayMethod == null)
            {
                throw new InvalidOperationException("Cannot find OfArray method in ListModule");
            }

            var genericOfArrayMethod = ofArrayMethod.MakeGenericMethod(itemsType);

            return genericOfArrayMethod.Invoke(null, new object[] { arr });
        }
    }
}
