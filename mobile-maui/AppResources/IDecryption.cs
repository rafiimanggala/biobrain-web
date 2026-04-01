using System.IO;

namespace BioBrain.AppResources
{
    public interface IDecryption
    {
        /// <summary>
        /// Decrypt file
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Stream Def(Stream inFile, string password);
    }
}