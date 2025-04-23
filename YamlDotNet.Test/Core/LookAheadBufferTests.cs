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
using System.IO;
using FluentAssertions;
using Xunit;
using YamlDotNet.Core;

namespace YamlDotNet.Test.Core
{
    /// <summary>
    /// The look ahead buffer tests.
    /// </summary>
    public class LookAheadBufferTests
    {
        private const string TestString = "abcdefghi";
        private const int Capacity = 4;

        /// <summary>
        /// Shoulds the have read once when peeking at offset zero.
        /// </summary>
        [Fact]
        public void ShouldHaveReadOnceWhenPeekingAtOffsetZero()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(0).Should().Be('a');
        }

        /// <summary>
        /// Shoulds the have read twice when peeking at offset one.
        /// </summary>
        [Fact]
        public void ShouldHaveReadTwiceWhenPeekingAtOffsetOne()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(0);

            buffer.Peek(1).Should().Be('b');
        }

        /// <summary>
        /// Shoulds the have read thrice when peeking at offset two.
        /// </summary>
        [Fact]
        public void ShouldHaveReadThriceWhenPeekingAtOffsetTwo()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(0);
            buffer.Peek(1);

            buffer.Peek(2).Should().Be('c');
        }

        /// <summary>
        /// Shoulds the not have read after skipping one character.
        /// </summary>
        [Fact]
        public void ShouldNotHaveReadAfterSkippingOneCharacter()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);

            buffer.Skip(1);

            buffer.Peek(0).Should().Be('b');
            buffer.Peek(1).Should().Be('c');
        }

        /// <summary>
        /// Shoulds the have read once after skipping one character.
        /// </summary>
        [Fact]
        public void ShouldHaveReadOnceAfterSkippingOneCharacter()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);

            buffer.Skip(1);
            buffer.Peek(2).Should().Be('d');
        }

        /// <summary>
        /// Shoulds the have read twice after skipping one character.
        /// </summary>
        [Fact]
        public void ShouldHaveReadTwiceAfterSkippingOneCharacter()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);

            buffer.Skip(1);
            buffer.Peek(3).Should().Be('e');
        }

        /// <summary>
        /// Shoulds the have read once after skipping five characters.
        /// </summary>
        [Fact]
        public void ShouldHaveReadOnceAfterSkippingFiveCharacters()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);
            buffer.Skip(1);
            buffer.Peek(3);

            buffer.Skip(4);
            buffer.Peek(0).Should().Be('f');
        }

        /// <summary>
        /// Shoulds the have read once after skipping six characters.
        /// </summary>
        [Fact]
        public void ShouldHaveReadOnceAfterSkippingSixCharacters()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);
            buffer.Skip(1);
            buffer.Peek(3);
            buffer.Skip(4);
            buffer.Peek(0);

            buffer.Skip(1);
            buffer.Peek(0).Should().Be('g');
        }

        /// <summary>
        /// Shoulds the have read once after skipping seven characters.
        /// </summary>
        [Fact]
        public void ShouldHaveReadOnceAfterSkippingSevenCharacters()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);
            buffer.Skip(1);
            buffer.Peek(3);
            buffer.Skip(4);
            buffer.Peek(1);

            buffer.Skip(2);
            buffer.Peek(0).Should().Be('h');
        }

        /// <summary>
        /// Shoulds the have read once after skipping eight characters.
        /// </summary>
        [Fact]
        public void ShouldHaveReadOnceAfterSkippingEightCharacters()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);
            buffer.Skip(1);
            buffer.Peek(3);
            buffer.Skip(4);
            buffer.Peek(2);

            buffer.Skip(3);
            buffer.Peek(0).Should().Be('i');
        }

        /// <summary>
        /// Shoulds the have read once after skipping nine characters.
        /// </summary>
        [Fact]
        public void ShouldHaveReadOnceAfterSkippingNineCharacters()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);
            buffer.Skip(1);
            buffer.Peek(3);
            buffer.Skip(4);
            buffer.Peek(3);

            buffer.Skip(4);
            buffer.Peek(0).Should().Be('\0');
        }

        /// <summary>
        /// Shoulds the find end of input.
        /// </summary>
        [Fact]
        public void ShouldFindEndOfInput()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(2);
            buffer.Skip(1);
            buffer.Peek(3);
            buffer.Skip(4);
            buffer.Peek(3);
            buffer.Skip(4);
            buffer.Peek(0);

            buffer.EndOfInput.Should().BeTrue();
        }

        /// <summary>
        /// Shoulds the throw when skipping beyond current buffer.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenSkippingBeyondCurrentBuffer()
        {
            var reader = new StringReader(TestString);
            var buffer = CreateBuffer(reader, Capacity);

            buffer.Peek(3);
            Action action = () => buffer.Skip(5);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        private static LookAheadBuffer CreateBuffer(TextReader reader, int capacity)
        {
            return new LookAheadBuffer(reader, capacity);
        }
    }
}
