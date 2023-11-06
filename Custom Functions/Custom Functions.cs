using Flow.Plugin.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow
{
    [FunctionLibraryDescriptorAttribute("Vendor-Utilities", "This library contains custom functions")]
    public class Utilities
    {
        [Description("Returns the Compressed String from the Uncompressed string.")]
        public static string CompressString(string text)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            var memoryStream = new System.IO.MemoryStream();
            using (var gZipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            System.Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return System.Convert.ToBase64String(gZipBuffer);
        }


        [Description("Returns the Decompressed String from the compressed binary string.")]
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = System.Convert.FromBase64String(compressedText);
            using (var memoryStream = new System.IO.MemoryStream())
            {
                int dataLength = System.BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return System.Text.Encoding.UTF8.GetString(buffer);
            }
        }
    }
}