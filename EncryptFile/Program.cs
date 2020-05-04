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

      byte[] random32 = new Byte[32];
      byte[] random16 = new Byte[16];

      RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
      rng.GetBytes(random32);
      rng.GetBytes(random16);
      
      var aesAlg = new RijndaelManaged();
      aesAlg.Key = random32;
      aesAlg.IV = random16;

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
