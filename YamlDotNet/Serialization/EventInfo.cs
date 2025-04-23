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

using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
    /// <summary>
    /// The event info.
    /// </summary>
    public abstract class EventInfo
    {
        /// <summary>
        /// Gets the source.
        /// </summary>
        public IObjectDescriptor Source { get; }

        protected EventInfo(IObjectDescriptor source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }
    }

    /// <summary>
    /// The alias event info.
    /// </summary>
    public class AliasEventInfo : EventInfo
    {
        public AliasEventInfo(IObjectDescriptor source, AnchorName alias)
            : base(source)
        {
            if (alias.IsEmpty)
            {
                throw new ArgumentNullException(nameof(alias));
            }
            Alias = alias;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public AnchorName Alias { get; }
        /// <summary>
        /// Gets or sets a value indicating whether needs expansion.
        /// </summary>
        public bool NeedsExpansion { get; set; }
    }

    /// <summary>
    /// The object event info.
    /// </summary>
    public class ObjectEventInfo : EventInfo
    {
        protected ObjectEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets the anchor.
        /// </summary>
        public AnchorName Anchor { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public TagName Tag { get; set; }
    }

    /// <summary>
    /// The scalar event info.
    /// </summary>
    public sealed class ScalarEventInfo : ObjectEventInfo
    {
        public ScalarEventInfo(IObjectDescriptor source)
            : base(source)
        {
            Style = source.ScalarStyle;
            RenderedValue = string.Empty;
        }

        /// <summary>
        /// Gets or sets the rendered value.
        /// </summary>
        public string RenderedValue { get; set; }
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public ScalarStyle Style { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether plain is implicit.
        /// </summary>
        public bool IsPlainImplicit { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether quoted is implicit.
        /// </summary>
        public bool IsQuotedImplicit { get; set; }
    }

    /// <summary>
    /// The mapping start event info.
    /// </summary>
    public sealed class MappingStartEventInfo : ObjectEventInfo
    {
        public MappingStartEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether is implicit.
        /// </summary>
        public bool IsImplicit { get; set; }
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public MappingStyle Style { get; set; }
    }

    /// <summary>
    /// The mapping end event info.
    /// </summary>
    public sealed class MappingEndEventInfo : EventInfo
    {
        public MappingEndEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }
    }

    /// <summary>
    /// The sequence start event info.
    /// </summary>
    public sealed class SequenceStartEventInfo : ObjectEventInfo
    {
        public SequenceStartEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether is implicit.
        /// </summary>
        public bool IsImplicit { get; set; }
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public SequenceStyle Style { get; set; }
    }

    /// <summary>
    /// The sequence end event info.
    /// </summary>
    public sealed class SequenceEndEventInfo : EventInfo
    {
        public SequenceEndEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }
    }
}
