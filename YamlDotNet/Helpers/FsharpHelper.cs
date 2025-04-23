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

using YamlDotNet.Serialization;

namespace YamlDotNet.Helpers
{
    /// <summary>
    /// The fsharp helper.
    /// </summary>
    public static class FsharpHelper
    {
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public static IFsharpHelper? Instance { get; set; }

        /// <summary>
        /// Are the option type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>A bool.</returns>
        public static bool IsOptionType(Type t) => Instance?.IsOptionType(t) ?? false;

        /// <summary>
        /// Gets the option underlying type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>A Type? .</returns>
        public static Type? GetOptionUnderlyingType(Type t) => Instance?.GetOptionUnderlyingType(t);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="objectDescriptor">The object descriptor.</param>
        /// <returns>An object? .</returns>
        public static object? GetValue(IObjectDescriptor objectDescriptor) => Instance?.GetValue(objectDescriptor);

        /// <summary>
        /// Are the fsharp list type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>A bool.</returns>
        public static bool IsFsharpListType(Type t) => Instance?.IsFsharpListType(t) ?? false;

        /// <summary>
        /// Creates the fsharp list from array.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="itemsType">The items type.</param>
        /// <param name="arr">The arr.</param>
        /// <returns>An object? .</returns>
        public static object? CreateFsharpListFromArray(Type t, Type itemsType, Array arr) => Instance?.CreateFsharpListFromArray(t, itemsType, arr);
    }
}
