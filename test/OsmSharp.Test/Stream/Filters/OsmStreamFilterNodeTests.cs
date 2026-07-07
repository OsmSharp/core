// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Linq;
using NUnit.Framework;
using OsmSharp.Streams;

namespace OsmSharp.Test.Stream.Filters;

/// <summary>
/// Node filter tests.
/// </summary>
[TestFixture]
internal class OsmStreamFilterNodeTests
{
    /// <summary>
    /// A test with just one node.
    /// </summary>
    [Test]
    public void TestNode()
    {
        var result = (new OsmGeo[] { new Node() { Id = 1 } }).FilterNodes(x => x.Id == 1).ToList();
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(1));

        result = (new OsmGeo[] { new Node() { Id = 1 } }).FilterNodes(x => x.Id == 2).ToList();
        Assert.That(result.Count, Is.EqualTo(0));
    }

    /// <summary>
    /// A test with just one way.
    /// </summary>
    [Test]
    public void TestWay()
    {
        var result = (new OsmGeo[]
        {
            new Node() { Id = 1 },
            new Node() { Id = 2 },
            new Way() { Id = 1, Nodes = new long[] { 1, 2 } }
        }).FilterNodes(x => x.Id == 1).ToList();
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Type, Is.EqualTo(OsmGeoType.Node));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[1].Type, Is.EqualTo(OsmGeoType.Way));
        Assert.That(result[1].Id, Is.EqualTo(1));

        result = (new OsmGeo[]
        {
            new Node() { Id = 1 },
            new Node() { Id = 2 },
            new Way() { Id = 1, Nodes = new long[] { 1, 2 } }
        }).FilterNodes(x => x.Id == 2).ToList();
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Type, Is.EqualTo(OsmGeoType.Node));
        Assert.That(result[0].Id, Is.EqualTo(2));
        Assert.That(result[1].Type, Is.EqualTo(OsmGeoType.Way));
        Assert.That(result[1].Id, Is.EqualTo(1));

        result = (new OsmGeo[]
        {
            new Node() { Id = 1 },
            new Node() { Id = 2 },
            new Way() { Id = 1, Nodes = new long[] { 1, 2 } }
        }).FilterNodes(x => x.Id == 3).ToList();
        Assert.That(result.Count, Is.EqualTo(0));
    }
}
