using EVoting.Common.Crypto;
using EVoting.Common.DTOs;
using EVoting.Common.Utils;
using System;
using System.Collections.Generic;

namespace EVoting.Common.Blockchain
{
    public class Blockchain
    {
        private readonly List<Block> _chain;
        private readonly List<Transaction> _transactions;
        private byte[] _publicKey;


        public Blockchain(bool initialiseChain = true)
        {
            _chain = new List<Block>();
            _transactions = new List<Transaction>();

            if (initialiseChain)
            {
                var genesisBlock = new Block()
                {
                    Index = 0,
                    Transactions = new List<Transaction>(),
                    PreviousHash = new byte[64]
                };
                _chain.Add(genesisBlock);
            }
        }

        public void AddPublicKey(string node, byte[] publicKey)
        {
            _publicKey = publicKey;
        }
        //
        // public bool AddTransaction(Transaction t)
        // {
        //     var vote = new Vote()
        //     {
        //
        //     }
        //     // var signatureCheck = CryptoService.VerifySignature(Converters.ToByteArray(t.))
        //     _transactions.Add(t);
        // }

        public void MineBlock()
        {
            Block block = null;
            try
            {
                block = new Block()
                {
                    Index = _chain[_chain.Count - 1].Index + 1,
                    Transactions = new List<Transaction>(_transactions),
                    PreviousHash = CryptoService.HashItem(Converters.ConvertToByteArray(_chain[_chain.Count - 1]))
                };
            }
            catch (Exception e)
            {

            }
            
            _transactions.Clear();
            _chain.Add(block);
        }

        public void AddBlock(Block b)
        {
            _chain.Add(b);
            _transactions.Clear();
        }

        public bool AddTransaction(Transaction t)
        {
            var vote = new Vote()
            {
                
                EncryptedIV = t.EncryptedIV,
                EncryptedKey = t.EncryptedKey,
                EncryptedVote = t.EncryptedVote,
                VoterId = t.VoterId
            };

            bool signatureCheck = false;
            try
            {
                 signatureCheck =
                    CryptoService.VerifySignature(Converters.ConvertToByteArray(vote), t.Signature, _publicKey);
            }
            catch (Exception e)
            {

            }
            
            if (!signatureCheck) return false;
            foreach (var tx in _transactions)
            {
                if (tx.VoterId == t.VoterId) return false;
            }

            foreach (var block in _chain)
            {
                foreach (var tx in block.Transactions)
                {
                    if (tx.VoterId == t.VoterId) return false;
                }
            }

            _transactions.Add(t);
            return true;
        }

        public Dictionary<Guid, int> CountVotes(byte[] privateKey)
        {
            var votesCount = new Dictionary<Guid, int>();
            votesCount.Add(Guid.Empty, 0);
            foreach (var block in _chain)
            {
                foreach (var transaction in block.Transactions)
                {

                    var symmetricKey = CryptoService.DecryptAsymmetric(transaction.EncryptedKey, privateKey);
                    var symmetricIV = CryptoService.DecryptAsymmetric(transaction.EncryptedIV, privateKey);
                    var byteVote = CryptoService.DecryptSymmetric(transaction.EncryptedVote, symmetricKey, symmetricIV);
                    try
                    {
                        var vote = new Guid(byteVote);
                        if (votesCount.ContainsKey(vote))
                        {
                            votesCount[vote]++;
                        }
                        else
                        {
                            votesCount.Add(vote, 1);
                        }
                    }
                    catch
                    {
                        votesCount[Guid.Empty]++;
                    }
                }
            }

            return votesCount;
        }
    }
}
