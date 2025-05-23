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

using System.Diagnostics;

namespace YamlDotNet.Core
{
    /// <summary>
    /// The cursor.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class Cursor
    {
        /// <summary>
        /// Gets the index.
        /// </summary>
        public long Index { get; private set; }
        /// <summary>
        /// Gets the line.
        /// </summary>
        public long Line { get; private set; }
        /// <summary>
        /// Gets the line offset.
        /// </summary>
        public long LineOffset { get; private set; }

        public Cursor()
        {
            Line = 1;
        }

        public Cursor(Cursor cursor)
        {
            Index = cursor.Index;
            Line = cursor.Line;
            LineOffset = cursor.LineOffset;
        }

        /// <summary>
        /// Marks the.
        /// </summary>
        /// <returns>A Mark.</returns>
        public Mark Mark()
        {
            return new Mark(Index, Line, LineOffset + 1);
        }

        /// <summary>
        /// Skips the.
        /// </summary>
        public void Skip()
        {
            Index++;
            LineOffset++;
        }

        /// <summary>
        /// Skips the line by offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SkipLineByOffset(int offset)
        {
            Index += offset;
            Line++;
            LineOffset = 0;
        }

        /// <summary>
        /// Forces the skip line after non break.
        /// </summary>
        public void ForceSkipLineAfterNonBreak()
        {
            if (LineOffset != 0)
            {
                Line++;
                LineOffset = 0;
            }
        }
    }
}
