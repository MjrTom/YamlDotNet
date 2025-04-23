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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using YamlDotNet.Core;

namespace YamlDotNet.Test
{
    /// <summary>
    /// The yaml.
    /// </summary>
    public static class Yaml
    {
        /// <summary>
        /// Readers the from.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A TextReader.</returns>
        public static TextReader ReaderFrom(string name)
        {
            return new StreamReader(StreamFrom(name));
        }

        /// <summary>
        /// Streams the from.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A Stream.</returns>
        public static Stream StreamFrom(string name)
        {
            var fromType = typeof(Yaml);
            var assembly = fromType.GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(name) ??
                         assembly.GetManifestResourceStream(fromType.Namespace + ".files." + name);
            return stream;
        }

        /// <summary>
        /// Templateds the on.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>A string.</returns>
        public static string TemplatedOn<T>(this TextReader reader)
        {
            var text = reader.ReadToEnd();
            return text.TemplatedOn<T>();
        }

        /// <summary>
        /// Templateds the on.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A string.</returns>
        public static string TemplatedOn<T>(this string text)
        {
            return Regex.Replace(text, @"{type}", match =>
                Uri.EscapeDataString(typeof(T).Name));
        }

        /// <summary>
        /// Parsers the for empty content.
        /// </summary>
        /// <returns>An IParser.</returns>
        public static IParser ParserForEmptyContent()
        {
            return new Parser(new StringReader(string.Empty));
        }

        /// <summary>
        /// Parsers the for resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>An IParser.</returns>
        public static IParser ParserForResource(string name)
        {
            return new Parser(Yaml.ReaderFrom(name));
        }

        /// <summary>
        /// Parsers the for text.
        /// </summary>
        /// <param name="yamlText">The yaml text.</param>
        /// <returns>An IParser.</returns>
        public static IParser ParserForText(string yamlText)
        {
            return new Parser(ReaderForText(yamlText));
        }

        /// <summary>
        /// Scanners the for resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A Scanner.</returns>
        public static Scanner ScannerForResource(string name)
        {
            return new Scanner(Yaml.ReaderFrom(name));
        }

        /// <summary>
        /// Scanners the for text.
        /// </summary>
        /// <param name="yamlText">The yaml text.</param>
        /// <returns>A Scanner.</returns>
        public static Scanner ScannerForText(string yamlText)
        {
            return new Scanner(ReaderForText(yamlText));
        }

        /// <summary>
        /// Readers the for text.
        /// </summary>
        /// <param name="yamlText">The yaml text.</param>
        /// <returns>A StringReader.</returns>
        public static StringReader ReaderForText(string yamlText)
        {
            return new StringReader(Text(yamlText));
        }

        /// <summary>
        /// Texts the.
        /// </summary>
        /// <param name="yamlText">The yaml text.</param>
        /// <returns>A string.</returns>
        public static string Text(string yamlText)
        {
            var lines = yamlText
                .Split('\n')
                .Select(l => l.TrimEnd())
                .SkipWhile(l => l.Length == 0)
                .ToList();

            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            if (lines.Count > 0)
            {
                var indent = Regex.Match(lines[0], @"^(\s*)");
                if (!indent.Success)
                {
                    throw new ArgumentException("Invalid indentation");
                }

                var indentation = indent.Groups[1].Length;
#pragma warning disable IDE0055 // Bug in Linux where IDE0055 is failing on the comments in the inline if statements
                lines = lines
                    .Select((l, num) => l.Length == 0 ?
                        // Blank lines don't need to be indented.
                        string.Empty :
                        l.TakeWhile(c => c == ' ' || c == '\t').Count() < indentation ?
                            // However, other lines must be indented at least as much as the first line.
                            throw new ArgumentException($"Incorrectly indented line '{l}', #{num}.", nameof(yamlText)) :
                            l.Substring(indentation))
#pragma warning restore IDE0055
                    .ToList();
            }

            return string.Join("\n", lines.ToArray());
        }
    }
}
