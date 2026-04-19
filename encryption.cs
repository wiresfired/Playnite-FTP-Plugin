using Playnite.SDK.Plugins;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ftp
{
    internal static class Encryption
    {
        /// <summary>
        /// Encrypts the specified data using DPAPI.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted data as a Base64 string.</returns>
        internal static string ProtectData(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] protectedData = ProtectedData.Protect(dataBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedData);
        }

        /// <summary>
        /// Decrypts the specified encrypted data using DPAPI.
        /// </summary>
        /// <param name="data">The encrypted data as a Base64 string.</param>
        /// <returns>The decrypted data.</returns>
        internal static string UnprotectData(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            byte[] protectedDataBytes = Convert.FromBase64String(data);
            byte[] unprotectedDataBytes = ProtectedData.Unprotect(protectedDataBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(unprotectedDataBytes);
        }
    }
}
