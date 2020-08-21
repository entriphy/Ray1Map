﻿using System;
using System.IO;
using System.IO.Compression;

namespace R1Engine {
    /// <summary>
    /// Compresses/decompresses data using Zlib
    /// </summary>
    public class ZlibEncoder : IStreamEncoder
    {
        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            var memStream = new MemoryStream();

            using (var zlibStream = new DeflateStream(s, CompressionMode.Decompress))
                zlibStream.CopyTo(memStream);

            // Set the position to the beginning
            memStream.Position = 0;

            // Return the compressed data stream
            return memStream;
        }

        public Stream EncodeStream(Stream s) {
            var memStream = new MemoryStream();

            using (DeflateStream compressionStream = new DeflateStream(memStream, CompressionMode.Compress)) {
                s.CopyTo(compressionStream);
            }
            memStream.Position = 0;
            return memStream;
        }
    }
}