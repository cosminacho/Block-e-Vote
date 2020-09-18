using EVoting.Common.Crypto;
using EVoting.Common.DTOs;
using EVoting.Common.DTOs.VotingDTOs;
using EVoting.Common.Utils;
using System;
using System.Security.Cryptography;

namespace EVoting.Client.Services.VotingService
{
    public class VotingService
    {
        private byte[] _publicKey;
        private byte[] _encryptedPrivateKey;
        private byte[] _privateKey;


        public VotingService()
        {

        }

        public void InitialiseKeys(byte[] encryptedPrivateKey, byte[] publicKey, string password)
        {
            _encryptedPrivateKey = encryptedPrivateKey;
            _publicKey = publicKey;
            using (var rsa = RSA.Create())
            {

                rsa.ImportEncryptedPkcs8PrivateKey(password, encryptedPrivateKey, out _);
                _privateKey = rsa.ExportPkcs8PrivateKey();
            }
        }



        public UserSignedVoteDTO CastVote(Guid voterId, Guid candidateId, byte[] caPublicKey)
        {
            (var key, var iv) = CryptoService.GenerateSymmetricKeys();

            var encryptedVote = CryptoService.EncryptSymmetric(candidateId.ToByteArray(), key, iv);
            var encryptedKey = CryptoService.EncryptAsymmetric(key, caPublicKey);
            var encryptedIV = CryptoService.EncryptAsymmetric(iv, caPublicKey);

            var voterHash = CryptoService.HmacItem(voterId.ToByteArray(), _privateKey);

            var vote = new Vote()
            {
                VoterId = voterHash,
                EncryptedKey = encryptedKey,
                EncryptedIV = encryptedIV,
                EncryptedVote = encryptedVote
            };

            var signature = CryptoService.SignItem(Converters.ConvertToByteArray(vote), _privateKey);

            var userSignedVote = new UserSignedVoteDTO()
            {
                Vote = vote,
                Signature = signature
            };
            return userSignedVote;
        }

    }
}
