using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using ComponentAce.Compression.Libs.zlib;

namespace TiledMax
{
    /// <summary>
    /// Special thanks and credits goto seeseekey on #tiled for this.
    /// </summary>
    public static class GZip
    {
        public static void Compress(Stream sourceStream, Stream destinationStream)
        {
            using (GZipStream gzipStream = new GZipStream(destinationStream, CompressionMode.Compress))
            {
                CopyData(sourceStream, gzipStream);
            }
        }

        public static byte[] Compress(byte[] data)
        {
            byte[] res = null;

            using (MemoryStream sourceStream = new MemoryStream(data, false))
            {
                using (MemoryStream destinationStream = new MemoryStream())
                {
                    Compress(sourceStream, destinationStream);
                    res = destinationStream.ToArray();
                }
            }

            return res;
        }

        public static void Decompress(System.IO.Stream sourceStream, System.IO.Stream destinationStream)
        {
            using (GZipStream gzipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                CopyData(gzipStream, destinationStream);
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            byte[] res = null;

            using (System.IO.MemoryStream sourceStream = new System.IO.MemoryStream(data, false))
            {
                using (System.IO.MemoryStream destinationStream = new System.IO.MemoryStream())
                {
                    Decompress(sourceStream, destinationStream);
                    res = destinationStream.ToArray();
                }
            }

            return res;
        }

        private static void CopyData(Stream sourceStream, Stream destinationStream)
        {
            byte[] buffer = new byte[4096];
            Int32 bytesRead = 0;
            do
            {
                bytesRead = sourceStream.Read(buffer, 0, buffer.Length);
                if (bytesRead != 0)
                {
                    destinationStream.Write(buffer, 0, bytesRead);
                }
            }
            while (bytesRead != 0);
        }

        public static void CopyStream(Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }


        public static byte[] DecompressZlib(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            try
            {
                MemoryStream input = new MemoryStream(data);
                using (ZOutputStream zs = new ZOutputStream(output))
                {
                    CopyStream(input, zs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return output.ToArray();
        }
    }
}
