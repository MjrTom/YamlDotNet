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

using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.Utilities
{
    internal sealed class ObjectAnchorCollection
    {
        private readonly Dictionary<string, object> objectsByAnchor = [];
        private readonly Dictionary<object, string> anchorsByObject = [];

        /// <summary>
        /// Adds the specified anchor.
        /// </summary>
        /// <param name="anchor">The anchor.</param>
        /// <param name="object">The @object.</param>
        public void Add(string anchor, object @object)
        {
            objectsByAnchor.Add(anchor, @object);
            if (@object != null)
            {
                anchorsByObject.Add(@object, anchor);
            }
        }

        /// <summary>
        /// Gets the anchor for the specified object.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="anchor">The anchor.</param>
        /// <returns></returns>
        public bool TryGetAnchor(object @object, [MaybeNullWhen(false)] out string? anchor)
        {
            return anchorsByObject.TryGetValue(@object, out anchor);
        }

        /// <summary>
        /// Gets the <see cref="object"/> with the specified anchor.
        /// </summary>
        /// <value></value>
        public object this[string anchor]
        {
            get
            {
                if (objectsByAnchor.TryGetValue(anchor, out var value))
                {
                    return value;
                }

                throw new AnchorNotFoundException($"The anchor '{anchor}' does not exists");
            }
        }
    }
}
