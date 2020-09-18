using EVoting.Server.Data;
using EVoting.Server.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace EVoting.Server.Utils.Seeders
{
    public class CandidatesSeed
    {
        private readonly EVotingDbContext _context;
        public CandidatesSeed(EVotingDbContext dbContext)
        {
            _context = dbContext;
        }

        public void SeedCandidates()
        {
            if (_context.Candidates.Any()) return;
            var candidate1 = new Candidate()
            {
                Name = "Basescu",
                Party = "PNL"
            };
            var candidate2 = new Candidate()
            {
                Name = "Becali",
                Party = "PMP"
            };
            var candidate3 = new Candidate()
            {
                Name = "Nacho",
                Party = "Forta"
            };

            _context.Candidates.Add(candidate1);
            _context.Candidates.Add(candidate2);
            _context.Candidates.Add(candidate3);
            _context.SaveChanges();
        }
    }
}
