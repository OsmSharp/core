// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using System.Linq;
using System.Text;
using System.IO;
using OsmSharp.Progress;

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
        /// <param name="reporter"></param>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        /// <param name="firstRowHasHeaders"></param>
        /// <returns></returns>
        public static string[][] ReadDelimitedFile(
            IProgressReporter reporter,
            Stream stream,
            DelimiterType delimiter,
            bool firstRowHasHeaders)
        {
            if (reporter == null)
            {
                reporter = EmptyProgressReporter.Instance;
            }

            char delimiterChar = DelimitedFileHandler.GetDelimiterChar(delimiter);

            List<string[]> delimited_data_set;
            //int iCounter = 0;
            ProgressStatus status;

            // Build dataset
            delimited_data_set = new List<string[]>();
            //Add the table               'Read the delimited file

            System.Text.Encoding enc = null;
            enc = System.Text.Encoding.GetEncoding("iso-8859-1");
            StringBuilder strBuild = new StringBuilder(Convert.ToInt32(stream.Length));

            // report the status.
            status = new ProgressStatus();
            status.Status = ProgressStatus.ProgressStatusEnum.Busy;
            status.CurrentNumber = 0;
            status.Message = "Opening file...";

            reporter.Report(status);

            for (int i = 0; i <= Convert.ToInt32(stream.Length) - 1; i++)
            {
                byte[] bytes = new byte[] { Convert.ToByte(stream.ReadByte()) };
                strBuild.Append(enc.GetString(bytes, 0, bytes.Length));
            }

            string str = strBuild.ToString();
            StringReader strReader = new StringReader(str);
            List<string> lines = new List<string>();
            while ((strReader.Peek() > -1))
            {
                lines.Add(strReader.ReadLine());
            }

            // Now add in the Rows
            // report the status.
            status = new ProgressStatus();
            status.Status = ProgressStatus.ProgressStatusEnum.Busy;
            status.CurrentNumber = 0;
            status.Message = "Reading file...";
            reporter.Report(status);

            int startLine = 0;
            if (firstRowHasHeaders)
            {
                startLine = 1;
            }

            //Loop while there are rows in the delimited file
            for (int l = startLine; l < lines.Count; l++)
            {
                string line = lines[l];

                //Add the items to the DataSet
                delimited_data_set.Add(line.Split(delimiterChar));

                // report the status.
                status = new ProgressStatus();
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
                status.CurrentNumber = l;
                status.TotalNumber = lines.Count - 1;
                status.Message = "Reading file...";
                reporter.Report(status);
            }

            // report the status.
            status = new ProgressStatus();
            status.Status = ProgressStatus.ProgressStatusEnum.Succeeded;
            status.CurrentNumber = 0;
            status.Message = "Reading file...";
            reporter.Report(status);

            return delimited_data_set.ToArray<string[]>();
        }

        #region Read Delimited Files

        /// <summary>
        /// Reads a delimited file into an array of an array of strings.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        /// <param name="ignoreHeader"></param>
        /// <returns></returns>
        public static string[][] ReadDelimitedFileFromStream(
            Stream stream,
            DelimiterType delimiter,
            bool ignoreHeader)
        {
            // converts the stream into a text reader.
            TextReader tr = new StreamReader(stream);

            // get the lines.
            StringReader strReader = new StringReader(tr.ReadToEnd());
            List<string> lines = new List<string>();
            bool isheader = ignoreHeader;
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
            string[][] values = new string[lines.Count][];
            char split = DelimitedFileHandler.GetDelimiterChar(delimiter);
            for (int idx = 0; idx < lines.Count; idx++)
            {
                values[idx] = lines[idx].Split(split);
            }
            return values;
        }

        /// <summary>
        /// Reads a delimited file into an array of an array of strings.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[][] ReadDelimitedFileFromStream(
            Stream stream,
            DelimiterType delimiter)
        {
            return DelimitedFileHandler.ReadDelimitedFileFromStream(stream, delimiter, true);
        }

        #endregion

        /// <summary>
        /// Writes a delimited file using the given format.
        /// </summary>
        /// <param name="reporter"></param>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        /// <param name="delimiter_type"></param>
        /// <param name="format"></param>
        public static void WriteDelimitedFile(
            IProgressReporter reporter,
            string[][] data,
            TextWriter writer,
            DelimiterType delimiter_type,
            IDelimitedFormat format)
        {
            // get the delimiter character
            char delimiter = GetDelimiterChar(delimiter_type);

            // initialize the progress status.
            ProgressStatus status = new ProgressStatus();
            if (reporter != null)
            {
                status.TotalNumber = data.Length;
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
                status.CurrentNumber = 0;
                status.Message = "Creating File...";

                // report the status.
                reporter.Report(status);
            }

            // export data
            if (reporter != null)
            {
                status.Message = "Exporting... {progress}!";
            }
            for (int idx = 0; idx < data.Length; idx++)
            {
                string[] line = data[idx];
                if (line != null &&
                    line.Length > 0)
                {
                    for (int col_idx = 0; col_idx < line.Length; col_idx++)
                    {
                        if (format.DoExport(idx))
                        {
                            object field_data = line[col_idx];
                            string field_data_string = format.ConvertValue(col_idx, field_data);
                            writer.Write(field_data_string);
                            // only delimiter at the end
                            if (col_idx < line.Length - 1)
                            {
                                writer.Write(delimiter);
                            }
                        }
                    }
                }
                if (reporter != null)
                {
                    status.CurrentNumber = idx + 1;
                    reporter.Report(status);
                }
                writer.WriteLine();
            }

            // report done
            if (reporter != null)
            {
                status.Message = "Exporting Done!";
                status.Status = ProgressStatus.ProgressStatusEnum.Succeeded;
                reporter.Report(status);
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

        /// <summary>
        /// Writes a delimited file using a default format.
        /// </summary>
        /// <param name="reporter"></param>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        /// <param name="delimiter_type"></param>
        public static void WriteDelimitedFile(
            IProgressReporter reporter,
            string[][] data,
            TextWriter writer,
            DelimiterType delimiter_type)
        {
            WriteDelimitedFile(reporter, data, writer, delimiter_type, new DefaultDelimitedFormat());
        }
    }
}