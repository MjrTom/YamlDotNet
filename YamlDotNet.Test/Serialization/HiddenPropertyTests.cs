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
using FluentAssertions;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Test.Serialization
{
    /// <summary>
    /// The hidden property tests.
    /// </summary>
    public class HiddenPropertyTests
    {
        /// <summary>
        /// The hidden property base.
        /// </summary>
        public class HiddenPropertyBase
        {
            protected object value;

            /// <summary>
            /// Gets the value.
            /// </summary>
            public object Value => this.value;

            /// <summary>
            /// Gets or sets the set only.
            /// </summary>
            [YamlIgnore]
            public object SetOnly { set => this.value = value; }

            /// <summary>
            /// Gets or sets the get and set.
            /// </summary>
            [YamlIgnore]
            public object GetAndSet { get; set; }
        }

        /// <summary>
        /// The hidden property derived.
        /// </summary>
        public class HiddenPropertyDerived<T> : HiddenPropertyBase
        {
            /// <summary>
            /// Gets the value.
            /// </summary>
            public new T Value { get => (T)this.value; }
            /// <summary>
            /// Gets or sets the set only.
            /// </summary>
            public new T SetOnly { set => this.value = value; }
            /// <summary>
            /// Gets or sets the get and set.
            /// </summary>
            public new T GetAndSet { get; set; }
        }

        /// <summary>
        /// The duplicate property base.
        /// </summary>
        public class DuplicatePropertyBase
        {
            protected object value;

            /// <summary>
            /// Gets the value.
            /// </summary>
            public object Value => this.value;
            /// <summary>
            /// Gets or sets the set only.
            /// </summary>
            public object SetOnly { set => this.value = value; }
            /// <summary>
            /// Gets or sets the get and set.
            /// </summary>
            public object GetAndSet { get; set; }
        }

        /// <summary>
        /// The duplicate property derived.
        /// </summary>
        public class DuplicatePropertyDerived<T> : DuplicatePropertyBase
        {
            /// <summary>
            /// Gets the value.
            /// </summary>
            public new T Value { get => (T)this.value; }
            /// <summary>
            /// Gets or sets the set only.
            /// </summary>
            public new T SetOnly { set => this.value = value; }
            /// <summary>
            /// Gets or sets the get and set.
            /// </summary>
            public new T GetAndSet { get; set; }
        }

        /// <summary>
        /// Tests the hidden.
        /// </summary>
        [Fact]
        public void TestHidden()
        {
            var yaml = @"
setOnly: set
getAndSet: getAndSet
";
            var d = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var o = d.Deserialize<HiddenPropertyDerived<string>>(yaml);
            Assert.Equal("set", o.Value);
            Assert.Equal("getAndSet", o.GetAndSet);
        }

        /// <summary>
        /// Tests the duplicate.
        /// </summary>
        [Fact]
        public void TestDuplicate()
        {
            var yaml = @"
setOnly: set
getAndSet: getAndSet
";
            var d = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            Action action = () => { d.Deserialize<DuplicatePropertyDerived<string>>(yaml); };
            action.Should().Throw<YamlException>();
        }
    }
}
