using EVoting.Common.DTOs.BaseDTOs;
using EVoting.Common.DTOs.VotingDTOs;
using EVoting.Server.Data;
using EVoting.Server.Models;
using EVoting.Server.Services.CAuthService;
using EVoting.Server.Services.EMailService;
using EVoting.Server.Utils.ShellExecutors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EVoting.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ICAuthService _cAuthService;
        private readonly EVotingDbContext _context;
        private const string _FACES_PATH = "C:\\Users\\acosm\\Desktop\\EVoting\\EVoting.Face";

        public AuthController(EVotingDbContext dbContext,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ICAuthService cAuthService

            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = dbContext;
            _cAuthService = cAuthService;
            _userManager.RegisterTokenProvider("EVoting", new AuthenticatorTokenProvider<User>());

        }



        [HttpPost("login-voter")]
        public async Task<IActionResult> LoginVoter(InVoterLoginDTO inVoterLoginDto)
        {
            var user = await _userManager.FindByNameAsync(inVoterLoginDto.UserName);
            if (user == null) return BadRequest();
            var userDetail = await _context.UserDetails.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (userDetail.CNP != inVoterLoginDto.CNP || userDetail.Series != inVoterLoginDto.Series ||
                userDetail.Number != inVoterLoginDto.Number)
            {
                return Unauthorized();
            }


            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, inVoterLoginDto.Password, false);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    return Unauthorized();
                }
            }

            string token = string.Empty;
            try
            {
                token = await _userManager.GenerateUserTokenAsync(user, "EVoting", "Vote");
            }
            catch (Exception e)
            {

            }


            var caPublicKey = _cAuthService.GetPublicKey();



            var outVoterLoginDto = new OutVoterLoginDTO()
            {
                UserId = user.Id,
                CAuthPublicKey = caPublicKey,
                Token = token,
                EncryptedPrivateKey = userDetail.EncryptedPrivateKey,
                PublicKey = userDetail.PublicKey
            };

            return Ok(outVoterLoginDto);

        }

        [HttpPost("login-voter-face")]
        public async Task<IActionResult> LoginVoterFace([FromForm(Name = "image")] IFormFile imageFile,
            [FromForm] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest();

            var filePath = Path.GetTempFileName() + ".jpeg";

            // var filePath = Path.Combine(_FACES_PATH, Path.GetRandomFileName() + ".jpeg");
            if (imageFile.Length > 0)
            {
                using (var fileStream = System.IO.File.Create(filePath))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }

            var result = await ShellHelper.VerifyImageAsync(filePath, username);
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch
            {

            }
            if (!result.Success) return BadRequest(result.Message);
            if (!result.Result) return Unauthorized(result.Message);
            return Ok(new BaseResponseDTO<bool>()
            {
                Success = true,
                Result = true,
                Message = "User authentificated."
            });


        }



        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(InConfirmEmailDTO inConfirmEmailDto)
        {
            var user = await _userManager.FindByNameAsync(inConfirmEmailDto.UserName);
            var result = await _userManager.ConfirmEmailAsync(user, inConfirmEmailDto.Token.Replace(" ", "+"));
            return Ok(new BaseResponseDTO<bool>()
            {
                Message = String.Empty,
                Result = result.Succeeded,
                Success = true
            });
        }


        [HttpPost("register-voter")]
        public async Task<IActionResult> RegisterVoter(InVoterRegisterDTO inVoterRegisterDto)
        {
            if (inVoterRegisterDto.Password != inVoterRegisterDto.ConfirmPassword) return BadRequest();
            var g = Guid.NewGuid();
            var username = Convert.ToBase64String(g.ToByteArray()).Replace("=", string.Empty)
                .Replace("+", String.Empty).Replace("/", String.Empty);
            var userDetail = new UserDetail()
            {
                CNP = inVoterRegisterDto.CNP,
                EncryptedPrivateKey = inVoterRegisterDto.EncryptedPrivateKey,
                PublicKey = inVoterRegisterDto.PublicKey,
                FirstName = inVoterRegisterDto.FirstName,
                LastName = inVoterRegisterDto.LastName,
                Number = inVoterRegisterDto.Number,
                Series = inVoterRegisterDto.Series
            };
            var user = new User()
            {
                Email = inVoterRegisterDto.Email,
                PhoneNumber = inVoterRegisterDto.Phone,

            };
            user.UserDetail = userDetail;
            user.UserName = username;
            var result = await _userManager.CreateAsync(user, inVoterRegisterDto.Password);
            if (!result.Succeeded) return Unauthorized();

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var emailHtml = await System.IO.File.ReadAllTextAsync("wwwroot/confirmEmailToken.html");
            emailHtml = emailHtml.Replace("{BaseUrl}", "http://localhost:8080/#/verify")
                .Replace("{EmailToken}", emailToken)
                .Replace("{UserName}", username);

            var emailSuccess = await EMailService.SendEmail(inVoterRegisterDto.Email,
                "EVoting Username and Confirm Email",
                emailHtml);

            return Ok(new BaseResponseDTO<string>()
            {
                Success = true,
                Message = "User registered successfully! Please also verify your email.",
                Result = username
            });
        }

        [HttpPost("register-voter-face")]
        public async Task<IActionResult> OnImageRegister([FromForm(Name = "image")] IFormFile imageFile,
            [FromForm] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest();

            var filePath = Path.Combine(_FACES_PATH, Path.GetRandomFileName() + ".jpeg");
            if (imageFile.Length > 0)
            {
                using (var fileStream = System.IO.File.Create(filePath))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }

            var result = await ShellHelper.RegisterImageAsync(filePath, username);
            if (!result.Success) return BadRequest(result.Message);
            if (!result.Result) return Unauthorized(result.Message);

            return Ok(new BaseResponseDTO<bool>()
            {
                Success = true,
                Result = true,
                Message = result.Message
            });
        }
    }
}