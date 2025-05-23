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

using System.Text.RegularExpressions;

namespace YamlDotNet.Core
{
    public readonly struct AnchorName : IEquatable<AnchorName>
    {
        public static readonly AnchorName Empty;

        // https://yaml.org/spec/1.2/spec.html#id2785586
        private static readonly Regex AnchorPattern = new Regex(@"^[^\[\]\{\},]+$", StandardRegexOptions.Compiled);

        private readonly string? value;

        public string Value => value ?? throw new InvalidOperationException("Cannot read the Value of an empty anchor");

        public bool IsEmpty => value is null;

        public AnchorName(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));

            if (!AnchorPattern.IsMatch(value))
            {
                throw new ArgumentException($"Anchor cannot be empty or contain disallowed characters: []{{}},\nThe value was '{value}'.", nameof(value));
            }
        }

        /// <inheritdoc />
        public override string ToString() => value ?? "[empty]";

        /// <inheritdoc />
        public bool Equals(AnchorName other) => Equals(value, other.value);

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is AnchorName other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(AnchorName left, AnchorName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AnchorName left, AnchorName right)
        {
            return !(left == right);
        }

        public static implicit operator AnchorName(string? value) => value == null ? Empty : new AnchorName(value);
    }
}
