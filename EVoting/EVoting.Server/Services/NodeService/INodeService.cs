namespace EVoting.Server.Services.NodeService
{
    public interface INodeService
    {
        public byte[] GetPrivateKey();
        public byte[] GetPublicKey();
    }
}
