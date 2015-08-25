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
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp
{
    /// <summary>
    /// Holds extension methods that compensate for missing API calls when using PCL's.
    /// 
    /// PCL changes:
    /// http://msdn.microsoft.com/en-us/library/gg597392%28v=vs.110%29.aspx
    /// </summary>
    public static class PCLExtensions
    {
        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array into a string.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetString(this Encoding encoding, byte[] bytes)
        {
            return encoding.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Gets an AssemblyName for this assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static AssemblyName GetName(this Assembly assembly)
        {
            return new AssemblyName(assembly.FullName);
        }

        /// <summary>
        /// Performs the specified action on each element of the List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this List<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Gets a Stream object to use to write request data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Stream GetRequestStream(this WebRequest request)
        {
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                request.BeginGetRequestStream(iar =>
                {
                    try
                    {
                        var response = request.EndGetRequestStream(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }
            return tcs.Task.Result;
        }

        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static WebResponse GetResponse(this WebRequest request)
        {
            var tcs = new TaskCompletionSource<HttpWebResponse>();

            try
            {
                request.BeginGetResponse(iar =>
                 {
                     try
                     {
                         var response = (HttpWebResponse)request.EndGetResponse(iar);
                         tcs.SetResult(response);
                     }
                     catch (Exception exc)
                     {
                         tcs.SetException(exc);
                     }
                 }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }
            try
            {
                return tcs.Task.Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is WebException)
                { // re-throw the webexception.
                    throw ex.InnerException;
                }
                throw ex;
            }
        }
    }
}