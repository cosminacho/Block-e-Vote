namespace EVoting.Server.Services.CAuthService
{
    public interface ICAuthService
    {
        public byte[] GetPublicKey();
        public byte[] GetPrivateKey();
    }
}
