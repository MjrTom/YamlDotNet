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

using System.Globalization;

namespace YamlDotNet.Serialization
{
    /// <summary>
    /// The yaml formatter.
    /// </summary>
    public class YamlFormatter
    {
        /// <summary>
        /// Gets the default.
        /// </summary>
        public static YamlFormatter Default { get; } = new YamlFormatter();

        /// <summary>
        /// Gets or sets the number format.
        /// </summary>
        public NumberFormatInfo NumberFormat { get; set; } = new NumberFormatInfo
        {
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = "_",
            CurrencyGroupSizes = [3],
            CurrencySymbol = string.Empty,
            CurrencyDecimalDigits = 99,
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = "_",
            NumberGroupSizes = [3],
            NumberDecimalDigits = 99,
            NaNSymbol = ".nan",
            PositiveInfinitySymbol = ".inf",
            NegativeInfinitySymbol = "-.inf"
        };

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A string.</returns>
        public string FormatNumber(object number)
        {
            return Convert.ToString(number, NumberFormat)!;
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A string.</returns>
        public string FormatNumber(double number)
        {
            return number.ToString("G", NumberFormat);
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A string.</returns>
        public string FormatNumber(float number)
        {
            return number.ToString("G", NumberFormat);
        }

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// Formats the boolean.
        /// </summary>
        /// <param name="boolean">The boolean.</param>
        /// <returns>A string.</returns>
        public string FormatBoolean(object boolean)
#pragma warning restore CA1822
        {
            return boolean.Equals(true) ? "true" : "false";
        }

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A string.</returns>
        public string FormatDateTime(object dateTime)
#pragma warning restore CA1822
        {
            return ((DateTime)dateTime).ToString("o", CultureInfo.InvariantCulture);
        }

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// Formats the time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>A string.</returns>
        public string FormatTimeSpan(object timeSpan)
#pragma warning restore CA1822
        {
            return ((TimeSpan)timeSpan).ToString();
        }

        /// <summary>
        /// Converts an enum to it's string representation.
        /// By default it will be the string representation of the enum passed through the naming convention.
        /// </summary>
        /// <returns>A string representation of the enum</returns>
        public virtual Func<object, ITypeInspector, INamingConvention, string> FormatEnum { get; set; } = (value, typeInspector, enumNamingConvention) =>
        {
            var result = string.Empty;

            if (value == null)
            {
                result = string.Empty;
            }
            else
            {
                result = typeInspector.GetEnumValue(value);
            }

            result = enumNamingConvention.Apply(result);

            return result;
        };

        /// <summary>
        /// If this function returns true, the serializer will put quotes around the formatted enum value if necessary. Defaults to true.
        /// </summary>
        public virtual Func<object, bool> PotentiallyQuoteEnums { get; set; } = (_) => true;
    }
}
