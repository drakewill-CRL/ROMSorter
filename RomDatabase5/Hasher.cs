using System;
using System.Text;
using System.Security.Cryptography;
using Force.Crc32;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RomDatabase5
{
    public class Hasher
    {
        MD5 md5;
        SHA1 sha1;
        Crc32Algorithm crc;

        //TODO: are there any other ways I can speed this up? 
        //should HashToString(ComputeHash()) be a task for big files? it takes 4.5 seconds to hash a decent sized DS file, threading that would be good.
        //And do those hurt performance on small files more than they help on big ones?

        public Hasher()
        {
            md5 = MD5.Create();
            sha1 = SHA1.Create();
            crc = new Crc32Algorithm();
        }

        string HashToString(ref byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sb.ToString().ToLower();
        }

        string HashToString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sb.ToString().ToLower();
        }

        public string[] HashFile(Stream fileData)
        {
            //hashes files all 3 ways. 
            string[] results = new string[3];
            results[0] = HashToString(md5.ComputeHash(fileData));
            fileData.Seek(0, SeekOrigin.Begin);
            results[1] = HashToString(sha1.ComputeHash(fileData));
            fileData.Seek(0, SeekOrigin.Begin);
            results[2] = HashToString(crc.ComputeHash(fileData));
            return results;
        }

        //Testing looks like multithread hashing is faster on files over 4kb in size.
        //I had expected that threshold to be higher, but that's what my testing showed.
        //RECHECK if this gives consistent values or if this causes conflicts while reading the byte array on mulitple threads?
        public string[] HashFile(byte[] fileData)
        {
            //hashes files all 3 ways.  Faster on bigger files with threading, might be slower on small files.
            string[] results = new string[3];
            var m = Task<string>.Factory.StartNew(() => { return HashToString(md5.ComputeHash(fileData)); });
            var s = Task<string>.Factory.StartNew(() => { return HashToString(sha1.ComputeHash(fileData)); });
            var c = Task<string>.Factory.StartNew(() => { return HashToString(crc.ComputeHash(fileData)); });
            Task.WaitAll(m, s, c);
            results[0] = m.Result;
            results[1] = s.Result;
            results[2] = c.Result;
            return results;
        }

        public string[] HashFileRef(ref byte[] fileData)
        {
            string[] results = new string[3];
            var m = HashToString(md5.ComputeHash(fileData)); 
            var s = HashToString(sha1.ComputeHash(fileData)); 
            var c = HashToString(crc.ComputeHash(fileData)); 
            results[0] = m;
            results[1] = s;
            results[2] = c;
            return results;
        }

        public string[] HashZipEntry(SharpCompress.Archives.IArchiveEntry entry, bool detectOffsets)
        {
            //NOTE: different dat files do or don't use the headers. TOSEC includes headers in its hashes, NO-INTRO doesn't.
            try
            {
                //NOTE: NES roms need to skip the header to identify correctly. thats 16 bytes.
                var br = new BinaryReader(entry.OpenEntryStream());
                byte[] alldata;
                int offset = 0;
                if (detectOffsets)
                {
                    if (entry.Key.EndsWith(".nes") && (entry.Size % 8192 != 0)) //NES data pages are 8kb or 16kb, header adds 16 bytes
                        offset = 16;
                }

                alldata = new byte[(int)entry.Size];
                br.Read(alldata, 0, (int)entry.Size);

                var data = alldata.Skip(offset).ToArray();
                
                var hashes = HashFileRef(ref data);
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

        public string[] HashArchiveEntry(SharpCompress.Archives.IArchiveEntry entry)
        {
            var br = new BinaryReader(entry.OpenEntryStream());
            byte[] data = new byte[(int)entry.Size];
            br.Read(data, 0, (int)entry.Size);
            //var hashes = HashFileRef(ref data);
            var hashes = HashFile(data);
            data = null;
            br.Close();
            br.Dispose();
            return hashes;
        }

        public List<LookupEntry> HashFromArchive(string file)
        {
            try
            {
                var fs = File.OpenRead(file);
                SharpCompress.Archives.IArchive existingZip = SharpCompress.Archives.ArchiveFactory.Open(fs);
                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public List<LookupEntry> HashFromZip(string file, bool useOffsets)
        {
            try
            {
                List<LookupEntry> zippedFiles = new List<LookupEntry>();
                var fs = new FileStream(file, FileMode.Open);
                var zf = SharpCompress.Archives.ArchiveFactory.Open(fs);
                foreach (var entry in zf.Entries)
                {
                    if (entry.Size > 0)
                    {
                        var ziphashes = HashZipEntry(entry, useOffsets);
                        if (ziphashes != null) //is null if the zip file entry couldn't be read.
                        {
                            LookupEntry le = new LookupEntry();
                            le.fileType = LookupEntryType.ZipEntry;
                            le.originalFileName = file;
                            le.entryPath = entry.Key;
                            le.crc = ziphashes[2];
                            le.sha1 = ziphashes[1];
                            le.md5 = ziphashes[0];
                            le.size = entry.Size;
                            zippedFiles.Add(le);
                        }
                    }
                }
                fs.Close(); fs.Dispose();
                zf.Dispose();
                return zippedFiles.Count > 0 ? zippedFiles : null;
            }
            catch(Exception ex)
            {
                //Usually means zip file is invalid.
                //TODO: track and report specific errors.
                return null;
            }
        }

        public List<LookupEntry> HashFromRar(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.Rar.RarArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = HashArchiveEntry(entry);
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

        public List<LookupEntry> HashFromTar(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.Tar.TarArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = HashArchiveEntry(entry);
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

        public List<LookupEntry> HashFrom7z(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = HashArchiveEntry(entry);
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

        public List<LookupEntry> HashFromGzip(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.GZip.GZipArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = HashArchiveEntry(entry);
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
