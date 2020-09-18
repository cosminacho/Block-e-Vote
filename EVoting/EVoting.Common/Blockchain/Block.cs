using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.Blockchain
{
    public class Block
    {
        public Guid BlockId { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public int Index { get; set; }
        public List<Transaction> Transactions { get; set; }
        public byte[] PreviousHash { get; set; }
    }
}
