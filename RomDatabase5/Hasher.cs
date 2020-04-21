using System;
using System.Text;
using System.Security.Cryptography;
using Force.Crc32;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;

namespace RomDatabase5
{
    class Hasher
    {
        static string HashToString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sb.ToString().ToLower();
        }

        public static string[] HashFile(byte[] fileData)
        {
            //hashes files all 3 ways. 
            MD5 md5 = MD5.Create();
            SHA1 sha1 = SHA1.Create();
            Crc32Algorithm crc = new Crc32Algorithm();

            string[] results = new string[3];
            results[0] = HashToString(md5.ComputeHash(fileData));
            results[1] = HashToString(sha1.ComputeHash(fileData));
            results[2] = HashToString(crc.ComputeHash(fileData));

            return results;
        }

        public static string[] HashZipEntry(ZipArchiveEntry entry)
        {
            try
            {
                var br = new BinaryReader(entry.Open());
                byte[] data = new byte[(int)entry.Length];
                br.Read(data, 0, (int)entry.Length);
                var hashes = Hasher.HashFile(data);
                data = null;
                br.Close();
                br.Dispose();
                return hashes;
            }
            catch (Exception ex)
            {
                return null; //most likely the zip wasn't readable.
            }
        }

        public static string[] HashArchiveEntry(SharpCompress.Archives.IArchiveEntry entry)
        {
            var br = new BinaryReader(entry.OpenEntryStream());
            byte[] data = new byte[(int)entry.Size];
            br.Read(data, 0, (int)entry.Size);
            var hashes = Hasher.HashFile(data);
            data = null;
            br.Close();
            br.Dispose();
            return hashes;
        }

        public static List<LookupEntry> HashFromZip(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            ZipArchive zf = new ZipArchive(new FileStream(file, FileMode.Open));
            foreach (var entry in zf.Entries)
            {
                if (entry.Length > 0)
                {
                    var ziphashes = Hasher.HashZipEntry(entry);
                    if (ziphashes != null) //is null if the zip file entry couldn't be read.
                    {
                        LookupEntry le = new LookupEntry();
                        le.fileType = LookupEntryType.ZipEntry;
                        le.originalFileName = file;
                        le.entryPath = entry.FullName;
                        le.crc = ziphashes[2];
                        le.sha1 = ziphashes[1];
                        le.md5 = ziphashes[0];
                        le.size = entry.Length;
                        zippedFiles.Add(le);
                    }
                }
            }
            zf.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> HashFromRar(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.Rar.RarArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashArchiveEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.RarEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> HashFromTar(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.Tar.TarArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashArchiveEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.TarEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> HashFrom7z(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashArchiveEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.SevenZEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> HashFromGzip(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.GZip.GZipArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashArchiveEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.GZipEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }
    }
}
