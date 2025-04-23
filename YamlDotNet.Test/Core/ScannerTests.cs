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
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Test.Core
{
    /// <summary>
    /// The scanner tests.
    /// </summary>
    public class ScannerTests : TokenHelper
    {
        /// <summary>
        /// Verifies the tokens on example1.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample1()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("01-directives.yaml"),
                StreamStart,
                VersionDirective(1, 1),
                TagDirective("!", "!foo"),
                TagDirective("!yaml!", "tag:yaml.org,2002:"),
                DocumentStart,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example2.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample2()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("02-scalar-in-imp-doc.yaml"),
                StreamStart,
                SingleQuotedScalar("a scalar"),
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example3.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample3()
        {
            var scanner = Yaml.ScannerForResource("03-scalar-in-exp-doc.yaml");
            AssertSequenceOfTokensFrom(scanner,
                StreamStart,
                DocumentStart,
                SingleQuotedScalar("a scalar"),
                DocumentEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example4.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample4()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("04-scalars-in-multi-docs.yaml"),
                StreamStart,
                SingleQuotedScalar("a scalar"),
                DocumentStart,
                SingleQuotedScalar("another scalar"),
                DocumentStart,
                SingleQuotedScalar("yet another scalar"),
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example5.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample5()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("05-circular-sequence.yaml"),
                StreamStart,
                Anchor("A"),
                FlowSequenceStart,
                AnchorAlias("A"),
                FlowSequenceEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example6.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample6()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("06-float-tag.yaml"),
                StreamStart,
                Tag("!!", "float"),
                DoubleQuotedScalar("3.14"),
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example7.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample7()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("07-scalar-styles.yaml"),
                StreamStart,
                DocumentStart,
                DocumentStart,
                PlainScalar("a plain scalar"),
                DocumentStart,
                SingleQuotedScalar("a single-quoted scalar"),
                DocumentStart,
                DoubleQuotedScalar("a double-quoted scalar"),
                DocumentStart,
                LiteralScalar("a literal scalar"),
                DocumentStart,
                FoldedScalar("a folded scalar"),
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example8.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample8()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("08-flow-sequence.yaml"),
                StreamStart,
                FlowSequenceStart,
                PlainScalar("item 1"),
                FlowEntry,
                PlainScalar("item 2"),
                FlowEntry,
                PlainScalar("item 3"),
                FlowSequenceEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example9.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample9()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("09-flow-mapping.yaml"),
                StreamStart,
                FlowMappingStart,
                Key,
                PlainScalar("a simple key"),
                Value,
                PlainScalar("a value"),
                FlowEntry,
                Key,
                PlainScalar("a complex key"),
                Value,
                PlainScalar("another value"),
                FlowEntry,
                FlowMappingEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example10.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample10()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("10-mixed-nodes-in-sequence.yaml"),
                StreamStart,
                BlockSequenceStart,
                BlockEntry,
                PlainScalar("item 1"),
                BlockEntry,
                PlainScalar("item 2"),
                BlockEntry,
                BlockSequenceStart,
                BlockEntry,
                PlainScalar("item 3.1"),
                BlockEntry,
                PlainScalar("item 3.2"),
                BlockEnd,
                BlockEntry,
                BlockMappingStart,
                Key,
                PlainScalar("key 1"),
                Value,
                PlainScalar("value 1"),
                Key,
                PlainScalar("key 2"),
                Value,
                PlainScalar("value 2"),
                BlockEnd,
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example11.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample11()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("11-mixed-nodes-in-mapping.yaml"),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("a simple key"),
                Value,
                PlainScalar("a value"),
                Key,
                PlainScalar("a complex key"),
                Value,
                PlainScalar("another value"),
                Key,
                PlainScalar("a mapping"),
                Value,
                BlockMappingStart,
                Key,
                PlainScalar("key 1"),
                Value,
                PlainScalar("value 1"),
                Key,
                PlainScalar("key 2"),
                Value,
                PlainScalar("value 2"),
                BlockEnd,
                Key,
                PlainScalar("a sequence"),
                Value,
                BlockSequenceStart,
                BlockEntry,
                PlainScalar("item 1"),
                BlockEntry,
                PlainScalar("item 2"),
                BlockEnd,
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example12.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample12()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("12-compact-sequence.yaml"),
                StreamStart,
                BlockSequenceStart,
                BlockEntry,
                BlockSequenceStart,
                BlockEntry,
                PlainScalar("item 1"),
                BlockEntry,
                PlainScalar("item 2"),
                BlockEnd,
                BlockEntry,
                BlockMappingStart,
                Key,
                PlainScalar("key 1"),
                Value,
                PlainScalar("value 1"),
                Key,
                PlainScalar("key 2"),
                Value,
                PlainScalar("value 2"),
                BlockEnd,
                BlockEntry,
                BlockMappingStart,
                Key,
                PlainScalar("complex key"),
                Value,
                PlainScalar("complex value"),
                BlockEnd,
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example13.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample13()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("13-compact-mapping.yaml"),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("a sequence"),
                Value,
                BlockSequenceStart,
                BlockEntry,
                PlainScalar("item 1"),
                BlockEntry,
                PlainScalar("item 2"),
                BlockEnd,
                Key,
                PlainScalar("a mapping"),
                Value,
                BlockMappingStart,
                Key,
                PlainScalar("key 1"),
                Value,
                PlainScalar("value 1"),
                Key,
                PlainScalar("key 2"),
                Value,
                PlainScalar("value 2"),
                BlockEnd,
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Verifies the tokens on example14.
        /// </summary>
        [Fact]
        public void VerifyTokensOnExample14()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForResource("14-mapping-wo-indent.yaml"),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("key"),
                Value,
                BlockEntry,
                PlainScalar("item 1"),
                BlockEntry,
                PlainScalar("item 2"),
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Supports the realy long strings.
        /// </summary>
        [Fact]
        public void SupportRealyLongStrings()
        {
            var longKey = string.Concat(Enumerable.Repeat("x", 1500));
            var yamlString = $"{longKey}: value";
            var scanner = new Scanner(new StringReader(yamlString), true, 1500);
            AssertSequenceOfTokensFrom(scanner,
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar(longKey),
                Value,
                PlainScalar("value"),
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Comments the are returned when requested.
        /// </summary>
        [Fact]
        public void CommentsAreReturnedWhenRequested()
        {
            AssertSequenceOfTokensFrom(new Scanner(Yaml.ReaderForText(@"
                    # Top comment
                    - first # Comment on first item
                    - second
                    # Bottom comment
                "), skipComments: false),
                StreamStart,
                StandaloneComment("Top comment"),
                BlockSequenceStart,
                BlockEntry,
                PlainScalar("first"),
                InlineComment("Comment on first item"),
                BlockEntry,
                PlainScalar("second"),
                StandaloneComment("Bottom comment"),
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Comments the are correctly marked.
        /// </summary>
        [Fact]
        public void CommentsAreCorrectlyMarked()
        {
            var sut = new Scanner(Yaml.ReaderForText(@"
                - first # Comment on first item
            "), skipComments: false);

            while (sut.MoveNext())
            {
                if (sut.Current is Comment comment)
                {
                    Assert.Equal(8, comment.Start.Index);
                    Assert.Equal(31, comment.End.Index);

                    return;
                }
            }

            Assert.Fail("Did not find a comment");
        }

        /// <summary>
        /// Comments the are omitted unless requested.
        /// </summary>
        [Fact]
        public void CommentsAreOmittedUnlessRequested()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText(@"
                    # Top comment
                    - first # Comment on first item
                    - second
                    # Bottom comment
                "),
                StreamStart,
                BlockSequenceStart,
                BlockEntry,
                PlainScalar("first"),
                BlockEntry,
                PlainScalar("second"),
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Marks the on double quoted scalars are correct.
        /// </summary>
        [Fact]
        public void MarksOnDoubleQuotedScalarsAreCorrect()
        {
            var scanner = Yaml.ScannerForText(@"
                ""x""
            ");

            Scalar scalar = null;
            while (scanner.MoveNext() && scalar == null)
            {
                scalar = scanner.Current as Scalar;
            }
            Assert.Equal(4, scalar.End.Column);
        }

        /// <summary>
        /// Slow_stream_is_parsed_correctlies the.
        /// </summary>
        [Fact]
        public void Slow_stream_is_parsed_correctly()
        {
            var buffer = new MemoryStream();
            Yaml.StreamFrom("04-scalars-in-multi-docs.yaml").CopyTo(buffer);

            var slowStream = new SlowStream(buffer.ToArray());

            var scanner = new Scanner(new StreamReader(slowStream));

            scanner.MoveNext();

            // Should not fail
            scanner.MoveNext();
        }

        /// <summary>
        /// Issue_553_562S the.
        /// </summary>
        [Fact]
        public void Issue_553_562()
        {
            var yaml = "MainItem4:\n" + string.Join("\n", Enumerable.Range(1, 100).Select(e => $"- {{item: {{foo1: {e}, foo2: 'bar{e}' }}}}"));

            var scanner = new Scanner(new StringReader(yaml));
            while (scanner.MoveNext())
            {
            }
        }

        /// <summary>
        /// Plain_S the scalar_outside_of_flow_allows_braces.
        /// </summary>
        [Fact]
        public void Plain_Scalar_outside_of_flow_allows_braces()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText(@"value: -[123]"),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("value"),
                Value,
                PlainScalar("-[123]"),
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Plain_S the scalar_inside_flow_does_not_allow_braces.
        /// </summary>
        [Fact]
        public void Plain_Scalar_inside_flow_does_not_allow_braces()
        {
            AssertPartialSequenceOfTokensFrom(Yaml.ScannerForText(@"
[
  value: -[123]
]"),
                StreamStart,
                FlowSequenceStart,
                Key,
                PlainScalar("value"),
                Value,
                Error("Invalid key indicator format."));
        }

        /// <summary>
        /// Keys_can_start_with_colons_in_nested_blocks the.
        /// </summary>
        [Fact]
        public void Keys_can_start_with_colons_in_nested_block()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText("root:\n  :first: 1\n  :second: 2"),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("root"),
                Value,
                BlockMappingStart,
                Key,
                PlainScalar(":first"),
                Value,
                PlainScalar("1"),
                Key,
                PlainScalar(":second"),
                Value,
                PlainScalar("2"),
                BlockEnd,
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Keys_can_start_with_colons_after_quoted_values the.
        /// </summary>
        [Fact]
        public void Keys_can_start_with_colons_after_quoted_values()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText(":first: '1'\n:second: 2"),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar(":first"),
                Value,
                SingleQuotedScalar("1"),
                Key,
                PlainScalar(":second"),
                Value,
                PlainScalar("2"),
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Keys_can_start_with_colons_after_single_quoted_values_in_nested_blocks the.
        /// </summary>
        [Fact]
        public void Keys_can_start_with_colons_after_single_quoted_values_in_nested_block()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText("xyz:\n  :hello: 'world'\n  :goodbye: world"),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("xyz"),
                Value,
                BlockMappingStart,
                Key,
                PlainScalar(":hello"),
                Value,
                SingleQuotedScalar("world"),
                Key,
                PlainScalar(":goodbye"),
                Value,
                PlainScalar("world"),
                BlockEnd,
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Keys_can_start_with_colons_after_double_quoted_values_in_nested_blocks the.
        /// </summary>
        [Fact]
        public void Keys_can_start_with_colons_after_double_quoted_values_in_nested_block()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText("xyz:\n  :hello: \"world\"\n  :goodbye: world"),
                 StreamStart,
                 BlockMappingStart,
                 Key,
                 PlainScalar("xyz"),
                 Value,
                 BlockMappingStart,
                 Key,
                 PlainScalar(":hello"),
                 Value,
                 DoubleQuotedScalar("world"),
                 Key,
                 PlainScalar(":goodbye"),
                 Value,
                 PlainScalar("world"),
                 BlockEnd,
                 BlockEnd,
                 StreamEnd);
        }

        /// <summary>
        /// Utf16S the strings as utf8 surrogates work correctly.
        /// </summary>
        [Fact]
        public void Utf16StringsAsUtf8SurrogatesWorkCorrectly()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText("Test: \"\\uD83D\\uDC4D\""),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("Test"),
                Value,
                DoubleQuotedScalar("\uD83D\uDC4D"), // guaranteed thumbs up emoticon that will work in Windows Terminal since it pukes on displaying it.
                BlockEnd,
                StreamEnd);
        }

        /// <summary>
        /// Utf16S the characters are read correctly.
        /// </summary>
        [Fact]
        public void Utf16CharactersAreReadCorrectly()
        {
            AssertSequenceOfTokensFrom(Yaml.ScannerForText("Test: \"\uD83D\uDC4D\""),
                StreamStart,
                BlockMappingStart,
                Key,
                PlainScalar("Test"),
                Value,
                DoubleQuotedScalar("\uD83D\uDC4D"), // guaranteed thumbs up emoticon that will work in Windows Terminal since it pukes on displaying it.
                BlockEnd,
                StreamEnd);
        }

        private void AssertPartialSequenceOfTokensFrom(Scanner scanner, params Token[] tokens)
        {
            var tokenNumber = 1;
            foreach (var expected in tokens)
            {
                scanner.MoveNext().Should().BeTrue("Missing token number {0}", tokenNumber);
                AssertToken(expected, scanner.Current, tokenNumber);
                tokenNumber++;
            }
        }

        private void AssertSequenceOfTokensFrom(Scanner scanner, params Token[] tokens)
        {
            AssertPartialSequenceOfTokensFrom(scanner, tokens);
            scanner.MoveNext().Should().BeFalse("Found extra tokens");
        }

        private void AssertToken(Token expected, Token actual, int tokenNumber)
        {
            actual.Should().NotBeNull();
            actual.GetType().Should().Be(expected.GetType(), "Token {0} is not of the expected type", tokenNumber);

            foreach (var property in expected.GetType().GetTypeInfo().GetProperties())
            {
                if (property.PropertyType != typeof(Mark) && property.CanRead && property.Name != "IsKey")
                {
                    var value = property.GetValue(actual, null);
                    var expectedValue = property.GetValue(expected, null);
                    value.Should().Be(expectedValue, "Comparing property {0} in token {1}", property.Name, tokenNumber);
                }
            }
        }

        /// <summary>
        /// A stream that reads one byte at the time.
        /// </summary>
        public class SlowStream : Stream
        {
            private readonly byte[] data;
            private int position;

            public SlowStream(byte[] data)
            {
                this.data = data;
            }

            /// <summary>
            /// Gets a value indicating whether can read.
            /// </summary>
            public override bool CanRead => true;

            /// <summary>
            /// Gets a value indicating whether can seek.
            /// </summary>
            public override bool CanSeek => false;

            /// <summary>
            /// Gets a value indicating whether can write.
            /// </summary>
            public override bool CanWrite => false;

            /// <summary>
            /// Gets the length.
            /// </summary>
            public override long Length => data.Length;

            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            public override long Position
            {
                get => position;
                set => throw new NotSupportedException();
            }

            /// <summary>
            /// Flushes the.
            /// </summary>
            public override void Flush()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Reads the.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="count">The count.</param>
            /// <returns>An int.</returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                if (count == 0 || position == data.Length)
                {
                    return 0;
                }

                buffer[offset] = data[position];
                ++position;
                return 1;
            }

            /// <summary>
            /// Seeks the.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="origin">The origin.</param>
            /// <returns>A long.</returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Sets the length.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Writes the.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="count">The count.</param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
