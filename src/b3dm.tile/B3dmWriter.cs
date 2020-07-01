﻿using System.IO;
using System.Text;

namespace B3dm.Tile
{
    public static class B3dmWriter
    {
        public static string WriteB3dm(string path, B3dm b3dm)
        {
            var header_length = 28;

            b3dm.FeatureTableJson = BufferPadding.AddPadding(b3dm.FeatureTableJson, header_length);
            b3dm.BatchTableJson = BufferPadding.AddPadding(b3dm.BatchTableJson);
            b3dm.FeatureTableBinary = BufferPadding.AddPadding(b3dm.FeatureTableBinary);
            b3dm.BatchTableBinary = BufferPadding.AddPadding(b3dm.BatchTableBinary);

            b3dm.B3dmHeader.ByteLength = b3dm.GlbData.Length + header_length + b3dm.FeatureTableJson.Length + b3dm.BatchTableJson.Length + b3dm.BatchTableBinary.Length + b3dm.FeatureTableBinary.Length;

            b3dm.B3dmHeader.FeatureTableJsonByteLength = b3dm.FeatureTableJson.Length;
            b3dm.B3dmHeader.BatchTableJsonByteLength = b3dm.BatchTableJson.Length;
            b3dm.B3dmHeader.FeatureTableBinaryByteLength= b3dm.FeatureTableBinary.Length;
            b3dm.B3dmHeader.BatchTableBinaryByteLength = b3dm.BatchTableBinary.Length;

            var fileStream = File.Open(path, FileMode.Create);
            var binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(b3dm.B3dmHeader.AsBinary());
            binaryWriter.Write(Encoding.UTF8.GetBytes(b3dm.FeatureTableJson));
            if (b3dm.FeatureTableBinary != null) {
                binaryWriter.Write(b3dm.FeatureTableBinary);
            }
            binaryWriter.Write(Encoding.UTF8.GetBytes(b3dm.BatchTableJson));
            if (b3dm.BatchTableBinary != null) {
                binaryWriter.Write(b3dm.BatchTableBinary);
            }
            binaryWriter.Write(b3dm.GlbData);
            binaryWriter.Flush();
            binaryWriter.Close();
            return fileStream.Name;
        }
    }
}