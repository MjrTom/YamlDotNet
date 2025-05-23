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
using FluentAssertions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Test.Serialization.BufferedDeserialization
{
    /// <summary>
    /// The unique key type discriminator tests.
    /// </summary>
    public class UniqueKeyTypeDiscriminatorTests
    {
        /// <summary>
        /// Uniques the key type discriminator_ with interface base type.
        /// </summary>
        [Fact]
        public void UniqueKeyTypeDiscriminator_WithInterfaceBaseType()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddUniqueKeyTypeDiscriminator<ICharacter>(
                        new Dictionary<string, Type>()
                        {
                            { "cheeseSupply", typeof(Mouse) },
                            { "avgDailyMeows", typeof(Cat) }
                        }
                    );
                },
                    maxDepth: 3,
                    maxLength: 10)
                .Build();

            var characters = bufferedDeserializer.Deserialize<List<ICharacter>>(TomAndJerryYaml);
            characters[0].Should().BeOfType<Mouse>();
            characters[1].Should().BeOfType<Cat>();
        }

        /// <summary>
        /// Uniques the key type discriminator_ with object base type.
        /// </summary>
        [Fact]
        public void UniqueKeyTypeDiscriminator_WithObjectBaseType()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddUniqueKeyTypeDiscriminator<object>(
                        new Dictionary<string, Type>()
                        {
                            { "cheeseSupply", typeof(Mouse) },
                            { "avgDailyMeows", typeof(Cat) }
                        }
                    );
                },
                    maxDepth: 3,
                    maxLength: 10)
                .Build();

            var charactersObj = bufferedDeserializer.Deserialize<object>(TomAndJerryYaml);
            var characters = (List<object>)charactersObj;
            characters[0].Should().BeOfType<Mouse>();
            characters[1].Should().BeOfType<Cat>();
        }

        public const string TomAndJerryYaml = @"
- name: Jerry
  cheeseSupply: 5
- name: Tom
  avgDailyMeows: 20.0
";

        public interface ICharacter { }

        /// <summary>
        /// The mouse.
        /// </summary>
        public class Mouse : ICharacter
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the cheese supply.
            /// </summary>
            public int CheeseSupply { get; set; }
        }

        /// <summary>
        /// The cat.
        /// </summary>
        public class Cat : ICharacter
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the avg daily meows.
            /// </summary>
            public float AvgDailyMeows { get; set; }
        }
    }
}
