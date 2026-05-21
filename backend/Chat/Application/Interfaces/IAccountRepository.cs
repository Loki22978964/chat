using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAccountRepository
    {
        public Task<User?> GetByEmailAsync(string email);
        public Task AddAsync(User user);
        public Task<bool> ExistsAsync(string email);
    }
}
