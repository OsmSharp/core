using NUnit.Framework;

namespace OsmSharp.Test.Geo
{  [TestFixture]
    public class ComparisonTests
    {

        [Test]
        public void OsmGeo_Nodes_CompareId()
        {
            var n1 = new Node() {
                Id = 42,
                Version = 0,
            };
            
            var n2 = new Node() {
                Id = 43,
                Version = 0,
            };

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeo_Ways_CompareId()
        {
            var n1 = new Way() {
                Id = 42,
                Version = 0,
            };
            
            var n2 = new Way() {
                Id = 43,
                Version = 0,
            };

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeo_NodeAndWay_CompareId()
        {
            var n1 = new Way() {
                Id = 42,
                Version = 0,
            };
            
            var n2 = new Way() {
                Id = 0,
                Version = 0,
            };

            Assert.AreEqual(1, n1.CompareTo(n2));
            Assert.AreEqual(-1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeoKey_Nodes_CompareId()
        {
            var n1 = new OsmGeoKey(OsmGeoType.Node, 42);

            var n2 = new OsmGeoKey(OsmGeoType.Node, 43);

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeoKey_Ways_CompareId()
        {
            var n1 = new OsmGeoKey(OsmGeoType.Way, 42);

            var n2 = new OsmGeoKey(OsmGeoType.Way, 43);

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeoKey_NodeAndWay_CompareId()
        {
            var n1 = new OsmGeoKey(OsmGeoType.Node, 42);

            var n2 = new OsmGeoKey(OsmGeoType.Way, 0);

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeoVersionKey_Nodes_CompareVersion()
        {
            var n1 = new OsmGeoVersionKey(OsmGeoType.Node, 42, 0);

            var n2 = new OsmGeoVersionKey(OsmGeoType.Node, 42, 1);

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeoVersionKey_Nodes_CompareId()
        {
            var n1 = new OsmGeoVersionKey(OsmGeoType.Node, 42, 0);

            var n2 = new OsmGeoVersionKey(OsmGeoType.Node, 43, 0);

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeoVersionKey_Ways_CompareId()
        {
            var n1 = new OsmGeoVersionKey(OsmGeoType.Way, 42, 0);

            var n2 = new OsmGeoVersionKey(OsmGeoType.Way, 43, 0);

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
        
        [Test]
        public void OsmGeoVersionKey_NodeAndWay_CompareId()
        {
            var n1 = new OsmGeoVersionKey(OsmGeoType.Node, 42, 0);

            var n2 = new OsmGeoVersionKey(OsmGeoType.Way, 0, 0);

            Assert.AreEqual(-1, n1.CompareTo(n2));
            Assert.AreEqual(1, n2.CompareTo(n1));

        }
    }
}