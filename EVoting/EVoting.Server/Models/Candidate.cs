using EVoting.Server.Models.Base;

namespace EVoting.Server.Models
{
    public class Candidate : BaseEntity
    {
        public string Name { get; set; }
        public string Party { get; set; }
        public int Votes { get; set; }
    }
}
