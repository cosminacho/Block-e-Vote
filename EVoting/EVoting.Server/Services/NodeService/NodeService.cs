using EVoting.Common.Crypto;

namespace EVoting.Server.Services.NodeService
{
    public class NodeService : INodeService
    {
        private byte[] _privateKey;
        private byte[] _publicKey;

        public NodeService()
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
