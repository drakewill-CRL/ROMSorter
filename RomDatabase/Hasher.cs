using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Force.Crc32;

namespace RomDatabase
{
    public static class Hasher
    {
        public static bool MatchFile(byte[] fileData, string md5Hash, string sha1Hash, string crcHash)
        {
            //must be instantiated here, since these arent thread-safe.
            //hashes files all 3 ways.
            MD5 md5 = MD5.Create();
            SHA1 sha1 = SHA1.Create();
            Crc32Algorithm crc = new Crc32Algorithm();

            if (HashToString(md5.ComputeHash(fileData)) == md5Hash
                && HashToString(sha1.ComputeHash(fileData)) == sha1Hash
                && HashToString(crc.ComputeHash(fileData)) == crcHash)
                return true;

            return false;
        }
        
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
    }
}
