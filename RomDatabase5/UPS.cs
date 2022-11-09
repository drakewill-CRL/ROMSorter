using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

//Code sourced directly from Nintenlord's site. Unedited.
//https://forums.serenesforest.net/index.php?/topic/26913-nintenlords-hacking-utilities/
namespace Nintenlord.UPSpatcher
{
    public unsafe class UPSfile
    {
        bool validPatch;
        public bool ValidPatch
        {
            get { return validPatch; }
        }
        uint originalFileCRC32;
        uint newFileCRC32;
        uint patchCRC32;
        ulong oldFileSize;
        ulong newFileSize;
        ulong[] changedOffsets;
        byte[][] XORbytes;

        public UPSfile(string filePath)
        {
            List<ulong> changedOffsetsList = new List<ulong>();
            List<byte[]> XORbytesList = new List<byte[]>();

            validPatch = false;
            if (!File.Exists(filePath))
                return;
            byte[] UPSfile;
            try
            {
                BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open));
                UPSfile = br.ReadBytes((int)br.BaseStream.Length);
                br.Close();
            }
            catch (Exception)
            {
                return;
            }

            fixed (byte* UPSptr = &UPSfile[0])
            {
                //header
                byte* currentPtr = UPSptr;
                string header = new string((sbyte*)currentPtr,0,4,Encoding.ASCII);
                if (header != "UPS1")
                    return;
                currentPtr += 4;
                oldFileSize = Decrypt(&currentPtr);
                newFileSize = Decrypt(&currentPtr);

                //body
                ulong filePosition = 0;
                while (currentPtr - UPSptr + 1 < UPSfile.Length - 12)
                {
                    filePosition += Decrypt(&currentPtr);
                    changedOffsetsList.Add(filePosition);
                    List<byte> newXORdata = new List<byte>();

                    while (*currentPtr != 0)
                    {
                        newXORdata.Add(*(currentPtr++));
                    }
                    XORbytesList.Add(newXORdata.ToArray());
                    filePosition += (ulong)newXORdata.Count + 1;
                    currentPtr++;
                }

                //end
                originalFileCRC32 = *((uint*)(currentPtr));
                newFileCRC32 = *((uint*)(currentPtr + 4));
                patchCRC32 = *((uint*)(currentPtr + 8));                            
            }


            changedOffsets = changedOffsetsList.ToArray();
            XORbytes = XORbytesList.ToArray();

            if (patchCRC32 != calculatePatchCRC32())
                return;

            validPatch = true;
        }

        public UPSfile(byte[] originalFile, byte[] newFile)
        {
            List<ulong> changedOffsetsList = new List<ulong>();
            List<byte[]> XORbytesList = new List<byte[]>();
            validPatch = true;
            oldFileSize = (ulong)originalFile.Length;
            newFileSize = (ulong)newFile.Length;

            ulong maxSize;
            if (oldFileSize > newFileSize)
                maxSize = oldFileSize;
            else
                maxSize = newFileSize;

            for (ulong i = 0; i < maxSize; i++)
            {
                byte x = i < oldFileSize ? originalFile[i] : (byte)0x00;
                byte y = i < newFileSize ? newFile[i] : (byte)0x00;

                if (x != y)
                {
                    changedOffsetsList.Add((ulong)i);
                    List<byte> newXORbytes = new List<byte>();
                    while (x != y && i < maxSize)
                    {
                        newXORbytes.Add((byte)(x ^ y));
                        i++;
                        x = i < oldFileSize ? originalFile[i] : (byte)0x00;
                        y = i < newFileSize ? newFile[i] : (byte)0x00;
                    }
                    XORbytesList.Add(newXORbytes.ToArray());
                }
            }
            originalFileCRC32 = CRC32.crc32_calculate(originalFile);
            newFileCRC32 = CRC32.crc32_calculate(newFile);
            changedOffsets = changedOffsetsList.ToArray();
            XORbytes = XORbytesList.ToArray();
            patchCRC32 = calculatePatchCRC32();
        }
        
        static byte[] Encrypt(ulong offset)
        {
            List<byte> bytes = new List<byte>(8);

            ulong x = offset & 0x7f;
            offset >>= 7;
            while (offset != 0)
            {
                bytes.Add((byte)x);
                offset--;
                x = offset & 0x7f;
                offset >>= 7;
            }
            bytes.Add((byte)(0x80 | x));
            return bytes.ToArray();
        }
        
        static ulong Decrypt(byte** pointer)
        {
            ulong value = 0;
            int shift = 1;
            byte x = *((*pointer)++);
            value += (ulong)((x & 0x7F) * shift);
            while ((x & 0x80) == 0)
            {
                shift <<= 7;
                value += (ulong)shift;
                x = *((*pointer)++);
                value += (ulong)((x & 0x7F) * shift);
            }
            return value;
        }

        private uint calculatePatchCRC32()
        {
            return CRC32.crc32_calculate(ToBinary());
        }
        
        public bool ValidToApply(byte[] file)
        {
            uint fileCRC32 = CRC32.crc32_calculate(file);
            bool fitsAsOld = oldFileSize == (ulong)file.Length && fileCRC32 == originalFileCRC32;
            bool fitsAsNew = newFileSize == (ulong)file.Length && fileCRC32 == newFileCRC32;

            return validPatch && (fitsAsOld || fitsAsNew);
        }

        public byte[] Apply(byte[] file)
        {
            ulong lenght = (ulong)file.LongLength;
            if (lenght < newFileSize)
                lenght = newFileSize;

            byte[] result = new byte[lenght];

            fixed (byte* resultPtr = &result[0])
            {
                
                Marshal.Copy(file, 0, new IntPtr(resultPtr), Math.Min(file.Length, result.Length));
                //int index = file.Length;
                //while (index < result.Length)
                //    resultPtr[index++] = 0;

                for (int i = 0; i < changedOffsets.LongLength; i++)
                    for (ulong u = 0; u < (ulong)XORbytes[i].LongLength; u++)
                        resultPtr[changedOffsets[i] + u] ^= XORbytes[i][u];
            }
            return result;
        }

        public byte[] Apply(string path)
        {
            if (!validPatch || !File.Exists(path))
                return null;

            BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open));
            byte[] file = br.ReadBytes((int)br.BaseStream.Length);
            br.Close();
            return Apply(file);
        }

        private byte[] ToBinary()
        {
            List<byte> file = new List<byte>();
            file.Add((byte)'U');
            file.Add((byte)'P');
            file.Add((byte)'S');
            file.Add((byte)'1');
            file.AddRange(Encrypt(oldFileSize));
            file.AddRange(Encrypt(newFileSize));

            for (int i = 0; i < changedOffsets.LongLength; i++)
            {
                ulong relativeOffset = changedOffsets[i];
                if (i != 0)
                    relativeOffset -= changedOffsets[i - 1] + (ulong)XORbytes[i - 1].Length + 1;

                file.AddRange(Encrypt(relativeOffset));
                file.AddRange(XORbytes[i]);
                file.Add(0);
            }

            file.AddRange(BitConverter.GetBytes(originalFileCRC32));
            file.AddRange(BitConverter.GetBytes(newFileCRC32));

            return file.ToArray();
        }

        public void writeToFile(string path)
        {
            BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create));
            byte[] file = ToBinary();
            bw.Write(file);
            bw.Write(CRC32.crc32_calculate(file));
            bw.Close();
        }

        public int[,] getData()
        {
            int[,] result = new int[changedOffsets.Length, 2];
            for (int i = 0; i < changedOffsets.Length; i++)
            {
                result[i, 0] = (int)changedOffsets[i];
                result[i, 1] = XORbytes[i].Length;
            }
            return result;
        }
    }
    
}
