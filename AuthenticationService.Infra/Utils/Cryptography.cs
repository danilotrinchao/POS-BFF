using AuthenticationService.Application.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Infra.Utils
{
    public class Cryptography: ICryptography
    {
        private readonly HashAlgorithm _hashAlgorithm;
        public Cryptography(HashAlgorithm hashAlgorithm)
        {     
            _hashAlgorithm = hashAlgorithm;
        }
        public string CriptografarSenha(string senha)
        {
            var encodedValue = Encoding.UTF8.GetBytes(senha);
            var encryptedPassword = _hashAlgorithm.ComputeHash(encodedValue);

            var sb = new StringBuilder();
            foreach (var caracter in encryptedPassword)
            {
                sb.Append(caracter.ToString("X2"));
            }

            return sb.ToString();
        }
        public string CryptographyPassword(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }
        public bool VerificarSenha(string senhaDigitada, string senhaCadastrada)
        {
            if (string.IsNullOrEmpty(senhaCadastrada))
                throw new NullReferenceException("Cadastre uma senha.");

            // Converta a senha cadastrada de hexadecimal de volta para bytes
            byte[] storedPasswordBytes = new byte[senhaCadastrada.Length / 2];
            for (int i = 0; i < senhaCadastrada.Length; i += 2)
            {
                storedPasswordBytes[i / 2] = Convert.ToByte(senhaCadastrada.Substring(i, 2), 16);
            }

            // Gere o hash da senha digitada
            var inputPasswordBytes = Encoding.UTF8.GetBytes(senhaDigitada);
            var inputPasswordHash = _hashAlgorithm.ComputeHash(inputPasswordBytes);

            // Compare os hashes
            return inputPasswordHash.SequenceEqual(storedPasswordBytes);
        }

        public byte[] EncryptPassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                byte[] salt = hmac.Key; // Gera um salt aleatório
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = hmac.ComputeHash(passwordBytes);

                // Combine o salt e o hash e retorne
                byte[] combinedBytes = new byte[salt.Length + hash.Length];
                Array.Copy(salt, 0, combinedBytes, 0, salt.Length);
                Array.Copy(hash, 0, combinedBytes, salt.Length, hash.Length);
                return combinedBytes;
            }
        }

        public byte[] DecryptPassword(byte[] passwordHash, byte[] passwordSalt)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256; // Tamanho da chave em bits
                aes.BlockSize = 128; // Tamanho do bloco em bits
                aes.Mode = CipherMode.CBC; // Modo de operação: Cipher Block Chaining
                aes.Padding = PaddingMode.PKCS7; // Preenchimento dos blocos

                // Deriva a chave a partir do salt usando PBKDF2
                var keyDerivationFunction = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(passwordSalt), passwordSalt, 10000);
                aes.Key = keyDerivationFunction.GetBytes(aes.KeySize / 8);
                aes.IV = keyDerivationFunction.GetBytes(aes.BlockSize / 8);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var msDecrypt = new MemoryStream(passwordHash))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var decryptedData = new MemoryStream())
                {
                    csDecrypt.CopyTo(decryptedData);
                    return decryptedData.ToArray();
                }
            }
        }

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

       
    }
}
