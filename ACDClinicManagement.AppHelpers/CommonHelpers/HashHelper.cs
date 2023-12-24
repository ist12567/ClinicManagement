using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{

    public static class HashHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Enum

        public enum HashType
        {
            Md5,
            Sha1,
            Sha256,
            Sha512
        }

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static byte[] ToHashByte(this string value, HashType hashType = HashType.Md5)

        public static byte[] ToHashByte(this string value, HashType hashType = HashType.Md5)
        {
            HashAlgorithm hashAlgorithm;
            switch (hashType)
            {
                case HashType.Md5:
                    hashAlgorithm = MD5.Create();
                    break;

                case HashType.Sha1:
                    hashAlgorithm = SHA1.Create();
                    break;

                case HashType.Sha256:
                    hashAlgorithm = SHA256.Create();
                    break;

                case HashType.Sha512:
                    hashAlgorithm = SHA512.Create();
                    break;

                default:
                    throw new ArgumentException("Invalid hash type", "hashType");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] hash = hashAlgorithm.ComputeHash(bytes);
            return hash;
        }

        #endregion

        #region public static string ToHashString(this string value, HashType hashType = HashType.Md5)

        public static string ToHashString(this string value, HashType hashType = HashType.Md5)
        {
            IEnumerable<byte> hash = value.ToHashByte(hashType);
            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion
    }
}
