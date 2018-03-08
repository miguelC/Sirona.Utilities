using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Security.Cryptography;
using Sirona.Utilities.Compression.Microsoft;

namespace Sirona.Utilities.WCF.Message.Encoders.GZipEncoder
{
    internal class GZipMessageEncoderFactory : MessageEncoderFactory
    {
        private readonly MessageEncoder _encoder;

        public GZipMessageEncoderFactory(MessageEncoderFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory", "A valid message encoder factory must be passed to the GzipEncoder");
            }

            _encoder = new GzipMessageEncoder(factory.Encoder);
        }

        public override MessageEncoder Encoder
        {
            get { return _encoder; }
        }

        public override MessageVersion MessageVersion
        {
            get { return _encoder.MessageVersion; }
        }

        #region Nested type: GzipMessageEncoder

        private class GzipMessageEncoder : MessageEncoder
        {
            private const string GZipContentType = "application/x-gzip";

            private readonly MessageEncoder _innerEncoder;

            internal GzipMessageEncoder(MessageEncoder encoder)
            {
                if (encoder == null)
                {
                    throw new ArgumentNullException("encoder", "A valid message encoder must be passed to the GZipEncoder.");
                }
                _innerEncoder = encoder;
            }

            public override string ContentType
            {
                get { return GZipContentType; }
            }

            public override string MediaType
            {
                get { return GZipContentType; }
            }

            public override MessageVersion MessageVersion
            {
                get { return _innerEncoder.MessageVersion; }
            }

            public override System.ServiceModel.Channels.Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                // Decode Message
                byte[] bytes = buffer.Array;
                bytes = Encoding.DecodePackage(bytes);
                byte[] bufferedBytes = bufferManager.TakeBuffer(bytes.Length);
                Array.Copy(bytes, 0, bufferedBytes, 0, bytes.Length);
                buffer = new ArraySegment<byte>(bufferedBytes);
                // End Decode

                ArraySegment<byte> decompressedBuffer = GZipStream.DecompressBuffer(buffer, bufferManager);

                System.ServiceModel.Channels.Message returnMessage = _innerEncoder.ReadMessage(decompressedBuffer, bufferManager);
                returnMessage.Properties.Encoder = this;

                return returnMessage;
            }

            public override ArraySegment<byte> WriteMessage(System.ServiceModel.Channels.Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                ArraySegment<byte> buffer = _innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);

                buffer = GZipStream.CompressBuffer(buffer, bufferManager, messageOffset);

                // Encode Message
                byte[] bytes = buffer.Array;
                bytes = Encoding.EncodePackage(bytes);
                byte[] bufferedBytes = bufferManager.TakeBuffer(bytes.Length);
                Array.Copy(bytes, 0, bufferedBytes, 0, bytes.Length);
                buffer = new ArraySegment<byte>(bufferedBytes);
                // End Encode

                return buffer;
            }

            public override System.ServiceModel.Channels.Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                System.IO.Compression.GZipStream gzStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress, true);
                return _innerEncoder.ReadMessage(gzStream, maxSizeOfHeaders);
            }

            public override void WriteMessage(System.ServiceModel.Channels.Message message, Stream stream)
            {
                using (System.IO.Compression.GZipStream gzStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress, true))
                {
                    _innerEncoder.WriteMessage(message, stream);
                }

                stream.Flush();
            }
        }

        #endregion
    }
}
