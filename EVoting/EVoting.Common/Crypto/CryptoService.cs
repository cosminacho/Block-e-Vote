using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EVoting.Common.Crypto
{
    public class CryptoService
    {
        public static byte[] HashItem(byte[] item)
        {
            using (var sha = new SHA512CryptoServiceProvider())
            {
                return sha.ComputeHash(item);
            }
        }

        public static byte[] HmacItem(byte[] item, byte[] key)
        {
            using (var hmac = new HMACSHA512(key))
            {
                return hmac.ComputeHash(item);
            }
        }

        public static byte[] EncryptSymmetric(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                var encryptor = aes.CreateEncryptor(key, iv);
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }

        }

        public static byte[] DecryptSymmetric(byte[] encryptedData, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                var decryptor = aes.CreateDecryptor(key, iv);
                using (var memoryStream = new MemoryStream(encryptedData))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    var count = cryptoStream.Read(encryptedData);
                    byte[] data = new byte[count];
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.Read(data, 0, count);

                    return data;
                }
            }
        }

        public static (byte[], byte[]) GenerateSymmetricKeys()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.GenerateKey();
                return (aes.Key, aes.IV);
            }
        }

        public static (byte[], byte[]) GenerateAsymmetricKeys()
        {
            using (var rsa = RSA.Create())
            {
                var privateKey = rsa.ExportPkcs8PrivateKey();
                var publicKey = rsa.ExportSubjectPublicKeyInfo();
                return (privateKey, publicKey);
            }
        }

        public static byte[] EncryptAsymmetric(byte[] data, byte[] publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA512);
            }
        }

        public static byte[] DecryptAsymmetric(byte[] encryptedData, byte[] privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPkcs8PrivateKey(privateKey, out _);
                return rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA512);
            }
        }


        public static (byte[], byte[], byte[]) GenerateAsymmetricKeys(string password)
        {
            using (var rsa = RSA.Create())
            {
                var encryptedPrivateKey = rsa.ExportEncryptedPkcs8PrivateKey(password,
                    new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA512, 10));
                var privateKey = rsa.ExportPkcs8PrivateKey();
                var publicKey = rsa.ExportSubjectPublicKeyInfo();
                return (encryptedPrivateKey, privateKey, publicKey);
            }
        }


        public static byte[] SignItem(byte[] data, byte[] privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPkcs8PrivateKey(privateKey, out _);
                return rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            }
        }

        public static byte[] SignItem(byte[] data, byte[] encryptedPrivateKey, string password)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportEncryptedPkcs8PrivateKey(password, encryptedPrivateKey, out _);
                return rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            }
        }

        public static bool VerifySignature(byte[] data, byte[] signature, byte [] publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            }
        }
    }


}
