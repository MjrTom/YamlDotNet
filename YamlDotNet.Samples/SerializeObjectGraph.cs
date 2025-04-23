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
using Xunit.Abstractions;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;

namespace YamlDotNet.Samples
{
    /// <summary>
    /// The serialize object graph.
    /// </summary>
    public class SerializeObjectGraph
    {
        private readonly ITestOutputHelper output;

        public SerializeObjectGraph(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Mains the.
        /// </summary>
        [Sample(
            DisplayName = "Serializing an object graph",
            Description = "Shows how to convert an object to its YAML representation."
        )]
        public void Main()
        {
            var address = new Address
            {
                street = "123 Tornado Alley\nSuite 16",
                city = "East Westville",
                state = "KS"
            };

            var receipt = new Receipt
            {
                receipt = "Oz-Ware Purchase Invoice",
                date = new DateTime(2007, 8, 6),
                customer = new Customer
                {
                    given = "Dorothy",
                    family = "Gale"
                },
                items = new Item[]
                {
                    new Item
                    {
                        part_no = "A4786",
                        descrip = "Water Bucket (Filled)",
                        price = 1.47M,
                        quantity = 4
                    },
                    new Item
                    {
                        part_no = "E1628",
                        descrip = "High Heeled \"Ruby\" Slippers",
                        price = 100.27M,
                        quantity = 1
                    }
                },
                bill_to = address,
                ship_to = address,
                specialDelivery = "Follow the Yellow Brick\n" +
                                  "Road to the Emerald City.\n" +
                                  "Pay no attention to the\n" +
                                  "man behind the curtain."
            };

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(receipt);
            output.WriteLine(yaml);
        }
    }

#pragma warning disable IDE1006 // Naming Styles
    /// <summary>
    /// The address.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        public string street { get; set; }
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public string state { get; set; }
    }

    /// <summary>
    /// The receipt.
    /// </summary>
    public class Receipt
    {
        /// <summary>
        /// Gets or sets the receipt.
        /// </summary>
        public string receipt { get; set; }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        public Customer customer { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public Item[] items { get; set; }
        /// <summary>
        /// Gets or sets the bill_to.
        /// </summary>
        public Address bill_to { get; set; }
        /// <summary>
        /// Gets or sets the ship_to.
        /// </summary>
        public Address ship_to { get; set; }
        /// <summary>
        /// Gets or sets the special delivery.
        /// </summary>
        public string specialDelivery { get; set; }
    }

    /// <summary>
    /// The customer.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the given.
        /// </summary>
        public string given { get; set; }
        /// <summary>
        /// Gets or sets the family.
        /// </summary>
        public string family { get; set; }
    }

    /// <summary>
    /// The item.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Gets or sets the part_no.
        /// </summary>
        public string part_no { get; set; }
        /// <summary>
        /// Gets or sets the descrip.
        /// </summary>
        public string descrip { get; set; }
        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int quantity { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
