using EVoting.Client.Services.VotingService;
using EVoting.Common.Crypto;
using EVoting.Common.DTOs.BaseDTOs;
using EVoting.Common.DTOs.VotingDTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace EVoting.Client
{
    class Program
    {
        static void Vote(HttpClient client)
        {
            var votingService = new VotingService();
            Console.WriteLine("You are voting now");

            Console.WriteLine("UserName: ");
            var username = Console.ReadLine();

            Console.WriteLine("Password");
            var password = Console.ReadLine();

            Console.WriteLine("CNP: ");
            var cnp = Console.ReadLine();

            Console.WriteLine("ID Series: ");
            var series = Console.ReadLine();

            Console.WriteLine("ID Number: ");
            var number = Console.ReadLine();

            var inVoterLoginDto = new InVoterLoginDTO()
            {
                UserName = username,
                CNP = cnp,
                Number = number,
                Series = series,
                Password = password
            };

            var res = client.PostAsync("https://localhost:44355/api/auth/login-voter",
                    new StringContent(JsonSerializer.Serialize(inVoterLoginDto, typeof(InVoterLoginDTO)),
                        Encoding.UTF8, "application/json"))
                .Result;

            var credentials = JsonSerializer.Deserialize<OutVoterLoginDTO>(res.Content.ReadAsStringAsync().Result, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });


            var res2 = client.GetStringAsync("https://localhost:44355/api/voting/candidates").Result;
            var candidates = JsonSerializer.Deserialize<BaseResponseDTO<List<CandidateDTO>>>(res2, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }).Result;


            int i = 0;
            foreach (var candidate in candidates)
            {
                Console.WriteLine($"Option {i}, {candidate.Name}, {candidate.Party}\n\n");
                i++;
            }

            Console.WriteLine("\n\nYour option: ");
            var option = int.Parse(Console.ReadLine());
            var candidateSelected = candidates[option];
            var candidateId = candidateSelected.Id;


            votingService.InitialiseKeys(credentials.EncryptedPrivateKey, credentials.PublicKey, password);
            var userSignedVote = votingService.CastVote(credentials.UserId, candidateId, credentials.CAuthPublicKey);
            userSignedVote.Token = credentials.Token;
            userSignedVote.UserName = username;

            var res3 = client.PostAsync("https://localhost:44355/api/voting/cast-vote", new StringContent(
                    JsonSerializer.Serialize(userSignedVote, typeof(UserSignedVoteDTO)), Encoding.UTF8,
                    "application/json"))
                .Result;

            var response = res3.Content.ReadAsStringAsync().Result;
            Console.WriteLine(response);
        }

        static void Register(HttpClient httpClient)
        {
            Console.WriteLine("FirstName: ");
            var firstName = Console.ReadLine();

            Console.WriteLine("LastName: ");
            var lastName = Console.ReadLine();

            Console.WriteLine("Email: ");
            var email = Console.ReadLine();

            Console.WriteLine("Phone: ");
            var phone = Console.ReadLine();

            Console.WriteLine("CNP: ");
            var cnp = Console.ReadLine();

            Console.WriteLine("ID Series: ");
            var series = Console.ReadLine();

            Console.WriteLine("ID Number: ");
            var number = Console.ReadLine();

            string password = String.Empty, confirmPassword = String.Empty;
            while (password == String.Empty || password != confirmPassword)
            {
                Console.WriteLine("Please insert a password (>12 chars, 1 number, 1 capital, 1 non alpha numeric)");

                Console.WriteLine("Password: ");
                password = Console.ReadLine();

                Console.WriteLine("Confirm Password: ");
                confirmPassword = Console.ReadLine();
            }

            var inVoterRegisterDTO = new InVoterRegisterDTO()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                CNP = cnp,
                Series = series,
                Number = number,
                Password = password,
                ConfirmPassword = confirmPassword,
            };

            (var encryptedPrivateKey, var privateKey, var publicKey) =
                CryptoService.GenerateAsymmetricKeys(password);

            inVoterRegisterDTO.EncryptedPrivateKey = encryptedPrivateKey;
            inVoterRegisterDTO.PublicKey = publicKey;


            var response = httpClient.PostAsync("https://localhost:44355/api/auth/register-voter",
                new StringContent(JsonSerializer.Serialize(inVoterRegisterDTO, typeof(InVoterRegisterDTO)),
                    Encoding.UTF8, "application/json")).Result;

        }

        static void Main(string[] args)
        {
            var httpClient = new HttpClient();

            Console.WriteLine("(1) - Register\n(2) - Login + Vote");
            var option = int.Parse(Console.ReadLine());

            if (option == 1)
            {
                Register(httpClient);
            }
            else if (option == 2)
            {
                Vote(httpClient);
            }



        }
    }
}
