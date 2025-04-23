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
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Test.Serialization.BufferedDeserialization
{
    /// <summary>
    /// The key value type discriminator tests.
    /// </summary>
    public class KeyValueTypeDiscriminatorTests
    {
        /// <summary>
        /// Keys the value type discriminator_ with parent base type_ single.
        /// </summary>
        [Fact]
        public void KeyValueTypeDiscriminator_WithParentBaseType_Single()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddKeyValueTypeDiscriminator<KubernetesResource>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Namespace", typeof(KubernetesNamespace) },
                            { "Service", typeof(KubernetesService) }
                        });
                },
                    maxDepth: 3,
                    maxLength: 40)
                .Build();

            var service = bufferedDeserializer.Deserialize<KubernetesResource>(KubernetesServiceYaml);
            service.Should().BeOfType<KubernetesService>();
        }

        /// <summary>
        /// Keys the value type discriminator_ with parent base type_ list.
        /// </summary>
        [Fact]
        public void KeyValueTypeDiscriminator_WithParentBaseType_List()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddKeyValueTypeDiscriminator<KubernetesResource>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Namespace", typeof(KubernetesNamespace) },
                            { "Service", typeof(KubernetesService) }
                        });
                },
                    maxDepth: 3,
                    maxLength: 40)
                .Build();

            var resources = bufferedDeserializer.Deserialize<List<KubernetesResource>>(ListOfKubernetesYaml);
            resources[0].Should().BeOfType<KubernetesNamespace>();
            resources[1].Should().BeOfType<KubernetesService>();
        }

        /// <summary>
        /// Keys the value type discriminator_ with object base type_ single.
        /// </summary>
        [Fact]
        public void KeyValueTypeDiscriminator_WithObjectBaseType_Single()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddKeyValueTypeDiscriminator<object>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Namespace", typeof(KubernetesNamespace) },
                            { "Service", typeof(KubernetesService) }
                        });
                },
                    maxDepth: 3,
                    maxLength: 40)
                .Build();

            var service = bufferedDeserializer.Deserialize<object>(KubernetesServiceYaml);
            service.Should().BeOfType<KubernetesService>();
        }

        /// <summary>
        /// Keys the value type discriminator_ with object base type_ list.
        /// </summary>
        [Fact]
        public void KeyValueTypeDiscriminator_WithObjectBaseType_List()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddKeyValueTypeDiscriminator<object>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Namespace", typeof(KubernetesNamespace) },
                            { "Service", typeof(KubernetesService) }
                        });
                },
                    maxDepth: 3,
                    maxLength: 30)
                .Build();

            var resources = bufferedDeserializer.Deserialize<List<object>>(ListOfKubernetesYaml);
            resources[0].Should().BeOfType<KubernetesNamespace>();
            resources[1].Should().BeOfType<KubernetesService>();
        }

        /// <summary>
        /// Keys the value type discriminator_ with interface base type_ single.
        /// </summary>
        [Fact]
        public void KeyValueTypeDiscriminator_WithInterfaceBaseType_Single()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddKeyValueTypeDiscriminator<IKubernetesResource>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Namespace", typeof(KubernetesNamespace) },
                            { "Service", typeof(KubernetesService) }
                        });
                },
                    maxDepth: 3,
                    maxLength: 40)
                .Build();

            var service = bufferedDeserializer.Deserialize<IKubernetesResource>(KubernetesServiceYaml);
            service.Should().BeOfType<KubernetesService>();
        }

        /// <summary>
        /// Keys the value type discriminator_ with interface base type_ list.
        /// </summary>
        [Fact]
        public void KeyValueTypeDiscriminator_WithInterfaceBaseType_List()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddKeyValueTypeDiscriminator<IKubernetesResource>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Namespace", typeof(KubernetesNamespace) },
                            { "Service", typeof(KubernetesService) }
                        });
                },
                    maxDepth: 3,
                    maxLength: 30)
                .Build();

            var resources = bufferedDeserializer.Deserialize<List<IKubernetesResource>>(ListOfKubernetesYaml);
            resources[0].Should().BeOfType<KubernetesNamespace>();
            resources[1].Should().BeOfType<KubernetesService>();
        }

        /// <summary>
        /// Keys the value type discriminator_ multiple with same key.
        /// </summary>
        [Fact]
        public void KeyValueTypeDiscriminator_MultipleWithSameKey()
        {
            var bufferedDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer(options =>
                {
                    options.AddKeyValueTypeDiscriminator<KubernetesResource>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Namespace", typeof(KubernetesNamespace) },
                        });
                    options.AddKeyValueTypeDiscriminator<KubernetesResource>(
                        "kind",
                        new Dictionary<string, Type>()
                        {
                            { "Service", typeof(KubernetesService) }
                        });
                },
                    maxDepth: 3,
                    maxLength: 40)
                .Build();

            var resources = bufferedDeserializer.Deserialize<List<KubernetesResource>>(ListOfKubernetesYaml);
            resources[0].Should().BeOfType<KubernetesNamespace>();
            resources[1].Should().BeOfType<KubernetesService>();
        }

        public const string ListOfKubernetesYaml = @"
- apiVersion: v1
  kind: Namespace
  metadata:
    name: test-namespace
- apiVersion: v1
  kind: Service
  metadata:
    name: my-service
  spec:
    selector:
      app.kubernetes.io/name: MyApp
    ports:
      - protocol: TCP
        port: 80
        targetPort: 9376
";

        public const string KubernetesServiceYaml = @"
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  selector:
    app.kubernetes.io/name: MyApp
  ports:
    - protocol: TCP
      port: 80
      targetPort: 9376
";

        public interface IKubernetesResource { }

        /// <summary>
        /// The kubernetes resource.
        /// </summary>
        public class KubernetesResource : IKubernetesResource
        {
            /// <summary>
            /// Gets or sets the api version.
            /// </summary>
            public string ApiVersion { get; set; }
            /// <summary>
            /// Gets or sets the kind.
            /// </summary>
            public string Kind { get; set; }
            /// <summary>
            /// Gets or sets the metadata.
            /// </summary>
            public KubernetesMetadata Metadata { get; set; }

            /// <summary>
            /// The kubernetes metadata.
            /// </summary>
            public class KubernetesMetadata
            {
                /// <summary>
                /// Gets or sets the name.
                /// </summary>
                public string Name { get; set; }
            }
        }

        /// <summary>
        /// The kubernetes service.
        /// </summary>
        public class KubernetesService : KubernetesResource
        {
            /// <summary>
            /// Gets or sets the spec.
            /// </summary>
            public KubernetesServiceSpec Spec { get; set; }
            /// <summary>
            /// The kubernetes service spec.
            /// </summary>
            public class KubernetesServiceSpec
            {
                /// <summary>
                /// Gets or sets the selector.
                /// </summary>
                public Dictionary<string, string> Selector { get; set; }
                /// <summary>
                /// Gets or sets the ports.
                /// </summary>
                public List<KubernetesServicePort> Ports { get; set; }
                /// <summary>
                /// The kubernetes service port.
                /// </summary>
                public class KubernetesServicePort
                {
                    /// <summary>
                    /// Gets or sets the protocol.
                    /// </summary>
                    public string Protocol { get; set; }
                    /// <summary>
                    /// Gets or sets the port.
                    /// </summary>
                    public int Port { get; set; }
                    /// <summary>
                    /// Gets or sets the target port.
                    /// </summary>
                    public int TargetPort { get; set; }
                }
            }
        }

        /// <summary>
        /// The kubernetes namespace.
        /// </summary>
        public class KubernetesNamespace : KubernetesResource
        {

        }
    }
}
