using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth.Service
{
    public class AccountService(IAccountRepository accountRepository
                                ,JwtService jwtService
                                ,IPasswordHasher<User> passwordHasher)
    {
        public async Task RegisterAsync(string userName, string email, string password)
        {
            if (await accountRepository.ExistsAsync(email))
                throw new InvalidOperationException("An account with this email already exists.");

            var userAccount = new User
            {
                Id = Guid.NewGuid(),
                Name = userName,
                Email = email
            };

            var passwordHash = passwordHasher.HashPassword(userAccount, password);

            userAccount.PasswordHash = passwordHash;

            await accountRepository.AddAsync(userAccount);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var userAccount = await accountRepository.GetByEmailAsync(email);

            if(userAccount == null)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }

            var result = passwordHasher
               .VerifyHashedPassword   
                (userAccount, userAccount.PasswordHash, password);

            if(result == PasswordVerificationResult.Success)
            {
                //generate token
                return jwtService.GenerateToken(userAccount);
            }
            else
            {
                throw new Exception("Unauthorized");
            }
        }
    }
}
