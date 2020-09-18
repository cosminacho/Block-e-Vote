using EVoting.Common.Crypto;

namespace EVoting.Server.Services.CAuthService
{
    public class CAuthService : ICAuthService
    {
        private readonly byte[] _privateKey;
        private readonly byte[] _publicKey;
        public CAuthService()
        {
            (_privateKey, _publicKey) = CryptoService.GenerateAsymmetricKeys();
        }

        public byte[] GetPublicKey()
        {
            return _publicKey;
        }

        public byte[] GetPrivateKey()
        {
            return _privateKey;
        }
    }
}
