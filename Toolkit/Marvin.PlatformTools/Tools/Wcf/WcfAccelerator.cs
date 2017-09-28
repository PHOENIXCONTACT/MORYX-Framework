using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// This class converts ID collections into fast transmittable strings
    /// Structure: 8    Char entry count n
    ///            2    Char entry length calculated from greatest value
    ///            16   Char offset for smallest value         
    ///            n*l  Char id collection
    /// </summary>
    public static class WcfAccelerator
    {
        private const int HeaderSize = 26;

        /// <summary>
        /// Converts a collection of Int64 ids to a compressed string
        /// </summary>
        /// <param name="ids">Id collection</param>
        /// <returns>Compressed string</returns>
        public static string ConvertToString(ICollection<long> ids)
        {
            // Quick check for empty collection
            if (ids == null || !ids.Any())
                return string.Empty;

            // Determine offset and biggest value minus offset
            long offset = long.MaxValue, max = 0;
            foreach (var id in ids)
            {
                if (id > max)
                    max = id;
                if (id < offset)
                    offset = id;
            }
            var entryLength = (max - offset).ToString("X").Length;

            // Create header from entry and collection length
            var builder = new StringBuilder();
            builder.Append(ids.Count.ToString("x8"));
            builder.Append(entryLength.ToString("x2"));
            builder.Append(offset.ToString("x16"));

            // Append all IDs
            var formatString = string.Format("x{0}", entryLength);
            foreach (var id in ids)
            {
                builder.Append((id - offset).ToString(formatString));
            }

            // Build
            return builder.ToString();
        }

        /// <summary>
        /// Decompresses the id string to a collection
        /// </summary>
        /// <param name="compressedCollection">Compressed collection string</param>
        /// <exception cref="FormatException">Exception if string format or size does not match</exception>
        /// <returns>Collection of ids</returns>
        public static ICollection<long> ConvertToIds(string compressedCollection)
        {
            // Validate against empty
            if (string.IsNullOrEmpty(compressedCollection))
                return new long[0];

            // Validate header existence
            if (compressedCollection.Length < HeaderSize)
                throw new FormatException("Header missing or incomplete!");

            // Parse header and create array
            var length = int.Parse(compressedCollection.Substring(0, 8), NumberStyles.HexNumber);
            var entryLength = int.Parse(compressedCollection.Substring(8, 2), NumberStyles.HexNumber);
            var offset = long.Parse(compressedCollection.Substring(10, 16), NumberStyles.HexNumber);
            var ids = new long[length];

            // Confirm length
            if (compressedCollection.Length != length * entryLength + HeaderSize)
                throw new FormatException("String size does not match header collection size!");

            var index = HeaderSize;
            for (var i = 0; i < length; i++)
            {
                ids[i] = long.Parse(compressedCollection.Substring(index, entryLength), NumberStyles.HexNumber) + offset;
                index += entryLength;
            }

            return ids;
        }

        /// <summary>
        /// Enumerate lazy over smaller chunks to provide fluent loading
        /// </summary>
        /// <param name="compressedCollection">Compressed id string received over wcf</param>
        /// <param name="minChunkSize">Minimal size of a chunk to avoid overhead</param>
        /// <param name="maxChunkCount">Maximal number of chunks for fluent loading</param>
        /// <returns>Compressed id chunks to transmit over wcf</returns>
        public static ICollection<string> EnumerateChunks(string compressedCollection, int minChunkSize, int maxChunkCount)
        {
            // Decompress collection
            var ids = ConvertToIds(compressedCollection);
            return ids.Count <= minChunkSize
                ? new[] { compressedCollection }
                : CalculateChunks(ids, minChunkSize, maxChunkCount);
        }

        /// <summary>
        /// Enumerate lazy over smaller chunks to provide fluent loading
        /// </summary>
        /// <param name="collection">Compressed id string received over wcf</param>
        /// <param name="minChunkSize">Minimal size of a chunk to avoid overhead</param>
        /// <param name="maxChunkCount">Maximal number of chunks for fluent loading</param>
        /// <returns>Compressed id chunks to transmit over wcf</returns>
        public static ICollection<string> EnumerateChunks(ICollection<long> collection, int minChunkSize, int maxChunkCount)
        {
            // Determine appropriate chunk size
            return collection.Count <= minChunkSize
                ? new[] { ConvertToString(collection) }
                : CalculateChunks(collection, minChunkSize, maxChunkCount);
        }

        private static ICollection<string> CalculateChunks(ICollection<long> collection, int minChunkSize, int maxChunkCount)
        {
            var totalAmount = collection.Count;
            var sizeForMaxCount = Math.Ceiling(totalAmount / (double)maxChunkCount);

            // Determine ideal size
            var chunkSize = (int)Math.Ceiling(minChunkSize + sizeForMaxCount);

            // Create chunks
            var chunks = new List<string>();
            var temp = new List<long>();

            int count = 0, rem = totalAmount;
            foreach (var id in collection)
            {
                // Add to temp list and reduce total
                temp.Add(id);
                rem--;

                // Keep going if we are smaller than chunk or additional chunk is pointless
                if (count <= chunkSize || rem < chunkSize / 2)
                {
                    count++;
                    continue;
                }
                chunks.Add(ConvertToString(temp));
                temp.Clear();
                count = 0;
            }
            // Add remaining
            chunks.Add(ConvertToString(temp));

            return chunks;
        }
    }
}