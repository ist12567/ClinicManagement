using System;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{
    public static class SecurityHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Variables

        const string Passphrase = "A$iaCanDo";

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static byte[] ToHashPassword(this string value)
        public static byte[] ToHashPassword(this string value)
        {
            return value.ToHashByte(HashHelper.HashType.Sha256);
        }

        #endregion

        #region public static bool VerifyPassword(string enteredPassword, byte[] userPassword)
        public static bool VerifyPassword(string enteredPassword, byte[] userPassword)
        {
            var enteredPasswordHash = enteredPassword.ToHashPassword();
            var isMatched = enteredPasswordHash.Length == userPassword.Length;
            if (isMatched)
            {
                for (var i = 0; i < enteredPasswordHash.Length; i++)
                {
                    isMatched &= enteredPasswordHash[i] == userPassword[i];
                    if (!isMatched) break;
                }
            }

            return isMatched;
        }

        #endregion

        #region public static string ToEncryptData(this string localString)
        public static string ToEncryptData(this string localString)
        {
            byte[] results;
            var utf8 = new UTF8Encoding();
            var hashProvider = new MD5CryptoServiceProvider();
            var tdesKey = hashProvider.ComputeHash(utf8.GetBytes(Passphrase));
            var tdesAlgorithm = new TripleDESCryptoServiceProvider
            {
                Key = tdesKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var dataToEncrypt = utf8.GetBytes(localString);
            try
            {
                var encryptor = tdesAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }
            return Convert.ToBase64String(results);
        }

        #endregion

        #region public static string ToDecryptData(this string localString)
        public static string ToDecryptData(this string localString)
        {
            byte[] results;
            var utf8 = new UTF8Encoding();
            var hashProvider = new MD5CryptoServiceProvider();
            var tdesKey = hashProvider.ComputeHash(utf8.GetBytes(Passphrase));
            var tdesAlgorithm = new TripleDESCryptoServiceProvider
            {
                Key = tdesKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var dataToDecrypt = Convert.FromBase64String(localString);
            try
            {
                var decryptor = tdesAlgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }
            return utf8.GetString(results);
        }

        #endregion

        #region public static string GenerateRandomPassword(this int value)

        public static string GenerateRandomPassword(this int value)
        {
            var allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomNumbers = new byte[value];
            rngCryptoServiceProvider.GetNonZeroBytes(randomNumbers);
            return randomNumbers.Aggregate(string.Empty,
                (current, randomNumber) => current + allowedChars[randomNumber % allowedChars.Length]);
        }

        #endregion

        #region public static void EncodeConfigFile(Assembly assembly)
        public static void EncodeConfigFile(Assembly assembly)
        {
            if (!ApplicationDeployment.IsNetworkDeployed || !ApplicationDeployment.CurrentDeployment.IsFirstRun) return;
            var exeName = Path.GetFileName(assembly.Location);
            var configName = exeName + ".config";

            // ClickOnce deploys config into 2 dirs, so the parent dir is traversed to encrypt all
            var parentPath = Directory.GetParent(Directory.GetCurrentDirectory());

            foreach (var configuration in parentPath.GetDirectories().SelectMany(directory => (from file in directory.GetFiles()
                                                                                               where file.Name == configName
                                                                                               select new ExeConfigurationFileMap
                                                                                               {
                                                                                                   ExeConfigFilename = file.FullName
                                                                                               }
                                                                    into exeConfigurationFileMap
                                                                                               select
                                                                                                   ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None))))
            {
                ProtectSection("connectionStrings", configuration);
                configuration.Save(ConfigurationSaveMode.Modified);
            }
        }
        #endregion

        #region private static void ProtectSection(string sectionName, Configuration configuration)
        private static void ProtectSection(string sectionName, Configuration configuration)
        {
            var section = configuration.GetSection(sectionName);

            if (section == null)
                throw new Exception(string.Format("Section {0} not found in {1}.", sectionName, configuration.FilePath));

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("");
            }
            section.SectionInformation.ForceSave = true;
        }
        #endregion
    }
}
