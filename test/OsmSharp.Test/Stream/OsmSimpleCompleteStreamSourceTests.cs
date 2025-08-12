using System.Linq;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Complete;
using OsmSharp.Streams;
using OsmSharp.Streams.Complete;

namespace OsmSharp.Test.Stream;

[TestFixture]
public class OsmSimpleCompleteStreamSourceTests
{
    [Test]
    public void SimpleComplete()
    {
        var streamSource = new XmlOsmStreamSource(Assembly.GetExecutingAssembly().GetManifestResourceStream(
            "OsmSharp.Test.data.xml.circular-relation.osm"));
        var completeSource = new OsmSimpleCompleteStreamSource(streamSource);
        var element = completeSource.OfType<CompleteRelation>().ToList();
        Assert.That(element.Count, Is.EqualTo(2));
    }
}