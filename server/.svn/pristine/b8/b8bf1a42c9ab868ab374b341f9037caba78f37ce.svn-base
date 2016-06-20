using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libNet;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using libCommon.Gzip;
using libCommon.BJson;

namespace libNet.APIManage
{

    public class GameSerialize : INetSerialize
    {
        private static readonly int HEAD_MAGIC = 0x00000A0C;
        private static readonly int HEAD_ZIPED = 0x00010000;
        public byte[] Serialize<T>(T data)
        {
            byte[] b = BJSON.Instance.ToBJSON(data);
            byte[] buffer = new byte[1];
            int head = HEAD_MAGIC;
            byte[] headbytes;
            if (b.Length < 1024)
            {
                headbytes = BitConverter.GetBytes(head);
                buffer = new byte[headbytes.Length + b.Length];
                Buffer.BlockCopy(b, 0, buffer, headbytes.Length, b.Length);
                return buffer;
            }
            head |= HEAD_ZIPED;
            headbytes = BitConverter.GetBytes(head);
            using (MemoryStream msTemp = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(msTemp, CompressionMode.Compress, true))
                {
                    gz.Write(b, 0, b.Length);
                    gz.Close();
                    byte[] dedata = msTemp.ToArray();
                    buffer = new byte[headbytes.Length + dedata.Length];
                    Buffer.BlockCopy(headbytes, 0, buffer, 0, headbytes.Length);
                    Buffer.BlockCopy(dedata, 0, buffer, headbytes.Length, dedata.Length);
                    return buffer;
                }
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            int head = BitConverter.ToInt32(data, 0);
            if ((head & HEAD_ZIPED) == HEAD_ZIPED)//如果数据有压缩.
            {
                byte[] zipdata = new byte[data.Length - sizeof(int)];
                Buffer.BlockCopy(data, sizeof(int), zipdata, 0, zipdata.Length);
                using (MemoryStream tmpms = new MemoryStream())
                {
                    using (MemoryStream ms = new MemoryStream(zipdata))
                    {
                        using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress, true))
                        {
                            BinaryReader br = new BinaryReader(gz);
                            byte[] buffer = new byte[1024];
                            int readlength = 0;
                            while ((readlength = br.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                tmpms.Write(buffer, 0, readlength);
                            }
                            gz.Close();
                            return BJSON.Instance.ToObject<T>(tmpms.ToArray());
                        }
                    }
                }
            }
            else
            {
                byte[] unzipdata = new byte[data.Length - sizeof(int)];
                Buffer.BlockCopy(data, sizeof(int), unzipdata, 0, unzipdata.Length);
                return BJSON.Instance.ToObject<T>(unzipdata);
            }

        }

        public static GameSerialize Instance;

        static GameSerialize()
        {
            Instance = new GameSerialize();
        }
    } 
}