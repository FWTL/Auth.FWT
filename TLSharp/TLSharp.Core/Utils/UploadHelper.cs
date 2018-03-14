using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TeleSharp.TL;
using TeleSharp.TL.Upload;

namespace TLSharp.Core.Utils
{
    public static class UploadHelper
    {
        //public static TLAbsInputFile UploadFile(this TelegramClient client, string name, StreamReader reader)
        //{
        //    const long tenMb = 10 * 1024 * 1024;
        //    return await UploadFile(name, reader, client, reader.BaseStream.Length >= tenMb);
        //}
        private static byte[] GetFile(StreamReader reader)
        {
            var file = new byte[reader.BaseStream.Length];

            using (reader)
            {
                reader.BaseStream.Read(file, 0, (int)reader.BaseStream.Length);
            }

            return file;
        }

        private static string GetFileHash(byte[] data)
        {
            string md5_checksum;
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                var hashResult = new StringBuilder(hash.Length * 2);

                foreach (byte t in hash)
                    hashResult.Append(t.ToString("x2"));

                md5_checksum = hashResult.ToString();
            }

            return md5_checksum;
        }

        private static Queue<byte[]> GetFileParts(byte[] file)
        {
            var fileParts = new Queue<byte[]>();

            const int maxFilePart = 512 * 1024;

            using (var stream = new MemoryStream(file))
            {
                while (stream.Position != stream.Length)
                {
                    if ((stream.Length - stream.Position) > maxFilePart)
                    {
                        var temp = new byte[maxFilePart];
                        stream.Read(temp, 0, maxFilePart);
                        fileParts.Enqueue(temp);
                    }
                    else
                    {
                        var length = stream.Length - stream.Position;
                        var temp = new byte[length];
                        stream.Read(temp, 0, (int)(length));
                        fileParts.Enqueue(temp);
                    }
                }
            }

            return fileParts;
        }
    }
}
