using EVoting.Common.Crypto;

namespace EVoting.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly byte[] _privateKey;
        private readonly byte[] _publicKey;
        public AuthService()
        {
            (_privateKey, _publicKey) = CryptoService.GenerateAsymmetricKeys();
        }


        public byte[] GetPrivateKey()
        {
            return _privateKey;
        }

        public byte[] GetPublicKey()
        {
            return _publicKey;
        }
    }
}
