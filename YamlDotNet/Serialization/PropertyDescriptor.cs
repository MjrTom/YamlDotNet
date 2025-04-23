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

namespace YamlDotNet.Serialization
{
    /// <summary>
    /// The property descriptor.
    /// </summary>
    public sealed class PropertyDescriptor : IPropertyDescriptor
    {
        private readonly IPropertyDescriptor baseDescriptor;
        public PropertyDescriptor(IPropertyDescriptor baseDescriptor)
        {
            this.baseDescriptor = baseDescriptor;
            Name = baseDescriptor.Name;
        }

        /// <summary>
        /// Gets a value indicating whether allow nulls.
        /// </summary>
        public bool AllowNulls
        {
            get
            {
                return baseDescriptor.AllowNulls;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether required.
        /// </summary>
        public bool Required { get => baseDescriptor.Required; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get { return baseDescriptor.Type; } }

        /// <summary>
        /// Gets or sets the type override.
        /// </summary>
        public Type? TypeOverride
        {
            get { return baseDescriptor.TypeOverride; }
            set { baseDescriptor.TypeOverride = value; }
        }

        /// <summary>
        /// Gets the converter type.
        /// </summary>
        public Type? ConverterType => baseDescriptor.ConverterType;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the scalar style.
        /// </summary>
        public ScalarStyle ScalarStyle
        {
            get { return baseDescriptor.ScalarStyle; }
            set { baseDescriptor.ScalarStyle = value; }
        }

        /// <summary>
        /// Gets a value indicating whether can write.
        /// </summary>
        public bool CanWrite
        {
            get { return baseDescriptor.CanWrite; }
        }

        /// <summary>
        /// Writes the.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public void Write(object target, object? value)
        {
            baseDescriptor.Write(target, value);
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <returns>A T? .</returns>
        public T? GetCustomAttribute<T>() where T : Attribute
        {
            return baseDescriptor.GetCustomAttribute<T>();
        }

        /// <summary>
        /// Reads the.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>An IObjectDescriptor.</returns>
        public IObjectDescriptor Read(object target)
        {
            return baseDescriptor.Read(target);
        }
    }
}
