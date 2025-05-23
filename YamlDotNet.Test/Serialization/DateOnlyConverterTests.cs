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

#if NET6_0_OR_GREATER
using System;
using System.Globalization;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Test.Serialization
{
    /// <summary>
    /// This represents the test entity for the <see cref="DateOnlyConverter"/> class.
    /// </summary>
    public class DateOnlyConverterTests
    {
        /// <summary>
        /// Tests whether the Accepts() method should return expected result or not.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to check.</param>
        /// <param name="expected">Expected result.</param>
        [Theory]
        [InlineData(typeof(DateOnly), true)]
        [InlineData(typeof(string), false)]
        public void Given_Type_Accepts_ShouldReturn_Result(Type type, bool expected)
        {
            var converter = new DateOnlyConverter();

            var result = converter.Accepts(type);

            result.Should().Be(expected);
        }

        /// <summary>
        /// Tests whether the ReadYaml() method should throw <see cref="FormatException"/> or not.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="day">Day value.</param>
        /// <remarks>The converter instance uses its default parameter of "d".</remarks>
        [Theory]
        [InlineData(2016, 12, 31)]
        public void Given_Yaml_WithInvalidDateTimeFormat_WithDefaultParameter_ReadYaml_ShouldThrow_Exception(int year, int month, int day)
        {
            var yaml = $"{year}-{month:00}-{day:00}";

            var parser = A.Fake<IParser>();
            A.CallTo(() => parser.Current).ReturnsLazily(() => new Scalar(yaml));

            var converter = new DateOnlyConverter();

            Action action = () => { converter.ReadYaml(parser, typeof(DateOnly), null); };

            action.Should().Throw<FormatException>();
        }

        /// <summary>
        /// Tests whether the ReadYaml() method should return expected result or not.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="day">Day value.</param>
        /// <remarks>The converter instance uses its default parameter of "d".</remarks>
        [Theory]
        [InlineData(2016, 12, 31)]
        public void Given_Yaml_WithValidDateTimeFormat_WithDefaultParameter_ReadYaml_ShouldReturn_Result(int year, int month, int day)
        {
            var yaml = $"{month:00}/{day:00}/{year}"; // This is the DateOnly format of "d"

            var parser = A.Fake<IParser>();
            A.CallTo(() => parser.Current).ReturnsLazily(() => new Scalar(yaml));

            var converter = new DateOnlyConverter();

            var result = converter.ReadYaml(parser, typeof(DateOnly), null);

            result.Should().BeOfType<DateOnly>();
            ((DateOnly)result).Year.Should().Be(year);
            ((DateOnly)result).Month.Should().Be(month);
            ((DateOnly)result).Day.Should().Be(day);
        }

        /// <summary>
        /// Tests whether the ReadYaml() method should return expected result or not.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="day">Day value.</param>
        /// <param name="format1">Designated date/time format 1.</param>
        /// <param name="format2">Designated date/time format 2.</param>
        [Theory]
        [InlineData(2016, 12, 31, "yyyy-MM-dd", "yyyy/MM/dd")]
        public void Given_Yaml_WithValidDateTimeFormat_ReadYaml_ShouldReturn_Result(int year, int month, int day, string format1, string format2)
        {
            var yaml = $"{year}-{month:00}-{day:00}";

            var parser = A.Fake<IParser>();
            A.CallTo(() => parser.Current).ReturnsLazily(() => new Scalar(yaml));

            var converter = new DateOnlyConverter(formats: new[] { format1, format2 });

            var result = converter.ReadYaml(parser, typeof(DateOnly), null);

            result.Should().BeOfType<DateOnly>();
            ((DateOnly)result).Year.Should().Be(year);
            ((DateOnly)result).Month.Should().Be(month);
            ((DateOnly)result).Day.Should().Be(day);
        }

        /// <summary>
        /// Tests whether the ReadYaml() method should return expected result or not.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="day">Day value.</param>
        /// <param name="format1">Designated date/time format 1.</param>
        /// <param name="format2">Designated date/time format 2.</param>
        [Theory]
        [InlineData(2016, 12, 31, "yyyy-MM-dd", "yyyy/MM/dd")]
        public void Given_Yaml_WithSpecificCultureAndValidDateTimeFormat_ReadYaml_ShouldReturn_Result(int year, int month, int day, string format1, string format2)
        {
            var yaml = $"{year}-{month:00}-{day:00}";

            var parser = A.Fake<IParser>();
            A.CallTo(() => parser.Current).ReturnsLazily(() => new Scalar(yaml));

            var culture = new CultureInfo("ko-KR"); // Sample specific culture
            var converter = new DateOnlyConverter(provider: culture, formats: new[] { format1, format2 });

            var result = converter.ReadYaml(parser, typeof(DateOnly), null);

            result.Should().BeOfType<DateOnly>();
            ((DateOnly)result).Year.Should().Be(year);
            ((DateOnly)result).Month.Should().Be(month);
            ((DateOnly)result).Day.Should().Be(day);
        }

        /// <summary>
        /// Tests whether the ReadYaml() method should return expected result or not.
        /// </summary>
        /// <param name="format">Date/Time format.</param>
        /// <param name="value">Date/Time value.</param>
        [Theory]
        [InlineData("d", "01/11/2017")]
        [InlineData("D", "Wednesday, 11 January 2017")]
        [InlineData("f", "Wednesday, 11 January 2017 02:36")]
        [InlineData("F", "Wednesday, 11 January 2017 02:36:16")]
        [InlineData("g", "01/11/2017 02:36")]
        [InlineData("G", "01/11/2017 02:36:16")]
        [InlineData("M", "January 11")]
        [InlineData("s", "2017-01-11T02:36:16")]
        [InlineData("u", "2017-01-11 02:36:16Z")]
        [InlineData("Y", "2017 January")]
        public void Given_Yaml_WithDateTimeFormat_ReadYaml_ShouldReturn_Result(string format, string value)
        {
            var expected = DateOnly.ParseExact(value, format, CultureInfo.InvariantCulture);
            var converter = new DateOnlyConverter(formats: new[] { "d", "D", "f", "F", "g", "G", "M", "s", "u", "Y" });

            var parser = A.Fake<IParser>();
            A.CallTo(() => parser.Current).ReturnsLazily(() => new Scalar(value));

            var result = converter.ReadYaml(parser, typeof(DateOnly), null);

            result.Should().Be(expected);
        }

        /// <summary>
        /// Tests whether the ReadYaml() method should return expected result or not.
        /// </summary>
        /// <param name="format">Date/Time format.</param>
        /// <param name="locale">Locale value.</param>
        /// <param name="value">Date/Time value.</param>
        [Theory]
        [InlineData("d", "fr-FR", "13/01/2017")]
        [InlineData("D", "fr-FR", "vendredi 13 janvier 2017")]
        [InlineData("f", "fr-FR", "vendredi 13 janvier 2017 05:25")]
        [InlineData("F", "fr-FR", "vendredi 13 janvier 2017 05:25:08")]
        [InlineData("g", "fr-FR", "13/01/2017 05:25")]
        [InlineData("G", "fr-FR", "13/01/2017 05:25:08")]
        [InlineData("M", "fr-FR", "13 janvier")]
        [InlineData("s", "fr-FR", "2017-01-13T05:25:08")]
        [InlineData("u", "fr-FR", "2017-01-13 05:25:08Z")]
        [InlineData("Y", "fr-FR", "janvier 2017")]
        // [InlineData("d", "ko-KR", "2017-01-13")]
        [InlineData("D", "ko-KR", "2017년 1월 13일 금요일")]
        // [InlineData("f", "ko-KR", "2017년 1월 13일 금요일 오전 5:32")]
        // [InlineData("F", "ko-KR", "2017년 1월 13일 금요일 오전 5:32:06")]
        // [InlineData("g", "ko-KR", "2017-01-13 오전 5:32")]
        // [InlineData("G", "ko-KR", "2017-01-13 오전 5:32:06")]
        [InlineData("M", "ko-KR", "1월 13일")]
        [InlineData("s", "ko-KR", "2017-01-13T05:32:06")]
        [InlineData("u", "ko-KR", "2017-01-13 05:32:06Z")]
        [InlineData("Y", "ko-KR", "2017년 1월")]
        public void Given_Yaml_WithLocaleAndDateTimeFormat_ReadYaml_ShouldReturn_Result(string format, string locale, string value)
        {
            var culture = new CultureInfo(locale);

            var expected = default(DateOnly);
            try
            {
                expected = DateOnly.ParseExact(value, format, culture);
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed to parse the test argument to DateOnly. The expected date/time format should look like this: '{0}'", DateTime.Now.ToString(format, culture));
                throw new Exception(message, ex);
            }

            var converter = new DateOnlyConverter(provider: culture, formats: new[] { "d", "D", "f", "F", "g", "G", "M", "s", "u", "Y" });

            var parser = A.Fake<IParser>();
            A.CallTo(() => parser.Current).ReturnsLazily(() => new Scalar(value));

            var result = converter.ReadYaml(parser, typeof(DateOnly), null);

            result.Should().Be(expected);
        }

        /// <summary>
        /// Tests whether the WriteYaml method should return expected result or not.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="day">Day value.</param>
        /// <remarks>The converter instance uses its default parameter of "d".</remarks>
        [Theory]
        [InlineData(2016, 12, 31)]
        public void Given_Values_WriteYaml_ShouldReturn_Result(int year, int month, int day)
        {
            var dateOnly = new DateOnly(year, month, day);
            var formatted = dateOnly.ToString("d", CultureInfo.InvariantCulture);
            var obj = new TestObject() { DateOnly = dateOnly };

            var builder = new SerializerBuilder();
            builder.WithNamingConvention(CamelCaseNamingConvention.Instance);
            builder.WithTypeConverter(new DateOnlyConverter());

            var serialiser = builder.Build();

            var serialised = serialiser.Serialize(obj);

            serialised.Should().ContainEquivalentOf($"dateonly: {formatted}");
        }

        /// <summary>
        /// Tests whether the WriteYaml method should return expected result or not.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="day">Day value.</param>
        /// <param name="locale">Locale value.</param>
        /// <remarks>The converter instance uses its default parameter of "d".</remarks>
        [Theory]
        [InlineData(2016, 12, 31, "es-ES")]
        [InlineData(2016, 12, 31, "ko-KR")]
        public void Given_Values_WithLocale_WriteYaml_ShouldReturn_Result(int year, int month, int day, string locale)
        {
            var dateOnly = new DateOnly(year, month, day);
            var culture = new CultureInfo(locale);
            var formatted = dateOnly.ToString("d", culture);
            var obj = new TestObject() { DateOnly = dateOnly };

            var builder = new SerializerBuilder();
            builder.WithNamingConvention(CamelCaseNamingConvention.Instance);
            builder.WithTypeConverter(new DateOnlyConverter(provider: culture));

            var serialiser = builder.Build();

            var serialised = serialiser.Serialize(obj);

            serialised.Should().ContainEquivalentOf($"dateonly: {formatted}");
        }

        /// <summary>
        /// Tests whether the WriteYaml method should return expected result or not.
        /// </summary>
        /// <param name="year">Year value.</param>
        /// <param name="month">Month value.</param>
        /// <param name="day">Day value.</param>
        /// <remarks>The converter instance uses its default parameter of "d".</remarks>
        [Theory]
        [InlineData(2016, 12, 31)]
        public void Given_Values_WithFormats_WriteYaml_ShouldReturn_Result_WithFirstFormat(int year, int month, int day)
        {
            var dateOnly = new DateOnly(year, month, day);
            var format = "yyyy-MM-dd";
            var formatted = dateOnly.ToString(format, CultureInfo.InvariantCulture);
            var obj = new TestObject() { DateOnly = dateOnly };

            var builder = new SerializerBuilder();
            builder.WithNamingConvention(CamelCaseNamingConvention.Instance);
            builder.WithTypeConverter(new DateOnlyConverter(formats: new[] { format, "d" }));

            var serialiser = builder.Build();

            var serialised = serialiser.Serialize(obj);

            serialised.Should().ContainEquivalentOf($"dateonly: {formatted}");
        }

        /// <summary>
        /// Jsons the compatible_ encase date only in double quotes.
        /// </summary>
        [Fact]
        public void JsonCompatible_EncaseDateOnlyInDoubleQuotes()
        {
            var serializer = new SerializerBuilder().JsonCompatible().Build();
            var testObject = new TestObject { DateOnly = new DateOnly(2023, 01, 14) };
            var actual = serializer.Serialize(testObject);

            actual.TrimNewLines().Should().ContainEquivalentOf("{\"DateOnly\": \"01/14/2023\"}");
        }

        /// <summary>
        /// This represents the test object entity.
        /// </summary>
        private class TestObject
        {
            /// <summary>
            /// Gets or sets the <see cref="System.DateOnly"/> value.
            /// </summary>
            public DateOnly DateOnly { get; set; }
        }
    }
}
#endif
