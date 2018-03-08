using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.ServiceModel.Channels;

namespace Sirona.Utilities.Compression.Microsoft
{
    public class GZipStream
    {
        public static ArraySegment<byte> CompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager, int messageOffset)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(buffer.Array, 0, messageOffset);

            using (System.IO.Compression.GZipStream gzStrream = new System.IO.Compression.GZipStream(stream, CompressionMode.Compress, true))
            {
                gzStrream.Write(buffer.Array, messageOffset, buffer.Count);
            }

            byte[] compressedBytes = stream.ToArray();
            byte[] bufferedBytes = bufferManager.TakeBuffer(compressedBytes.Length);

            Array.Copy(compressedBytes, 0, bufferedBytes, 0, compressedBytes.Length);

            bufferManager.ReturnBuffer(buffer.Array);
            ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferedBytes, messageOffset, bufferedBytes.Length - messageOffset);

            return byteArray;
        }

        public static ArraySegment<byte> DecompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
        {
            MemoryStream stream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count - buffer.Offset);
            MemoryStream decompressedStream = new MemoryStream();

            //int totalRead = 0;
            const int blockSize = 1024;
            byte[] tempBuffer = bufferManager.TakeBuffer(blockSize);

            using (System.IO.Compression.GZipStream gzStream = new System.IO.Compression.GZipStream(stream, CompressionMode.Decompress))
            {
                while (true)
                {
                    int bytesRead = gzStream.Read(tempBuffer, 0, blockSize);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    decompressedStream.Write(tempBuffer, 0, bytesRead);
                    //totalRead += bytesRead;
                }
            }

            bufferManager.ReturnBuffer(tempBuffer);

            byte[] decompressedBytes = decompressedStream.ToArray();
            byte[] bufferManagerBuffer = bufferManager.TakeBuffer(decompressedBytes.Length + buffer.Offset);
            Array.Copy(buffer.Array, 0, bufferManagerBuffer, 0, buffer.Offset);
            Array.Copy(decompressedBytes, 0, bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);

            var byteArray = new ArraySegment<byte>(bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            return byteArray;
        }
    }
}
