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
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Node, 2)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2).CompareTo(new OsmGeoKey(OsmGeoType.Node, 1)) > 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Node, 1)) == 0, Is.True);
        }
        
        [Test]
        public void OsmGeoKey_CompareTo_TypeWay_ShouldCompareId()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 2)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 2).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) > 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) == 0, Is.True);
        }
            
        [Test]
        public void OsmGeoKey_CompareTo_TypeRelation_ShouldCompareId()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Relation, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 2)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Relation, 2).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) > 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Relation, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) == 0, Is.True);
        }

        [Test]
        public void OsmGeoKey_CompareTo_TypeNode_ShouldSmallerThanWay()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 2)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Way, 1)) < 0, Is.True);
        }

        [Test]
        public void OsmGeoKey_CompareTo_TypeNode_ShouldSmallerThanRelation()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 2)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0, Is.True);
        }

        [Test]
        public void OsmGeoKey_CompareTo_TypeWay_ShouldSmallerThanRelation()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 2)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 2).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0, Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1).CompareTo(new OsmGeoKey(OsmGeoType.Relation, 1)) < 0, Is.True);
        }
        
        [Test]
        public void OsmGeoKey_CompareOperators_TypeNode_ShouldSmallerThanWay()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) < new OsmGeoKey(OsmGeoType.Way, 2), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) < new OsmGeoKey(OsmGeoType.Way, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) < new OsmGeoKey(OsmGeoType.Way, 1), Is.True);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) <= new OsmGeoKey(OsmGeoType.Way, 2), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) <= new OsmGeoKey(OsmGeoType.Way, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) <= new OsmGeoKey(OsmGeoType.Way, 1), Is.True);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) > new OsmGeoKey(OsmGeoType.Way, 2), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) > new OsmGeoKey(OsmGeoType.Way, 1), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) > new OsmGeoKey(OsmGeoType.Way, 1), Is.False);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) >= new OsmGeoKey(OsmGeoType.Way, 2), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) >= new OsmGeoKey(OsmGeoType.Way, 1), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) >= new OsmGeoKey(OsmGeoType.Way, 1), Is.False);
        }

        [Test]
        public void OsmGeoKey_CompareOperators_TypeNode_ShouldSmallerThanRelation()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) < new OsmGeoKey(OsmGeoType.Relation, 2), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) < new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) < new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) <= new OsmGeoKey(OsmGeoType.Relation, 2), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) <= new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) <= new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) > new OsmGeoKey(OsmGeoType.Relation, 2), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) > new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) > new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) > new OsmGeoKey(OsmGeoType.Relation, 2), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 2) > new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) > new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
        }

        [Test]
        public void OsmGeoKey_CompareOperators_TypeWay_ShouldSmallerThanRelation()
        {
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) < new OsmGeoKey(OsmGeoType.Relation, 2), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 2) < new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) < new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) <= new OsmGeoKey(OsmGeoType.Relation, 2), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 2) <= new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) <= new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) > new OsmGeoKey(OsmGeoType.Relation, 2), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 2) > new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) > new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) >= new OsmGeoKey(OsmGeoType.Relation, 2), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 2) >= new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) >= new OsmGeoKey(OsmGeoType.Relation, 1), Is.False);
        }

        [Test]
        public void OsmGeoKey_CompareOperators_WhenEqual_ShouldSmallerThanOrEqual()
        {
            // ReSharper disable EqualExpressionComparison
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) <= new OsmGeoKey(OsmGeoType.Node, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) <= new OsmGeoKey(OsmGeoType.Way, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Relation, 1) <= new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
            
            Assert.That(new OsmGeoKey(OsmGeoType.Node, 1) >= new OsmGeoKey(OsmGeoType.Node, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Way, 1) >= new OsmGeoKey(OsmGeoType.Way, 1), Is.True);
            Assert.That(new OsmGeoKey(OsmGeoType.Relation, 1) <= new OsmGeoKey(OsmGeoType.Relation, 1), Is.True);
        }
    }
}