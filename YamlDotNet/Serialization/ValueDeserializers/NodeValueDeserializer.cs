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

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ValueDeserializers
{
    /// <summary>
    /// The node value deserializer.
    /// </summary>
    public sealed class NodeValueDeserializer : IValueDeserializer
    {
        private readonly IList<INodeDeserializer> deserializers;
        private readonly IList<INodeTypeResolver> typeResolvers;
        private readonly ITypeConverter typeConverter;
        private readonly INamingConvention enumNamingConvention;
        private readonly ITypeInspector typeInspector;

        public NodeValueDeserializer(IList<INodeDeserializer> deserializers,
            IList<INodeTypeResolver> typeResolvers,
            ITypeConverter typeConverter,
            INamingConvention enumNamingConvention,
            ITypeInspector typeInspector)
        {
            this.deserializers = deserializers ?? throw new ArgumentNullException(nameof(deserializers));
            this.typeResolvers = typeResolvers ?? throw new ArgumentNullException(nameof(typeResolvers));
            this.typeConverter = typeConverter ?? throw new ArgumentNullException(nameof(typeConverter));
            this.enumNamingConvention = enumNamingConvention ?? throw new ArgumentNullException(nameof(enumNamingConvention));
            this.typeInspector = typeInspector;
        }

        /// <summary>
        /// Deserializes the value.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="state">The state.</param>
        /// <param name="nestedObjectDeserializer">The nested object deserializer.</param>
        /// <returns>An object? .</returns>
        public object? DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
        {
            parser.Accept<NodeEvent>(out var nodeEvent);
            var nodeType = GetTypeFromEvent(nodeEvent, expectedType);
            var rootDeserializer = new ObjectDeserializer(x => DeserializeValue(parser, x, state, nestedObjectDeserializer));

            try
            {
                foreach (var deserializer in deserializers)
                {
                    var result = deserializer.Deserialize(parser, nodeType, (r, t) => nestedObjectDeserializer.DeserializeValue(r, t, state, nestedObjectDeserializer), out var value, rootDeserializer);
                    if (result)
                    {
                        return typeConverter.ChangeType(value, expectedType, enumNamingConvention, typeInspector);
                    }
                }
            }
            catch (YamlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new YamlException(
                    nodeEvent?.Start ?? Mark.Empty,
                    nodeEvent?.End ?? Mark.Empty,
                    "Exception during deserialization",
                    ex
                );
            }

            throw new YamlException(
                nodeEvent?.Start ?? Mark.Empty,
                nodeEvent?.End ?? Mark.Empty,
                $"No node deserializer was able to deserialize the node into type {expectedType.AssemblyQualifiedName}"
            );
        }

        private Type GetTypeFromEvent(NodeEvent? nodeEvent, Type currentType)
        {
            foreach (var typeResolver in typeResolvers)
            {
                if (typeResolver.Resolve(nodeEvent, ref currentType))
                {
                    break;
                }
            }
            return currentType;
        }
    }
}
