// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.IO.Xml.Kml;
using OsmSharp.IO.Xml.Sources;
using OsmSharp.Math.Geo;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Geo.Streams.Kml
{
    /// <summary>
    /// KML-stream source.
    /// </summary>
    public class KmlFeatureStreamSource : FeatureCollectionStreamSource
    {
        private readonly Stream _stream;

        /// <summary>
        /// Creates a new Kml-geometry stream.
        /// </summary>
        /// <param name="stream"></param>
        public KmlFeatureStreamSource(Stream stream)
            : base(new FeatureCollection())
        {
            _stream = stream;
        }

        /// <summary>
        /// Called when initializing this source.
        /// </summary>
        public override void Initialize()
        {
            // read the kml-data.
            if (!_read) { this.DoReadKml(); }

            base.Initialize();
        }

        #region Read Kml

        /// <summary>
        /// The kml object.
        /// </summary>
        private bool _read = false;

        /// <summary>
        /// Reads the actual Kml.
        /// </summary>
        private void DoReadKml()
        {
            // seek to the beginning of the stream.
            if (_stream.CanSeek) { _stream.Seek(0, SeekOrigin.Begin); }

            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(_stream);
            KmlDocument document = new KmlDocument(source);
            object kml = document.Kml;

            switch (document.Version)
            {
                case KmlVersion.Kmlv2_1:
                    this.ConvertKml(kml as OsmSharp.IO.Xml.Kml.v2_1.KmlType);
                    break;
                case KmlVersion.Kmlv2_0_response:
                    this.ConvertKml(kml as OsmSharp.IO.Xml.Kml.v2_0_response.kml);
                    break;
                case KmlVersion.Kmlv2_0:
                    this.ConvertKml(kml as OsmSharp.IO.Xml.Kml.v2_0.kml);
                    break;
            }
        }

        /// <summary>
        /// Reads a kml v2.1 object into corresponding geometries.
        /// </summary>
        /// <param name="kmlType"></param>
        private void ConvertKml(IO.Xml.Kml.v2_1.KmlType kmlType)
        {
            this.FeatureCollection.Clear();

            this.ConvertFeature(kmlType.Item);
        }

        /// <summary>
        /// Reads a kml v2.0 response object into corresponding geometries.
        /// </summary>
        /// <param name="kml"></param>
        private void ConvertKml(IO.Xml.Kml.v2_0_response.kml kml)
        {
            this.FeatureCollection.Clear();

            if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0_response.Document)
            {
                this.ConvertDocument(kml.Item as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
            }
            else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0_response.Folder)
            {
                this.ConvertFolder(kml.Item as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
            }
            else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0_response.Placemark)
            {
                this.ConvertPlacemark(kml.Item as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
            }
            else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0_response.Response)
            {
                this.ConvertResponse(kml.Item as OsmSharp.IO.Xml.Kml.v2_0_response.Response);
            }
        }

        /// <summary>
        /// Reads a kml v2.0 object into corresponding geometries.
        /// </summary>
        /// <param name="kml"></param>
        private void ConvertKml(OsmSharp.IO.Xml.Kml.v2_0.kml kml)
        {
            this.FeatureCollection.Clear();

            if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0.Document)
            {
                this.ConvertDocument(kml.Item as OsmSharp.IO.Xml.Kml.v2_0.Document);
            }
            else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0.Folder)
            {
                this.ConvertFolder(kml.Item as OsmSharp.IO.Xml.Kml.v2_0.Folder);
            }
            else if (kml.Item is OsmSharp.IO.Xml.Kml.v2_0.Placemark)
            {
                this.ConvertPlacemark(kml.Item as OsmSharp.IO.Xml.Kml.v2_0.Placemark);
            }
        }

        #endregion

        #region v2.0

        /// <summary>
        /// Converts a placemark into an osm object.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private void ConvertPlacemark(OsmSharp.IO.Xml.Kml.v2_0.Placemark placemark)
        {
            for (int idx = 0; idx < placemark.Items.Length; idx++)
            {
                switch (placemark.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.LineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertLineString(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.LineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiGeometry:
                        this.ConvertMultiGeometry(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiGeometry);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiLineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiLineString(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiLineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiPoint:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPoint(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiPoint));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.MultiPolygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPolygon(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiPolygon));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.Point:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPoint(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Point));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType1.Polygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPolygon(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static Feature ConvertPolygon(OsmSharp.IO.Xml.Kml.v2_0.Polygon polygon)
        {
            var inner = KmlFeatureStreamSource.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
            var outer = KmlFeatureStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);

            return new Feature(new Polygon(outer.Geometry as LineairRing, new LineairRing[] { inner.Geometry as LineairRing }));
        }

        /// <summary>
        /// Converts a lineairring into an osm object.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private static Feature ConvertLinearRing(OsmSharp.IO.Xml.Kml.v2_0.LinearRing linearRing)
        {
            // convert the coordinates.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(linearRing.coordinates);

            // create the ring.
            var feature = new Feature(new LineairRing(coordinates));
            feature.Attributes.Add("id", linearRing.id);

            return feature;
        }

        /// <summary>
        /// Converts a point into an osm object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Feature ConvertPoint(OsmSharp.IO.Xml.Kml.v2_0.Point point)
        {
            // convert the coordiantes.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(point.coordinates);

            // create the point.
            var feature = new Feature(new Point(coordinates[0]));

            if (point.altitudeModeSpecified) { feature.Attributes.Add("altitude", point.altitudeMode); }
            if (point.extrudeSpecified) { feature.Attributes.Add("extrude", point.extrude); }
            if (point.id != null) { feature.Attributes.Add("id", point.id); }

            return feature;
        }

        /// <summary>
        /// Converts a multipolygon into osm objects.
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <returns></returns>
        private static Feature ConvertMultiPolygon(OsmSharp.IO.Xml.Kml.v2_0.MultiPolygon multiPolygon)
        {
            return new Feature(new MultiPolygon(new Polygon[] { KmlFeatureStreamSource.ConvertPolygon(multiPolygon.Polygon).Geometry as Polygon }));
        }

        /// <summary>
        /// Converts a multipoint to osm objects.
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <returns></returns>
        private static Feature ConvertMultiPoint(OsmSharp.IO.Xml.Kml.v2_0.MultiPoint multiPoint)
        {
            return new Feature(new MultiPoint(new Point[] { KmlFeatureStreamSource.ConvertPoint(multiPoint.Point).Geometry as Point }));
        }

        /// <summary>
        /// Converts a multilinestring to osm objects.
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <returns></returns>
        private static Feature ConvertMultiLineString(OsmSharp.IO.Xml.Kml.v2_0.MultiLineString multiLineString)
        {
            return new Feature(new MultiLineString(new LineString[] { KmlFeatureStreamSource.ConvertLineString(multiLineString.LineString).Geometry as LineString }));
        }

        /// <summary>
        /// Converts a multigeometry to osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private void ConvertMultiGeometry(OsmSharp.IO.Xml.Kml.v2_0.MultiGeometry multiGeometry)
        {
            for (int idx = 0; idx < multiGeometry.Items.Length; idx++)
            {
                switch (multiGeometry.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.LineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertLineString(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.LineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiGeometry:
                        this.ConvertMultiGeometry(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiGeometry);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiLineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiLineString(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiLineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiPoint:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPoint(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiPoint));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.MultiPolygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPolygon(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.MultiPolygon));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.Point:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPoint(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Point));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType.Polygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPolygon(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a linestring to osm objects.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private static Feature ConvertLineString(OsmSharp.IO.Xml.Kml.v2_0.LineString lineString)
        {
            // convert the coordinates.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(lineString.coordinates);

            // create the ring.
            var feature = new Feature(new LineString(coordinates));
            feature.Attributes.Add("id", lineString.id);

            return feature;
        }

        /// <summary>
        /// Converts a folder into an osm object.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private void ConvertFolder(OsmSharp.IO.Xml.Kml.v2_0.Folder folder)
        {
            for (int idx = 0; idx < folder.Items.Length; idx++)
            {
                switch (folder.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType2.Document:
                        this.ConvertDocument(folder.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Document);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType2.Folder:
                        this.ConvertFolder(folder.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Folder);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType2.Placemark:
                        this.ConvertPlacemark(folder.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Placemark);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a document into osm elements.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private void ConvertDocument(OsmSharp.IO.Xml.Kml.v2_0.Document document)
        {
            for (int idx = 0; idx < document.Items.Length; idx++)
            {
                switch (document.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType3.Document:
                        this.ConvertDocument(document.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Document);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType3.Folder:
                        this.ConvertFolder(document.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Folder);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0.ItemsChoiceType3.Placemark:
                        this.ConvertPlacemark(document.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0.Placemark);
                        break;
                }
            }
        }

        #endregion

        #region v2.0.response


        /// <summary>
        /// Converts a response into an osm object.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private void ConvertResponse(OsmSharp.IO.Xml.Kml.v2_0_response.Response response)
        {
            foreach (object item in response.Items)
            {
                if (item is OsmSharp.IO.Xml.Kml.v2_0_response.Document)
                {
                    this.ConvertDocument(item as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
                }
                else if (item is OsmSharp.IO.Xml.Kml.v2_0_response.Folder)
                {
                    this.ConvertFolder(item as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
                }
                else if (item is OsmSharp.IO.Xml.Kml.v2_0_response.Placemark)
                {
                    this.ConvertPlacemark(item as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
                }
                else if (item is OsmSharp.IO.Xml.Kml.v2_0_response.Response)
                {
                    this.ConvertResponse(item as OsmSharp.IO.Xml.Kml.v2_0_response.Response);
                }
            }
        }

        /// <summary>
        /// Converts a placemark into an osm object.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private void ConvertPlacemark(OsmSharp.IO.Xml.Kml.v2_0_response.Placemark placemark)
        {
            for (int idx = 0; idx < placemark.Items.Length; idx++)
            {
                switch (placemark.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.LineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertLineString(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.LineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiGeometry:
                        this.ConvertMultiGeometry(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiGeometry);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiLineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiLineString(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiLineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPoint:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPoint(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPoint));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPolygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPolygon(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPolygon));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.Point:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPoint(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Point));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType1.Polygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPolygon(placemark.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static Feature ConvertPolygon(OsmSharp.IO.Xml.Kml.v2_0_response.Polygon polygon)
        {
            var inner = KmlFeatureStreamSource.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
            var outer = KmlFeatureStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);

            return new Feature(new Polygon(outer.Geometry as LineairRing, new LineairRing[] { inner.Geometry as LineairRing }));
        }

        /// <summary>
        /// Converts a lineairring into an osm object.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private static Feature ConvertLinearRing(OsmSharp.IO.Xml.Kml.v2_0_response.LinearRing linearRing)
        {
            // convert the coordinates.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(linearRing.coordinates);

            // create the ring.
            var feature = new Feature(new LineairRing(coordinates));
            feature.Attributes.Add("id", linearRing.id);

            return feature;
        }

        /// <summary>
        /// Converts a point into an osm object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Feature ConvertPoint(OsmSharp.IO.Xml.Kml.v2_0_response.Point point)
        {
            // convert the coordiantes.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(point.coordinates);

            // create the point.
            var feature = new Feature(new Point(coordinates[0]));
            if (point.altitudeModeSpecified) { feature.Attributes.Add("altitude", point.altitudeMode); }
            if (point.extrudeSpecified) { feature.Attributes.Add("extrude", point.extrude); }
            if (point.id != null) { feature.Attributes.Add("id", point.id); }

            return feature;
        }

        /// <summary>
        /// Converts a multipolygon into osm objects.
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <returns></returns>
        private static Feature ConvertMultiPolygon(OsmSharp.IO.Xml.Kml.v2_0_response.MultiPolygon multiPolygon)
        {
            return new Feature(new MultiPolygon(new Polygon[] { KmlFeatureStreamSource.ConvertPolygon(multiPolygon.Polygon).Geometry as Polygon }));
        }

        /// <summary>
        /// Converts a multipoint to osm objects.
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <returns></returns>
        private static Feature ConvertMultiPoint(OsmSharp.IO.Xml.Kml.v2_0_response.MultiPoint multiPoint)
        {
            return new Feature(new MultiPoint(new Point[] { KmlFeatureStreamSource.ConvertPoint(multiPoint.Point).Geometry as Point }));
        }

        /// <summary>
        /// Converts a multilinestring to osm objects.
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <returns></returns>
        private static Feature ConvertMultiLineString(OsmSharp.IO.Xml.Kml.v2_0_response.MultiLineString multiLineString)
        {
            return new Feature(new MultiLineString(new LineString[] { KmlFeatureStreamSource.ConvertLineString(multiLineString.LineString).Geometry as LineString }));
        }

        /// <summary>
        /// Converts a multigeometry to osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private void ConvertMultiGeometry(OsmSharp.IO.Xml.Kml.v2_0_response.MultiGeometry multiGeometry)
        {
            for (int idx = 0; idx < multiGeometry.Items.Length; idx++)
            {
                switch (multiGeometry.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.LineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertLineString(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.LineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiGeometry:
                        this.ConvertMultiGeometry(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiGeometry);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiLineString:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiLineString(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiLineString));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPoint:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPoint(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPoint));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPolygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertMultiPolygon(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.MultiPolygon));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.Point:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPoint(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Point));
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType.Polygon:
                        this.FeatureCollection.Add(
                            KmlFeatureStreamSource.ConvertPolygon(multiGeometry.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a linestring to osm objects.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private static Feature ConvertLineString(OsmSharp.IO.Xml.Kml.v2_0_response.LineString lineString)
        {
            // convert the coordinates.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(lineString.coordinates);

            // create the ring.
            var feature = new Feature(new LineString(coordinates));
            feature.Attributes.Add("id", lineString.id);

            return feature;
        }

        /// <summary>
        /// Converts a folder into an osm object.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private void ConvertFolder(OsmSharp.IO.Xml.Kml.v2_0_response.Folder folder)
        {
            for (int idx = 0; idx < folder.Items.Length; idx++)
            {
                switch (folder.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType2.Document:
                        this.ConvertDocument(folder.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType2.Folder:
                        this.ConvertFolder(folder.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType2.Placemark:
                        this.ConvertPlacemark(folder.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a document into osm elements.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private void ConvertDocument(OsmSharp.IO.Xml.Kml.v2_0_response.Document document)
        {
            for (int idx = 0; idx < document.Items.Length; idx++)
            {
                switch (document.ItemsElementName[idx])
                {
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType3.Document:
                        this.ConvertDocument(document.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Document);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType3.Folder:
                        this.ConvertFolder(document.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Folder);
                        break;
                    case OsmSharp.IO.Xml.Kml.v2_0_response.ItemsChoiceType3.Placemark:
                        this.ConvertPlacemark(document.Items[idx] as OsmSharp.IO.Xml.Kml.v2_0_response.Placemark);
                        break;
                }
            }
        }

        #endregion

        #region v2.1

        /// <summary>
        /// Converts all the features into osm elements.
        /// </summary>
        /// <param name="featureType"></param>
        private void ConvertFeatures(OsmSharp.IO.Xml.Kml.v2_1.FeatureType[] featureType)
        {
            foreach (OsmSharp.IO.Xml.Kml.v2_1.FeatureType feature in featureType)
            {
                this.ConvertFeature(feature);
            }
        }

        /// <summary>
        /// Converts a feature and all it's contents to osm elements.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private void ConvertFeature(OsmSharp.IO.Xml.Kml.v2_1.FeatureType feature)
        {
            if (feature is OsmSharp.IO.Xml.Kml.v2_1.ContainerType)
            {
                this.ConvertContainer(feature as OsmSharp.IO.Xml.Kml.v2_1.ContainerType);
            }
            else if (feature is OsmSharp.IO.Xml.Kml.v2_1.PlacemarkType)
            {
                this.ConvertPlacemark(feature as OsmSharp.IO.Xml.Kml.v2_1.PlacemarkType);
            }
        }

        /// <summary>
        /// Conversts a placemark and all it's contents to osm elements.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private void ConvertPlacemark(OsmSharp.IO.Xml.Kml.v2_1.PlacemarkType placemark)
        {
            this.ConvertGeometry(placemark.Item1);
        }

        /// <summary>
        /// Converts a geometry to a list of osm objects.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private void ConvertGeometry(OsmSharp.IO.Xml.Kml.v2_1.GeometryType geometry)
        {
            if (geometry is OsmSharp.IO.Xml.Kml.v2_1.PointType)
            {
                this.FeatureCollection.Add(
                    KmlFeatureStreamSource.ConvertPoint(geometry as OsmSharp.IO.Xml.Kml.v2_1.PointType));
            }
            else if (geometry is OsmSharp.IO.Xml.Kml.v2_1.LineStringType)
            {
                this.FeatureCollection.Add(
                    KmlFeatureStreamSource.ConvertLineString(geometry as OsmSharp.IO.Xml.Kml.v2_1.LineStringType));
            }
            else if (geometry is OsmSharp.IO.Xml.Kml.v2_1.LinearRingType)
            {
                this.FeatureCollection.Add(
                    KmlFeatureStreamSource.ConvertLinearRing(geometry as OsmSharp.IO.Xml.Kml.v2_1.LinearRingType));
            }
            else if (geometry is OsmSharp.IO.Xml.Kml.v2_1.PolygonType)
            {
                this.FeatureCollection.Add(
                    KmlFeatureStreamSource.ConvertPolygon(geometry as OsmSharp.IO.Xml.Kml.v2_1.PolygonType));
            }
            else if (geometry is OsmSharp.IO.Xml.Kml.v2_1.MultiGeometryType)
            {
                this.ConvertMultiGeometry(geometry as OsmSharp.IO.Xml.Kml.v2_1.MultiGeometryType);
            }
        }

        /// <summary>
        /// Converts the multi geometry to multi osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private void ConvertMultiGeometry(OsmSharp.IO.Xml.Kml.v2_1.MultiGeometryType multiGeometry)
        {
            foreach (OsmSharp.IO.Xml.Kml.v2_1.GeometryType geo in multiGeometry.Items)
            {
                this.ConvertGeometry(geo);
            }
        }

        /// <summary>
        /// Convests the polygon to osm objects.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static Feature ConvertPolygon(OsmSharp.IO.Xml.Kml.v2_1.PolygonType polygon)
        {
            IEnumerable<LineairRing> inners = KmlFeatureStreamSource.ConvertBoundary(polygon.innerBoundaryIs);
            LineairRing outer = KmlFeatureStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing).Geometry as LineairRing;

            return new Feature(new Polygon(outer, inners));
        }

        /// <summary>
        /// Converts boundary type into an osm object.
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns></returns>
        private static IEnumerable<LineairRing> ConvertBoundary(OsmSharp.IO.Xml.Kml.v2_1.boundaryType[] boundary)
        {
            List<LineairRing> rings = new List<LineairRing>();
            foreach (OsmSharp.IO.Xml.Kml.v2_1.boundaryType geo in boundary)
            {
                rings.Add(KmlFeatureStreamSource.ConvertLinearRing(geo.LinearRing).Geometry as LineairRing);
            }
            return rings;
        }

        /// <summary>
        /// Converts a lineairring into osm objects.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private static Feature ConvertLinearRing(OsmSharp.IO.Xml.Kml.v2_1.LinearRingType linearRing)
        {
            // convert the coordinates.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(linearRing.coordinates);

            // create the ring.
            var feature = new Feature(new LineairRing(coordinates));
            feature.Attributes.Add("id", linearRing.id);

            return feature;
        }

        /// <summary>
        /// Converts a line string into an osm object.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private static Feature ConvertLineString(OsmSharp.IO.Xml.Kml.v2_1.LineStringType lineString)
        {
            // convert the coordinates.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(lineString.coordinates);

            // create the ring.
            var feature = new Feature(new LineString(coordinates));
            feature.Attributes.Add("id", lineString.id);

            return feature;
        }

        /// <summary>
        /// Converts a point into an oms object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Feature ConvertPoint(OsmSharp.IO.Xml.Kml.v2_1.PointType point)
        {
            // convert the coordiantes.
            var coordinates = KmlFeatureStreamSource.ConvertCoordinates(point.coordinates);

            // create the point.
            var feature = new Feature(new Point(coordinates[0]));
            if (point.targetId != null) { feature.Attributes.Add("targetId", point.targetId); }
            feature.Attributes.Add("altitude", point.altitudeMode);
            if (point.extrude) { feature.Attributes.Add("extrude", point.extrude); }
            if (point.id != null) { feature.Attributes.Add("id", point.id); }

            return feature;
        }

        /// <summary>
        /// Converts a container and it's contents to osm elements.
        /// </summary>
        /// <param name="container"></param>
        private void ConvertContainer(OsmSharp.IO.Xml.Kml.v2_1.ContainerType container)
        {
            // get the features.
            if (container is OsmSharp.IO.Xml.Kml.v2_1.FolderType)
            {
                OsmSharp.IO.Xml.Kml.v2_1.FolderType folder = (container as OsmSharp.IO.Xml.Kml.v2_1.FolderType);

                // items1 are the features. 
                this.ConvertFeatures(folder.Items1);
            }
            else if (container is OsmSharp.IO.Xml.Kml.v2_1.DocumentType)
            {
                OsmSharp.IO.Xml.Kml.v2_1.DocumentType document = (container as OsmSharp.IO.Xml.Kml.v2_1.DocumentType);

                // items1 are the features.
                this.ConvertFeatures(document.Items1);
            }
        }

        #endregion

        /// <summary>
        /// Converts a list of coordinates to geocoordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private static IList<GeoCoordinate> ConvertCoordinates(string coordinates)
        {
            var geoCoordinates = new List<GeoCoordinate>();
            var coordinateStrings = coordinates.Split('\n');
            for (int idx = 0; idx < coordinateStrings.Length; idx++)
            {
                var coordinateString = coordinateStrings[idx];
                if (coordinateString != null &&
                    coordinateString.Length > 0 &&
                    coordinateString.Trim().Length > 0)
                {
                    var coordinate_split = coordinateString.Split(',');
                    var longitude = 0.0;
                    if (!double.TryParse(coordinate_split[0],
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out longitude))
                    {
                        // parsing failed!
                    }
                    var latitude = 0.0;
                    if (!double.TryParse(coordinate_split[1],
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out latitude))
                    {
                        // parsing failed!
                    }

                    geoCoordinates.Add(new GeoCoordinate(latitude, longitude));
                }
            }
            return geoCoordinates;
        }
    }
}