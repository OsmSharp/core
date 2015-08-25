using System;
using System.Net;

namespace OsmSharp
{
    /// <summary>
    /// Silverlight doesn't have an ASCII encoder, so here is one!
    /// 
    /// http://stackoverflow.com/questions/4022281/asciiencoding-in-windows-phone-7
    /// </summary>
    public class ASCIIEncoding : System.Text.Encoding
    {
        /// <summary>
        /// Returns an instance of the ASCIIEncoding class.
        /// </summary>
        public static ASCIIEncoding ASCII
        {
            get
            {
                return new ASCIIEncoding();
            }
        }

        /// <summary>
        /// Calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        /// <summary>
        /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding a set of characters from the specified character array.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string GetString(byte[] bytes)
        {
            return this.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public override int GetCharCount(byte[] bytes)
        {
            return bytes.Length;
        }

        /// <summary>
        /// Encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="charIndex"></param>
        /// <param name="charCount"></param>
        /// <param name="bytes"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                bytes[byteIndex + i] = (byte)chars[charIndex + i];
            }
            return charCount;
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding all the bytes in the specified byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// Cecodes all the bytes in the specified byte array into a set of characters.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="byteIndex"></param>
        /// <param name="byteCount"></param>
        /// <param name="chars"></param>
        /// <param name="charIndex"></param>
        /// <returns></returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                chars[charIndex + i] = (char)bytes[byteIndex + i];
            }
            return byteCount;
        }
    }
}
