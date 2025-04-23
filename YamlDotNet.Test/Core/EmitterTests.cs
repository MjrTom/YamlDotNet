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
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace YamlDotNet.Test.Core
{
    /// <summary>
    /// The emitter tests.
    /// </summary>
    public class EmitterTests : EmitterTestsHelper
    {
        /// <summary>
        /// Compares the original and emitted text.
        /// </summary>
        /// <param name="filename">The filename.</param>
        [Theory]
        [InlineData("01-directives.yaml")]
        [InlineData("02-scalar-in-imp-doc.yaml")]
        [InlineData("03-scalar-in-exp-doc.yaml")]
        [InlineData("04-scalars-in-multi-docs.yaml")]
        [InlineData("05-circular-sequence.yaml")]
        [InlineData("06-float-tag.yaml")]
        [InlineData("07-scalar-styles.yaml")]
        [InlineData("08-flow-sequence.yaml")]
        [InlineData("09-flow-mapping.yaml")]
        [InlineData("10-mixed-nodes-in-sequence.yaml")]
        [InlineData("11-mixed-nodes-in-mapping.yaml")]
        [InlineData("12-compact-sequence.yaml")]
        [InlineData("13-compact-mapping.yaml")]
        [InlineData("14-mapping-wo-indent.yaml")]
        public void CompareOriginalAndEmittedText(string filename)
        {
            var stream = Yaml.ReaderFrom(filename);

            var originalEvents = ParsingEventsOf(stream.ReadToEnd());
            var emittedText = EmittedTextFrom(originalEvents);
            var emittedEvents = ParsingEventsOf(emittedText);

            emittedEvents.Should().BeEquivalentTo(originalEvents, opt => opt
                .Excluding(@event => @event.Start)
                .Excluding(@event => @event.End)
                .Excluding(@event => ((DocumentEnd)@event).IsImplicit)
            );
        }

        private IList<ParsingEvent> ParsingEventsOf(string text)
        {
            var parser = new Parser(new StringReader(text));
            return EnumerationOf(parser).ToList();
        }



        /// <summary>
        /// Plains the scalar can be followed by implicit document.
        /// </summary>
        [Fact]
        public void PlainScalarCanBeFollowedByImplicitDocument()
        {
            var events = StreamOf(
                DocumentWith(PlainScalar("test")),
                DocumentWith(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("test", "--- test"));
        }

        /// <summary>
        /// Plains the scalar can be followed by document with version.
        /// </summary>
        [Fact]
        public void PlainScalarCanBeFollowedByDocumentWithVersion()
        {
            var events = StreamOf(
                DocumentWith(PlainScalar("test")),
                DocumentWithVersion(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("test", "...", "%YAML 1.1", "--- test"));
        }

        /// <summary>
        /// Plains the scalar can be followed by document with default tags.
        /// </summary>
        [Fact]
        public void PlainScalarCanBeFollowedByDocumentWithDefaultTags()
        {
            var events = StreamOf(
                DocumentWith(PlainScalar("test")),
                DocumentWithDefaultTags(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("test", "--- test"));
        }

        /// <summary>
        /// Plains the scalar can be followed by document with custom tags.
        /// </summary>
        [Fact]
        public void PlainScalarCanBeFollowedByDocumentWithCustomTags()
        {
            var events = StreamOf(
                DocumentWith(PlainScalar("test")),
                DocumentWithCustomTags(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("test", "...", FooTag, ExTag, ExExTag, "--- test"));
        }

        /// <summary>
        /// Blocks the can be followed by implicit document.
        /// </summary>
        [Fact]
        public void BlockCanBeFollowedByImplicitDocument()
        {
            var events = StreamOf(
                DocumentWith(SequenceWith(SingleQuotedScalar("test"))),
                DocumentWith(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("- 'test'", "--- test"));
        }

        /// <summary>
        /// Blocks the can be followed by document with version.
        /// </summary>
        [Fact]
        public void BlockCanBeFollowedByDocumentWithVersion()
        {
            var events = StreamOf(
                DocumentWith(SequenceWith(SingleQuotedScalar("test"))),
                DocumentWithVersion(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("- 'test'", "...", "%YAML 1.1", "--- test"));
        }

        /// <summary>
        /// Blocks the can be followed by document with default tags.
        /// </summary>
        [Fact]
        public void BlockCanBeFollowedByDocumentWithDefaultTags()
        {
            var events = StreamOf(
                DocumentWith(SequenceWith(SingleQuotedScalar("test"))),
                DocumentWithDefaultTags(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("- 'test'", "--- test"));
        }

        /// <summary>
        /// Blocks the can be followed by document with custom tags.
        /// </summary>
        [Fact]
        public void BlockCanBeFollowedByDocumentWithCustomTags()
        {
            var events = StreamOf(
                DocumentWith(SequenceWith(SingleQuotedScalar("test"))),
                DocumentWithCustomTags(PlainScalar("test")));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Contain(Lines("- 'test'", "...", FooTag, ExTag, ExExTag, "--- test"));
        }

        /// <summary>
        /// Blocks the style generates indentation indicator.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData("test", ">-\r\n  test\r\n")]    // No indentation indicator when no indent.
        [InlineData("  test", ">2-\r\n    test\r\n")]
        public void BlockStyleGeneratesIndentationIndicator(string input, string expected)
        {
            var events = StreamOf(
                DocumentWith(FoldedScalar(input)));

            var yaml = EmittedTextFrom(events);

            yaml.Should().Be(expected.NormalizeNewLines());
        }

        /// <summary>
        /// Foldeds the style does not loose characters.
        /// </summary>
        /// <param name="text">The text.</param>
        [Theory]
        [InlineData("LF hello\nworld")]
        [InlineData("CRLF hello\r\nworld")]
        public void FoldedStyleDoesNotLooseCharacters(string text)
        {
            var events = SequenceWith(FoldedScalar(text));

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            yaml.Should().Contain("world");
        }

        /// <summary>
        /// Foldeds the style is selected when new lines are found in literal.
        /// </summary>
        [Fact]
        public void FoldedStyleIsSelectedWhenNewLinesAreFoundInLiteral()
        {
            var events = SequenceWith(Scalar("hello\nworld").ImplicitPlain);

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            yaml.Should().Contain(">");
        }


        /// <summary>
        /// Allows the block style in multiline scalars with trailing spaces.
        /// </summary>
        [Fact]
        [Trait("motive", "pr #540")]
        public void AllowBlockStyleInMultilineScalarsWithTrailingSpaces()
        {
            var events = SequenceWith(Scalar("hello  \nworld").ImplicitPlain);

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            yaml.Should().Contain("\n");
        }


        /// <summary>
        /// Foldeds the style does not generate extra line breaks.
        /// </summary>
        [Fact]
        public void FoldedStyleDoesNotGenerateExtraLineBreaks()
        {
            var events = SequenceWith(FoldedScalar("hello\nworld"));

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            // Todo: Why involve the rep. model when testing the Emitter? Can we match using a regex?
            var stream = new YamlStream();
            stream.Load(new StringReader(yaml));
            var sequence = (YamlSequenceNode)stream.Documents[0].RootNode;
            var scalar = (YamlScalarNode)sequence.Children[0];

            scalar.Value.Should().Be("hello\nworld");
        }

        /// <summary>
        /// Foldeds the style does not collapse line breaks.
        /// </summary>
        [Fact]
        public void FoldedStyleDoesNotCollapseLineBreaks()
        {
            var events = SequenceWith(FoldedScalar(">+\n"));

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            var stream = new YamlStream();
            stream.Load(new StringReader(yaml));
            var sequence = (YamlSequenceNode)stream.Documents[0].RootNode;
            var scalar = (YamlScalarNode)sequence.Children[0];

            scalar.Value.Should().Be(">+\n");
        }

        /// <summary>
        /// Foldeds the style preserves new lines.
        /// </summary>
        [Fact]
        [Trait("motive", "issue #39")]
        public void FoldedStylePreservesNewLines()
        {
            var input = "id: 0\nPayload:\n  X: 5\n  Y: 6\n";
            var events = MappingWith(
                Scalar("Payload").ImplicitPlain,
                FoldedScalar(input));

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            var stream = new YamlStream();
            stream.Load(new StringReader(yaml));

            var mapping = (YamlMappingNode)stream.Documents[0].RootNode;
            var value = (YamlScalarNode)mapping.Children.First().Value;

            value.Value.Should().Be(input);
        }

        /// <summary>
        /// Comments the are emitted correctly.
        /// </summary>
        [Fact]
        public void CommentsAreEmittedCorrectly()
        {
            var events = SequenceWith(
                StandaloneComment("Top comment"),
                StandaloneComment("Second line"),
                Scalar("first").ImplicitPlain,
                InlineComment("The first value"),
                Scalar("second").ImplicitPlain,
                InlineComment("The second value"),
                StandaloneComment("Bottom comment")
            );

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            yaml.Should()
                .Contain("# Top comment")
                .And.Contain("# Second line")
                .And.NotContain("# Top comment # Second line")
                .And.Contain("first # The first value")
                .And.Contain("second # The second value")
                .And.Contain("# Bottom comment");
        }

        /// <summary>
        /// Comments the between mapping key and value are emitted correctly.
        /// </summary>
        [Fact]
        public void CommentsBetweenMappingKeyAndValueAreEmittedCorrectly()
        {
            var events = MappingWith(
                Scalar("key").ImplicitPlain,
                InlineComment("inline comment"),
                StandaloneComment("standalone comment"),
                BlockSequenceStart,
                Scalar("value").ImplicitPlain,
                SequenceEnd
            );

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            yaml.Should()
                .Contain(Lines(
                    "key: # inline comment",
                    "# standalone comment",
                    "  - value"
                ));
        }

        /// <summary>
        /// AS the comment as the first event adds a new line.
        /// </summary>
        [Fact]
        public void ACommentAsTheFirstEventAddsANewLine()
        {
            var events = new ParsingEvent[]
            {
                StandaloneComment("Top comment"),
                Scalar("first").ImplicitPlain,
            };

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));

            yaml.Should()
                .Contain("# Top comment")
                .And.Contain("first")
                .And.NotContain("# Top commentfirst");
        }

        /// <summary>
        /// Unicodes the in scalars can be single quoted when output encoding supports it.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="codePage">The code page.</param>
        [Theory]
        [InlineData("Гранит", 28595)] // Cyrillic (ISO)
        [InlineData("ГÀƊȽ˱ώҔׂۋᵷẁό₩וּﺪﺸﻸﭧ╬♫₹Ὰỗ᷁ݭ٭ӢР͞ʓǈĄë0", 65001)] // UTF-8
        public void UnicodeInScalarsCanBeSingleQuotedWhenOutputEncodingSupportsIt(string text, int codePage)
        {
            var document = StreamedDocumentWith(
                SequenceWith(
                    SingleQuotedScalar(text)
                )
            );
            var buffer = new MemoryStream();
#if (NETCOREAPP2_1_OR_GREATER)
            // Code pages such as Cyrillic are not recognized by default in
            // .NET Core.  We need to register this provider.
            // https://msdn.microsoft.com/en-us/library/mt643899(v=vs.110).aspx#Remarks
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            var encoding = Encoding.GetEncoding(codePage);
            using (var writer = new StreamWriter(buffer, encoding))
            {
                var emitter = new Emitter(writer, 2, int.MaxValue, false);
                foreach (var evt in document)
                {
                    emitter.Emit(evt);
                }
            }

            var yaml = encoding.GetString(buffer.ToArray());

            yaml.Should()
                .Contain("'" + text + "'");
        }

        /// <summary>
        /// Empties the strings are quoted.
        /// </summary>
        [Fact]
        public void EmptyStringsAreQuoted()
        {
            var events = SequenceWith(
                Scalar(string.Empty).ImplicitPlain
            );

            var yaml = EmittedTextFrom(StreamedDocumentWith(events));
            yaml.Should()
                .Contain("- ''");
        }

        /// <summary>
        /// News the lines are not duplicated when emitted.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData("b-carriage-return,b-line-feed\r\nlll", "b-carriage-return,b-line-feed\nlll")]
        [InlineData("b-carriage-return,b-line-feed\r\n\r\nlll", "b-carriage-return,b-line-feed\n\nlll")]
        [InlineData("b-carriage-return\rlll", "b-carriage-return\nlll")]
        [InlineData("b-line-feed\nlll", "b-line-feed\nlll")]
        [InlineData("b-next-line\x85lll", "b-next-line\nlll")]
        [InlineData("b-line-separator\x2028lll", "b-line-separator\x2028lll")]
        [InlineData("b-paragraph-separator\x2029lll", "b-paragraph-separator\x2029lll")]
        public void NewLinesAreNotDuplicatedWhenEmitted(string input, string expected)
        {
            var yaml = EmittedTextFrom(StreamOf(DocumentWith(
                LiteralScalar(input)
            )));

            AssertSequenceOfEventsFrom(Yaml.ParserForText(yaml),
                StreamStart,
                DocumentStart(Implicit),
                LiteralScalar(expected),
                DocumentEnd(Implicit),
                StreamEnd);
        }

        /// <summary>
        /// News the lines are not duplicated when emitted in folded scalar.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData("b-carriage-return,b-line-feed\r\nlll", "b-carriage-return,b-line-feed\nlll")]
        [InlineData("b-carriage-return,b-line-feed\r\n\r\nlll", "b-carriage-return,b-line-feed\n\nlll")]
        [InlineData("b-carriage-return\rlll", "b-carriage-return\nlll")]
        [InlineData("b-line-feed\nlll", "b-line-feed\nlll")]
        [InlineData("b-next-line\x85lll", "b-next-line\nlll")]
        [InlineData("b-line-separator\x2028lll", "b-line-separator\x2028lll")]
        [InlineData("b-paragraph-separator\x2029lll", "b-paragraph-separator\x2029lll")]
        public void NewLinesAreNotDuplicatedWhenEmittedInFoldedScalar(string input, string expected)
        {
            var yaml = EmittedTextFrom(StreamOf(DocumentWith(
                                                             FoldedScalar(input)
                                                            )));

            AssertSequenceOfEventsFrom(Yaml.ParserForText(yaml),
                                       StreamStart,
                                       DocumentStart(Implicit),
                                       FoldedScalar(expected),
                                       DocumentEnd(Implicit),
                                       StreamEnd);
        }

        /// <summary>
        /// Singles the quotes are double quoted.
        /// </summary>
        /// <param name="input">The input.</param>
        [Theory]
        [InlineData("'.'test")]
        [InlineData("'")]
        [InlineData("'.'")]
        [InlineData("'test")]
        [InlineData("'test'")]
        public void SingleQuotesAreDoubleQuoted(string input)
        {
            var events = StreamOf(DocumentWith(new Scalar(input)));
            var yaml = EmittedTextFrom(events);

            var expected = string.Format("\"{0}\"", input);

            yaml.Should().Contain(expected);
        }

        /// <summary>
        /// Singles the quotes are not double quoted unless necessary.
        /// </summary>
        /// <param name="input">The input.</param>
        [Theory]
        [InlineData("hello\n'world")]
        public void SingleQuotesAreNotDoubleQuotedUnlessNecessary(string input)
        {
            var events = StreamOf(DocumentWith(new Scalar(input)));
            var yaml = EmittedTextFrom(events);
            yaml.Should().NotContain("\"");
        }

        /// <summary>
        /// Leadings the backslash is not quoted.
        /// </summary>
        /// <param name="input">The input.</param>
        [Theory]
        [InlineData(@"\hello world")]
        public void LeadingBackslashIsNotQuoted(string input)
        {
            var events = StreamOf(DocumentWith(new Scalar(input)));
            var yaml = EmittedTextFrom(events);
            yaml.Should().NotContain("\'");
            yaml.Should().NotContain("\"");
        }

        private string Lines(params string[] lines)
        {
            return string.Join(Environment.NewLine, lines);
        }
    }
}
