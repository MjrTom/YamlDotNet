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

using System.Collections.Generic;
using System.IO;
using Xunit.Abstractions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;

namespace YamlDotNet.Samples
{
    /// <summary>
    /// The deserializing multiple documents.
    /// </summary>
    public class DeserializingMultipleDocuments
    {
        private readonly ITestOutputHelper output;

        public DeserializingMultipleDocuments(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Mains the.
        /// </summary>
        [Sample(
            DisplayName = "Deserializing multiple documents",
            Description = "Explains how to load multiple YAML documents from a stream."
        )]
        public void Main()
        {
            var input = new StringReader(Document);

            var deserializer = new DeserializerBuilder().Build();

            var parser = new Parser(input);

            // Consume the stream start event "manually"
            parser.Consume<StreamStart>();

            while (parser.Accept<DocumentStart>(out var _))
            {
                // Deserialize the document
                var doc = deserializer.Deserialize<List<string>>(parser);

                output.WriteLine("## Document");
                foreach (var item in doc)
                {
                    output.WriteLine(item);
                }
            }
        }

        private const string Document = @"---
- Prisoner
- Goblet
- Phoenix
---
- Memoirs
- Snow 
- Ghost		
...";
    }
}
