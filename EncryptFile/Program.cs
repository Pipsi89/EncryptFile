namespace EncryptFile
{
  using System;
  using System.IO;
  using System.Security.Cryptography;

  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("ARG1 = Input text file name; ARG2 = Output text file name");
        return;
      }

      var sourceFilename = args[0];
      var destinationFilename = args[1];
      byte[] providerKey;
      byte[] initVector;

      var aesAlg = new RijndaelManaged();
      aesAlg.Key = new byte[32] { 118, 123, 23, 17, 161, 152, 35, 68, 126, 213, 16, 115, 68, 217, 58, 108, 56, 218, 5, 78, 28, 128, 113, 208, 61, 56, 10, 87, 187, 162, 233, 38 };
      aesAlg.IV = new byte[16] { 33, 241, 14, 16, 103, 18, 14, 248, 4, 54, 18, 5, 60, 76, 16, 191 };

      providerKey = aesAlg.Key;
      initVector = aesAlg.IV;

      //Encrypt and write to "testOutput"
      using (var sourceStream = File.OpenRead(sourceFilename))
      using (var destinationStream = File.Create(destinationFilename))
      using (var cryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
      using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
      {
        sourceStream.CopyTo(cryptoStream);

        Console.WriteLine(System.Convert.ToBase64String(providerKey));
        Console.WriteLine(System.Convert.ToBase64String(initVector));
      }

      //Decrypt and log to console
      using (var encryptedStream = File.OpenRead(destinationFilename))
      using (var provider = new AesCryptoServiceProvider())
      {
        using (var cryptoTransform = provider.CreateDecryptor(providerKey, initVector))
        using (var decryptedStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Read))
        {
          using (StreamReader sr = new StreamReader(decryptedStream))
          {
            Console.WriteLine(sr.ReadToEnd());
          }
        }
      }

    }
  }
}
