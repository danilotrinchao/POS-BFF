using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Application.Contracts
{
    public interface ICryptography
    {
        byte[] EncryptPassword(string password);
        byte[] GenerateSalt();
        byte[] DecryptPassword(byte[] passwordHash, byte[] passwordSalt);
        string CriptografarSenha(string senha);
        bool VerificarSenha(string senhaDigitada, string senhaCadastrada);
        string CryptographyPassword(string senha);
    }
}
