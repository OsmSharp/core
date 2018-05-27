//﻿ Copyright © 2000-2016 SharpZipLib Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using OsmSharp.IO.Zip.Streams;
using System;

namespace OsmSharp.IO.Zip
{
    /// <summary>
    /// Huffman tree used for inflation
    /// </summary>
    public class InflaterHuffmanTree
    {
        #region Constants
        const int MAX_BITLEN = 15;
        #endregion

        #region Instance Fields
        short[] tree;
        #endregion

        /// <summary>
        /// Literal length tree
        /// </summary>
        public static InflaterHuffmanTree defLitLenTree;

        /// <summary>
        /// Distance tree
        /// </summary>
        public static InflaterHuffmanTree defDistTree;

        static InflaterHuffmanTree()
        {
            try
            {
                byte[] codeLengths = new byte[288];
                int i = 0;
                while (i < 144)
                {
                    codeLengths[i++] = 8;
                }
                while (i < 256)
                {
                    codeLengths[i++] = 9;
                }
                while (i < 280)
                {
                    codeLengths[i++] = 7;
                }
                while (i < 288)
                {
                    codeLengths[i++] = 8;
                }
                defLitLenTree = new InflaterHuffmanTree(codeLengths);

                codeLengths = new byte[32];
                i = 0;
                while (i < 32)
                {
                    codeLengths[i++] = 5;
                }
                defDistTree = new InflaterHuffmanTree(codeLengths);
            }
            catch (Exception)
            {
                throw new Exception("InflaterHuffmanTree: static tree length illegal");
            }
        }

        #region Constructors
        /// <summary>
        /// Constructs a Huffman tree from the array of code lengths.
        /// </summary>
        /// <param name = "codeLengths">
        /// the array of code lengths
        /// </param>
        public InflaterHuffmanTree(byte[] codeLengths)
        {
            BuildTree(codeLengths);
        }
        #endregion

        void BuildTree(byte[] codeLengths)
        {
            int[] blCount = new int[MAX_BITLEN + 1];
            int[] nextCode = new int[MAX_BITLEN + 1];

            for (int i = 0; i < codeLengths.Length; i++)
            {
                int bits = codeLengths[i];
                if (bits > 0)
                {
                    blCount[bits]++;
                }
            }

            int code = 0;
            int treeSize = 512;
            for (int bits = 1; bits <= MAX_BITLEN; bits++)
            {
                nextCode[bits] = code;
                code += blCount[bits] << (16 - bits);
                if (bits >= 10)
                {
                    /* We need an extra table for bit lengths >= 10. */
                    int start = nextCode[bits] & 0x1ff80;
                    int end = code & 0x1ff80;
                    treeSize += (end - start) >> (16 - bits);
                }
            }

            /* -jr comment this out! doesnt work for dynamic trees and pkzip 2.04g
						if (code != 65536) 
						{
							throw new SharpZipBaseException("Code lengths don't add up properly.");
						}
			*/
            /* Now create and fill the extra tables from longest to shortest
			* bit len.  This way the sub trees will be aligned.
			*/
            tree = new short[treeSize];
            int treePtr = 512;
            for (int bits = MAX_BITLEN; bits >= 10; bits--)
            {
                int end = code & 0x1ff80;
                code -= blCount[bits] << (16 - bits);
                int start = code & 0x1ff80;
                for (int i = start; i < end; i += 1 << 7)
                {
                    tree[DeflaterHuffman.BitReverse(i)] = (short)((-treePtr << 4) | bits);
                    treePtr += 1 << (bits - 9);
                }
            }

            for (int i = 0; i < codeLengths.Length; i++)
            {
                int bits = codeLengths[i];
                if (bits == 0)
                {
                    continue;
                }
                code = nextCode[bits];
                int revcode = DeflaterHuffman.BitReverse(code);
                if (bits <= 9)
                {
                    do
                    {
                        tree[revcode] = (short)((i << 4) | bits);
                        revcode += 1 << bits;
                    } while (revcode < 512);
                }
                else
                {
                    int subTree = tree[revcode & 511];
                    int treeLen = 1 << (subTree & 15);
                    subTree = -(subTree >> 4);
                    do
                    {
                        tree[subTree | (revcode >> 9)] = (short)((i << 4) | bits);
                        revcode += 1 << bits;
                    } while (revcode < treeLen);
                }
                nextCode[bits] = code + (1 << (16 - bits));
            }

        }

        /// <summary>
        /// Reads the next symbol from input.  The symbol is encoded using the
        /// huffman tree.
        /// </summary>
        /// <param name="input">
        /// input the input source.
        /// </param>
        /// <returns>
        /// the next symbol, or -1 if not enough input is available.
        /// </returns>
        public int GetSymbol(StreamManipulator input)
        {
            int lookahead, symbol;
            if ((lookahead = input.PeekBits(9)) >= 0)
            {
                if ((symbol = tree[lookahead]) >= 0)
                {
                    input.DropBits(symbol & 15);
                    return symbol >> 4;
                }
                int subtree = -(symbol >> 4);
                int bitlen = symbol & 15;
                if ((lookahead = input.PeekBits(bitlen)) >= 0)
                {
                    symbol = tree[subtree | (lookahead >> 9)];
                    input.DropBits(symbol & 15);
                    return symbol >> 4;
                }
                else
                {
                    int bits = input.AvailableBits;
                    lookahead = input.PeekBits(bits);
                    symbol = tree[subtree | (lookahead >> 9)];
                    if ((symbol & 15) <= bits)
                    {
                        input.DropBits(symbol & 15);
                        return symbol >> 4;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else
            {
                int bits = input.AvailableBits;
                lookahead = input.PeekBits(bits);
                symbol = tree[lookahead];
                if (symbol >= 0 && (symbol & 15) <= bits)
                {
                    input.DropBits(symbol & 15);
                    return symbol >> 4;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
