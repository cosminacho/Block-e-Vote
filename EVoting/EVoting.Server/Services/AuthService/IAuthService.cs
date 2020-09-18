namespace EVoting.Server.Services.AuthService
{
    public interface IAuthService
    {
        public byte[] GetPrivateKey();
        public byte[] GetPublicKey();
    }
}
