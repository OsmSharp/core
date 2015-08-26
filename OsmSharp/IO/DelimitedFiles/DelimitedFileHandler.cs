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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.DelimitedFiles
{
    /// <summary>
    /// Handles common delimited file functions.
    /// </summary>
    public static class DelimitedFileHandler
    {
        /// <summary>
        /// Reads a delimited file.
        /// </summary>
        /// <returns></returns>
        public static string[][] ReadDelimitedFile(Stream stream, DelimiterType delimiter, bool firstRowHasHeaders)
        {
            var delimiterChar = DelimitedFileHandler.GetDelimiterChar(delimiter);
            var delimitedDataSet = new List<string[]>();

            var enc = System.Text.Encoding.GetEncoding("iso-8859-1");
            var strBuild = new StringBuilder(Convert.ToInt32(stream.Length));

            for (var i = 0; i <= Convert.ToInt32(stream.Length) - 1; i++)
            {
                var bytes = new byte[] { 
                    Convert.ToByte(stream.ReadByte()) };
                strBuild.Append(enc.GetString(bytes, 0, 
                    bytes.Length));
            }

            var str = strBuild.ToString();
            var strReader = new StringReader(str);
            var lines = new List<string>();
            while ((strReader.Peek() > -1))
            {
                lines.Add(strReader.ReadLine());
            }

            var startLine = 0;
            if (firstRowHasHeaders)
            {
                startLine = 1;
            }

            //Loop while there are rows in the delimited file
            for (var l = startLine; l < lines.Count; l++)
            {
                delimitedDataSet.Add(lines[l].Split(delimiterChar));
            }
            return delimitedDataSet.ToArray<string[]>();
        }

        /// <summary>
        /// Reads a delimited file into an array of an array of strings.
        /// </summary>
        /// <returns></returns>
        public static string[][] ReadDelimitedFileFromStream(Stream stream, DelimiterType delimiter, bool ignoreHeader)
        {
            // converts the stream into a text reader.
            var tr = new StreamReader(stream);

            // get the lines.
            var strReader = new StringReader(tr.ReadToEnd());
            var lines = new List<string>();
            var isheader = ignoreHeader;
            while ((strReader.Peek() > -1))
            {
                if (isheader)
                {
                    isheader = false;
                    strReader.ReadLine();
                }
                else
                {
                    lines.Add(strReader.ReadLine());
                }
            }
            
            // get the columns.
            var values = new string[lines.Count][];
            var split = DelimitedFileHandler.GetDelimiterChar(delimiter);
            for (var idx = 0; idx < lines.Count; idx++)
            {
                values[idx] = lines[idx].Split(split);
            }
            return values;
        }

        /// <summary>
        /// Reads a delimited file into an array of an array of strings.
        /// </summary>
        /// <returns></returns>
        public static string[][] ReadDelimitedFileFromStream(Stream stream, DelimiterType delimiter)
        {
            return DelimitedFileHandler.ReadDelimitedFileFromStream(stream, delimiter, true);
        }

        /// <summary>
        /// Writes a delimited file using a default format.
        /// </summary>
        public static void WriteDelimitedFile(string[][] data, TextWriter writer, DelimiterType delimiterType)
        {
            DelimitedFileHandler.WriteDelimitedFile(data, writer, delimiterType, new DefaultDelimitedFormat());
        }

        /// <summary>
        /// Writes a delimited file using the given format.
        /// </summary>
        public static void WriteDelimitedFile(string[][] data, TextWriter writer, DelimiterType delimiterType, IDelimitedFormat format)
        {
            // get the delimiter character
            var delimiter = DelimitedFileHandler.GetDelimiterChar(delimiterType);

            // export data
            for (var row = 0; row < data.Length; row++)
            {
                var line = data[row];
                if (line != null &&
                    line.Length > 0)
                {
                    for (var col = 0; col < line.Length; col++)
                    {
                        if (format.DoExport(row))
                        {
                            var fieldData = line[col];
                            var fieldDataString = format.ConvertValue(col, fieldData);
                            writer.Write(fieldDataString);
                            // only delimiter at the end
                            if (col < line.Length - 1)
                            {
                                writer.Write(delimiter);
                            }
                        }
                    }
                }
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Returns the delimiter char for a delimiter type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static char GetDelimiterChar(DelimiterType type)
        {
            char delimiter;
            switch (type)
            {
                case DelimiterType.CommaSeperated:
                    delimiter = ',';
                    break;
                case DelimiterType.DotCommaSeperated:
                    delimiter = ';';
                    break;
                case DelimiterType.TabSeperated:
                    delimiter = (char)9;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return delimiter;
        }
    }
}