using System.Text;
using BioBrain.AppResources;
using Microsoft.Maui.Controls;

namespace BioBrain.Helpers
{
    /// <summary>    
    /// Common cryptographic helper    
    /// </summary>    
    public static class CryptoHelper
    {
        /// <summary>    
        /// Encrypts given data using symmetric algorithm AES    
        /// </summary>    
        /// <param name="data">Data to encrypt</param>    
        /// <param name="password">Password</param>    
        /// <param name="salt">Salt</param>    
        /// <returns>Encrypted bytes</returns>    
        //public static byte[] EncryptAes(byte[] data, string password)
        //{
        //    ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        //    ICryptographicKey symetricKey = aes.CreateSymmetricKey(Encoding.UTF8.GetBytes(password));
        //    var bytes = WinRTCrypto.CryptographicEngine.Encrypt(symetricKey, data);
        //    return bytes;
        //}
        /// <summary>    
        /// Decrypts given bytes using symmetric alogrithm AES    
        /// </summary>    
        /// <param name="data">data to decrypt</param>    
        /// <param name="password">Password used for encryption</param>    
        /// <param name="salt">Salt used for encryption</param>    
        /// <returns></returns>    
        //public static byte[] DecryptAes(byte[] data, string password)
        //{
        //    ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCcm);
        //    ICryptographicKey symetricKey = aes.CreateSymmetricKey(DependencyService.Get<IEncription>().CreateKey(password));
        //    var bytes = WinRTCrypto.CryptographicEngine.Decrypt(symetricKey, data);
        //    return bytes;
        //}

    }
}