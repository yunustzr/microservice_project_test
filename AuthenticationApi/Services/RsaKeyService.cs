using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Services
{
    public interface IRsaKeyService
    {
        Task<(string PublicKey, string PrivateKey)> GenerateKeysAsync(int keySize = 2048);
        Task<string> EncryptPrivateKeyAsync(string privateKey);
        Task<string> DecryptPrivateKeyAsync(string encryptedPrivateKey);
        Task<string> DecryptDataAsync(string encryptedDataBase64, string encryptedPrivateKeyBase64);
    }

    public class RsaKeyService : IRsaKeyService
    {
        private readonly byte[] _aesKey;

        public RsaKeyService(IConfiguration configuration)
        {
            var aesKeyBase64 = configuration["SystemSettings:AES_ENCRYPTION_KEY"]
                ?? throw new InvalidOperationException("Configuration key 'SystemSettings:AES_ENCRYPTION_KEY' not found.");
            try
            {
                _aesKey = Convert.FromBase64String(aesKeyBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Invalid Base64 format for AES_ENCRYPTION_KEY.", ex);
            }
        }

        public Task<(string PublicKey, string PrivateKey)> GenerateKeysAsync(int keySize = 2048)
        {
            using var rsa = RSA.Create(keySize);
            return Task.FromResult((
                Convert.ToBase64String(rsa.ExportRSAPublicKey()),
                Convert.ToBase64String(rsa.ExportRSAPrivateKey())
            ));
        }

        public Task<string> EncryptPrivateKeyAsync(string privateKey)
        {
            using var aes = Aes.Create();
            aes.Key = _aesKey;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();

            // Prepend IV
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cs, Encoding.UTF8))
            {
                writer.Write(privateKey);
            }
            return Task.FromResult(Convert.ToBase64String(ms.ToArray()));
        }

        public Task<string> DecryptPrivateKeyAsync(string encryptedPrivateKey)
        {
            var cipher = Convert.FromBase64String(encryptedPrivateKey);
            using var aes = Aes.Create();
            aes.Key = _aesKey;

            var ivLength = aes.BlockSize / 8;
            var iv = cipher.Take(ivLength).ToArray();
            var ciphertext = cipher.Skip(ivLength).ToArray();

            aes.IV = iv;
            using var ms = new MemoryStream(ciphertext);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(cs, Encoding.UTF8);
            return Task.FromResult(reader.ReadToEnd());
        }

        public async Task<string> DecryptDataAsync(string encryptedDataBase64, string encryptedPrivateKeyBase64)
        {
            var privateKeyBase64 = await DecryptPrivateKeyAsync(encryptedPrivateKeyBase64);
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);

            var data = Convert.FromBase64String(encryptedDataBase64);
            var decrypted = rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
