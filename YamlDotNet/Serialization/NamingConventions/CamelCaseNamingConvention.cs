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

using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NamingConventions
{
    /// <summary>
    /// Convert the string with underscores (this_is_a_test) or hyphens (this-is-a-test) to 
    /// camel case (thisIsATest). Camel case is the same as Pascal case, except the first letter
    /// is lowercase.
    /// </summary>
    public sealed class CamelCaseNamingConvention : INamingConvention
    {
        [Obsolete("Use the Instance static field instead of creating new instances")]
        public CamelCaseNamingConvention() { }

        /// <summary>
        /// Applies the.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        public string Apply(string value)
        {
            return value.ToCamelCase();
        }

        /// <summary>
        /// Reverses the.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        public string Reverse(string value)
        {
            var result = value.ToPascalCase();
            return result;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public static readonly INamingConvention Instance = new CamelCaseNamingConvention();
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
