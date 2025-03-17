using System.Text.Json;
using NUnit.Framework;
using OsmSharp.API;

namespace OsmSharp.Test.IO.Json.Overpass;

public class OverpassTests
{
        [Test]
        public void Osm_FromJson_Complete_ShouldReadCompleteJson()
        {
            var osm = JsonSerializer.Deserialize<Osm>(
                "{\n  \"version\": 0.6,\n  \"generator\": \"Overpass API 0.7.62.1 084b4234\",\n  \"osm3s\": {\n    \"timestamp_osm_base\": \"2024-06-03T14:30:53Z\",\n    \"copyright\": \"The data included in this document is from www.openstreetmap.org. The data is made available under ODbL.\"\n  },\n  \"elements\": [\n    {\n      \"type\": \"node\",\n      \"id\": 357179926,\n      \"lat\": 51.2290388,\n      \"lon\": 4.4249355,\n      \"tags\": {\n        \"amenity\": \"bicycle_rental\",\n        \"bicycle_rental\": \"docking_station\",\n        \"brand:wikidata\": \"Q2413118\",\n        \"brand:wikipedia\": \"nl:Velo Antwerpen\",\n        \"capacity\": \"18\",\n        \"contact:phone\": \"+32 3 206 50 30\",\n        \"name\": \"Park Spoor Noord 2\",\n        \"network\": \"Velo\",\n        \"network:wikidata\": \"Q2413118\",\n        \"network:wikipedia\": \"nl:Velo Antwerpen\",\n        \"operator\": \"Velo\",\n        \"operator:website\": \"https://www.velo-antwerpen.be/\",\n        \"operator:wikidata\": \"Q2413118\",\n        \"operator:wikipedia\": \"nl:Velo Antwerpen\",\n        \"payment:app\": \"yes\",\n        \"payment:cash\": \"no\",\n        \"payment:credit_cards\": \"no\",\n        \"payment:membership_card\": \"yes\",\n        \"ref\": \"123\",\n        \"website\": \"https://www.velo-antwerpen.be/\",\n        \"wikipedia\": \"nl:Velo Antwerpen\"\n      }\n    }\n  ]\n}");
            
            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Version, Is.EqualTo(0.6));
            Assert.That(osm.Generator, Is.EqualTo("Overpass API 0.7.62.1 084b4234"));

            Assert.That(osm.Nodes.Length, Is.EqualTo(1));
        }
}