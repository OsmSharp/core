using NUnit.Framework;
using OsmSharp.Db;

namespace OsmSharp.Test.Db
{
    [TestFixture]
    public class OsmGeoKeyTests
    {
        [Test]
        public void OsmGeoKey_CompareTo_TypeNode_ShouldCompareId()
        {
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Node, 2)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 2).CompareTo(new OsmGeoKey(OsmGeoType.Node, 1)) > 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Node, 1)) == 0);
        }
        
        [Test]
        public void OsmGeoKey_CompareTo_TypeWay_ShouldCompareId()
        {
            Assert.True(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 2)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Way, 2).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) > 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) == 0);
        }
            
        [Test]
        public void OsmGeoKey_CompareTo_TypeRelation_ShouldCompareId()
        {
            Assert.True(new OsmGeoKey(OsmGeoType.Relation, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 2)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Relation, 2).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) > 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Relation, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) == 0);
        }

        [Test]
        public void OsmGeoKey_CompareTo_TypeNode_ShouldSmallerThanWay()
        {
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 2)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 2).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) < 0);
        }

        [Test]
        public void OsmGeoKey_CompareTo_TypeNode_ShouldSmallerThanRelation()
        {
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 2)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 2).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0);
        }

        [Test]
        public void OsmGeoKey_CompareTo_TypeWay_ShouldSmallerThanRelation()
        {
            Assert.True(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 2)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Way, 2).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0);
            Assert.True(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0);
        }
    }
}