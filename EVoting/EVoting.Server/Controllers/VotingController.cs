using EVoting.Common.Blockchain;
using EVoting.Common.Crypto;
using EVoting.Common.DTOs;
using EVoting.Common.DTOs.BaseDTOs;
using EVoting.Common.DTOs.VotingDTOs;
using EVoting.Common.Utils;
using EVoting.Server.Data;
using EVoting.Server.Models;
using EVoting.Server.Services.AuthService;
using EVoting.Server.Services.CAuthService;
using EVoting.Server.Services.NodeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVoting.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class VotingController : ControllerBase
    {
        private readonly EVotingDbContext _context;
        private readonly INodeService _nodeService;
        private readonly IAuthService _authService;
        private readonly ICAuthService _cAuthService;
        private readonly UserManager<User> _userManager;
        private readonly Blockchain _blockchain;
        public VotingController(EVotingDbContext dbContext, INodeService nodeService, IAuthService authService, ICAuthService cAuthService, UserManager<User> userManager, Blockchain blockchain)
        {
            _context = dbContext;
            _nodeService = nodeService;
            _authService = authService;
            _cAuthService = cAuthService;
            _userManager = userManager;
            _blockchain = blockchain;
        }


        [HttpGet("candidates")]
        public async Task<IActionResult> GetCandidates()
        {
            var candidates = await _context.Candidates.ToListAsync();
            var result = new List<CandidateDTO>();
            foreach (var candidate in candidates)
            {
                result.Add(new CandidateDTO()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Party = candidate.Party
                });
            }
            return Ok(new BaseResponseDTO<List<CandidateDTO>>()
            {
                Success = true,
                Result = result,
                Message = "Vote wisely."
            });

        }

        [HttpPost("cast-vote")]
        public async Task<IActionResult> CastVote(UserSignedVoteDTO userSignedVoteDto)
        {
            var user = await _userManager.FindByNameAsync(userSignedVoteDto.UserName);
            // var result = await _userManager.VerifyUserTokenAsync(user, "EVoting", "Vote", userSignedVoteDto.Token);
            // if (!result) return Unauthorized();
            var userDetail = await _context.UserDetails.FirstOrDefaultAsync(x => x.UserId == user.Id);

            var voteToCheck = new Vote()
            {
                EncryptedIV = userSignedVoteDto.Vote.EncryptedIV,
                EncryptedKey = userSignedVoteDto.Vote.EncryptedKey,
                EncryptedVote = userSignedVoteDto.Vote.EncryptedVote,
                VoterId = userSignedVoteDto.Vote.VoterId
            };
            var userSignatureCheck = CryptoService.VerifySignature(Converters.ConvertToByteArray(voteToCheck),
                userSignedVoteDto.Signature, userDetail.PublicKey);

            if (!userSignatureCheck) return Unauthorized();
            var authPrivateKey = _authService.GetPrivateKey();
            var authSignedVoteDto = new AuthSignedVoteDTO()
            {
                Vote = userSignedVoteDto.Vote,
                Signature = CryptoService.SignItem(Converters.ConvertToByteArray(userSignedVoteDto.Vote), authPrivateKey)
            };

            //////////////////////// NODE PART TODO: Move to real nodes ////////////////////////////////////


            var authPublicKey = _authService.GetPublicKey();
            var authSignatureCheck = CryptoService.VerifySignature(Converters.ConvertToByteArray(authSignedVoteDto.Vote)
                , authSignedVoteDto.Signature, authPublicKey);

            if (!authSignatureCheck) return Unauthorized();
            var nodePrivateKey = _nodeService.GetPrivateKey();

            var nodeSignature = CryptoService.SignItem(Converters.ConvertToByteArray(authSignedVoteDto.Vote), nodePrivateKey);

            var transaction = new Transaction()
            {
                EncryptedIV = authSignedVoteDto.Vote.EncryptedIV,
                EncryptedKey = authSignedVoteDto.Vote.EncryptedKey,
                EncryptedVote = authSignedVoteDto.Vote.EncryptedVote,
                VoterId = authSignedVoteDto.Vote.VoterId,
                Signature = nodeSignature
            };

            var nodePublicKey = _nodeService.GetPublicKey();
            _blockchain.AddPublicKey("node", nodePublicKey);

            var transactionAddCheck = _blockchain.AddTransaction(transaction);
            if (!transactionAddCheck) return Unauthorized();
            _blockchain.MineBlock();
            return Ok(new BaseResponseDTO()
            {
                Success = true,
                Message = "Vote registered succesfully."
            });
        }

        [HttpGet("count-votes")]
        public async Task<IActionResult> CountVotes()
        {
            var result = _blockchain.CountVotes(_cAuthService.GetPrivateKey());
            var candidates = await _context.Candidates.ToListAsync();
            foreach (var candidate in candidates)
            {
                if (result.ContainsKey(candidate.Id))
                {
                    candidate.Votes += result[candidate.Id];
                }
            }

            _context.Candidates.UpdateRange(candidates);
            await _context.SaveChangesAsync();
            return Ok(candidates);


        }
    }
}